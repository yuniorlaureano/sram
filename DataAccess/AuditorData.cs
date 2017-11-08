using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Repository;
using Entities;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess
{
    public class AuditorData
    {
        SqlBasicOperations sqlBasicsOperations = null;
        
        public AuditorData()
        {
            sqlBasicsOperations = new SqlBasicOperations();
        }
        
        public Auditors GetAuditorsCredentials(string userName)
        {
            Auditors auditor = new Auditors();
            SqlDataReader resultset = null;
           
            try
            {
                resultset = sqlBasicsOperations.ExecuteDataReader("evaluaciones.sram.sp_get_sram_usr_credentials", new SqlParameter[]{ 
                    new SqlParameter{ ParameterName = "@usr_name", SqlDbType = SqlDbType.VarChar, Value = userName, Direction = ParameterDirection.Input}
                }, CommandType.StoredProcedure, Schema.GESTION);

                while (resultset.Read())
                {
                    auditor.UserCode = resultset["usr_codigo"].ToString();
                    auditor.GroupCode = resultset["grp_codigo"].ToString();
                }
            }
            catch (SqlException excep)
            {                
                throw excep;
            }
            finally 
            {
                if (resultset != null)
                {
                    resultset.Close();
                }

                this.sqlBasicsOperations.CloseConnection();
            }

            return auditor;
        }

        public List<Auditors>GetAuditors()
        {
            List<Auditors> auditors = new List<Auditors>();
            SqlDataReader resultset = null;

            try
            {
                resultset = sqlBasicsOperations.ExecuteDataReader("evaluaciones.sram.sp_get_sram_auditors", null, CommandType.StoredProcedure, Schema.GESTION);

                while (resultset.Read())
                {
                    auditors.Add(new Auditors {
                        UserCode = resultset["usr_codigo"].ToString(),
                        Name = resultset["usr_nombre"].ToString()
                    });
                    
                }
            }
            catch (SqlException excep)
            {
                throw excep;
            }
            finally
            {
                if (resultset != null)
                {
                    resultset.Close();
                }

                this.sqlBasicsOperations.CloseConnection();
            }

            return auditors;
        }
    }
}
