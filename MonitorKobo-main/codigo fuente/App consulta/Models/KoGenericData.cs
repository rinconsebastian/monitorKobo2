using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace App_consulta.Models
{
    public class KoGenericData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("kobo_id")]
        public string IdKobo { get; set; }

        [BsonElement("state")]
        public int State { get; set; }

        [BsonElement("user")]
        public int User { get; set; }

        [BsonExtraElements]
        public IDictionary<string, object> DynamicProperties { get; set; }
    }
}
