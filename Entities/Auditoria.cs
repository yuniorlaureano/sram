using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Entities
{
    public class Auditoria
    {
        public string AssignmentId { get; set; }
        public string AuditId { get; set; }
        public string RazonSocial { get; set; }
        public string SubscriberId { get; set; }
        public string Telefono { get; set; }
        public string Canvass { get; set; }
        public string CanvEdition { get; set; }
        public string BookCodeDetail { get; set; }
        public string Venta { get; set; }
        public string Ejecutivo { get; set; }
        public string Unidad { get; set; }
        public string FechaRPC { get; set; }
        public string Cargo { get; set; }
        public string CallId { get; set; }
        public string ControlVerballCallId { get; set; }
        public string ComentarioEjecutivo { get; set; }
        public string ComentarioAuditor { get; set; }
        public string PDCDimmas { get; set; }
        public string PDCCodetel { get; set; }
        public string Compania { get; set; }
        public string TipoDeServicio { get; set; }
        public string AuditorAsignado { get; set; }
        public string PrevioCallId { get; set; }
        public string CustSource { get; set; }
        public string AccountId { get; set; }
        public string Status { get; set; }
        public string AuditCreationDate { get; set; }
        public string AuditorName { get; set; }
        public string AuditResult { get; set; }
        public string InvalidQuestions { get; set; }
        public bool HassClaim { get; set; }
        public bool HassCredit { get; set; }

    }
}