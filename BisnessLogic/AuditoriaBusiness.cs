using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using DataAccess;

namespace BisnessLogic
{
    public class AuditoriaBusiness 
    {
        public List<Auditoria> GetPendingAudit(string serCode, string salesDate)
        {
            return new AuditoriaData().GetPendingAudit(serCode, salesDate) ;
        }

        public List<AuditoriaResume> GetPendingAuditResume(string UserCode, string SalesDate)
        {
            return new AuditoriaData().GetPendingAuditResume(UserCode, SalesDate);
        }

        public List<UnAssignedAudit> GetUnAssignedAudit()
        {
            return new AuditoriaData().GetUnAssignedAudit();
        }

        public int AssignAudit(string salesDate, string selectedAuditor, string userCode)
        {
            if (string.IsNullOrEmpty(salesDate) || string.IsNullOrEmpty(selectedAuditor) || selectedAuditor == "null" || string.IsNullOrEmpty(userCode))
            {
                throw new ArgumentNullException("Campos Requeridos: Fecha, Auditor y Usuario.");
            }

            return new AuditoriaData().AssignAudit(salesDate, selectedAuditor, userCode);
        }

        //public bool ReAssignAudit(List<Entities.ReassignedAudit> reassignedAudit)
        //{
        //    bool resultset = false;
        //    AuditoriaData auditoriaData = new AuditoriaData();

        //    if (reassignedAudit.Count == 0)
        //    {
        //        throw new ArgumentNullException("Debe proveer la informacion requerida");
        //    }

        //    foreach (ReassignedAudit item in reassignedAudit)
        //    {
        //        resultset = auditoriaData.ReAssignAudit(item);
        //    }

        //    auditoriaData.CloseConnnection();

        //    return resultset;
        //}

        public bool ReAssignAudit(int [] assigmentId, int userAssigned, int userCode)
        {
            AuditoriaData auditoriaData = new AuditoriaData();
            string sentencia = "";

            foreach (var item in assigmentId)
            {
                sentencia += "select '" + item + "' as assignment_id, '" + userCode + "' as user_code, '" + userAssigned + "' as user_assigned from dual union ";
            }

            sentencia = sentencia.Substring(0, sentencia.Length - 7);

            return auditoriaData.ReAssignAudit(sentencia);
        }

        public List<PenddingAudit> GetAcctInfoByAssignment(string AssignmentId)
        {
            return new AuditoriaData().GetAcctInfoByAssignment(AssignmentId);
        }

        public bool Auditar(int AssignmentId, int AccountId, string AuditorComments, int[] Answers, string UserCode,
            byte IsDescargaAdministrativo, byte IsDescargarReauditoria)
        {

            return new AuditoriaData().Auditar(AssignmentId, AccountId, AuditorComments, Answers, UserCode,
             IsDescargaAdministrativo, IsDescargarReauditoria);
        }

        public List<Auditoria> GetDoneAudits(string SubscrId, string Auditor, string SalesDate, string CreationDate, string CallId, string PhoneNo)
        {
            return new AuditoriaData().GetDoneAudits(SubscrId, Auditor, SalesDate, CreationDate, CallId, PhoneNo);
        }

        public bool DeleteAudit(int? AuditId, string UserCode)
        {
            
            return new AuditoriaData().DeleteAudit(AuditId, UserCode);
        }

    }
}
