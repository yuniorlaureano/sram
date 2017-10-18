using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SRAM.Models
{
    public class InputSearchAuditory
    {
        public string SubscrId{ get; set; } 
        public string Auditor{ get; set; }
        public string SalesDate{ get; set; }
        public string CreationDate{ get; set; }
        public string CallId { get; set; } 
        public string PhoneNo { get; set; }
    }
}