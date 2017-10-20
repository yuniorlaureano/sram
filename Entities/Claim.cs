using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Entities
{
    public class Claim
    {
        public string ClaimNumber { get; set; }
        public string Book { get; set; }
        public string ClaimDescription { get; set; }
        public string ClientComment { get; set; }
        public string CanvCode { get; set; }
    }
}