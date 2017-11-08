using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Entities
{
    public class UnAssignedAudit
    {        
        private string fechaVenta;
        public string RazonSocial { get; set; }
        public string SubscriberId { get; set; }
        public string SubscriberName { get; set; }
        public string Telefono { get; set; }
        public string Canvass { get; set; }
        public string CanvEdition { get; set; }
        public string BookCodeDetail { get; set; }
        public string Venta { get; set; }
        public string Ejecutivo { get; set; }
        public string NombreEJecutivo { get; set; }
        public string Unidad { get; set; }        
        public string Cargo { get; set; }
        public string CallId { get; set; }
        public string Comentario { get; set; }
        public string PDCDimmas { get; set; }
        public string PDCCodetel { get; set; }
        public string AccountId { get; set; }
        public bool HassClaim { get; set; }
        public bool HassCredit { get; set; }

        public string FechaVenta { 
            get
            {
                return fechaVenta;
            }
            set
            {
                if (value != null)
                {
                    this.fechaVenta = Convert.ToDateTime(value).ToString("MM/dd/yyyy");
                }
            } 
        }
    }
}