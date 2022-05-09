using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Controllers
{
    public static class SessionExtensions
    {

        public static void SetComplex(this ISession session, string key, object value)
        {
            session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
        }

        public static T GetComplex<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : System.Text.Json.JsonSerializer.Deserialize<T>(value);
        }


        public static T CloneObject<T>(this object source)
        {
            T result = Activator.CreateInstance<T>();
            //// **** made things  
            return result;
        }

    }

}

