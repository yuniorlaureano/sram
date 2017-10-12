using DAL.Util;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DAL
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Dal : IDal
    {

        #region Authentication
        public DataSet GetUserCode(string UserName)
        {
            string query = string.Format("select usr_codigo from evaluaciones.dbo.evausrm where usr_nombre = '{0}'", UserName);
            return new ConnectionManager("gestion").OpenQuery(query);
        }
        public DataSet GetUserRole(string UserCode)
        {
            string query = string.Format("select grp_codigo from evaluaciones.dbo.evausrd where usr_codigo = '{0}' and pyc_codigo = 'sra'", UserCode);
            return new ConnectionManager("gestion").OpenQuery(query);
        }
        public DataSet GetUserLevel(string UserCode)
        {
            string query = string.Format("select usr_nivel from evaluaciones.dbo.evausrd where usr_codigo = '{0}' and pyc_codigo = 'sra'", UserCode);
            return new ConnectionManager("gestion").OpenQuery(query);
        }
        #endregion

    }
}
