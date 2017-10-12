using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace DataAccess.Repository
{
    public class SqlBasicOperations
    {
        private ConnectionManager connectionManager;
        private SqlConnection sqlConnection;

        public SqlBasicOperations()
        {
            connectionManager = new ConnectionManager();
        }

        public void OpenConnection(Schema schema)
        {
            try
            {
                if (this.sqlConnection == null)
                {
                    sqlConnection = new SqlConnection(connectionManager.SetConnectionString(schema));
                }

                if (this.sqlConnection.State == System.Data.ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }
            }
            catch (SqlException excep)
            {
                throw excep;
            }
        }

        public void CloseConnection()
        {
            try
            {
                if (this.sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    this.sqlConnection.Close();
                }
            }
            catch (SqlException excep)
            {
                throw excep;
            }
        }


        public DataSet ExecuteDataAdapter(string query, SqlParameter[] sqlParameters, CommandType commandType, Schema schema)
        {
            DataSet resultset = new DataSet();
            
            try
            {
                this.OpenConnection(schema);
                SqlCommand sqlCommand = new SqlCommand(query, this.sqlConnection);

                if (sqlParameters == null)
                {
                    sqlCommand.Parameters.AddRange(sqlParameters);    
                }
                
                SqlDataAdapter oracledataAdapter = new SqlDataAdapter(sqlCommand);
                oracledataAdapter.Fill(resultset);
            }
            catch (SqlException excep)
            {                
                throw excep;
            }
            finally
            {
                this.CloseConnection();
            }

            return resultset;
        }

        public bool ExecuteNonQuery(string query, SqlParameter[] sqlParameters, CommandType commandType, Schema schema)
        {
            bool resultset = false;

            try
            {
                this.OpenConnection(schema);
                SqlCommand sqlCommand = new SqlCommand(query, this.sqlConnection);

                if (sqlParameters == null)
                {
                    sqlCommand.Parameters.AddRange(sqlParameters);
                }

                resultset = sqlCommand.ExecuteNonQuery() > 0;
                
            }
            catch (SqlException excep)
            {
                throw excep;
            }
            finally
            {
                this.CloseConnection();
            }

            return resultset;
        }
    }
}
