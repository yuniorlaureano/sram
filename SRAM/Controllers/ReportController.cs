using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public FileResult GetReportByUnit(string from, string to)
        {
            return File("~/Content/Report","");
        }

        public FileResult GetReportByVitalData(string from, string to)
        {
            return File("", "");
        }

        public FileResult GetReportByAuditory(string from, string to)
        {
            return File("", "");
        }

        public FileResult GetReportByAsesor(string from, string to)
        {
            return File("", "");
        }
	}
}