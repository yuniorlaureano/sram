using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SRAM.Utils
{
    public class Security
    {
        private IDal dal;

        public string getWinUser()
        {
            dal = new Dal();
            string user = HttpContext.Current.User.Identity.Name;
            if (user.Substring(0, 4) == "CSID" && user.Length > 4)
            {
                user = user.Substring(5);
            }
            return user;

        }

        public void getWinUserSession()
        {
            dal = new Dal();
            var Session = HttpContext.Current.Session;
            string user = HttpContext.Current.User.Identity.Name;

            if (user.Substring(0, 4) == "CSID" && user.Length > 4)
            {
                user = user.Substring(5) ;
                //string userCode = dal.GetUserCode("Jose Nunez").Tables[0].Rows.Count > 0 ? dal.GetUserCode("Jose Nunez").Tables[0].Rows[0]["usr_codigo"].ToString() : "";
                string userCode = dal.GetUserCode(user).Tables[0].Rows.Count > 0 ? dal.GetUserCode(user).Tables[0].Rows[0]["usr_codigo"].ToString() : "";

                Session["UserName"] = user;
                Session["UserCode"] = userCode;
                string userRole = dal.GetUserRole(userCode).Tables[0].Rows.Count > 0 ? dal.GetUserRole(userCode).Tables[0].Rows[0]["grp_codigo"].ToString() : "";
                Session["Grp_Codigo"] = userRole;
                string userLevel = dal.GetUserLevel(userCode).Tables[0].Rows.Count > 0 ? dal.GetUserLevel(userCode).Tables[0].Rows[0]["usr_nivel"].ToString() : "";
                Session["usr_nivel"] = userLevel;

            }
        }

    }
}