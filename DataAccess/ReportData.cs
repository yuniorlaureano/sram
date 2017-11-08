using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Entities;
using DataAccess.Repository;
using AppUtil.Data;
using ClosedXML;
using System.Web;
using System.Web.Hosting;
using System.Globalization;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;


namespace DataAccess
{
    public class ReportData
    {

        private OracleBasicsOperations oracleBasicsOperations = null;

        public ReportData()
        {
            oracleBasicsOperations = new OracleBasicsOperations();
        }

        public List<ResumenPorUnidad> GetReportByUnit(string dateFrom, string dateTo)
        {
            List<ResumenPorUnidad> resumentPorUnidad = new List<ResumenPorUnidad>();
            OracleDataReader resultset = null;
            decimal porcementajeValido = 0;
            decimal porcementajeInvalido = 0;
            int totalDeLlamadas = 0;
            int validas = 0;
            int invalidas = 0;

            try
            {

                OracleParameter[] oracleParameter = new OracleParameter[] { 
                    new OracleParameter {ParameterName = "v_DateFrom", OracleDbType = OracleDbType.Varchar2, Value = dateFrom},
                    new OracleParameter {ParameterName = "v_DateTo", OracleDbType = OracleDbType.Varchar2, Value = dateTo},
                    new OracleParameter {ParameterName = "resultset", OracleDbType= OracleDbType.RefCursor, Direction = ParameterDirection.Output}
                };

                resultset = oracleBasicsOperations.ExecuteDataReader("sram.sp_get_stat_by_unit", oracleParameter, CommandType.StoredProcedure, DataAccess.Repository.Schema.SFA);

                
                while(resultset.Read())
                {
                    resumentPorUnidad.Add(
                        new ResumenPorUnidad() {
                            Unidad = resultset["UNIDAD"].ToString(),
                            Calificacion = resultset["CALIFICACION"].ToString(),
                            TotalLamadas = int.Parse(resultset["Total de Llamadas"].ToString()),
                            Porcentaje = decimal.Parse(resultset["PORCENTAJE"].ToString()),
                            NiTotal = double.Parse(resultset["NI Total"].ToString())
                        });
                }

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
            }
            catch (OracleException excep)
            {
                throw excep;
            }
            finally
            {
                if (resultset != null)
                {
                    resultset.Close();
                }

                this.oracleBasicsOperations.CloseConnection();
            }

            return resumentPorUnidad;
        }

        public List<ResumenPorAsesor> GetReportByAsesor(string dateFrom, string dateTo)
        {
            List<ResumenPorAsesor> reportePorAsesor = new List<ResumenPorAsesor>();
            OracleDataReader resultset = null;
            decimal porcentajeInvalidas = 0;
            int totalDeLlamadas = 0;
            int invalidas = 0;

            try
            {

                OracleParameter[] oracleParameter = new OracleParameter[] { 
                    new OracleParameter {ParameterName = "v_DateFrom", OracleDbType = OracleDbType.Varchar2, Value = dateFrom},
                    new OracleParameter {ParameterName = "v_DateTo", OracleDbType = OracleDbType.Varchar2, Value = dateTo},
                    new OracleParameter {ParameterName = "resultset", OracleDbType= OracleDbType.RefCursor, Direction = ParameterDirection.Output}
                };

                resultset = oracleBasicsOperations.ExecuteDataReader("sram.sp_get_stat_by_assesor", oracleParameter, CommandType.StoredProcedure, DataAccess.Repository.Schema.SFA);

                while(resultset.Read())
                {
                    reportePorAsesor.Add(new ResumenPorAsesor() {

                        Unidad = resultset["Unidad"].ToString(),
                        Ejecutivo = resultset["Ejecutivo"].ToString(),
                        TotalLamadas = int.Parse(resultset["Total de Llamadas"].ToString()),
                        TotalInvalidas = int.Parse(resultset["Total Invalidas"].ToString()),
                        PorcentajeInvalidas = decimal.Parse(resultset["Porcentaje Invalidas"].ToString())
                    });
                }

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

            }
            catch (OracleException excep)
            {
                throw excep;
            }
            finally
            {
                if (resultset != null)
                {
                    resultset.Close();
                }

                this.oracleBasicsOperations.CloseConnection();
            }

            return reportePorAsesor;
        }

        public List<ResumenPorAuditoria> GetReportByAuditory(string dateFrom, string dateTo, string userCode, string path)
        {
            List<ResumenPorAuditoria> reportePorAsesorList = new List<ResumenPorAuditoria>();
            ResumenPorAuditoria reportePorAssesor = null;
            OracleDataReader reader = null;

            try
            {
                OracleParameter[] oracleParameter = new OracleParameter[] { 
                    new OracleParameter {ParameterName = "v_DateFrom", OracleDbType = OracleDbType.Varchar2, Value = dateFrom},
                    new OracleParameter {ParameterName = "v_DateTo", OracleDbType = OracleDbType.Varchar2, Value = dateTo},
                    new OracleParameter {ParameterName = "resultset", OracleDbType= OracleDbType.RefCursor, Direction = ParameterDirection.Output}
                };

                reader = oracleBasicsOperations.ExecuteDataReader("sram.sp_get_report_by_audit", oracleParameter, CommandType.StoredProcedure, DataAccess.Repository.Schema.SFA);

                while(reader.Read())
                {
                    reportePorAssesor = new ResumenPorAuditoria();

                    reportePorAssesor.Unidad = reader["REP_UNIT"].ToString();
                    reportePorAssesor.CallId = reader["CALL_ID"].ToString();
                    reportePorAssesor.Ejecutivo = reader["STAFFNAME"].ToString();
                    reportePorAssesor.Canva = reader["CANV_CODE"].ToString();
                    reportePorAssesor.cargo = double.Parse(reader["CARGO"].ToString());
                    reportePorAssesor.EsDescargaAdministrativa = reader["ISDESCARGAADM"].ToString();
                    reportePorAssesor.EsDescargaReauditoria = reader["ISDESCARGAREAUDITORIA"].ToString();
                    reportePorAssesor.FechaRDV = ((DateTime)reader["REP_SALES_DATE"]).ToString("MM/dd/yyyy");
                    reportePorAssesor.isCodigo34 = reader["is_34_code"].ToString();
                    reportePorAssesor.Monto = double.Parse(reader["REP_NETO"].ToString());
                    reportePorAssesor.P1 = reader["PREGUNTA_1"].ToString();
                    reportePorAssesor.P10 = reader["PREGUNTA_10"].ToString();
                    reportePorAssesor.P2 = reader["PREGUNTA_2"].ToString();
                    reportePorAssesor.P3 = reader["PREGUNTA_3"].ToString();
                    reportePorAssesor.P4 = reader["PREGUNTA_4"].ToString();
                    reportePorAssesor.P5 = reader["PREGUNTA_5"].ToString();
                    reportePorAssesor.P6 = reader["PREGUNTA_6"].ToString();
                    reportePorAssesor.P7 = reader["PREGUNTA_7"].ToString();
                    reportePorAssesor.P8 = reader["PREGUNTA_8"].ToString();
                    reportePorAssesor.P9 = reader["PREGUNTA_9"].ToString();
                    reportePorAssesor.RazonSocial = reader["NAME"].ToString();
                    reportePorAssesor.Result = reader["RESULTADO"].ToString();
                    reportePorAssesor.SubscriberId = reader["REP_SUBSCR_ID"].ToString();
                    reportePorAssesor.Tarjeta = reader["STF_CODIGO"].ToString();
                    reportePorAssesor.Edicion = reader["CANV_EDITION"].ToString();
                    reportePorAssesor.BookCode = reader["Book_Code"].ToString();

                    reportePorAsesorList.Add(reportePorAssesor);
                }
                            
            }
            catch (OracleException excep)
            {
                throw excep;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();                    
                }

                this.oracleBasicsOperations.CloseConnection();
            }

            return reportePorAsesorList;
        }

        public List<ResumenPorDatoVital> GetReportByVitalData(string dateFrom, string dateTo)
        {
            List<ResumenPorDatoVital> resumentPorDatoVital = new List<ResumenPorDatoVital>();
            OracleDataReader resultset = null;
            decimal cargoInvalidoVsTotalCargo = 0;
            decimal InvalidasVsTotalAuditadas = 0;

            try
            {                
                OracleParameter[] oracleParameter = new OracleParameter[] { 
                    new OracleParameter {ParameterName = "v_DateFrom", OracleDbType = OracleDbType.Varchar2, Value = dateFrom},
                    new OracleParameter {ParameterName = "v_DateTo", OracleDbType = OracleDbType.Varchar2, Value = dateTo},
                    new OracleParameter {ParameterName = "resultset", OracleDbType= OracleDbType.RefCursor, Direction = ParameterDirection.Output}
                };

                resultset = oracleBasicsOperations.ExecuteDataReader("sram.sp_get_stat_by_dato_vital", oracleParameter, CommandType.StoredProcedure, DataAccess.Repository.Schema.SFA);

                while(resultset.Read())
                {
                    resumentPorDatoVital.Add(new ResumenPorDatoVital() {

                        Unidad = (resultset["Unidad"].ToString() == null ? "Total:" : resultset["Unidad"].ToString()),
                        TotalLlamadas = int.Parse(resultset["Total de Llamadas Auditadas"].ToString()),
                        CargoTotal = int.Parse(resultset["Cargo Total"].ToString()),
                        Grabaciones = int.Parse(resultset["Grabaciones Invalidas"].ToString()),
                        CargoInvalido = int.Parse(resultset["Cargo Invalido"].ToString()),
                        InvalidasVsTotalAuditadas = int.Parse(resultset["Invalidas vs Total Auditadas"].ToString()),
                        CargoInvalidoVsTotalCargo = int.Parse(resultset["Cargo Invalido vs Total Cargo"].ToString()),
                        Pregunta3 = int.Parse(resultset["Pregunta 3"].ToString()),
                        Pregunta4 = int.Parse(resultset["Pregunta 4"].ToString()),
                        Pregunta5 = int.Parse(resultset["Pregunta 5"].ToString()),
                        Pregunta6 = int.Parse(resultset["Pregunta 6"].ToString()),
                        Pregunta7 = int.Parse(resultset["Pregunta 7"].ToString()),
                        Pregunta8 = int.Parse(resultset["Pregunta 8"].ToString()),
                        Pregunta9 = int.Parse(resultset["Pregunta 9"].ToString())                    
                    });
                }


                if (resumentPorDatoVital.Count > 0)
                {
                    resumentPorDatoVital[resumentPorDatoVital.Count - 1].Unidad = "Total";

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
            }
            catch (OracleException excep)
            {
                throw excep;
            }
            finally
            {
                if (resultset != null)
                {
                    resultset.Close();
                }

                this.oracleBasicsOperations.CloseConnection();
            }

            return resumentPorDatoVital;
        }
        
        public void WriteEcelHeader(string[] headers, OpenXmlWriter writer)
        {
            List<OpenXmlAttribute> row = new List<OpenXmlAttribute> { new OpenXmlAttribute("r", null, "1") };
            writer.WriteStartElement(new Row(), row);

            List<OpenXmlAttribute> cell = new List<OpenXmlAttribute> { new OpenXmlAttribute("t", null, "inlineStr") };

            foreach (string h in headers)
            {
                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(h)));
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    
        public void WriterAuditoriExcelValue(List<ResumenPorAuditoria> auditorias, OpenXmlWriter writer)
        {
            int currentRow = 0;
            List<OpenXmlAttribute> row;
            List<OpenXmlAttribute> cell = new List<OpenXmlAttribute> { new OpenXmlAttribute("t", null, "inlineStr") };

            for (int i = 1; i <= auditorias.Count; i++)
            {
                currentRow = i - 1;

                row = new List<OpenXmlAttribute> { new OpenXmlAttribute("r", null, (i + 1).ToString()) };
                writer.WriteStartElement(new Row(), row);

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].FechaRDV)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].RazonSocial)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].SubscriberId)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].Canva)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].Edicion)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].cargo.ToString())));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].Monto.ToString())));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].Tarjeta)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].Ejecutivo)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].Unidad)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].P1)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].P2)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].P3)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].P4)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].P5)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].P6)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].P7)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].P8)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].P9)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].P10)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].Result)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].isCodigo34)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].EsDescargaAdministrativa)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].EsDescargaReauditoria)));
                writer.WriteEndElement();

                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(auditorias[currentRow].CallId)));
                writer.WriteEndElement();
                
                writer.WriteEndElement();
            }
        }

        public void WriteAuditoryToExcel(List<ResumenPorAuditoria> auditorias, string sheetName, string fileName, string path)
        {            
            string archivo = path + fileName + ".xlsx";
    
            using(SpreadsheetDocument workbook = SpreadsheetDocument.Create(archivo, SpreadsheetDocumentType.Workbook))
            {
                OpenXmlWriter writer;

                workbook.AddWorkbookPart();
                WorksheetPart wsp = workbook.WorkbookPart.AddNewPart<WorksheetPart>();

                writer = OpenXmlWriter.Create(wsp);
                writer.WriteStartElement(new Worksheet());
                writer.WriteStartElement(new SheetData());

                WriteEcelHeader(new string[]{"Fecha RDV", "Rozón Social", "Subscriber Id", "Canva", "Edición", "Cargo", "Monto", "Tarjeta", "Ejecutivo", "Unidad", "P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "P9", "P10", "Resultado", "Es Código 34", "Es Descarga Administrativa", "Es Descarga Reauditoría", "Call Id"}, writer);
                WriterAuditoriExcelValue(auditorias, writer);

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Close();

                writer = OpenXmlWriter.Create(workbook.WorkbookPart);
                writer.WriteStartElement(new Workbook());
                writer.WriteStartElement(new Sheets());

                writer.WriteElement(new Sheet()
                {
                    Name = sheetName,
                    SheetId = 1,
                    Id = workbook.WorkbookPart.GetIdOfPart(wsp)
                });

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Close();

                workbook.Close();
            }

        }
    }
}
