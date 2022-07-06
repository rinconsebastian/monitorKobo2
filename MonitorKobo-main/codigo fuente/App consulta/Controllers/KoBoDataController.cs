using App_consulta.Data;
using App_consulta.Models;
using App_consulta.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
namespace App_consulta.Controllers
{
    public class KoBoDataController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment _env;
        private readonly MongoDatabaseService mdb;

        public KoBoDataController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> _userManager,
            IWebHostEnvironment env,
            MongoDatabaseService mongoService)
        {

            db = context;
            userManager = _userManager;
            _env = env;
            mdb = mongoService;
        }


        [HttpPost]
        [Authorize(Policy = "Registro.Imagen.Restablecer")]
        public async Task<RespuestaAccion> ResetImage(string filename, string idKobo, int idProject, string name = "")
        {
            var r = new RespuestaAccion();

            try
            {
                var _fileName = Path.GetFileName(filename);

                var project = await db.KoProject.FindAsync(idProject);
                var remoteUri = project.KoboAttachment + project.KoboUsername + "/attachments/";
                var _path = Path.Combine(_env.ContentRootPath, "Storage", project.Collection, idKobo);

                DownloadFile(remoteUri, _fileName, _path, "", project.KoboApiToken);

                r.Success = true;

                var log = new Logger(db);
                var registro = new RegistroLog { Usuario = User.Identity.Name, Accion = "ResetImage", Modelo = project.ValidationName + " " + idKobo, ValAnterior = "", ValNuevo = name + ": " + filename };
                await log.RegistrarDirecto(registro);

            }
            catch (Exception e)
            {
                r.Message = e.Message;
            }

            return r;
        }


        //Actualizar archivo de encuestas
        [Authorize(Policy = "Encuestas.Actualizar")]
        public async Task<ActionResult> ActualizarManual()
        {
            var error = "";
            var accion = "";
            var lineBreak = "";

            var projects = await db.KoProject.ToListAsync();

            foreach (var project in projects)
            {
                var resp = await LoadDataFromKobo(project);
                var r = ExecUpdateData(resp);
                if (r.Success) { accion += lineBreak + r.Message; }
                else { error += lineBreak + r.Message; }
                lineBreak = "<br>";
            }

            ViewBag.Error = error;
            ViewBag.Accion = accion;
            return View();
        }

        [HttpGet]
        public async Task<bool> Auto(string auth)
        {
            if (auth == "#51kS7.Jms22")
            {
                var projects = await db.KoProject.ToListAsync();

                foreach (var project in projects)
                {
                    var resp = await LoadDataFromKobo(project);
                    ExecUpdateData(resp);
                }

                return true;
            }
            return false;
        }


        [Authorize(Policy = "Registro.Validar")]
        public async Task<RespuestaAccion> LoadValidation(int projectId, string idDb, ApplicationUser user)
        {
            var r = new RespuestaAccion();

            var project = await db.KoProject.FindAsync(projectId);
            if (project == null || !project.Validable) { return r; }

            //valida los que el item exista y que no este en borrador
            var previo = await mdb.Find(project.Collection, idDb);
            if(previo == null){return r;}
            else if (previo.State >= KoGenericData.ESTADO_BORRADOR)
            {
                r.Url = "Validation/Edit/" + idDb + "?project=" + project.Id;
                r.Success = true;
                return r;
            }

            //Carga los datos de conexión desde la configuración 

            var listParams = await db.KoField.Where(n => n.IdProject == project.Id && n.NameKobo != null)
                .Select(n => n.NameKobo).ToArrayAsync();

            var mapParams = await db.KoField.Where(n => n.IdProject == project.Id && n.NameKobo != null)
                .ToListAsync();

            var valuesQuery = await db.KoVariable.ToListAsync();
            var allMapValues = valuesQuery.GroupBy(n => n.Group)
                .ToDictionary(n => n.Key, n => n.ToDictionary(k => k.Key, k => k.Value));

            var fields = JsonConvert.SerializeObject(listParams);
            var query = JsonConvert.SerializeObject(new { _id = previo.IdKobo });

            var url = project.KoboKpiUrl + "/assets/" + project.KoboAssetUid + "/submissions/?format=json" +
                "&query=" + HttpUtility.UrlEncode(query) +
                "&fields=" + HttpUtility.UrlEncode(fields);

            KoExtendData result;

            //consulta la información completa y la carga en la variable result
            try
            {
                List<KoGenericData> encuestas = new();

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Token " + project.KoboApiToken);
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                dynamic data = JsonConvert.DeserializeObject(responseBody);

                //Valida que se cargaran resultados
              
                if (data == null) { r.Message = "ERROR: No fue posible recuperar la información"; return r; }

                foreach (var resultKobo in data)
                {
                    var dataItem = new KoExtendData();
                    var props = new Dictionary<string, Object>();
                    foreach (var param in mapParams)
                    {
                        if (resultKobo[param.NameKobo] == null) { continue; }
                        string valueTemp = (String)resultKobo[param.NameKobo];

                        switch (param.NameDB)
                        {
                            case "kobo_id":
                                dataItem.IdKobo = valueTemp;
                                break;
                            case "user":
                                dataItem.User = valueTemp;
                                break;
                            default:
                                
                                props.Add(param.NameDB, MapValue(valueTemp, param.FormGroupSelect, allMapValues));
                                break;
                        }
                    }

                    dataItem.DynamicProperties = props;
                    dataItem.State = KoGenericData.ESTADO_BORRADOR;
                    dataItem.CreateDate = DateTime.Now;
                    dataItem.IdCreateByUser = user.Id;
                    dataItem.LastEditDate = DateTime.Now;
                    dataItem.IdLastEditByUser = user.Id;
                    dataItem.IdResponsable = user.IDDependencia;
                    encuestas.Add(dataItem);
                }

                result = (KoExtendData)encuestas[0];
            }
            catch (HttpRequestException e) { r.Message = e.Message; return r; }

            if (result != null)
            {
                // Descarga los adjuntos
                var _path = Path.Combine(_env.ContentRootPath, "Storage", project.Collection , result.IdKobo);
                var _relative = Path.Combine(project.Collection, result.IdKobo);
                var remoteUri = project.KoboAttachment + project.KoboUsername + "/attachments/";

                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }
                var filesErrors = false;

                var fileParams = mapParams.Where(n => n.NameDB != null && n.FormType == KoField.TYPE_FILE || n.FormType == KoField.TYPE_IMG)
                    .Select(n => n.NameDB)
                    .ToList();
                foreach(var mProperty in fileParams)
                {
                    if (result.DynamicProperties.ContainsKey(mProperty))
                    {
                        var value = result.DynamicProperties[mProperty];
                        if(value != null)
                        {
                            var filePath = DownloadFile(remoteUri, (String)value, _path, _relative, project.KoboApiToken);
                            if (filePath == "")
                            {
                                filesErrors = true;
                                break;
                            }
                            result.DynamicProperties[mProperty] = filePath;
                        }
                    }
                }
                

                //Valida los adjuntos
                if (filesErrors) { r.Message = "ERROR: El servidor esta bloqueado, intente más tarde."; return r; }

                //Consulta la dependencia del encuestador
                if (result.User != null && result.User != "")
                {
                    var encuestador = await db.Pollster.Where(n => n.DNI == result.User).FirstOrDefaultAsync();
                    result.IdResponsable = encuestador != null ? encuestador.IdResponsable : user.IDDependencia;
                }

                var log = new Logger(db);
                try
                {
                    //Validación del registro previo a guardar
                    previo = await mdb.Find(project.Collection, idDb);
                    if (previo.State >= KoGenericData.ESTADO_BORRADOR)
                    {
                        r.Url = "Validation/Edit/" + idDb + "?project=" + project.Id;
                        r.Success = true;
                        return r;
                    }

                    //Guarda el registro
                    result.Id = previo.Id;
                    var save = await mdb.Replace(project.Collection, result);
                    if(!save)
                    {
                        r.Message = "ERROR: No fue posible actualizar el registro."; return r;
                    }
                    r.Url = "Validation/Edit/" + idDb + "?project=" + project.Id;
                    r.Success = true;

                    var registro = new RegistroLog { Usuario = user.Email, Accion = "Create", Modelo = project.ValidationName, ValNuevo = result };
                    await log.Registrar(registro, typeof(KoExtendData), Int32.Parse(result.IdKobo));

                }
                catch (Exception e) { r.Message = e.InnerException.Message; return r; }
            }
            else
            {
                r.Message = "ERROR: No se encuentran resultados."; return r;
            }

            return r;
        }



        // EXTRA

        private async Task<string> LoadDataFromKobo(KoProject project)
        {
            var maxId = await mdb.MaxIdKobo(project.Collection);

            var listParams = await db.KoField.Where(n => n.IdProject == project.Id && n.NameKobo != null && n.Validable == false)
                .Select(n => n.NameKobo).ToArrayAsync();

            var mapParams  = await db.KoField.Where(n => n.IdProject == project.Id && n.NameKobo != null && n.Validable == false)
                .ToListAsync();

            var valuesQuery = await db.KoVariable.ToListAsync();
            var allMapValues = valuesQuery.GroupBy(n => n.Group)
                .ToDictionary(n => n.Key, n => n.ToDictionary(k => k.Key, k => k.Value));

            //Url de consulta para kobo
            var fields = JsonConvert.SerializeObject(listParams);
            var sort = "&sort=%7B%22_id%22%3A1%7D";
            var limit = "&limit=1000";
            var query = maxId != null ? "&query=%7B%22_id%22%3A%7B%22%24gt%22%3A" + maxId + "%7D%7D" : "";
            var url = project.KoboKpiUrl + "/assets/" + project.KoboAssetUid
                + "/submissions/?format=json&fields=" + HttpUtility.UrlEncode(fields)
                + sort + query + limit;

            string resp = "";
            try
            {
                //Consulta la información
                var encuestas = await GetDataFromUrl(url, project.KoboApiToken, mapParams, project.ValidationField, project.ValidationValue, allMapValues);

                //Guarda la información en MongoDB
                if (encuestas.Count == 0)
                {
                    resp = "ALERTA: No se encontraron nuevas encuestas de " + project.Name + ".";
                }
                else
                {
                    mdb.InsertMany(project.Collection, encuestas);
                    resp = "EXITO: Se cargaron " + encuestas.Count + " encuestas de " + project.Name + ".";
                }
                project.LastUpdate = DateTime.Now;
                db.Entry(project).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (HttpRequestException e)
            {
                resp = "ERROR: " + e.Message;
            }
            return resp;
        }

        private async Task<List<KoGenericData>> GetDataFromUrl(string url, string token, List<KoField> mapParams, string validationField, string validationValue, Dictionary<string, Dictionary<string, string>> allMapValues)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            dynamic data = JsonConvert.DeserializeObject(responseBody);

            //Valida que se cargaran resultados
            List<KoGenericData> encuestas = new();
            if (data == null) { return encuestas; }

            foreach (var result in data)
            {
                var dataItem = new KoGenericData();
                var props = new Dictionary<string, Object>();
                var state = KoGenericData.ESTADO_NUEVO;
                foreach (var param in mapParams)
                {
                    if(result[param.NameKobo] == null) { continue; }
                    string  valueTemp = (String)result[param.NameKobo];

                    var conditionalValue = validationValue != null ? validationValue == valueTemp : valueTemp != null;
                    if (validationField != null &&  param.NameDB == validationField && conditionalValue )
                    {
                        state = KoGenericData.ESTADO_PENDIENTE;
                    }

                    switch (param.NameDB)
                    {
                        case "kobo_id":
                            dataItem.IdKobo = valueTemp;
                            break;
                        case "user":
                            dataItem.User = valueTemp;
                            break;
                        default:
                            props.Add(param.NameDB, MapValue(valueTemp, param.FormGroupSelect, allMapValues));
                            break;
                    }
                }

                dataItem.State = state;
                dataItem.DynamicProperties = props;
                encuestas.Add(dataItem);
            }
            return encuestas;
        }


        private string DownloadFile(string remoteUri, string fileName, string path, string relative, string token)
        {
            var relativePath = Path.Combine(relative, fileName);
            var fullPath = Path.Combine(path, fileName);

            try
            {
                string myStringWebResource = remoteUri + fileName;
                var myWebClient = new WebClient();
                myWebClient.Headers.Add("User-Agent: Other");
                myWebClient.Headers.Add(HttpRequestHeader.Authorization, "token  " + token);

                myWebClient.DownloadFile(myStringWebResource, fullPath);

            }
            catch (Exception e)
            {
                var a = e.Message;
                if (a == "The remote server returned an error: (404) Not Found.")
                {
                    var storagePath = Path.Combine(_env.ContentRootPath, "Storage");
                    System.IO.File.Copy(Path.Combine(storagePath, "noload.png"), fullPath);
                }
            }

            return relativePath;
        }

        public String GetDatetimeData(String file)
        {
            var text = "";
            try
            {
                var _path = Path.Combine(_env.ContentRootPath, "Storage");
                DateTime creation = System.IO.File.GetCreationTime(Path.Combine(_path, file + ".json"));
                text = creation.ToString("f");
            }
            catch (Exception) { }
            return text;
        }

        /**
         * Guardar archivo de registro de actualización
         */
        private RespuestaAccion ExecUpdateData(String resp)
        {
            var r = new RespuestaAccion();

            var fecha = DateTime.Now.ToString("r");
            var accion = "[" + fecha + "] " + resp;


            var _path = Path.Combine(_env.ContentRootPath, "Storage");
            var archivo = _path + "/Logs.txt";
            try
            {
                if (!System.IO.File.Exists(archivo))
                {
                    using StreamWriter sw = System.IO.File.CreateText(archivo);
                    sw.WriteLine(accion);
                }
                else
                {
                    using StreamWriter sw = System.IO.File.AppendText(archivo);
                    sw.WriteLine(accion);
                }
                r.Message = accion;
                r.Success = true;
            }
            catch (Exception e)
            {
                r.Message = e.Message;
            }

            return r;
        }

        /*
         * Remapea los campos con la información de las variables
         * 
         */
        private static string MapValue(string key, string group, Dictionary<string,Dictionary<string, string>> allValues)
        {
            if(group == null || key == null) { return key; }

            if (allValues.ContainsKey(group))
            {
                var values = allValues[group];
                if (values.ContainsKey(key))
                {
                    key = values[key];
                }
            }
            return key;
        }
    }
}
