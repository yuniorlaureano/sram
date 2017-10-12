using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Collections;

namespace DAL.Util
{
    public interface IConnectionManager
    {
        OleDbConnection OpenConnection();
        OleDbConnection GetConnection(string ProjectCode);
        bool CloseConnection();
        DataSet OpenQuery(string SQLCommand);
        DataSet OpenQuery(string SQLCommand, bool NewConnectFlag);
        bool ExecuteCommand(string SQLCommand);
        string ExecuteSQLCommandTransaction(ArrayList CommandList);
        bool BeginTransaction();
        bool CommitTransaction();
        bool RollBackTransaction();
        
    }
}
