﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using System.Configuration;



namespace DataAccess.Repository
{
   
    public class OracleBasicsOperations
    {
        private ConnectionManager connectionManager;
        private OracleConnection oracleConnection;
        private Schema schema;

        public OracleBasicsOperations()
        {
            connectionManager = new ConnectionManager();
        }

        public void OpenConnection(Schema schema)
        {
            try
            {
                if (this.oracleConnection == null)
                {
                    oracleConnection = new OracleConnection(connectionManager.SetConnectionString(schema));
                    this.schema = schema;
                }

                if (this.oracleConnection.State == ConnectionState.Closed)
                {
                    oracleConnection.Open();
                }
            }
            catch (OracleException excep)
            {
                throw excep;
            }
        }

        public void CloseConnection()
        {
            try
            {
                if (this.oracleConnection != null && this.oracleConnection.State == ConnectionState.Open)
                {
                    this.oracleConnection.Close();
                }
            }
            catch (OracleException excep)
            {
                throw excep;
            }
        }


        public DataSet ExecuteDataAdapter(string query, OracleParameter[] oracleParameters, CommandType commandType, Schema schema)
        {
            DataSet resultset = new DataSet();
            
            try
            {
                if (this.schema != schema && this.oracleConnection != null)
                {
                    if (this.oracleConnection.State == ConnectionState.Open)
                    {
                        this.CloseConnection();
                    }

                    this.oracleConnection = null;
                }

                this.OpenConnection(schema);
                OracleCommand oracleCommand = new OracleCommand(query,this.oracleConnection);
                oracleCommand.CommandType = commandType;

                if (oracleParameters.Length > 0)
                {
                    oracleCommand.Parameters.AddRange(oracleParameters);
                }
                
                OracleDataAdapter oracledataAdapter = new OracleDataAdapter(oracleCommand);
                oracledataAdapter.Fill(resultset);
            }
            catch (OracleException excep)
            {                
                throw excep;
            }
            finally
            {
                this.CloseConnection();
            }

            return resultset;
        }

        public bool ExecuteNonQuery(string query, OracleParameter[] oracleParameters, CommandType commandType,Schema schema)
        {
            bool resultset = false;

            try
            {
                if (this.schema != schema && this.oracleConnection != null)
                {
                    if (this.oracleConnection.State == ConnectionState.Open)
                    {
                        this.CloseConnection();
                    }

                    this.oracleConnection = null;
                }

                this.OpenConnection(schema);
                OracleCommand oracleCommand = new OracleCommand(query,this.oracleConnection);
                oracleCommand.CommandType = commandType;

                if (oracleParameters.Length > 0)
                {
                    oracleCommand.Parameters.AddRange(oracleParameters);
                }

                resultset = oracleCommand.ExecuteNonQuery() > 0;
                
            }
            catch (OracleException excep)
            {
                throw excep;
            }

            return resultset;
        }

    }
}
