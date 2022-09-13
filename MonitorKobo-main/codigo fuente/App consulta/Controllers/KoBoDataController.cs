using App_consulta.Data;
using App_consulta.Models;
using App_consulta.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
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
        public async Task<RespuestaAccion> ResetImage(string id, int projectid, string name)
        {
            var r = new RespuestaAccion();

            var project = await db.KoProject.FindAsync(projectid);
            if (project == null || !project.Validable) { r.Message = "Error: proyecto no encontrado."; return r; }

            var item = await mdb.Find(project.Collection, id);
            if (item == null) { r.Message = "Error: Item no encontrado."; return r; }

            try
            {
                //Variables path
                var remoteUri = project.KoboAttachment + project.KoboUsername + "/attachments/";
                var pathStorage = Path.Combine(_env.ContentRootPath, "Storage");
                
                //Borra el archivo actual
                var currentFile = "";
                if (item.DynamicProperties.ContainsKey(name))
                {
                    if (item.DynamicProperties[name] != null)
                    {
                        currentFile =  (String)item.DynamicProperties[name];
                        var currentPath = Path.Combine(pathStorage, currentFile);
                        if (System.IO.File.Exists(currentPath))
                            System.IO.File.Delete(currentPath);                       
                    }   
                }
                
               
                //Carga el archivo desde reset
                var resetName = "reset_" + name;
                var _fileName = item.DynamicProperties.ContainsKey(resetName) ? item.DynamicProperties[resetName] : null;
                if (_fileName == null) { r.Message = "Error: Campo no encontrado."; return r; }
                var filename = (String)_fileName;

                var _path = Path.Combine(_env.ContentRootPath, "Storage", project.Collection, item.IdKobo);
                var _relative = Path.Combine(project.Collection, item.IdKobo);

                var resultFile = DownloadFile(remoteUri, filename, _path, _relative, project.KoboApiToken);
                if(resultFile == ""){ r.Message = "Error: No fue posible guardar los cambios."; return r; }

                //actualiza el campo
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                var update = Builders<KoExtendData>.Update.Set("edit_user", user.Id);
                var datetime = DateTime.Now;
                update = update.Set("edit_date", datetime);
                update = update.Set(name, resultFile);

                var filter = Builders<KoExtendData>.Filter.Eq(n => n.Id, item.Id);
                var save = await mdb.Update(project.Collection, update, filter);
                if (save) {
                    r.Success = true;
                    r.Message = resultFile;

                    var log = new Logger(db);
                    var registro = new RegistroLog
                    {
                        Usuario = User.Identity.Name,
                        Accion = "ResetFile",
                        Modelo = project.ValidationName + " " + item.IdKobo,
                        ValAnterior = name + ": " + currentFile,
                        ValNuevo = name + ": " + resultFile
                    };
                    await log.RegistrarDirecto(registro);
                }
            }
            catch (Exception e){r.Message = e.Message;}

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
                        string valueTemp = resultKobo[param.NameKobo] != null ? (String)resultKobo[param.NameKobo] : null;

                        switch (param.NameDB)
                        {
                            case "kobo_id":
                                dataItem.IdKobo = valueTemp;
                                break;
                            case "user":
                                dataItem.User = valueTemp;
                                break;
                            default:
                                
                                if(param.FormType == KoField.TYPE_IMG || param.FormType == KoField.TYPE_FILE)
                                {
                                    props.Add(param.NameDB, valueTemp);
                                    if(valueTemp != null)
                                    {
                                        props.Add("reset_"+param.NameDB, valueTemp);
                                    }
                                }
                                else
                                {
                                    props.Add(param.NameDB, MapValue(valueTemp, param.FormGroupSelect, allMapValues));
                                }
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

                //Add extra props
                foreach(var prop in previo.DynamicProperties)
                {
                    if (!result.DynamicProperties.ContainsKey(prop.Key))
                    {
                        result.DynamicProperties.Add(prop.Key, prop.Value);
                    }
                }
            }
            catch (Exception e) { r.Message = e.Message; return r; }

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

        //Completar datos no cargados
        [Authorize(Policy = "Acuicultura.Listado")]
        public async Task<RespuestaAccion> Complete(string id, int project)
        {
            var r = new RespuestaAccion();

            var projectObj = await db.KoProject.FindAsync(project);
            if (projectObj == null || !projectObj.Validable) { return r; }

            //valida los que el item exista y que no este en borrador
            var previo = await mdb.Find(projectObj.Collection, id);
            if (previo == null) { r.Message = "ERROR: registro no encontrado"; return r; }

            if (previo.DynamicProperties.ContainsKey("formato")) { r.Message = "Registro previamente cargado."; }


            //Carga los datos de conexión desde la configuración 
            var fieldsAq = await db.AquacultureField.Where(n => n.NameKobo != null).ToListAsync();
            var listParam = fieldsAq.Where(n => n.IdParent == null).Select(n => n.NameKobo).ToList();
            var fieldsKo = await db.KoField.Where(n => n.IdProject == projectObj.Id && n.NameKobo != null && n.Validable == false).ToListAsync();
            listParam.AddRange(fieldsKo.Select(n => n.NameKobo).ToList());
            var arrParam = listParam.Distinct().ToArray();
            var paramsClean = "['" + String.Join("','", arrParam) + "']";
            paramsClean = paramsClean.Replace("'", "\"");

            var query = JsonConvert.SerializeObject(new { _id = previo.IdKobo });

            var url = projectObj.KoboKpiUrl + "/assets/" + projectObj.KoboAssetUid + "/submissions/?format=json"
                + "&fields=" + paramsClean
                + "&query=" + HttpUtility.UrlEncode(query);
            
            KoGenericData result;

            //consulta la información completa y la carga en la variable result
            try
            {
                //Consulta la información
                var dataKobo = await GetDataFromUrl(url, projectObj.KoboApiToken);
                var encuestas = await FormatDataFromKobo(dataKobo, fieldsAq, fieldsKo, projectObj);
                if (encuestas.Count == 0) { r.Message = "ALERTA: No se encontraron registro."; return r; }
                result = encuestas[0];

                //Reemplazar datos previos
                foreach (var prop in previo.DynamicProperties)
                {

                    if (result.DynamicProperties.ContainsKey(prop.Key))
                    {
                        if (prop.Value != null)
                        {
                            result.DynamicProperties[prop.Key] = prop.Value;
                        }
                    }
                    else
                    {
                        result.DynamicProperties.Add(prop.Key, prop.Value);
                    }
                }
                if (result.DynamicProperties.ContainsKey("user"))
                    result.DynamicProperties.Remove("user");
            }
            catch (Exception e) { r.Message = e.Message; return r; }

            //Guarda el registro
            if (result != null)
            {
                try
                {
                    result.Id = previo.Id;
                    result.IdKobo = previo.IdKobo;
                    result.User = previo.User;
                    result.State = previo.State;
                    var save = await mdb.ReplaceGeneric(projectObj.Collection, result);
                    if (!save){r.Message = "ERROR: No fue posible actualizar el registro."; return r;}
                    r.Success = true;
                }
                catch (Exception e) { r.Message = e.InnerException.Message; return r; }
            }
            else{r.Message = "ERROR: Resultado nulo."; return r;}

            return r;
        }


        //Ocultar/mostrar registro
        [Authorize(Policy = "Acuicultura.Listado")]
        public async Task<RespuestaAccion> Toggle(string id, int project)
        {
            var r = new RespuestaAccion();

            var projectObj = await db.KoProject.FindAsync(project);
            if (projectObj == null) { return r; }

            //valida los que el item exista y que no este en borrador
            var previo = await mdb.Find(projectObj.Collection, id);
            if (previo == null) { r.Message = "ERROR: registro no encontrado"; return r; }

            var state = true;
            if (previo.DynamicProperties.ContainsKey("hidden"))
            {
                state = !((Boolean)previo.DynamicProperties["hidden"]);
            }
            var update = Builders<KoExtendData>.Update.Set("hidden", state);
            var filter = Builders<KoExtendData>.Filter.Eq(n => n.Id, previo.Id);
            var save = await mdb.Update(projectObj.Collection, update, filter);

            if (save) { r.Success = true;}
            else { r.Message = "Error: No fue posible cambiar el estado del registro."; }
            return r;
        }


        // EXTRA
        private async Task<string> LoadDataFromKobo(KoProject project)
        {
            string resp = "";

            var fieldsAq = await db.AquacultureField.Where(n => n.NameKobo != null).ToListAsync();
            var fieldsKo = await db.KoField.Where(n => n.IdProject == project.Id && n.NameKobo != null && n.Validable == false).ToListAsync();
            var maxId = await mdb.MaxIdKobo(project.Collection);

            List<string> listParam = new();
            
            if (project.Validable) {
                listParam = fieldsAq.Where(n => n.IdParent == null).Select(n => n.NameKobo).ToList();
                listParam.AddRange(fieldsKo.Select(n => n.NameKobo).ToList());
            }
            else
            {
                listParam = fieldsKo.Select(n => n.NameKobo).ToList();
            }

            var arrParam = listParam.Distinct().ToArray();
            var paramsClean= "['" + String.Join("','", arrParam) + "']";
            paramsClean = paramsClean.Replace("'", "\"");

            var sort = "&sort=%7B%22_id%22%3A1%7D";
            var limit = "&limit=1000";

            //var query = maxId != null ? "&query=%7B%22_id%22%3A%7B%22%24gt%22%3A" + maxId + "%7D%7D" : "";
            var userField = fieldsKo.Where(n => n.NameDB == "user").Select(n => n.NameKobo).FirstOrDefault();
            var queryFull = maxId != null ? "&query={'_id':{'$gt':" + maxId + "},'" + userField + "':{'$exists':1}}" : "&query={'" + userField + "': {'$exists':1}}";
            var query = queryFull.Replace("'", "\"");


            var url = project.KoboKpiUrl + "/assets/" + project.KoboAssetUid + "/submissions/?format=json"
                + "&fields=" + paramsClean
                + sort + query + limit;


            try
            {
                //Consulta la información
                var dataKobo = await GetDataFromUrl(url, project.KoboApiToken);

                var encuestas = await FormatDataFromKobo(dataKobo, fieldsAq, fieldsKo, project);

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

        //Recorre y organiza la información descarga desde KoBO
        private async Task<List<KoGenericData>> FormatDataFromKobo(dynamic data, List<AquacultureField> fieldsAq, List<KoField> fieldsKo, KoProject project)
        {
            List<KoGenericData> encuestas = new();
            if (data == null) { return encuestas; }

            //KO
            var valuesQuery = await db.KoVariable.ToListAsync();
            var allMapValues = valuesQuery.GroupBy(n => n.Group)
                .ToDictionary(n => n.Key, n => n.ToDictionary(k => k.Key, k => k.Value));

            //AQ
            var fieldsParent = fieldsAq.Where(n => n.IdParent == null).ToList();
            var fieldsChild = fieldsAq.Where(n => n.IdParent != null).GroupBy(n => n.IdParent)
                 .ToDictionary(n => n.Key, n => n);

            var valuesQueryAq = await db.AquacultureVariable.ToListAsync();
            var allMapValuesAq = valuesQueryAq.GroupBy(n => n.Group)
                .ToDictionary(n => n.Key, n => n.ToDictionary(k => k.Key, k => k.Value));


            foreach (var result in data)
            {
                var props = new Dictionary<string, Object>();
                var dataItem = new KoGenericData();
                dataItem.DynamicProperties = props;

                LoadKoFields(result, dataItem, fieldsKo, project.ValidationField, project.ValidationValue, allMapValues);
                if (project.Validable) { 
                    LoadAquacultureFields(result, dataItem, fieldsParent, fieldsChild, allMapValuesAq);
                }

                encuestas.Add(dataItem);
            }
            return encuestas;
        }

        private static void LoadKoFields(dynamic result, KoGenericData dataItem, List<KoField> fieldsKo,
            string validationField, string validationValue, Dictionary<string, Dictionary<string, string>> allMapValues)
        {
            var props = dataItem.DynamicProperties;

            var state = validationField != null ? KoGenericData.ESTADO_NUEVO : KoGenericData.ESTADO_SOLO_LECTURA;
            foreach (var param in fieldsKo)
            {
                string valueTemp = (String)result[param.NameKobo];

                var conditionalValue = validationValue != null ? validationValue == valueTemp : valueTemp != null;
                if (validationField != null && param.NameDB == validationField && conditionalValue)
                {
                    state = KoGenericData.ESTADO_PENDIENTE;
                }

                if (param.NameDB == "kobo_id") {
                    dataItem.IdKobo = valueTemp;
                }
                else if (param.NameDB == "kobo_id") {
                    dataItem.User = valueTemp;
                }
                else if(valueTemp != null) {
                    switch (param.FormType)
                    {
                        case KoField.TYPE_SELECT_ONE:
                            props.Add(param.NameDB, MapValue(valueTemp, param.FormGroupSelect, allMapValues));
                            break;
                        case KoField.TYPE_SELECT_MULTIPLE:
                            var listAux = new List<string>();
                            var splitList = ((String)result[param.NameKobo]).Split(' ');
                            foreach (var a in splitList)
                                listAux.Add(MapValue(a, param.FormGroupSelect, allMapValues));
                            props.Add(param.NameDB, listAux);
                            break;
                        default:
                            props.Add(param.NameDB, valueTemp);
                            break;
                    }
                }
                else
                {
                    props.Add(param.NameDB, null);
                }
                
            }
            dataItem.State = state;
        }
        private static void LoadAquacultureFields(dynamic result, KoGenericData dataItem,
            List<AquacultureField> fieldsParent, Dictionary<int?, IGrouping<int?, AquacultureField>> fieldsChild,
            Dictionary<string, Dictionary<string, string>> allMapValues)
        {
            var props = dataItem.DynamicProperties;

            foreach (var param in fieldsParent)
            {
                if (result[param.NameKobo] != null)
                {
                    switch (param.Type)
                    {
                        case AquacultureField.TYPE_TEXT:
                        case AquacultureField.TYPE_LOCATION:
                        case AquacultureField.TYPE_GEO:
                            props.Add(param.NameDB, (String)result[param.NameKobo]);
                            break;
                        case AquacultureField.TYPE_SELECT_ONE:
                            props.Add(param.NameDB, MapValue((String)result[param.NameKobo], param.GroupMap, allMapValues));
                            break;
                        case AquacultureField.TYPE_SELECT_MULTIPLE:
                            var listAux = new List<string>();
                            var splitList = ((String)result[param.NameKobo]).Split(' ');
                            foreach (var a in splitList)
                                listAux.Add(MapValue(a, param.GroupMap, allMapValues));
                            props.Add(param.NameDB, listAux);
                            break;
                        case AquacultureField.TYPE_GROUP:
                            var arrayGroup = new List<Dictionary<string, object>>();
                            foreach (var subresul in result[param.NameKobo])
                            {
                                var subProp = new Dictionary<string, object>();
                                foreach (var subparam in fieldsChild[param.Id])
                                {
                                    if (subresul[subparam.NameKobo] != null)
                                    {
                                        var valueAux = (String)subresul[subparam.NameKobo];
                                        switch (subparam.Type)
                                        {
                                            case AquacultureField.TYPE_TEXT:
                                            case AquacultureField.TYPE_LOCATION:
                                                subProp.Add(subparam.NameDB, valueAux);
                                                break;
                                            case AquacultureField.TYPE_SELECT_ONE:
                                                subProp.Add(subparam.NameDB, MapValue(valueAux, subparam.GroupMap, allMapValues));
                                                break;
                                            case AquacultureField.TYPE_SELECT_MULTIPLE:
                                                var sublistAux = new List<string>();
                                                var subSplitList = valueAux.Split(' ');
                                                foreach (var b in subSplitList)
                                                    sublistAux.Add(MapValue(b, subparam.GroupMap, allMapValues));
                                                subProp.Add(subparam.NameDB, sublistAux);
                                                break;
                                        }
                                    }
                                    else { subProp.Add(subparam.NameDB, null); }
                                }
                                arrayGroup.Add(subProp);
                            }
                            props.Add(param.NameDB, arrayGroup);
                            break;
                        case AquacultureField.TYPE_ID:
                            //dataItem.IdKobo = (String)result[param.NameKobo];
                            break;
                        case AquacultureField.TYPE_USER:
                            //dataItem.User = (String)result[param.NameKobo];
                            break;
                    }
                }
                else { props.Add(param.NameDB, null); }
            }
            props["formato"] = 1;
        }


        //Consulta información desde KOBO
        private static async Task<dynamic> GetDataFromUrl(string url, string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            dynamic data = JsonConvert.DeserializeObject(responseBody);

            return data;
            
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
