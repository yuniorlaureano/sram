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
    public class AuditoryController : Controller
    {
        /// <summary>
        /// Permite obtener las auditorias pendientes.
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Index()
        {
            List<Auditoria> pendingAudits = new AuditoriaBusiness().GetPendingAudit("", "");
            return View(pendingAudits);
        }

        /// <summary>
        /// Permite reasignar una cuanta.
        /// </summary>
        /// <param name="input">Input model con la data para realizar la reasignacion</param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        public JsonResult ReAssign(ReassignInput input)
        {
            bool reasignacionNoRealizada = false;
            if (input.Assigments != null)
            {
                reasignacionNoRealizada = new AuditoriaBusiness().ReAssignAudit(input.Assigments, input.User, Convert.ToInt32(Session["UserCode"]));
            }
            else
            {
                return Json(new { WARNING = "Las asignaciones estan llegando vacias." }, JsonRequestBehavior.AllowGet);
            }

            if (!reasignacionNoRealizada)
            {
                return Json(new { ERROR = "Error al realizar la reasignacion." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { OK = "Reasignacion realizada." }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Permite auditar una cuenta.
        /// </summary>
        /// <param name="auditResult">Input model con la data para realizar la auditoria</param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        public JsonResult Audit(Entities.AuditResult auditResult)
        {
            auditResult.UserCode = Session["UserCode"].ToString();
            bool resultset = new AuditoriaBusiness().Auditar(auditResult.AssignmentId, auditResult.AccountId, auditResult.AuditorComments,
                auditResult.Answers.ToArray(), auditResult.UserCode, auditResult.IsDescargaAdministrativo, auditResult.IsDescargarReauditoria);

            if (resultset)
            {
                return Json(new { OK = "Cuanta auditada exitosamente." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { ERROR = "La cuenta no se pudo auditar." }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Obtiene los detalles que ve el auditor para poder realizar la auditoria.
        /// </summary>
        /// <param name="AssignmentId">Id de la asignacion, represanta la cuanta que se esta auditando</param>
        /// <returns>JsonResult</returns>
        public JsonResult GetTransactionInfoByAssignment(string AssignmentId)
        {
            List<Entities.PenddingAudit> pendingAudit = new AuditoriaBusiness().GetAcctInfoByAssignment(AssignmentId);

            return Json(new { transaccion = pendingAudit }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Permite obtener las lista de la auditorias realizadas.
        /// </summary>
        /// <param name="SubscrId">Id de cliente</param>
        /// <param name="Auditor">Quien realiza la auditoria</param>
        /// <param name="SalesDate">La fecha de venta</param>
        /// <param name="CreationDate">Fecha de creacion</param>
        /// <param name="CallId">Id de la llamada</param>
        /// <param name="PhoneNo">Numero telefonico</param>
        /// <returns>ViewResult</returns>
        [HttpGet]
        public ViewResult SearchAuditedAccount()
        {
            ViewBag.input = new InputSearchAuditory();
            ViewBag.Error = null;
            ViewBag.doneAudit = true;
            return View(new List<Auditoria>());
        }

        /// <summary>
        /// Permite obtener las lista de la auditorias realizadas.
        /// </summary>
        /// <param name="SubscrId">Id de cliente</param>
        /// <param name="Auditor">Quien realiza la auditoria</param>
        /// <param name="SalesDate">La fecha de venta</param>
        /// <param name="CreationDate">Fecha de creacion</param>
        /// <param name="CallId">Id de la llamada</param>
        /// <param name="PhoneNo">Numero telefonico</param>
        /// <returns>ViewResult</returns>
        [HttpPost]
        public ActionResult SearchAuditedAccount(InputSearchAuditory input)
        {
            IEnumerable<Entities.Auditoria> transaccion = null;
            bool isEnyEmpty = (string.IsNullOrEmpty(input.SubscrId) && string.IsNullOrEmpty(input.Auditor) && string.IsNullOrEmpty(input.SalesDate) &&
                string.IsNullOrEmpty(input.CreationDate) && string.IsNullOrEmpty(input.CallId) && string.IsNullOrEmpty(input.PhoneNo));
            ViewBag.doneAudit = true;
            if (isEnyEmpty)
            {
                ViewBag.input = input;
                ViewBag.Error = "Debe proveer al menos uno de los campos requeridos.";
                return View(new List<Auditoria>());
            }
            else
            {
                transaccion = new AuditoriaBusiness().GetDoneAudits(input.SubscrId, input.Auditor, input.SalesDate, input.CreationDate, input.CallId, input.PhoneNo);
            }

            ViewBag.input = input;
            ViewBag.Error = null;
            return View(transaccion);
        }

        /// <summary>
        /// Permite eliminar una auditoria, solo es realizada por un administrador.
        /// </summary>
        /// <param name="AuditId"></param>
        /// <param name="UserCode"></param>
        /// <returns></returns>
        public JsonResult DeleteAudit(int? AuditId)
        {
            string role = HttpContext.Session["grp_codigo"].ToString();

            bool resulset = false;

            if (role == "adm")
            {
                resulset = new AuditoriaBusiness().DeleteAudit(AuditId, Session["UserCode"].ToString());
            }

            if (resulset)
            {
                return Json(new { OK = "Auditoria eliminada correctamete." }, JsonRequestBehavior.AllowGet);    
            }

            return Json(new { ERROR = "Error al eliminar la auditoria." }, JsonRequestBehavior.AllowGet);
        }
	}
}