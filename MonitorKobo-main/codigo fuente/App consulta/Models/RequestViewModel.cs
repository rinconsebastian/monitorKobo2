

using System;

namespace App_consulta.Models
{
    public class RequestViewModel
    {
        public int Id { get; set; }
        public string Request { get; set; }
        public int State { get; set; }
        public int FormalizationId { get; set; }
        public string FormalizationNumber { get; set; }
        public string File { get; set; }
        public string AlertUser { get; set; }
        public string AlertAdmin { get; set; }
        public string IdUser { get; set; }
        public string NameUser { get; set; }
        public string AdminName { get; set; }
        public DateTime CreateDate { get; set; }
        public string ValidationDate { get; set; }
    }
}
