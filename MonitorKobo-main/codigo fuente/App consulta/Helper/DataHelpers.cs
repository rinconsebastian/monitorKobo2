using System;
using System.Globalization;

namespace App_consulta.Helper
{
    public static class DataHelpers
    {
        public static string[] DateToString(object dateObject = null)
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
    }
}
