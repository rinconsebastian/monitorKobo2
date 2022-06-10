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
        public async Task<RespuestaAccion> ResetImage(string filename, string idKobo, int idProject, string record = "", string name = "")
        {
            var r = new RespuestaAccion();

            try
            {
                var _fileName = Path.GetFileName(filename);

                var config = await db.KoProject.FindAsync(idProject);
                var remoteUri = config.KoboAttachment + config.KoboUsername + "/attachments/";
                var nameProject = config.Name;
                var _path = Path.Combine(_env.ContentRootPath, "Storage", nameProject, idKobo);

                DownloadFile(remoteUri, _fileName, _path, "", config.KoboApiToken);

                r.Success = true;

                var log = new Logger(db);
                var registro = new RegistroLog { Usuario = User.Identity.Name, Accion = "ResetImage", Modelo = nameProject + " " + record, ValAnterior = "", ValNuevo = name + ": " + filename };
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

        // EXTRA

        private async Task<string> LoadDataFromKobo(KoProject project)
        {
            var maxId = await mdb.MaxIdKobo(project.Field);

            var listParams = await db.KoField.Where(n => n.IdProject == project.Id && n.Validable == false)
                .Select(n => n.Value).ToArrayAsync();

            var mapParams = await db.KoField.Where(n => n.IdProject == project.Id && n.Validable == false)
                .ToDictionaryAsync(n => n.Field, n => n.Value);

            //Url de consulta para kobo
            var fields = JsonConvert.SerializeObject(listParams);
            var sort = "&sort=%7B%22_id%22%3A1%7D";
            var limit = "&limit=10";
            var query = maxId != null ? "&query=%7B%22_id%22%3A%7B%22%24gt%22%3A" + maxId + "%7D%7D" : "";
            var url = project.KoboKpiUrl + "/assets/" + project.KoboAssetUid
                + "/submissions/?format=json&fields=" + HttpUtility.UrlEncode(fields)
                + sort + query + limit;

            string resp = "";
            try
            {
                //Consulta la información
                var encuestas = await GetDataFromUrl(url, project.KoboApiToken, mapParams);

                //Guarda la información en MongoDB
                if (encuestas.Count == 0)
                {
                    resp = "ALERTA: No se encontraron nuevas encuestas de " + project.Name + ".";
                }
                else
                {
                    mdb.InsertMany(project.Field, encuestas);
                    resp = "EXITO: Se cargaron " + encuestas.Count + " encuestas de " + project.Name + ".";
                }               
            }
            catch (HttpRequestException e)
            {
                resp = "ERROR: " + e.Message;
            }
            return resp;
        }

        private static async Task<List<KoGenericData>> GetDataFromUrl(string url, string token, Dictionary<string, string> mapParams)
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

                foreach (var param in mapParams)
                {
                    switch (param.Key)
                    {
                        case "kobo_id":
                            dataItem.IdKobo = (String)result[param.Value];
                            break;
                        case "user":
                            var user = result[param.Value];
                            dataItem.User = user != null ? (int)user : 0;
                            break;
                        default:
                            props.Add(param.Key, (String)result[param.Value]);
                            break;
                    }
                }

                dataItem.State = 1;
                dataItem.DynamicProperties = props;
                encuestas.Add(dataItem);
            }
            return encuestas;
        }


        private string DownloadFile(string remoteUri, string fileName, string path, string relative, String token)
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

    }
}
