

using System;

namespace App_consulta.Models
{
    public class RequestViewModel
    {
        public int Id { get; set; }
        public string Request { get; set; }
        public int State { get; set; }

        public string RecordId { get; set; }
        public int RecordProject { get; set; }
        public string RecordNumber { get; set; }

        public string File { get; set; }
        public string AlertUser { get; set; }
        public string AlertAdmin { get; set; }
        public string AlertEmail { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string AdminName { get; set; }

        public DateTime CreateDate { get; set; }
        public string ValidationDate { get; set; }

        public string Message { get; set; }
    }
}
