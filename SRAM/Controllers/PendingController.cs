using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entities;
using BisnessLogic;
using SRAM.Models;

namespace SRAM.Controllers
{
    public class PendingController : Controller
    {
        /// <summary>
        /// Muestra una lista con los pendientes a auditar.
        /// </summary>
        /// <returns>ViewResult</returns>
        public ActionResult Index()
        {
            ViewBag.input = new InputSearchAuditory();
            ViewBag.Error = null;
            ViewBag.doneAudit = false;
            List<Auditoria> pendingAudits = null;
            string role = Session["Grp_Codigo"].ToString();

            if (role.Equals("adm"))
            {
                pendingAudits = new AuditoriaBusiness().GetPendingAudit("", "");
                return View(pendingAudits);
            }

            pendingAudits = new AuditoriaBusiness().GetPendingAudit(Session["UserCode"].ToString(), "");
            return View(pendingAudits);
            
        }

        /// <summary>
        /// Muestra una lista con los pendientes a auditar. Filtrando por artista y por fecha.
        /// </summary>
        /// <param name="userCode">Codigo del usuario logueado</param>
        /// <param name="salesDate">Fecha de venta</param>
        /// <returns>ViewResult</returns>
        [HttpPost]
        public ViewResult GetPendingAud(string userCode, string salesDate)
        {
            ViewBag.input = new InputSearchAuditory();
            ViewBag.Error = null;
            ViewBag.doneAudit = false;
            var auditoriasPendientes = new AuditoriaBusiness().GetPendingAudit(userCode, salesDate);
            return View("Index",auditoriasPendientes);
        }
	}
}