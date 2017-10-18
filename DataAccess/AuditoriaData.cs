using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using DataAccess.Repository;
using System.Data;
using Oracle.DataAccess.Client;


using System.Globalization;namespace DataAccess
{
    public class AuditoriaData
    {
        OracleBasicsOperations oracleBasicsOperations = null;
        //SqlBasicOperations sqlBasicsOperations = null;

        public AuditoriaData()
        {
            oracleBasicsOperations = new OracleBasicsOperations();
        }

        /// <summary>
        /// Permite obtener las auditorias pendientes
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="SalesDate"></param>
        /// <returns>List<Auditoria></returns>
        public List<Auditoria> GetPendingAudit(string UserCode, string SalesDate)
        {
            List<Auditoria> auditoria = null;
            DataSet resultset = null;
            
            try
            {                                
                DateTime? salesDate = null;
                DateTime dummy;
                if (DateTime.TryParse(SalesDate, new CultureInfo("en-US"), DateTimeStyles.None, out dummy)) 
                    salesDate = dummy;

                OracleParameter[] oracleParameter = new OracleParameter[] { 
                    new OracleParameter("v_UserCode", OracleDbType.Varchar2) { Value = UserCode },
                    new OracleParameter("v_SalesDate", OracleDbType.Date) { Value = salesDate },
                    new OracleParameter("ResultSet", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
                };

                resultset = oracleBasicsOperations.ExecuteDataAdapter("sfa.sram.sra_get_auditor_pendings", oracleParameter, CommandType.StoredProcedure, Schema.SFA);
                
                auditoria = resultset.Tables[0].AsEnumerable().Select(
                    
                    pendiente => new Auditoria
                    {
                        AssignmentId = pendiente["AssignmentId"].ToString(),
                        RazonSocial = pendiente["SubscrName"].ToString(),
                        SubscriberId = pendiente["SubscrId"].ToString(),
                        Telefono = pendiente["PhoneNo"].ToString(),
                        Canvass = pendiente["CanvCode"].ToString(),
                        CanvEdition = pendiente["CanvEdition"].ToString(),
                        BookCode = pendiente["PRODUCTO_ID"].ToString(),
                        Venta = pendiente["ChargeIn"].ToString(),
                        Ejecutivo = pendiente["StaffName"].ToString(),
                        Unidad = pendiente["AccountUnit"].ToString(),
                        FechaRPC = pendiente["SalesDate"].ToString(),
                        Cargo = pendiente["ChargeOut"].ToString(),
                        CallId = pendiente["CallId"].ToString(),
                        ControlVerballCallId = pendiente["ControlVerbalId"].ToString(),
                        ComentarioEjecutivo = pendiente["Comment"].ToString(),
                        PDCDimmas = pendiente["PDCDimmas"].ToString(),
                        PDCCodetel = pendiente["PDCCodetel"].ToString(),
                        Compania = pendiente["Company"].ToString(),
                        TipoDeServicio = pendiente["Type_of_Service"].ToString(),
                        AuditorAsignado = pendiente["AuditorName"].ToString(),
                        PrevioCallId = pendiente["PrevAuditedCallId"].ToString(),
                        HassClaim = pendiente["HAS_CLAIM"].ToString() == "1",
                        HassCredit = pendiente["HAS_CREDIT"].ToString() == "1"

                    }).ToList();
            
            }
            catch (OracleException excep)
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

            return auditoria;
        }

        /// <summary>
        /// Obtiene un resumen de las auditorias, un resument por auditor
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="SalesDate"></param>
        /// <returns>List<AuditoriaResume></returns>
        public List<AuditoriaResume> GetPendingAuditResume(string UserCode, string SalesDate)
        {
            List<Auditoria> auditoria = this.GetPendingAudit(UserCode, SalesDate);
            IEnumerable<AuditoriaResume> peddingResume = null;

            try
            {
                 peddingResume = from r in auditoria
                                    group r by new
                                    {
                                        Name = r.AuditorAsignado,
                                    } into g
                                    select new AuditoriaResume
                                    {
                                         Name = g.Key.Name,
                                         Count = g.Count()
                                    }; 
            }
            catch (OracleException excep)
            {
                throw excep;
            }
            finally
            {
                auditoria = null;
            }

            return peddingResume.ToList();
        }

        /// <summary>
        /// Permite obtener las auditorias que aun no han sido asignadas
        /// </summary>
        /// <returns>List<UnAssignedAudit></returns>
        public List<UnAssignedAudit> GetUnAssignedAudit()
        {
            List<UnAssignedAudit> unAssignedAudit = null;
            DataSet resultset = null;
            
            OracleParameter[] oracleParameter = new OracleParameter[] { 
                    new OracleParameter("p_sales_date", OracleDbType.Date) {  },
                    new OracleParameter("ResultSet", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
                };
            
            try
            {

                resultset = oracleBasicsOperations.ExecuteDataAdapter("sra_get_unassigned_accounts", oracleParameter, CommandType.StoredProcedure, Schema.SFA);

                unAssignedAudit = resultset.Tables[0].AsEnumerable().Select(
                unAssigned =>
                    new UnAssignedAudit
                    {
                        Cantidad = unAssigned.Field<decimal>("CANTIDAD"),
                        SalesDate = unAssigned.Field<DateTime>("SALESDATE").ToString("MM/dd/yyyy")
                    }).ToList();

            }
            catch (OracleException excep)
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

            return unAssignedAudit.ToList();
        }

        /// <summary>
        /// Permite asigar auditorias
        /// </summary>
        /// <param name="salesDate"></param>
        /// <param name="selectedAuditor"></param>
        /// <param name="userCode"></param>
        /// <returns>int</returns>
        public int AssignAudit(string salesDate,string selectedAuditor,string userCode)
        {
            DataSet resultset = null;
            DateTime dtSalesDate = DateTime.Parse(salesDate, new CultureInfo("en-US"));
            int afectedRows;
                   
            OracleParameter[] oracleParameter = new OracleParameter[] { 
                    new OracleParameter("v_SalesDate", OracleDbType.Date) { Value = dtSalesDate },
                    new OracleParameter("v_UserCode", OracleDbType.Varchar2) { Value = userCode },
                    new OracleParameter("v_Auditors", OracleDbType.Varchar2) { Value = selectedAuditor },
                    new OracleParameter("ResultSet", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
                };

            try
            {

                resultset = oracleBasicsOperations.ExecuteDataAdapter("sra_assign_rdv", oracleParameter, CommandType.StoredProcedure, Schema.SFA);
                afectedRows = Convert.ToInt32(resultset.Tables[0].Rows[0]["AssignCount"]);

            }
            catch (OracleException excep)
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

            return afectedRows;
        }

        /// <summary>
        /// Permite reasignar una auditoria
        /// </summary>
        /// <param name="AssignmentId"></param>
        /// <param name="Auditor"></param>
        /// <param name="UserCode"></param>
        /// <returns>bool</returns>
        //public bool ReAssignAudit(ReassignedAudit reasignedAudit)
        //{
        //    bool resultset = false;

        //    OracleParameter[] oracleParameter = new OracleParameter[] { 
        //        new OracleParameter("v_AssignmentId", OracleDbType.Int32) { Value = reasignedAudit.AssignmentId },
        //        new OracleParameter("v_Auditor", OracleDbType.Varchar2) { Value = reasignedAudit.Auditor },
        //        new OracleParameter("v_UserCode", OracleDbType.Varchar2) { Value = reasignedAudit.UserCode }
        //    };

        //    try
        //    {
        //        resultset = oracleBasicsOperations.ExecuteNonQuery("sfa.sra_re_assign", oracleParameter, CommandType.StoredProcedure, Schema.SFA);
        //        resultset = true;
        //    }
        //    catch (OracleException excep)
        //    {
        //        throw excep;
        //    }

        //    return resultset;

        //}

        public bool ReAssignAudit(string sentencia)
        {
            bool resultset = false;

            OracleParameter[] oracleParameter = new OracleParameter[] { 
                new OracleParameter("in_sentencia", OracleDbType.Int32) { Value = sentencia , OracleDbType = OracleDbType.Varchar2 },
                new OracleParameter("resultset", OracleDbType.Int32) { Direction = ParameterDirection.Output, OracleDbType = OracleDbType.Int32}
            };

            try
            {
                oracleBasicsOperations.ExecuteNonQuery("sram.sp_assign", oracleParameter, CommandType.StoredProcedure, Schema.SFA);
                resultset = Convert.ToInt32(oracleParameter[1].Value.ToString()) > 0;
                this.CloseConnnection();
            }
            catch (OracleException excep)
            {
                throw excep;
            }

            return resultset;
        }

        /// <summary>
        /// Obtiene la informacion del la auditoria asignadas, por assigmentId
        /// </summary>
        /// <param name="AssignmentId"></param>
        /// <returns>List<PenddingAudit></returns>
        public List<PenddingAudit> GetAcctInfoByAssignment(string AssignmentId)
        {
            List<PenddingAudit> peddingAudit = null;
            DataSet resultset = null;

            OracleParameter[] oracleParameter = new OracleParameter[] { 
                new OracleParameter("p_assign_id", OracleDbType.Int32) { Value = AssignmentId },
                new OracleParameter("ResultSet", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
            };

            try
            {
                resultset = oracleBasicsOperations.ExecuteDataAdapter("sfa.sra_get_acct_info_by_assign", oracleParameter, CommandType.StoredProcedure, Schema.SFA);

                peddingAudit = resultset.Tables[0].AsEnumerable().Select(
                    trn => new PenddingAudit
                    {
                        RazonSocial = trn["SubscrName"].ToString(),
                        SubscriberId = trn["SubscrId"].ToString(),
                        Telefono = trn["PhoneNo"].ToString(),
                        Canvass = trn["CanvCode"].ToString(),
                        Ejecutivo = trn["StaffName"].ToString(),
                        Unidad = trn["AccountUnit"].ToString(),
                        FechaRPC = trn["SalesDate"].ToString(),
                        Cargo = trn["ChargeOut"].ToString(),
                        Venta = trn["ChargeIn"].ToString(),
                        CallId = trn["CallId"].ToString(),
                        ComentarioEjecutivo = trn["Comment"].ToString(),
                        PDCDimmas = trn["PDCDimmas"].ToString(),
                        PDCCodetel = trn["PDCCodetel"].ToString(),
                        Compania = trn["Company"].ToString(),
                        CustSource = trn["CustSource"].ToString(),
                        AccountId = trn["AccountId"].ToString(),
                        BookCode = trn["producto_id"].ToString(),
                        ControlVerballCallId = trn["ControlVerbalId"].ToString(),
                        AuditorName = trn["AuditorName"].ToString(),
                    }
                ).ToList();

            }
            catch (OracleException excep)
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

            return peddingAudit;
        }

        /// <summary>
        /// Guarda los resultados de la auditoria en la base de datas
        /// </summary>
        /// <param name="AssignmentId"></param>
        /// <param name="AccountId"></param>
        /// <param name="AuditorComments"></param>
        /// <param name="Answers"></param>
        /// <param name="UserCode"></param>
        /// <param name="IsDescargaAdministrativo"></param>
        /// <param name="IsDescargarReauditoria"></param>
        /// <returns>bool</returns>
        public bool Auditar(int AssignmentId, int AccountId, string AuditorComments, int[] Answers, string UserCode,
            byte IsDescargaAdministrativo, byte IsDescargarReauditoria)
        {

            DataSet resultset = null;
            bool result = false;

            // El resultado (Válida o no) depende solo de los
            // Datos Vitales
            int Result = ((Answers[0] == 0 || Answers[1] == 0 || Answers[2] == 0 || Answers[3] == 0 || Answers[4] == 0
                || Answers[5] == 0 || Answers[6] == 0 || Answers[7] == 0 || Answers[8] == 0 || Answers[9] == 0 
                || Answers[10] == 0 || Answers[11] == 0
                ) ? 0 : 1);

            int valueDescargaAdmin = (IsDescargaAdministrativo == 1) ? 1 : 0;
            int valueDescargaReaudit = (IsDescargarReauditoria == 1) ? 1 : 0;


            OracleParameter[] oracleParameter = new OracleParameter[] { 
                  new OracleParameter("v_AssignmentId", OracleDbType.Int32) { Value = AssignmentId },
                  new OracleParameter("v_Comments", OracleDbType.Varchar2) { Value = AuditorComments },
                  new OracleParameter("v_Result", OracleDbType.Int32) { Value = Result },
                  new OracleParameter("v_Value1", OracleDbType.Int32) { Value = Answers[0] },
                  new OracleParameter("v_Value2", OracleDbType.Int32) { Value = Answers[1] },
                  new OracleParameter("v_Value3", OracleDbType.Int32) { Value = Answers[2] },
                  new OracleParameter("v_Value4", OracleDbType.Int32) { Value = Answers[3] },
                  new OracleParameter("v_Value5", OracleDbType.Int32) { Value = Answers[4] },
                  new OracleParameter("v_Value6", OracleDbType.Int32) { Value = Answers[5] },
                  new OracleParameter("v_Value7", OracleDbType.Int32) { Value = Answers[6] },
                  new OracleParameter("v_Value8", OracleDbType.Int32) { Value = Answers[7] },
                  new OracleParameter("v_Value9", OracleDbType.Int32) { Value = Answers[8] },
                  new OracleParameter("v_Value10", OracleDbType.Int32) { Value = Answers[9] },
                  new OracleParameter("v_Value11", OracleDbType.Int32) { Value = Answers[10] },
                  new OracleParameter("v_Value12", OracleDbType.Int32) { Value = Answers[11] },
                  new OracleParameter("v_UserCode", OracleDbType.Varchar2) { Value = UserCode },
                  new OracleParameter("v_IsDescargaReaudit", OracleDbType.Int32) { Value = valueDescargaReaudit },
                  new OracleParameter("v_IsDescargaAdministrativo", OracleDbType.Int32) { Value = valueDescargaAdmin },
                  new OracleParameter("ResultSet", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
            };

            try
            {
                resultset = oracleBasicsOperations.ExecuteDataAdapter("sfa.sra_audit", oracleParameter, CommandType.StoredProcedure, Schema.SFA);
                
                if (Convert.ToInt32(resultset.Tables[0].Rows[0]["RejectResult"]) == 1)
                {
                    OracleParameter[] oracleParameterForReject = new OracleParameter[] { 
                        new OracleParameter("p_account_id", OracleDbType.Int32) { Value = AccountId },
                        new OracleParameter("p_reject_code", OracleDbType.Char) { Value = "I1" },
                        new OracleParameter("p_is_primero", OracleDbType.Int32) { Value = 1 },
                        new OracleParameter("p_puede_insertar", OracleDbType.Int32) { Value = 0 }
                    };

                    oracleBasicsOperations.ExecuteNonQuery("InsertarRejMotivos", oracleParameterForReject, CommandType.StoredProcedure, Schema.YBRDS_PROD);
                }

                result = (resultset != null ? true : false);
            }
            catch (OracleException excep)
            {
                throw excep;
            }
            finally
            {
                if (resultset != null)
                {
                    resultset.Dispose();
                }

                oracleBasicsOperations.CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// permite obtener las auditorias realizadas.
        /// </summary>
        /// <param name="SubscrId"></param>
        /// <param name="Auditor"></param>
        /// <param name="SalesDate"></param>
        /// <param name="CreationDate"></param>
        /// <param name="CallId"></param>
        /// <param name="PhoneNo"></param>
        /// <returns>List<PenddingAudit></returns>
        public List<Auditoria> GetDoneAudits(string SubscrId, string Auditor, string SalesDate, string CreationDate, string CallId, string PhoneNo)
        {

            List<Auditoria> peddingAudit = null;
            DataSet resultset = null;

            DateTime? salesDate = null, creationDate = null;
            DateTime dummy;

            if (DateTime.TryParse(SalesDate, new CultureInfo("en-US"), DateTimeStyles.None, out dummy)) salesDate = dummy;
            if (DateTime.TryParse(CreationDate, new CultureInfo("en-US"), DateTimeStyles.None, out dummy)) creationDate = dummy;

            OracleParameter[] oracleParameter = new OracleParameter[] { 
                new OracleParameter("v_SubscrId", OracleDbType.Int32) { Value = string.IsNullOrEmpty(SubscrId) ? null : SubscrId },
                new OracleParameter("v_Auditor", OracleDbType.Varchar2) { Value = Auditor },
                new OracleParameter("v_SalesDate", OracleDbType.Date) { Value = salesDate },
                new OracleParameter("v_CreationDate", OracleDbType.Date) { Value = creationDate },
                new OracleParameter("v_CallId", OracleDbType.Int32) { Value = string.IsNullOrEmpty(CallId) ? null : CallId },
                new OracleParameter("v_PhoneNo", OracleDbType.Varchar2) { Value = PhoneNo },
                new OracleParameter("ResultSet", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
            };

            try
            {
                resultset = oracleBasicsOperations.ExecuteDataAdapter("sfa.sra_get_audits", oracleParameter, CommandType.StoredProcedure, Schema.SFA);

                peddingAudit = resultset.Tables[0].AsEnumerable().Select(
                    trn => new Auditoria
                    {
                        Status = trn["STATUS"].ToString(),
                        AssignmentId = trn["AssignmentId"].ToString(),
                        RazonSocial = trn["SubscrName"].ToString(),
                        AuditId = trn["AuditId"].ToString(),
                        SubscriberId = trn["SubscrId"].ToString(),
                        Telefono = trn["PhoneNo"].ToString(),
                        Canvass = trn["CanvCode"].ToString(),
                        BookCode = trn["PRODUCTO_ID"].ToString(),
                        Ejecutivo = trn["StaffName"].ToString(),
                        Unidad = trn["AccountUnit"].ToString(),
                        FechaRPC = trn["SalesDate"].ToString(),
                        Cargo = trn["ChargeOut"].ToString(),
                        Venta = trn["ChargeIn"].ToString(),
                        CallId = trn["CallId"].ToString(),
                        ComentarioEjecutivo = trn["Comment"].ToString(),
                        ControlVerballCallId = trn["ControlVerbalId"].ToString(),
                        PDCDimmas = trn["PDCDimmas"].ToString(),
                        PDCCodetel = trn["PDCCodetel"].ToString(),
                        Compania = trn["Company"].ToString(),
                        CustSource = trn["CustSource"].ToString(),
                        AccountId = trn["AccountId"].ToString(),
                        PrevioCallId = trn["PrevAuditedCallId"].ToString(),
                        AuditCreationDate = trn["AuditCreationDate"].ToString(),
                        AuditorName = trn["AuditAuditorName"].ToString(),
                        AuditResult = trn["AuditResult"].ToString(),
                        InvalidQuestions = trn["InvalidQuestions"].ToString(),
                        ComentarioAuditor = trn["Comments"].ToString(),
                        CanvEdition = trn["CANVEDITION"].ToString(),
                    }
                ).ToList();

            }
            catch (OracleException excep)
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

            return peddingAudit;
        }

        /// <summary>
        /// Elimina la auditoria
        /// </summary>
        /// <param name="AuditId"></param>
        /// <param name="UserCode"></param>
        /// <returns></returns>
        public bool DeleteAudit(int? AuditId, string UserCode)
        {
            bool result = false;

            OracleParameter[] oracleParameter = new OracleParameter[] { 
                new OracleParameter("p_audit_id", OracleDbType.Int32) { Value = AuditId },
                new OracleParameter("p_updated_by", OracleDbType.Varchar2) { Value = UserCode }
            };

            try
            {
                oracleBasicsOperations.ExecuteNonQuery("sfa.sra_delete_audit", oracleParameter, CommandType.StoredProcedure, Schema.SFA);
                result = true;
            }
            catch (OracleException excep)
            {
                throw excep;
            }
            finally
            {
                this.CloseConnnection();
            }

            return result;

        }

        /// <summary>
        /// Cierra la conexion
        /// </summary>
        public void CloseConnnection()
        {
            oracleBasicsOperations.CloseConnection();
        }
    }
}
