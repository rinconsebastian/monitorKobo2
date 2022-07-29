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
                var keys = value != null ? (List<object>)value : new List<object>();
                var control = 0;
                foreach (var item in options)
                {
                    item.Selected = false;
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
                var control = 0;
                foreach (var item in options)
                {
                    item.Selected = false;
                    if (item.Key == key)
                    {
                        item.Selected = true;
                        control++;
                    }
                }
                alert = control != 1;
            }

            ViewBag.Class = css +(alert ? " alerta" : "");
            ViewBag.Inner = inner;

            return View(options);
        }
    }
}
