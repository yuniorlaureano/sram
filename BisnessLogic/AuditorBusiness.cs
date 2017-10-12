using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using DataAccess;


namespace BisnessLogic
{
    public class AuditorBusiness
    {
        public List<Auditors> GetAuditors()
        {
            return new AuditorData().GetAuditors();
        }
    }
}
