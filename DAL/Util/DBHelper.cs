using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;

//using System.Data.OracleClient;
using System.Configuration;
namespace DAL.Util
{
    public class DBHelper 
    {
        public static DataSet ExecuteQuery(OracleCommand oracleCommand)

        {
            OracleDataAdapter objAdapter = new OracleDataAdapter(oracleCommand);
            DataSet objDataSet = new DataSet();
           
            
            objAdapter.Fill(objDataSet);
            objAdapter.Dispose();
            oracleCommand.Dispose();
            return objDataSet;
        }



        public static OracleConnection GetOracleConnection(string ProjectCode)
        {
            var connection = new OracleConnection(ConfigurationManager.ConnectionStrings[ProjectCode + "_ConnectionString"].ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
