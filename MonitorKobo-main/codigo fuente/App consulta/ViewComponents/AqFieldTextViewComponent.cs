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
    public class AqFieldTextViewComponent : ViewComponent
    {
        public AqFieldTextViewComponent() { }

        public IViewComponentResult Invoke(object value, int columns = 0, string css = "", string title = "", bool number = false, Dictionary<string, string> locations = null, string extra = "")
        {
            var text = "";
            if (value != null)
            {
                text = (String)value;
                if (locations != null)
                {
                    text = locations.ContainsKey(text + extra) ? locations[text + extra] : text;
                }
            }

            if (text == "" && title != "")
            {
                text = title;
                css = "titled";
            }

            ViewBag.SeparateNumbers = number && text.Length <= columns;

            ViewBag.Text = text;
            ViewBag.Columns = columns;
            ViewBag.Title = title;
            ViewBag.Class = css;

            return View();
        }
    }
}
