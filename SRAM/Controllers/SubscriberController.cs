using BisnessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entities;

namespace SRAM.Controllers
{
    public class SubscriberController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Permite obtener la informacion de credito del subscriber
        /// </summary>
        /// <param name="SubscrId">cliente</param>
        /// <param name="CanvEdition">edicion de la camplania</param>
        /// <param name="CanvCode">campania</param>
        /// <param name="Book">libro</param>
        /// <returns>List<Credit></returns>
        public JsonResult GetCredits(int SubscrId, int CanvEdition, string CanvCode, string Book)
        {
            List<Credit> credits = new SubscriberBusiness().GetCredit(SubscrId, CanvEdition, CanvCode, Book);
            return Json(new { credit = credits }, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// Permite obtener la infromacion de reclamo de un cliente.
        /// </summary>
        /// <param name="SubscrId">cliente</param>
        /// <param name="CanvEdition">edicon de la campania</param>
        /// <param name="CanvCode">campania</param>
        /// <param name="Book">libro</param>
        /// <returns>List<Claim></returns>
        public JsonResult GetClaims(int SubscrId, int CanvEdition, string CanvCode, string Book)
        {
            List<Claim> claims = new SubscriberBusiness().GetClaims(SubscrId, CanvEdition, CanvCode, Book);
            return Json(new { claim = claims }, JsonRequestBehavior.AllowGet);
        }
	}
}