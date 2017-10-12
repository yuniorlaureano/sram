using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DAL
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IDal
    {

        #region Authentication
        DataSet GetUserCode(string UserName);
        DataSet GetUserRole(string UserCode);
        DataSet GetUserLevel(string UserCode);
        #endregion
   
    }
}
