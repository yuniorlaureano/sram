using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using System.Data;
using Oracle.DataAccess.Client;
using System.Globalization;
using DataAccess.Repository;

namespace DataAccess
{
    public class SubscriberData
    {
        private OracleBasicsOperations oracleBasicsOperations = null;

        public SubscriberData()
        {
            oracleBasicsOperations = new OracleBasicsOperations();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SubscrId"></param>
        /// <param name="CanvEdition"></param>
        /// <param name="CanvCode"></param>
        /// <param name="book"></param>
        /// <returns>List<SubscriberClaimWithCredit></returns>
        public List<Credit> GetCredit(int SubscrId, int CanvEdition, string CanvCode, string book)
        {
            List<Credit> credits = null;
            DataSet resultset = null;

            try
            {              
                OracleParameter[] oracleParameter = new OracleParameter[] { 
                    new OracleParameter("v_subscr_id",OracleDbType.Int32){ Value = SubscrId },
                    new OracleParameter("v_canv_edition",OracleDbType.Int32) { Value = CanvEdition },
                    new OracleParameter("v_canv_code", OracleDbType.Varchar2) { Value = CanvCode },
                    new OracleParameter("v_book_code", OracleDbType.Varchar2) { Value = book },
                    new OracleParameter("ref_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
                };

                resultset = oracleBasicsOperations.ExecuteDataAdapter("sram.get_credit", oracleParameter, CommandType.StoredProcedure, Schema.YBRDS_PROD);

                credits = resultset.Tables[0].AsEnumerable().Select(
                        credit => new Credit
                        {
                            ClaimNumber = credit["No. Reclamo"].ToString(),
                            Book = credit["Libro"].ToString(),
                            ClientComment = credit["Comentario Cliente"].ToString(),
                            InfoDescription = credit["Info Desc"].ToString()
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

            return credits;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SubscrId"></param>
        /// <param name="CanvEdition"></param>
        /// <param name="CanvCode"></param>
        /// <param name="book"></param>
        /// <returns>List<SubscriberClaimWithClaims></returns>
        public List<Claim> GetClaims(int SubscrId, int CanvEdition, string CanvCode, string book)
        {
            List<Claim> claims = null;
            DataSet resultset = null;

            try
            {
                OracleParameter[] oracleParameter = new OracleParameter[] { 
                    new OracleParameter("v_subscr_id", OracleDbType.Int32) { Value = SubscrId },
                    new OracleParameter("v_canv_edition", OracleDbType.Int32) { Value = CanvEdition },
                    new OracleParameter("v_canv_code", OracleDbType.Varchar2) { Value = CanvCode },
                    new OracleParameter("v_book_code", OracleDbType.Varchar2) { Value = book },
                    new OracleParameter("ref_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
                };

                resultset = oracleBasicsOperations.ExecuteDataAdapter("sram.get_claims", oracleParameter, CommandType.StoredProcedure, Schema.YBRDS_PROD);

                claims = resultset.Tables[0].AsEnumerable().Select(
                    claim => new Claim
                    {
                        ClaimNumber = claim["No. Reclamo"].ToString(),
                        Book = claim["Libro"].ToString(),
                        ClaimDescription = claim["Descripcion Reclamo"].ToString(),
                        ClientComment = claim["Comentario Cliente"].ToString(),
                        CanvCode = claim["Canv Code"].ToString()
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

            return claims;
        }

        /// <summary>
        /// Permite obtener el libro de ese subscriptor
        /// </summary>
        /// <param name="SubscrId"></param>
        /// <param name="CanvCode"></param>
        /// <param name="CanvEdition"></param>
        /// <returns>List<SubscriberCanvBook></returns>
        public List<SubscriberCanvBook> GetSubscribersCanvBooks(int SubscrId, string CanvCode, int CanvEdition)
        {
            List<SubscriberCanvBook> subscriberCanvAndBook = null;
            DataSet resultset = null;

            try
            {
                OracleParameter[] oracleParameter = new OracleParameter[] { 
                    new OracleParameter("v_SubscrId", OracleDbType.Int32) {Value = SubscrId},
                    new OracleParameter("v_CanvCode", OracleDbType.Varchar2) { Value = CanvCode },
                    new OracleParameter("v_CanvEdition", OracleDbType.Int32) {Value = CanvEdition},
                    new OracleParameter("ResultSet", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
                };

                resultset = oracleBasicsOperations.ExecuteDataAdapter("sfa.sra_get_subscr_canv_books", oracleParameter, CommandType.StoredProcedure, Schema.SFA);

                subscriberCanvAndBook = resultset.Tables[0].AsEnumerable().Select(
                    subscrb => new SubscriberCanvBook
                    {
                        BookCode = subscrb["Book_Code"].ToString()
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

            return subscriberCanvAndBook;
        }


    }
}
