using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entities;
using BisnessLogic;

namespace SRAM.Controllers
{
    public class PendingController : Controller
    {
        /// <summary>
        /// Muestra una lista con los pendientes a auditar.
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Index()
        {
            List<Auditoria> pendingAudits = new AuditoriaBusiness().GetPendingAudit("", "");
            return View(pendingAudits);
        }
	}
}