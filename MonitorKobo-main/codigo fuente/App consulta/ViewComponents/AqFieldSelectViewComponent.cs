using App_consulta.Data;
using App_consulta.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.ViewComponents
{
    public class AqFieldSelectViewComponent : ViewComponent
    {
        public AqFieldSelectViewComponent(){}

        public IViewComponentResult Invoke(object value, List<AqFieldSelectViewModel> options, string css = "", bool multiple = false, bool inner = false)
        {
            bool alert;
            if (multiple)
            {
                var keys = value != null ? (List<string>)value : new List<string>();
                var control = 0;
                foreach (var item in options)
                {
                    if (keys.Contains(item.Key))
                    {
                        item.Selected = true;
                        control++;
                    }
                }
                alert = keys.Count != control;
            }
            else
            {
                var key = value != null ? (String)value : "";
                alert = true;
                foreach(var item in options)
                {
                    if(item.Key == key)
                    {
                        item.Selected = true;
                        alert = false;
                        break;
                    }
                }
            }

            ViewBag.Class = css +(alert ? " alerta" : "");
            ViewBag.Inner = inner;

            return View(options);
        }
    }
}
