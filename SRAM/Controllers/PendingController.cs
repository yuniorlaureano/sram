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
        /// <returns>ViewResult</returns>
        public ViewResult Index()
        {
            List<Auditoria> pendingAudits = new AuditoriaBusiness().GetPendingAudit("", "");
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
            var auditoriasPendientes = new AuditoriaBusiness().GetPendingAudit(userCode, salesDate);
            return View("Index",auditoriasPendientes);
        }
	}
}