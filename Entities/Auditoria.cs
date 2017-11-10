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
        private string fechaRPC;
        public string FechaRPC 
        {
            get
            { 
                return fechaRPC; 
            } 
            set 
            {
                fechaRPC = (value != null ? Convert.ToDateTime(value).ToString("MM/dd/yyyy"): value);
            }
        }

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
        private string auditCreationDate;
        public string AuditCreationDate
        {
            get
            {
                return auditCreationDate;
            }
            set
            {
                auditCreationDate = (value != null ? Convert.ToDateTime(value).ToString("MM/dd/yyyy") : value);
            }
        }

        public string AuditorName { get; set; }
        public string AuditResult { get; set; }
        public string InvalidQuestions { get; set; }
        public bool HassClaim { get; set; }
        public bool HassCredit { get; set; }

    }
}