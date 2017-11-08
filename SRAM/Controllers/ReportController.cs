using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entities;
using BisnessLogic;

namespace SRAM.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Obtiene el reporte por unidad
        /// </summary>
        /// <param name="from">Fecha desde</param>
        /// <param name="to">Fecha hasta</param>
        /// <returns>FileResult</returns>
        public JsonResult GetReportByUnit(string from, string to)
        {
            string userCode = Session["UserCode"].ToString();
            ReportBusiness rbusiness = new ReportBusiness();
            rbusiness.GetReportByUnit(from, to, userCode, Server.MapPath("~/Content/Report/"));
            return Json(new { url = @Url.Action("DownLoadByUnit", "Report") }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DownLoadByUnit()
        {
            string file = "~/Content/Report/" + Session["UserCode"].ToString() + "_reporte por unidad.xlsx";
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        /// <summary>
        /// Obtiene el reporte por dato vital
        /// </summary>
        /// <param name="from">Fecha desde</param>
        /// <param name="to">Fecha hasta</param>
        /// <returns>FileResult</returns>
        public JsonResult GetReportByVitalData(string from, string to)
        {
            string userCode = Session["UserCode"].ToString();
            ReportBusiness rbusiness = new ReportBusiness();
            rbusiness.GetReportByVitalData(from, to, userCode, Server.MapPath("~/Content/Report/"));
            return Json(new { url = @Url.Action("DownloadByVitalData", "Report") }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DownloadByVitalData()
        {
            string file = "~/Content/Report/" + Session["UserCode"].ToString() + "_reporte por dato vital.xlsx";
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        /// <summary>
        /// Obtiene el reporte por Auditoria
        /// </summary>
        /// <param name="from">Fecha desde</param>
        /// <param name="to">Fecha hasta</param>
        /// <returns>FileResult</returns>
        public JsonResult GetReportByAuditory(string from, string to)
        {
            string userCode = Session["UserCode"].ToString();
            ReportBusiness rbusiness = new ReportBusiness();
            rbusiness.GetReportByAuditory(from, to, userCode, Server.MapPath("~/Content/Report/"));
            return Json(new { url = @Url.Action("DonwloadByAuditory", "Report") }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DonwloadByAuditory()
        {
            string file = "~/Content/Report/" + Session["UserCode"].ToString() + "_reporte por auditoria.xlsx";
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        /// <summary>
        /// Obtiene el reporte por asesor
        /// </summary>
        /// <param name="from">Fecha desde</param>
        /// <param name="to">Fecha hasta</param>
        /// <returns>JsonResult</returns>
        public JsonResult GetReportByAsesor(string from, string to)
        {
            string userCode = Session["UserCode"].ToString();
            ReportBusiness rbusiness = new ReportBusiness();
            rbusiness.GetReportByAsesor(from, to, userCode, Server.MapPath("~/Content/Report/"));
            return Json(new { url = @Url.Action("DonwloadByAsesor", "Report") }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DonwloadByAsesor()
        {
            string file = "~/Content/Report/" + Session["UserCode"].ToString() + "_reporte por ejecutivo.xlsx";
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
	}
}