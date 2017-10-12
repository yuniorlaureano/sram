using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using System.Data;
using Entities;
using DataAccess.Repository;
using AppUtil.Data;
using ClosedXML;
using System.Web;
using System.Web.Hosting;
using System.Globalization;


namespace DataAccess
{
    public class ReportData
    {

        private OracleBasicsOperations oracleBasicsOperations = null;

        public ReportData()
        {
            oracleBasicsOperations = new OracleBasicsOperations();
        }

        public List<ResumenPorUnidad> GetReporteResumenPorUnidad(string DateFrom, string DateTo)
        {
            List<ResumenPorUnidad> resumentPorUnidad = null;
            DataTable resultset = null;
            decimal porcementajeValido = 0;
            decimal porcementajeInvalido = 0;
            int totalDeLlamadas = 0;
            int validas = 0;
            int invalidas = 0;

            try
            {

                OracleParameter[] oracleParameter = new OracleParameter[] { 
                    new OracleParameter("v_DateFrom", OracleDbType.Date) {Value = DateTime.Parse(DateFrom, new CultureInfo("en-US"))},
                    new OracleParameter("v_DateTo", OracleDbType.Date) { Value = DateTime.Parse(DateTo, new CultureInfo("en-US")) },
                    new OracleParameter("ResultSet", OracleDbType.RefCursor) {Direction = ParameterDirection.Output},
                };

                resultset = oracleBasicsOperations.ExecuteDataAdapter("sp_get_stat_by_unit_yn", oracleParameter, CommandType.StoredProcedure, Schema.SFA).Tables[0];

                
                resumentPorUnidad = resultset.AsEnumerable().Select(
                    subscrb => new ResumenPorUnidad
                    {
                        Unidad = subscrb["UNIDAD"].ToString(),
                        Calificacion = subscrb["CALIFICACION"].ToString(),
                        TotalLamadas = int.Parse(subscrb["Total de Llamadas"].ToString()),
                        Porcentaje = decimal.Parse(subscrb["PORCENTAJE"].ToString()),
                        NiTotal = double.Parse(subscrb["NI Total"].ToString()),
                    }).ToList();

                //Iterar el resultset, cada ves que encuentre el label Total por unidad:
                int totalPorNidadCount = (resumentPorUnidad.Count - 1);
                for (int i = 0; i <= totalPorNidadCount; i++)
                {                    
                    if (resumentPorUnidad[i].Calificacion != "Total por unidad:")
                    {
                        continue;
                    }

                    if (i == totalPorNidadCount)
                    {
                        resumentPorUnidad[i].Porcentaje = -1;
                        break;
                    }


                    if (resumentPorUnidad[i + 1].Calificacion != "Total por unidad:")
                    {
                        validas = resumentPorUnidad[i].TotalLamadas;
                        totalDeLlamadas = resumentPorUnidad[i+1].TotalLamadas;
                        if (totalDeLlamadas > 0)
                        {
                            porcementajeValido = (((decimal)validas / (decimal)totalDeLlamadas)) * 100;
                            resumentPorUnidad[i].Porcentaje = Math.Round(porcementajeValido);
                        }
                    }
                    else
                    {
                        //realizar calculo de los porcientos.
                        //(valida/total por unidad)*100
                        validas = resumentPorUnidad[i - 2].TotalLamadas;
                        totalDeLlamadas = resumentPorUnidad[i].TotalLamadas;
                        if (totalDeLlamadas > 0)
                        {
                            porcementajeValido = (((decimal)validas / (decimal)totalDeLlamadas)) * 100;
                            resumentPorUnidad[i - 2].Porcentaje = Math.Round(porcementajeValido);
                        }

                        //(invalida/total por unidad)*100
                        invalidas = resumentPorUnidad[i - 1].TotalLamadas;
                        totalDeLlamadas = resumentPorUnidad[i].TotalLamadas;
                        if (totalDeLlamadas > 0)
                        {
                            porcementajeInvalido = ((decimal)(invalidas / (decimal)totalDeLlamadas) * 100);
                            resumentPorUnidad[i - 1].Porcentaje = Math.Round(porcementajeInvalido, 0);
                        }
                    }
                    resumentPorUnidad[i].Porcentaje = -1;
                    
                }
                                
                //string path = HttpContext.Current.Server.MapPath("~/Content/Report/");

                //var reporte = Excel.CreateExcelFileWithDataTable(resultset, HttpContext.Current.Session["UserCode"] + "_reporte Por Unidad", path, "xlsx", true);
                //HttpContext.Current.Session["reportePorUnidadPath"] = path + reporte;
                   
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

            return resumentPorUnidad;
        }

        public List<ResumenPorAsesor> GetReportePorAsesor(string DateFrom, string DateTo)
        {
            List<ResumenPorAsesor> reportePorAsesor = null;
            DataTable resultset = null;
            decimal porcentajeInvalidas = 0;
            int totalDeLlamadas = 0;
            int invalidas = 0;

            try
            {

                OracleParameter[] oracleParameter = new OracleParameter[] { 
                    new OracleParameter("v_DateFrom", OracleDbType.Date) {Value = DateTime.Parse(DateFrom, new CultureInfo("en-US"))},
                    new OracleParameter("v_DateTo", OracleDbType.Date) { Value = DateTime.Parse(DateTo , new CultureInfo("en-US")) },
                    new OracleParameter("ResultSet", OracleDbType.RefCursor) {Direction = ParameterDirection.Output},
                };

                resultset = oracleBasicsOperations.ExecuteDataAdapter("sp_get_stat_by_assesor_yn", oracleParameter, CommandType.StoredProcedure, Schema.SFA).Tables[0];

                reportePorAsesor = resultset.AsEnumerable().Select(
                    subscrb => new ResumenPorAsesor
                    {
                        Unidad = subscrb["Unidad"].ToString(),
                        Ejecutivo = subscrb["Ejecutivo"].ToString(),
                        TotalLamadas = int.Parse(subscrb["Total de Llamadas"].ToString()),
                        TotalInvalidas = int.Parse(subscrb["Total Invalidas"].ToString()),
                        PorcentajeInvalidas = decimal.Parse(subscrb["Porcentaje Invalidas"].ToString()),
                    }).ToList();

                int totalPorNidadCount = reportePorAsesor.Count;
                for (int i = 0; i < totalPorNidadCount; i++)
                {                                                                               
                    invalidas = reportePorAsesor[i].TotalInvalidas;
                    totalDeLlamadas = reportePorAsesor[i].TotalLamadas;
                    if (totalDeLlamadas > 0)
                    {
                        porcentajeInvalidas = ((decimal)(invalidas / (decimal)totalDeLlamadas) * 100);
                        reportePorAsesor[i].PorcentajeInvalidas = Math.Round(porcentajeInvalidas, 0);
                    }

                }



                //string path = HttpContext.Current.Server.MapPath("~/Content/Report/");

                //var reporte = Excel.CreateExcelFileWithDataTable(resultset, HttpContext.Current.Session["UserCode"] + "_reporte Por Unidad", path, "xlsx", true);
                //HttpContext.Current.Session["reportePorAssesorPath"] = path + reporte;
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

            return reportePorAsesor;
        }

        public List<ResumenPorAuditoria> GetReportePorAudit(string DateFrom, string DateTo)
        {
            List<ResumenPorAuditoria> reportePorAsesorList = new List<ResumenPorAuditoria>();
            DataTable resultset = null, resultsetCavnBook = null, resultsetCredit = null;
            ResumenPorAuditoria reportePorAssesor = null;

            DataTable tbl = new DataTable("Reporte Por Auditoria");
            DataColumn[] columnasAuditoria = { 
                new DataColumn(){ColumnName = "Fecha RDV"},
                new DataColumn(){ColumnName = "Rozón Social"},
                new DataColumn(){ColumnName = "Subscriber Id"},
                new DataColumn(){ColumnName = "Canva"},
                new DataColumn(){ColumnName = "Edición"},
                new DataColumn(){ColumnName = "Cargo"},
                new DataColumn(){ColumnName = "Monto"},
                new DataColumn(){ColumnName = "Tarjeta"},
                new DataColumn(){ColumnName = "Ejecutivo"},
                new DataColumn(){ColumnName = "Unidad"},
                new DataColumn(){ColumnName = "P1"},
                new DataColumn(){ColumnName = "P2"},
                new DataColumn(){ColumnName = "P3"},
                new DataColumn(){ColumnName = "P4"},
                new DataColumn(){ColumnName = "P5"},
                new DataColumn(){ColumnName = "P6"},
                new DataColumn(){ColumnName = "P7"},
                new DataColumn(){ColumnName = "P8"},
                new DataColumn(){ColumnName = "P9"},
                new DataColumn(){ColumnName = "P10"},
                new DataColumn(){ColumnName = "Resultado"},
                new DataColumn(){ColumnName = "Es Código 34"},
                new DataColumn(){ColumnName = "Es Descarga Administrativa"},
                new DataColumn(){ColumnName = "Es Descarga Reauditoría"},
                new DataColumn(){ColumnName = "Call Id"},
            };

            tbl.Columns.AddRange(columnasAuditoria);

            try
            {
                OracleParameter[] oracleParameter = new OracleParameter[] { 
                    new OracleParameter("v_DateFrom", OracleDbType.Date) {Value = DateTime.Parse(DateFrom, new CultureInfo("en-US"))},
                    new OracleParameter("v_DateTo", OracleDbType.Date) { Value = DateTime.Parse(DateTo, new CultureInfo("en-US")) },
                    new OracleParameter("ResultSet1", OracleDbType.RefCursor) {Direction = ParameterDirection.Output}
                };

                resultset = oracleBasicsOperations.ExecuteDataAdapter("sp_get_report_by_udit_yn", oracleParameter, CommandType.StoredProcedure, Schema.SFA).Tables[0];

                foreach (DataRow resume in resultset.Rows)
                {
                    reportePorAssesor = new ResumenPorAuditoria();

                    reportePorAssesor.Unidad = resume["REP_UNIT"].ToString();
                    reportePorAssesor.CallId = resume["CALL_ID"].ToString();
                    reportePorAssesor.Ejecutivo = resume["STAFFNAME"].ToString();
                    reportePorAssesor.Canva = resume["CANV_CODE"].ToString();
                    reportePorAssesor.cargo = double.Parse(resume["CARGO"].ToString());
                    reportePorAssesor.EsDescargaAdministrativa = resume["ISDESCARGAADM"].ToString();
                    reportePorAssesor.EsDescargaReauditoria = resume["ISDESCARGAREAUDITORIA"].ToString();
                    reportePorAssesor.FechaRDV = ((DateTime)resume["REP_SALES_DATE"]).ToString("MM/dd/yyyy");
                    reportePorAssesor.isCodigo34 = "No";
                    reportePorAssesor.Monto = double.Parse(resume["REP_NETO"].ToString());
                    reportePorAssesor.P1 = resume["PREGUNTA_1"].ToString();
                    reportePorAssesor.P10 = resume["PREGUNTA_10"].ToString();
                    reportePorAssesor.P2 = resume["PREGUNTA_2"].ToString();
                    reportePorAssesor.P3 = resume["PREGUNTA_3"].ToString();
                    reportePorAssesor.P4 = resume["PREGUNTA_4"].ToString();
                    reportePorAssesor.P5 = resume["PREGUNTA_5"].ToString();
                    reportePorAssesor.P6 = resume["PREGUNTA_6"].ToString();
                    reportePorAssesor.P7 = resume["PREGUNTA_7"].ToString();
                    reportePorAssesor.P8 = resume["PREGUNTA_8"].ToString();
                    reportePorAssesor.P9 = resume["PREGUNTA_9"].ToString();
                    reportePorAssesor.RazonSocial = resume["NAME"].ToString();
                    reportePorAssesor.Result = resume["RESULTADO"].ToString();
                    reportePorAssesor.SubscriberId = resume["REP_SUBSCR_ID"].ToString();
                    reportePorAssesor.Tarjeta = resume["STF_CODIGO"].ToString();
                    reportePorAssesor.Edicion = resume["CANV_EDITION"].ToString();
                    //reportePorAssesor.BookCode = resume["Book_Code"].ToString();

                    resultsetCavnBook = oracleBasicsOperations.ExecuteDataAdapter("sfa.sra_get_subscr_canv_books", new OracleParameter[] { 
                           new OracleParameter("v_SubscrId",OracleDbType.Int32){ Value      = reportePorAssesor.SubscriberId },
                           new OracleParameter("v_CanvCode",OracleDbType.Varchar2) { Value  = reportePorAssesor.Canva },
                           new OracleParameter("v_CanvEdition", OracleDbType.Int32) { Value = reportePorAssesor.Edicion },
                           new OracleParameter("ResultSet", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
                       }, CommandType.StoredProcedure, Schema.SFA).Tables[0];

                    foreach (DataRow rsBook in resultsetCavnBook.Rows)
                    {
                        reportePorAssesor.BookCode = rsBook["Book_Code"].ToString();
                       
                        resultsetCredit = oracleBasicsOperations.ExecuteDataAdapter("YBRDS_PROD.get_subscr_with_credit",
                        new OracleParameter[] { 
                           new OracleParameter("v_subscr_id",OracleDbType.Int32){ Value      = reportePorAssesor.SubscriberId },
                           new OracleParameter("v_canv_edition",OracleDbType.Int32) { Value  = reportePorAssesor.Edicion },
                           new OracleParameter("v_canv_code", OracleDbType.Varchar2) { Value = reportePorAssesor.Canva },
                           new OracleParameter("v_book_code", OracleDbType.Varchar2) { Value = reportePorAssesor.BookCode },
                           new OracleParameter("ref_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
                       }, CommandType.StoredProcedure, Schema.YBRDS_PROD).Tables[0];

                        if (resultsetCredit.Rows.Count > 0)
                        {
                            reportePorAssesor.isCodigo34 = "Si";
                        }
                    }

                    reportePorAsesorList.Add(reportePorAssesor);

                    var row = tbl.NewRow();

                    row["Fecha RDV"] = reportePorAssesor.FechaRDV;
                    row["Rozón Social"] = reportePorAssesor.RazonSocial;
                    row["Subscriber Id"] = reportePorAssesor.SubscriberId;
                    row["Canva"] = reportePorAssesor.Canva;
                    row["Edición"] = reportePorAssesor.Edicion;
                    row["Cargo"] = reportePorAssesor.cargo;
                    row["Monto"] = reportePorAssesor.Monto;
                    row["Tarjeta"] = reportePorAssesor.Tarjeta;
                    row["Ejecutivo"] = reportePorAssesor.Ejecutivo;
                    row["Unidad"] = reportePorAssesor.Unidad;
                    row["P1"] = reportePorAssesor.P1;
                    row["P2"] = reportePorAssesor.P2;
                    row["P3"] = reportePorAssesor.P3;
                    row["P4"] = reportePorAssesor.P4;
                    row["P5"] = reportePorAssesor.P5;
                    row["P6"] = reportePorAssesor.P6;
                    row["P7"] = reportePorAssesor.P7;
                    row["P8"] = reportePorAssesor.P8;
                    row["P9"] = reportePorAssesor.P9;
                    row["P10"] = reportePorAssesor.P10;
                    row["Resultado"] = reportePorAssesor.Result;
                    row["Es Código 34"] = reportePorAssesor.isCodigo34;
                    row["Es Descarga Administrativa"] = reportePorAssesor.EsDescargaAdministrativa;
                    row["Es Descarga Reauditoría"] = reportePorAssesor.EsDescargaReauditoria;
                    row["Call Id"] = reportePorAssesor.CallId;

                    tbl.Rows.Add(row);
                                          
                }

                string path = HttpContext.Current.Server.MapPath("~/Content/Report/");

                var reporte = Excel.CreateExcelFileWithDataTable(tbl, HttpContext.Current.Session["UserCode"] + "_reporte Por Auditoria", path, "xlsx", true);
                HttpContext.Current.Session["reportePorAuditoriaPath"] = path + reporte;
            
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

            return reportePorAsesorList;
        }

        public List<ResumenPorDatoVital> GetReportePorDatoVital(string DateFrom, string DateTo)
        {
            List<ResumenPorDatoVital> resumentPorDatoVital = null;
            DataSet resultset = null;
            decimal cargoInvalidoVsTotalCargo = 0;
            decimal InvalidasVsTotalAuditadas = 0;

            try
            {                
                OracleParameter[] oracleParameter = new OracleParameter[] { 
                    new OracleParameter("v_DateFrom", OracleDbType.Date) {Value = DateTime.Parse(DateFrom, new CultureInfo("en-US"))},
                    new OracleParameter("v_DateTo", OracleDbType.Date) { Value = DateTime.Parse(DateTo, new CultureInfo("en-US")) },
                    new OracleParameter("ResultSet", OracleDbType.RefCursor) {Direction = ParameterDirection.Output},
                };

                resultset = oracleBasicsOperations.ExecuteDataAdapter("sp_get_stat_by_dato_vital_yn", oracleParameter, CommandType.StoredProcedure, Schema.SFA);

                resumentPorDatoVital = resultset.Tables[0].AsEnumerable().Select(
                    subscrb => new ResumenPorDatoVital
                    {
                        Unidad = (subscrb["Unidad"].ToString() == null ? "Total:" : subscrb["Unidad"].ToString()),
                        TotalLlamadas = int.Parse(subscrb["Total de Llamadas Auditadas"].ToString()),
                        CargoTotal = int.Parse(subscrb["Cargo Total"].ToString()),
                        Grabaciones = int.Parse(subscrb["Grabaciones Invalidas"].ToString()),
                        CargoInvalido = int.Parse(subscrb["Cargo Invalido"].ToString()),
                        InvalidasVsTotalAuditadas = int.Parse(subscrb["Invalidas vs Total Auditadas"].ToString()),
                        CargoInvalidoVsTotalCargo = int.Parse(subscrb["Cargo Invalido vs Total Cargo"].ToString()),
                        Pregunta3 = int.Parse(subscrb["Pregunta 3"].ToString()),
                        Pregunta4 = int.Parse(subscrb["Pregunta 4"].ToString()),
                        Pregunta5 = int.Parse(subscrb["Pregunta 5"].ToString()),
                        Pregunta6 = int.Parse(subscrb["Pregunta 6"].ToString()),
                        Pregunta7 = int.Parse(subscrb["Pregunta 7"].ToString()),
                        Pregunta8 = int.Parse(subscrb["Pregunta 8"].ToString()),
                        Pregunta9 = int.Parse(subscrb["Pregunta 9"].ToString())
                    }).ToList();


                if (resumentPorDatoVital.Count > 0)
                {
                    resultset.Tables[0].Rows[resultset.Tables[0].Rows.Count - 1]["Unidad"] = "Total";

                    int totalPorNidadCount = resumentPorDatoVital.Count;
                    for (int i = 0; i < totalPorNidadCount; i++)
                    {
                        resumentPorDatoVital[i].TotalDeIncidencias = (
                            resumentPorDatoVital[i].Pregunta4 +
                            resumentPorDatoVital[i].Pregunta5 +
                            resumentPorDatoVital[i].Pregunta6 +
                            resumentPorDatoVital[i].Pregunta7 +
                            resumentPorDatoVital[i].Pregunta8 +
                            resumentPorDatoVital[i].Pregunta9);

                        //calculo de los porcientos
                        cargoInvalidoVsTotalCargo = ((decimal)resumentPorDatoVital[i].Grabaciones / (decimal)resumentPorDatoVital[i].TotalLlamadas) * 100;
                        resumentPorDatoVital[i].InvalidasVsTotalAuditadas = (int)Math.Round(cargoInvalidoVsTotalCargo, 0);

                        InvalidasVsTotalAuditadas = ((decimal)resumentPorDatoVital[i].CargoInvalido / (decimal)resumentPorDatoVital[i].CargoTotal) * 100;
                        resumentPorDatoVital[i].CargoInvalidoVsTotalCargo = (int)Math.Round(InvalidasVsTotalAuditadas,0);

                    }
                }


                //string path = HttpContext.Current.Server.MapPath("~/Content/Report/");

                //var reporte = Excel.CreateExcelFileWithDataTable(resultset.Tables[0], HttpContext.Current.Session["UserCode"] + "_reporte Por Dato Vital", path, "xlsx", true);
                //HttpContext.Current.Session["reportePorDatpVital"] = path + reporte;

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

            return resumentPorDatoVital;
        }
    
    }
}
