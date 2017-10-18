using BisnessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SRAM.Controllers
{
    public class AssignController : Controller
    {
        /// <summary>
        /// Permite obtener las cuentas pendientes en asignar.
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Index()
        {
            ViewBag.totalAsignado = 0;
            List<Entities.UnAssignedAudit> unAssignedAudits = new AuditoriaBusiness().GetUnAssignedAudit();
            return View(unAssignedAudits);
        }

        /// <summary>
        /// Permite asignar cuentas a usuarios.
        /// </summary>
        /// <param name="date">Fechas de las asignaciones</param>
        /// <param name="auditor">Usuarios a quienes le seran asignadas las cuentas</param>
        /// <returns>ViewResult</returns>
        [HttpPost]
        public ViewResult AssignAudit(string date, string auditor)
        {
            ViewBag.totalAsignado = new AuditoriaBusiness().AssignAudit(date, auditor, Session["UserCode"].ToString()); ;
            List<Entities.UnAssignedAudit> unAssignedAudits = new AuditoriaBusiness().GetUnAssignedAudit();
            return View("Index", unAssignedAudits);
        }
	}
}