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

        [BsonElement("Id Kobo")]
        public string IdKobo { get; set; }

        [BsonElement("Fecha solicitud")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string FechaSolicitud { get; set; }

        [BsonElement("Estado")]
        public int Estado { get; set; }

        [BsonElement("Coordinación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public int IdResponsable { get; set; }
        [ForeignKey("IdResponsable")]
        [JsonIgnore]
        public virtual Responsable Responsable { get; set; }

        [BsonElement("Creado por")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string IdCreateByUser { get; set; }

        [ForeignKey("IdCreateByUser")]
        [JsonIgnore]
        public virtual ApplicationUser CreateByUser { get; set; }

        [BsonElement("Última edición por")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string IdLastEditByUser { get; set; }

        [ForeignKey("IdLastEditByUser")]
        [JsonIgnore]
        public virtual ApplicationUser LastEditByUser { get; set; }

        [BsonElement("Fecha creación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public DateTime CreateDate { get; set; }

        [BsonElement("Fecha última edición")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public DateTime LastEditDate { get; set; }

        [BsonExtraElements]
        public IDictionary<string, object> DynamicProperties { get; set; }
    }
}
