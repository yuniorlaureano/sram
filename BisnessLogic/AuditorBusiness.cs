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
        public Auditors GetAuditorsCredentials(string userName)
        {
            return new AuditorData().GetAuditorsCredentials(userName);
        }

        public List<Auditors> GetAuditors()
        {
            return new AuditorData().GetAuditors();
        }
    }
}
