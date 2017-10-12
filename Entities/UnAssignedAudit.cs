using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Entities
{
    public class UnAssignedAudit
    {
        public decimal Cantidad { get; set; }
        public string SalesDate { get; set; }
    }
}