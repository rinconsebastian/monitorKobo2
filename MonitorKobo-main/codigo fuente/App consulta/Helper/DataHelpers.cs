using System;
using System.Collections.Generic;
using System.Globalization;

namespace App_consulta.Helper
{
    public static class DataHelpers
    {
        public static string[] DatetimeToString(object dateObject = null)
        {
            var r = new string[] { "", "", "" };
            if(dateObject != null)
            {
                var date = (String)dateObject;
                r[0] = date.Substring(0, 2);
                r[1] = date.Substring(3, 2);
                r[2] = date.Substring(6, 4);
            }
            return r;
        }
        public static string[] DateToString(object dateObject = null)
        {
            var r = new string[] { "", "", "" };
            if (dateObject != null)
            {
                var date = (String)dateObject;
                r[2] = date.Substring(0, 4);
                r[1] = date.Substring(5, 2);
                r[0] = date.Substring(8, 2);
            }
            return r;
        }
        public static string[] DecimalToDMS(string latS = null, string lonS = null)
        {
            var r = new string[] { "", "", "", "", "", "", "", "" };

            double value, sec;
            int deg,min;

            if(latS != null)
            {
                var lat = double.Parse(latS, CultureInfo.InvariantCulture);

                value = Math.Abs(lat);
                deg = (int)value;
                r[0] = deg + "°";

                value -= deg;
                min = (int)(value * 60);
                r[1] = min + "'";

                sec = Math.Round(((value * 60) - min) * 60, 3);
                r[2] = sec + "\"";

                r[3] = lat >= 0 ? "N" : "S";
            }

            if (lonS != null)
            {
                var lon = double.Parse(lonS, CultureInfo.InvariantCulture);
                value = Math.Abs(lon);
                deg = (int)value;
                r[4] = deg + "°";

                value -= deg;
                min = (int)(value * 60);
                r[5] = min + "'";

                sec = Math.Round(((value * 60) - min) * 60, 4);
                r[6] = sec + "\"";

                r[7] = lon >= 0 ? "E" : "W";
            }
            return r;
        }    
        public static string SelectToString(int type, object data = null, object other = null)
        {
            var r = "";
            if(type == 2)
            {
                if(other != null)
                    r = (String)other;
                else
                    r = data != null ? (String)data : "";
            }
            else if(type == 3)
            {
                var list = new List<String>();
                if(data!= null)
                {
                    var arr = (List<Object>)data;
                    foreach(var item in arr)
                    {
                        if (other != null && (String)item == "Otro") 
                            continue;
                        list.Add((String)item);
                    }
                }
                if (other != null)
                    list.Add((String)other);
                r = String.Join(", ", list);
            }
            else
            {
                r = data != null ? (String)data : "";
            }
            return r;
        }
        public static string StringDoubleFormat(string value = null, int precision = 2)
        {
            var r = "";
            if (value != null)
            {
                try
                {
                    var dec = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                    r = Math.Round(dec, precision).ToString();
                }
                catch (Exception) { }
            }
            return r;
        }   
        public static string StringNumberFormat(object value = null)
        {
            var r = value != null ? (String)value : "";
            return r == "0" ? "" : r;
        }    
        public static List<Dictionary<string,object>> GroupFormat(object value = null)
        {
            var r = new List<Dictionary<string, object>>();
            if(value != null)
            {
                foreach(var item in (List<Object>)value)
                {
                    r.Add((Dictionary<string, object>)item);
                }
            }
            return r;
        }
    }
}
