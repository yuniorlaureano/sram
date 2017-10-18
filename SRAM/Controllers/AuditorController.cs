using BisnessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SRAM.Controllers
{
    public class AuditorController : Controller
    {
        //
        // GET: /Auditor/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Permite obtener los auditores, desde sql server, para la app de sra
        /// </summary>
        /// <returns>JsonResult</returns>
        public JsonResult GetAuditors()
        {
            var auditors = new AuditorBusiness().GetAuditors();
            return Json(new { auditors = auditors }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Obtiene un resume de los auditorias asignadas que tiene cada auditor
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="salesDate"></param>
        /// <returns>JsonResult</returns>
        public JsonResult GetPendingAudResume(string userCode, string salesDate)
        {
            List<Entities.AuditoriaResume> peddingResume = new AuditoriaBusiness().GetPendingAuditResume(userCode, salesDate);
            return Json(new { pending = peddingResume }, JsonRequestBehavior.AllowGet);
        }
	}
}