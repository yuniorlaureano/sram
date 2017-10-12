using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Entities
{
    public class AuditResult
    {
        public int AssignmentId { get; set; }
        public int AccountId { get; set; }
        public string AuditorComments { get; set; }
        public List<int> Answers { get; set; }
        public string UserCode { get; set; }
        public byte IsDescargaAdministrativo { get; set; }
        public byte IsDescargarReauditoria { get; set; }
    }
}