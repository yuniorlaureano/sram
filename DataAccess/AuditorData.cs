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
        OracleBasicsOperations oracleBasicsOperations = null;
        SqlBasicOperations sqlBasicsOperations = null;
        
        public AuditorData()
        {
            oracleBasicsOperations = new OracleBasicsOperations();
            sqlBasicsOperations = new SqlBasicOperations();
        }
        
        public List<Auditors>GetAuditors()
        {
            List<Auditors> auditors = null;
            DataSet resultset = null;
            string query = "select usr_codigo, usr_nombre from evaluaciones.dbo.evausrm m where exists (select 1 from evaluaciones.dbo.evausrd d where d.usr_codigo = m.usr_codigo and d.pyc_codigo = 'sra' and d.grp_codigo in ('aud')) and sts_codigo = 'A'";

            try
            {
                resultset = sqlBasicsOperations.ExecuteDataAdapter(query, new SqlParameter[0], CommandType.Text, Schema.GESTION);
                auditors = resultset.Tables[0].AsEnumerable().Select(
                    aud => new Auditors
                    {
                        UserCode = aud.Field<string>("usr_codigo"),
                        Name = aud.Field<string>("usr_nombre")
                    }).ToList();
            }
            catch (SqlException excep)
            {                
                throw excep;
            }
            finally 
            {
                if (resultset != null)
                {
                    resultset.Dispose();
                }
            }

            return auditors;
        }
    }
}
