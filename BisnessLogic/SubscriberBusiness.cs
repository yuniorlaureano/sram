using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Entities;

namespace BisnessLogic
{
    public class SubscriberBusiness
    {
        public List<SubscriberClaimWithCredit> GetSubscriberClaimsWithCredit(int SubscrId, int CanvEdition, string CanvCode, string book)
        {
            return new SubscriberData().GetSubscriberClaimsWithCredit(SubscrId, CanvEdition, CanvCode, book);
        }

        public List<SubscriberClaimWithClaims> GetSubscriberClaimsWithClaims(int SubscrId, int CanvEdition, string CanvCode, string book)
        {
            return new SubscriberData().GetSubscriberClaimsWithClaims(SubscrId, CanvEdition, CanvCode, book);
        }

        public List<SubscriberCanvBook> GetSubscribersCanvBooks(int SubscrId, string CanvCode, int CanvEdition)
        {
            return new SubscriberData().GetSubscribersCanvBooks(SubscrId, CanvCode, CanvEdition);
        }
    }
}
