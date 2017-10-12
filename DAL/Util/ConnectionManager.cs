using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Collections;
//using System.Data.OracleClient;
using Oracle.DataAccess.Client;


namespace DAL.Util
{
    public class ConnectionManager : IConnectionManager
    {
        private OleDbConnection mobjConnection;
        private OleDbTransaction mobjTransaction;

        protected string mstrErrorDescription = "";
        private long gintErrorNumber = 0;
        public bool g_blnIndicadorTrans = false;
        private string _projectCode;

        public ConnectionManager(string ProjectCode)
        {
            try
            {
                _projectCode = ProjectCode;
                string appSettingKey = ProjectCode + "_ConnectionString";
                string connAttribute = ConfigurationManager.AppSettings[appSettingKey].ToString();
                mobjConnection = new OleDbConnection(connAttribute);
                mobjConnection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public OleDbConnection OpenConnection()
        {
            throw new NotImplementedException();
        }

        public OleDbConnection GetConnection(string ProjectCode)
        {
            try
            {
                string keyName = ProjectCode + "_ConnectionString";
                string connAttribute = System.Configuration.ConfigurationManager.AppSettings[keyName].ToString();
                OleDbConnection objNewConnection = new OleDbConnection(connAttribute);
                objNewConnection.Open();
                return objNewConnection;
            }
            catch (Exception ex)
            {
                /*
                mstrErrorDescription = objError.Message;
                return null;*/
                throw ex;
            }
        }

        public OracleConnection GetOracleConnection(string ProjectCode)
        {
            try
            {
                string keyName = ProjectCode + "_ConnectionString";
                string connAttribute = System.Configuration.ConfigurationManager.ConnectionStrings[keyName].ToString();
                OracleConnection objNewConnection = new OracleConnection(connAttribute);
                objNewConnection.Open();
                return objNewConnection;
            }
            catch (Exception ex)
            {
                /*
                mstrErrorDescription = objError.Message;
                return null;*/
                throw ex;
            }
        }

        public bool CloseConnection()
        {
            try
            {
                if (mobjConnection.State == ConnectionState.Open)
                    mobjConnection.Close();
                mobjConnection.Dispose();
                return true;
            }
            catch (Exception)
            {
                //mstrErrorDescription = objError.Message;
                return false;
            }
        }

        public DataSet OpenQuery(string SQLCommand)
        {
            return OpenQuery(SQLCommand, false);
        }

        public DataSet OpenOracleQuery(string SQLCommand)
        {
            return OpenOracleQuery(SQLCommand, false);
        }

        public DataSet OpenQuery(string SQLCommand, bool NewConnectFlag)
        {
            try
            {
                string strOtherProject = "";

                /* Comentado por no ser de utilidad, pues cuando
                 * se invoca OpenQuery siempre se manda false en
                 * el parametro NewConnectFlag
                 * C.E.C.F.
                 * 2/25/2014
                 * */

                /*
                if (objAmbient != null)
                {
                    strOtherProject = objAmbient.getDirectConnection();
                    NewConnectFlag |= objAmbient.isDirectModule();
                }*/

                OleDbCommand objDataCommand;

                if (strOtherProject != "" && NewConnectFlag)
                    objDataCommand = new OleDbCommand(SQLCommand, GetConnection(strOtherProject));
                else
                    objDataCommand = new OleDbCommand(SQLCommand, mobjConnection, mobjTransaction);

                objDataCommand.CommandTimeout = 0;
                OleDbDataAdapter objAdapter = new OleDbDataAdapter();
                objAdapter.SelectCommand = objDataCommand;
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                objAdapter.Dispose();
                objDataCommand.Dispose();
                return objDataSet;
            }
            catch (Exception ex)
            {
                /*gintErrorNumber = 0;// ----  Revisar
                mstrErrorDescription = objError.Message;
                return null;*/
                throw ex;
            }
        }

        public DataSet OpenOracleQuery(string SQLCommand, bool NewConnectFlag)
        {
            try
            {
                string strOtherProject = "";

                /* Comentado por no ser de utilidad, pues cuando
                 * se invoca OpenQuery siempre se manda false en
                 * el parametro NewConnectFlag
                 * C.E.C.F.
                 * 2/25/2014
                 * */

                /*
                if (objAmbient != null)
                {
                    strOtherProject = objAmbient.getDirectConnection();
                    NewConnectFlag |= objAmbient.isDirectModule();
                }*/

                OracleCommand objDataCommand;

                if (strOtherProject != "" && NewConnectFlag)
                    objDataCommand = new OracleCommand(SQLCommand, GetOracleConnection(strOtherProject));
                else
                {
                    //objDataCommand = new OracleCommand(SQLCommand, mobjConnection, mobjTransaction);
                    objDataCommand = new OracleCommand(SQLCommand, GetOracleConnection(_projectCode));
                }

                OracleParameter param = new OracleParameter("ResultSet", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                objDataCommand.Parameters.Add(param);
                objDataCommand.CommandTimeout = 0;
                OracleDataAdapter objAdapter = new OracleDataAdapter();
                objAdapter.SelectCommand = objDataCommand;
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                objAdapter.Dispose();
                objDataCommand.Dispose();
                return objDataSet;
            }
            catch (Exception ex)
            {
                /*gintErrorNumber = 0;// ----  Revisar
                mstrErrorDescription = objError.Message;
                return null;*/
                throw ex;
            }
        }

        public bool ExecuteCommand(string SQLCommand)
        {
            try
            {
                OleDbCommand objDataCommand = new OleDbCommand(SQLCommand, mobjConnection, mobjTransaction);
                objDataCommand.CommandTimeout = 0;
                objDataCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                /*
                gintErrorNumber = 0;
                mstrErrorDescription = objError.Message;
                return false;*/
                throw ex;
            }
        }

        public string ExecuteSQLCommandTransaction(ArrayList CommandList)
        {
            /// Variable to return
            string result = "Success";
            /// SqlConnectio Object
            OleDbConnection objConnSql = new OleDbConnection(ConfigurationManager.AppSettings["BIFW_connectionString"].ToString());
            try
            {
                /// The Sql Transaction
                OleDbTransaction transaction;
                /// Open the connection
                objConnSql.Open();
                /// Starting the transaction
                transaction = objConnSql.BeginTransaction();

                try
                {
                    /// calling the commands
                    for (int commands = 0; commands < CommandList.Count; commands++)
                    {
                        OleDbCommand command = (OleDbCommand)CommandList[commands];
                        /// Adding the connection
                        command.Connection = objConnSql;
                        /// Adding the transaction
                        command.Transaction = transaction;
                        /// Excute command
                        command.ExecuteNonQuery();
                    }
                    /// Commit transaction
                    transaction.Commit();
                    objConnSql.Close();
                }
                catch (OleDbException sqlError)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    result += sqlError.Message; //throw sqlError;
                }
            }
            catch (Exception ex)
            {
                result += ex.Message; //throw ex;
            }
            finally
            {
                if (objConnSql != null)
                {
                    if (objConnSql.State != ConnectionState.Closed)
                    {
                        objConnSql.Close();
                    }
                }
            }

            return result;
        }

        public bool BeginTransaction()
        {
            try
            {
                if (!g_blnIndicadorTrans)
                {
                    gintErrorNumber = 0;
                    mstrErrorDescription = "";
                    mobjTransaction = mobjConnection.BeginTransaction();
                }
                g_blnIndicadorTrans = true;
                return true;
            }
            catch (Exception objError)
            {
                gintErrorNumber = 222;// begin transaction 
                mstrErrorDescription = objError.Message;
                return false;
            }
        }

        public bool CommitTransaction()
        {
            try
            {
                if (g_blnIndicadorTrans && mobjTransaction != null)
                    if (gintErrorNumber == 0)
                        mobjTransaction.Commit();
                    else
                        RollBackTransaction();
                g_blnIndicadorTrans = false;
                mobjTransaction = null;
                return gintErrorNumber == 0;
            }
            catch (Exception objExc)
            {
                mstrErrorDescription = objExc.Message + ";CTS";
                return false;
            }
        }

        public bool RollBackTransaction()
        {
            try
            {
                if (g_blnIndicadorTrans && mobjTransaction != null)
                {
                    mobjTransaction.Rollback();
                }
                g_blnIndicadorTrans = false;
                mobjTransaction = null;
                mstrErrorDescription += ";1RollBack";
                return true;
            }
            catch (Exception objError)
            {
                mstrErrorDescription = objError.Message + ";-1RollBack";
                return false;
            }
        }
    }
}
