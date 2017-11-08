using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Entities;
using BisnessLogic;


namespace SRAM
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start(object sender, EventArgs e)
        {            
            string user = HttpContext.Current.User.Identity.Name;

            if (user.Substring(0, 4) == "CSID" && user.Length > 4)
            {
                user = user.Substring(5);

                Auditors auditor = new AuditorBusiness().GetAuditorsCredentials(user);
                Session["UserCode"] = auditor.UserCode;
                Session["Grp_Codigo"] = auditor.GroupCode;
                Session["UserName"] = user;
            }
        }

    }
}
