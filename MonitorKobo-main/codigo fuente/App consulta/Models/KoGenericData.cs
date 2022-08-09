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
        public const int ESTADO_NUEVO = 1;
        public const int ESTADO_PENDIENTE = 2;
        public const int ESTADO_BORRADOR = 3;
        public const int ESTADO_COMPLETO = 4;
        public const int ESTADO_CANCELADO = 5;
        public const int ESTADO_IMPRESO = 6;
        public const int ESTADO_CARNET_VIGENTE = 7;
        public const int ESTADO_DUPLICADO = 8;

        [BsonId]
        [JsonIgnore]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("kobo_id")]
        [BsonIgnoreIfNull]
        public string IdKobo { get; set; }

        [Display(Name = "Estado")]
        [BsonElement("state")]
        [BsonIgnoreIfNull]
        public int State { get; set; }

        [BsonElement("user")]
        [BsonIgnoreIfNull]
        public string User { get; set; }

        [BsonExtraElements]
        public IDictionary<string, object> DynamicProperties { get; set; }
    }

    public class KoExtendData : KoGenericData
    {     

        [BsonElement("dependence")]
        [Display(Name = "Coordinación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public int IdResponsable { get; set; }
       
        [BsonElement("create_user")]
        [Display(Name = "Creado por")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string IdCreateByUser { get; set; }

        [BsonElement("edit_user")]
        [Display(Name = "Última edición por")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string IdLastEditByUser { get; set; }


        [BsonElement("create_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [Display(Name = "Fecha creación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public DateTime CreateDate { get; set; }

        [BsonElement("edit_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [Display(Name = "Fecha última edición")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public DateTime LastEditDate { get; set; }

    }

    public class KoDataViewModel : KoGenericData
    {
        public KoDataViewModel() { }

        public KoDataViewModel(string Id, string IdKobo, int State, string User, int IdResponsable, string IdLastEditByUser, DateTime LastEditDate)
        {
            this.Id = Id;
            this.IdKobo = IdKobo;
            this.State = State;
            this.User = User;
            this.IdResponsable = IdResponsable;
            this.IdLastEditByUser = IdLastEditByUser;
            this.LastEditDate = LastEditDate;

            // var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(DynamicProperties);
            // Newtonsoft.Json.JsonConvert.DeserializeObject<IDictionary<string, object>>(serialized);
            this.DynamicProperties = new Dictionary<string, object>(); 
        }

        [BsonElement("dependence")]
        [Display(Name = "Coordinación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public int IdResponsable { get; set; }

        public Dictionary<string, string> Props { get; set; }


        [BsonElement("edit_user")]
        public string IdLastEditByUser { get; set; }

        [BsonElement("edit_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastEditDate { get; set; }

    }
}
