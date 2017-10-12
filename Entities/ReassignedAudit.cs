using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Entities
{
    public class ReassignedAudit
    {
        public int AssignmentId { get; set; }
        public string Auditor { get; set; }
        public string UserCode { get; set; }
    }
}