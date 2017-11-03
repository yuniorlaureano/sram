using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
//using SRAM.Models;
using BisnessLogic;
using Entities;


namespace SRAM.Controllers
{
    public class AssingmentController : Controller
    {
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //public JsonResult GetAuditors()
        //{
        //    var auditors = new AuditorBusiness().GetAuditors();

        //    return Json(new { auditors = auditors }, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult unassignedAccounts()
        //{
        //    List<Entities.UnAssignedAudit> unAssignedAudit = new AuditoriaBusiness().GetUnAssignedAudit();
            
        //    return Json(new { peddingAssignment = unAssignedAudit }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult AssignAudit(string salesDate, string selectedAuditor,string userCode)
        //{
        //    int totalAsignedCount = 0;
        //    try
        //    {
        //        totalAsignedCount = new AuditoriaBusiness().AssignAudit(salesDate, selectedAuditor, userCode);
        //    }
        //    catch (ArgumentNullException)
        //    {
        //        return Json(new { ERROR = "Campos Requeridos: Fecha Y Auditor." }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { total = totalAsignedCount }, JsonRequestBehavior.AllowGet);

        //}

        //public JsonResult GetPendingAudResume(string userCode, string salesDate) 
        //{
        //    List<Entities.AuditoriaResume> peddingResume = new AuditoriaBusiness().GetPendingAuditResume(userCode, salesDate);

        //    return Json(new { pending = peddingResume }, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetPendingAud(string userCode, string salesDate)
        //{
       
        //    var auditoriasPendientes = new AuditoriaBusiness().GetPendingAudit(userCode, salesDate);

        //    return Json(new { pending = auditoriasPendientes }, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetSubscrClaimWithCredit(int SubscrId, int CanvEdition, string CanvCode, string book)
        //{
        //    List<Entities.Credit> subscriberWithCredit = new SubscriberBusiness().GetCredit(SubscrId, CanvEdition, CanvCode, book);

        //    return Json(new { credit = subscriberWithCredit }, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetSubscrClaimWithClaims(int SubscrId, int CanvEdition, string CanvCode, string book)
        //{
        //    List<Entities.Claim> subscriberClaimsWithClaims = new SubscriberBusiness().GetClaims(SubscrId, CanvEdition, CanvCode, book);

        //    return Json(new { claim = subscriberClaimsWithClaims }, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetSubscrCanvBooks(int SubscrId, string CanvCode, int CanvEdition)
        //{
        //    List<Entities.SubscriberCanvBook> subscriberCanvAndBook = new SubscriberBusiness().GetSubscribersCanvBooks(SubscrId, CanvCode, CanvEdition);

        //    return Json(new { subscriberBook = subscriberCanvAndBook }, JsonRequestBehavior.AllowGet);
        //}

        ////[HttpPost]
        ////public JsonResult ReAssign(List<Entities.ReassignedAudit> reassignedAudit)
        ////{
        ////    bool resultset = false;

        ////    resultset = new AuditoriaBusiness().ReAssignAudit(reassignedAudit);

        ////    if (!resultset)
        ////    {
        ////        return Json("ERROR", JsonRequestBehavior.AllowGet);
        ////    }

        ////    return Json("DONE", JsonRequestBehavior.AllowGet);
        ////}

        //public JsonResult GetAcctInfoByAssignment(string AssignmentId)
        //{
        //    List<Entities.PenddingAudit> pendingAudit = new AuditoriaBusiness().GetAcctInfoByAssignment(AssignmentId);

        //    return Json(new { transaccion = pendingAudit }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult Audit(AuditResult auditResult)
        //{

        //    bool resultset = new AuditoriaBusiness().Auditar(auditResult.AssignmentId, auditResult.AccountId, auditResult.AuditorComments,
        //        auditResult.Answers.ToArray(), auditResult.UserCode, auditResult.IsDescargaAdministrativo, auditResult.IsDescargarReauditoria);

        //    return Json(new { RESUSLTSET = resultset ? "DONE" : "ERROR" }, JsonRequestBehavior.AllowGet);
        //}


        //public JsonResult GetDoneAudits(string SubscrId, string Auditor, string SalesDate, string CreationDate, string CallId, string PhoneNo)
        //{

        //    IEnumerable<Entities.Auditoria> transaccion = null;

        //    if (string.IsNullOrEmpty(SubscrId) && string.IsNullOrEmpty(Auditor) && string.IsNullOrEmpty(SalesDate) && string.IsNullOrEmpty(CreationDate)
        //        && string.IsNullOrEmpty(CallId) && string.IsNullOrEmpty(PhoneNo))
        //    {
        //        return Json(new { ERROR = "Debe proveer algun parametro de busqueda" }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        transaccion = new AuditoriaBusiness().GetDoneAudits(SubscrId, Auditor, SalesDate, CreationDate, CallId, PhoneNo);
        //    }

        //    return Json(new { transaccion = transaccion }, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult DeleteAudit(int? AuditId, string UserCode)
        //{
        //    string role = HttpContext.Session["grp_codigo"].ToString();

        //    bool resulset = false;

        //    if (role == "adm")
        //    {
        //        resulset = new AuditoriaBusiness().DeleteAudit(AuditId, UserCode);
        //    }

        //    return Json(new { RESULTSET = resulset ? "DONE":"ERROR"}, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetReportPerAssor(string DateFrom, string DateTo)
        //{
        //    List<ResumenPorAsesor> resultset = new ReportBusiness().GetReportePorAsesor(DateFrom, DateTo);

        //    return Json(new { RESULTSET = resultset, path = Session["reportePorAssesorPath"] }, JsonRequestBehavior.AllowGet);
        //}
        
        //public JsonResult GetRepordPerUnidad(string DateFrom, string DateTo)
        //{
        //    List<ResumenPorUnidad> resulset = new ReportBusiness().GetReporteResumenPorUnidad(DateFrom, DateTo);

        //    return Json(new { RESULTSET = resulset, path = Session["reportePorUnidadPath"] }, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetRepordPerAudit(string DateFrom, string DateTo)
        //{
        //    List<ResumenPorAuditoria> resulset = new ReportBusiness().GetReportePorAudit(DateFrom, DateTo);
            
        //    return Json(new { RESULTSET = resulset, path = Session["reportePorAuditoriaPath"] }, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult reportePorDatoVital(string DateFrom, string DateTo)
        //{
        //    List<ResumenPorDatoVital> resulset = new ReportBusiness().GetReportePorDatoVital(DateFrom, DateTo);

        //    return Json(new { RESULTSET = resulset, path = Session["reportePorDatpVital"] }, JsonRequestBehavior.AllowGet);
        //}
    }


}