using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using DataAccess;
using DocumentFormat;
using ClosedXML;
using ClosedXML.Excel;
using System.Web;
using System.Web.Hosting;

namespace BisnessLogic
{
    public class ReportBusiness
    {
        /// <summary>
        /// Permite obtener el reporte por unidad
        /// </summary>
        /// <param name="dateFrom">fecha de donde se tomara</param>
        /// <param name="dateTo">fecha hasta don de se tomara</param>
        /// <param name="userCode">userio logueado en sistema</param>
        /// <param name="path">ruta donde se guardara el reporte</param>
        /// <returns>void</returns>
        public void GetReportByUnit(string dateFrom, string dateTo, string userCode, string path)
        {
            List<ResumenPorUnidad> listaPorUnidad = new ReportData().GetReportByUnit(dateFrom, dateTo);

            if (listaPorUnidad != null)
            {
                GetReportByUnit(listaPorUnidad, userCode, path);
            }
        }

        /// <summary>
        /// Permite obtener el reporte por asesor
        /// </summary>
        /// <param name="dateFrom">fecha de donde se tomara</param>
        /// <param name="dateTo">fecha hasta don de se tomara</param>
        /// <param name="userCode">userio logueado en sistema</param>
        /// <param name="path">ruta donde se guardara el reporte</param>
        /// <returns>void</returns>
        public void GetReportByAsesor(string dateFrom, string dateTo, string userCode, string path)
        {

            List<ResumenPorAsesor> listaPorAsesor = new ReportData().GetReportByAsesor(dateFrom, dateTo);

            if (listaPorAsesor != null)
            {
                GetReportByAsesor(listaPorAsesor, userCode, path);
            }
        }

        /// <summary>
        /// Permite obtener el reporte por auditoria
        /// </summary>
        /// <param name="dateFrom">fecha de donde se tomara</param>
        /// <param name="dateTo">fecha hasta don de se tomara</param>
        /// <param name="userCode">userio logueado en sistema</param>
        /// <param name="path">ruta donde se guardara el reporte</param>
        /// <returns>void</returns>
        public void GetReportByAuditory(string dateFrom, string dateTo, string userCode, string path)
        {
            new ReportData().GetReportByAuditory(dateFrom, dateTo, userCode, path);
        }

        /// <summary>
        /// Permite obtener el reporte por dato vital
        /// </summary>
        /// <param name="dateFrom">fecha de donde se tomara</param>
        /// <param name="dateTo">fecha hasta don de se tomara</param>
        /// <param name="userCode">userio logueado en sistema</param>
        /// <param name="path">ruta donde se guardara el reporte</param>
        /// <returns>List<ResumenPorDatoVital></returns>
        public void GetReportByVitalData(string dateFrom, string dateTo, string userCode, string path)
        {
            List<ResumenPorDatoVital> listaPorDatoVital = new ReportData().GetReportByVitalData(dateFrom, dateTo);

            if (listaPorDatoVital != null)
            {
                GetReportByVitalData(listaPorDatoVital, userCode, path);
            }
        }

        private void GetReportByUnit(List<ResumenPorUnidad> listaPorUnidad, string userCode, string path)
        {
            string file = path + userCode + "_reporte Por Unidad.xlsx";

            int row = 0;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Reporte Por Unidad");


            ws.Cell(1, 1).Value = "Resumen de Desempeño Por Unidad";
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range("A1:E1").Merge();
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Cell(1, 1).Style.Font.Bold = true;

            ws.Cell(3, 1).Value = "Unidad";
            ws.Cell(3, 2).Value = "Calificación";
            ws.Cell(3, 3).Value = "Total de Llamadas";
            ws.Cell(3, 4).Value = "Porcentaje";
            ws.Cell(3, 5).Value = "NI Total";


            ws.Column("B").Width = 17;

            int count = listaPorUnidad.Count - 1;
            for (int i = 0; i <= count; i++)
            {

                row = i + 4;

                ws.Cell(row, 1).Value = listaPorUnidad[i].Unidad;
                ws.Cell(row, 3).Value = listaPorUnidad[i].TotalLamadas;

                if (listaPorUnidad[i].Porcentaje != -1)
                {
                    if (listaPorUnidad[i].Porcentaje > 20)
                    {
                        ws.Cell(row, 4).Value = listaPorUnidad[i].Porcentaje + "%";
                        ws.Range("A" + (row) + ":E" + (row) + "").Cells().Style.Font.FontColor = XLColor.Red;
                    }
                    else
                    {
                        ws.Cell(row, 4).Value = listaPorUnidad[i].Porcentaje + "%";
                    }
                }
                //Total por unidad:
                if (listaPorUnidad[i].Calificacion == "Total por unidad:")
                {
                    ws.Range("A" + (row) + ":E" + (row) + "").Cells().Style.Font.Bold = true;
                    if (i != count)
                    {
                        ws.Cell(row, 2).Value = listaPorUnidad[i].Calificacion;
                    }
                    else
                    {
                        ws.Cell(row, 2).Value = "Total:";
                    }
                }
                else
                {
                    ws.Cell(row, 2).Value = listaPorUnidad[i].Calificacion;
                }


                ws.Cell(row, 5).Value = listaPorUnidad[i].NiTotal;
                ws.Cell(row, 5).Style.NumberFormat.Format = "#,###";
            }

            ws.Range("A3:A" + (count + 3) + "").Rows().Style.Border.LeftBorder = XLBorderStyleValues.Thick;
            ws.Range("A3:A" + (count + 3) + "").Rows().Style.Border.LeftBorderColor = XLColor.DarkBlue;

            ws.Range("E2:E" + (count + 3) + "").Rows().Style.Border.RightBorder = XLBorderStyleValues.Thick;
            ws.Range("E2:E" + (count + 3) + "").Rows().Style.Border.RightBorderColor = XLColor.DarkBlue;

            ws.Range("A" + (count + 3) + ":E" + (count + 3) + "").Cells().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            ws.Range("A" + (count + 3) + ":E" + (count + 3) + "").Cells().Style.Border.BottomBorderColor = XLColor.DarkBlue;

            ws.Range("A2:E3").Cells().Style.Fill.BackgroundColor = XLColor.DarkBlue;
            ws.Range("A3:E3").Cells().Style.Font.FontColor = XLColor.White;
            ws.Column("D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;



            wb.SaveAs(file);
            wb.Dispose();
            //HttpContext.Current.Session["reportePorUnidadPath"] = file;

        }

        private void GetReportByVitalData(List<ResumenPorDatoVital> listaPorDatoVital, string userCode, string path)
        {
            string file = path + userCode + "_reporte Por dato vital.xlsx";

            int row = 0;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Reporte Por Dato Vital");


            ws.Cell(1, 1).Value = "Resumen Por Dato Vital";
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range("A1:O1").Merge();
            ws.Cell(1, 1).Style.Font.FontSize = 18;
            ws.Cell(1, 1).Style.Font.Bold = true;

            ws.Cell(2, 8).Value = "Número de inicidencias en No Cumplimiento de Datos";
            ws.Cell(2, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(2, 8).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range("H2:O2").Merge();
            ws.Cell(2, 8).Style.Font.FontSize = 12;
            ws.Cell(2, 8).Style.Font.Bold = true;


            ws.Cell(3, 1).Value = "Unidad";
            ws.Cell(3, 2).Value = "Total de Llamadas Auditadas";
            ws.Cell(3, 3).Value = "Cargo Total";
            ws.Cell(3, 4).Value = "Grabaciones Inválidas";
            ws.Cell(3, 5).Value = "Cargo Inválido";
            ws.Cell(3, 6).Value = "% Inválidas Vs Total Llamadas";
            ws.Cell(3, 7).Value = "% Cargo Inválido Vs Total Cargo";
            ws.Cell(3, 8).Value = "PDC se Indentifica con su Nombre Completo?";
            ws.Cell(3, 9).Value = "PDC Indica su Cargo o Posición en la Empresa?";
            ws.Cell(3, 10).Value = "PDC Confirma, Es la Persona Autorizada a Negociar Contratos de Publicidad para?";
            ws.Cell(3, 11).Value = "Vendedor Detalla monto mensual?";
            ws.Cell(3, 12).Value = "Vendedor Especifica Vigencia de la Facturación?";
            ws.Cell(3, 13).Value = "Vendedor Especifica Directorio Contratado?";
            ws.Cell(3, 14).Value = "PDC Confirma Estar de Acuerdo?";
            ws.Cell(3, 15).Value = "Total de Incidencias";
            //6,15,10,11,11,12,13,20,19,27,13,27,18,14,9
            //ws.Column("B").Width = 17;
            ws.Column("A").Width = 6;
            ws.Column("A").Style.Alignment.WrapText = true;
            ws.Column("B").Width = 15;
            ws.Column("B").Style.Alignment.WrapText = true;
            ws.Column("C").Width = 10;
            ws.Column("C").Style.Alignment.WrapText = true;
            ws.Column("D").Width = 11;
            ws.Column("D").Style.Alignment.WrapText = true;
            ws.Column("E").Width = 11;
            ws.Column("E").Style.Alignment.WrapText = true;
            ws.Column("F").Width = 12;
            ws.Column("F").Style.Alignment.WrapText = true;
            ws.Column("G").Width = 13;
            ws.Column("G").Style.Alignment.WrapText = true;
            ws.Column("H").Width = 20;
            ws.Column("H").Style.Alignment.WrapText = true;
            ws.Column("I").Width = 19;
            ws.Column("I").Style.Alignment.WrapText = true;
            ws.Column("J").Width = 27;
            ws.Column("J").Style.Alignment.WrapText = true;
            ws.Column("K").Width = 13;
            ws.Column("K").Style.Alignment.WrapText = true;
            ws.Column("L").Width = 27;
            ws.Column("L").Style.Alignment.WrapText = true;
            ws.Column("M").Width = 18;
            ws.Column("M").Style.Alignment.WrapText = true;
            ws.Column("N").Width = 14;
            ws.Column("N").Style.Alignment.WrapText = true;
            ws.Column("O").Width = 9;
            ws.Column("O").Style.Alignment.WrapText = true;

            int count = listaPorDatoVital.Count - 1;
            for (int i = 0; i <= count; i++)
            {

                row = i + 4;

                ws.Cell(row, 1).Value = listaPorDatoVital[i].Unidad;
                ws.Cell(row, 2).Value = listaPorDatoVital[i].TotalLlamadas;
                ws.Cell(row, 3).Value = listaPorDatoVital[i].CargoTotal;
                ws.Cell(row, 4).Value = listaPorDatoVital[i].Grabaciones;
                ws.Cell(row, 5).Value = listaPorDatoVital[i].CargoInvalido;
                ws.Cell(row, 6).Value = listaPorDatoVital[i].InvalidasVsTotalAuditadas + "%";
                ws.Cell(row, 7).Value = listaPorDatoVital[i].CargoInvalidoVsTotalCargo + "%";
                ws.Cell(row, 8).Value = listaPorDatoVital[i].Pregunta3;
                ws.Cell(row, 9).Value = listaPorDatoVital[i].Pregunta4;
                ws.Cell(row, 10).Value = listaPorDatoVital[i].Pregunta5;
                ws.Cell(row, 11).Value = listaPorDatoVital[i].Pregunta6;
                ws.Cell(row, 12).Value = listaPorDatoVital[i].Pregunta7;
                ws.Cell(row, 13).Value = listaPorDatoVital[i].Pregunta8;
                ws.Cell(row, 14).Value = listaPorDatoVital[i].Pregunta9;
                ws.Cell(row, 15).Value = listaPorDatoVital[i].TotalDeIncidencias;

                ws.Cell(row, 3).Style.NumberFormat.Format = "#,###";
                ws.Cell(row, 5).Style.NumberFormat.Format = "#,###";


                if (listaPorDatoVital[i].TotalDeIncidencias > 9)
                {
                    ws.Range("H" + row + ":O" + row + "").Cells().Style.Fill.BackgroundColor = XLColor.Orange;
                }

            }

            if (count > 0)
            {
                if (listaPorDatoVital[count].Pregunta3 > 9)
                {
                    ws.Range("H4:H" + (count + 4) + "").Columns().Style.Font.FontColor = XLColor.Red;
                }

                if (listaPorDatoVital[count].Pregunta4 > 9)
                {
                    ws.Range("I4:I" + (count + 4) + "").Columns().Style.Font.FontColor = XLColor.Red;
                }

                if (listaPorDatoVital[count].Pregunta5 > 9)
                {
                    ws.Range("J4:J" + (count + 4) + "").Columns().Style.Font.FontColor = XLColor.Red;
                }

                if (listaPorDatoVital[count].Pregunta6 > 9)
                {
                    ws.Range("K4:K" + (count + 4) + "").Columns().Style.Font.FontColor = XLColor.Red;
                }

                if (listaPorDatoVital[count].Pregunta7 > 9)
                {
                    ws.Range("L4:L" + (count + 4) + "").Columns().Style.Font.FontColor = XLColor.Red;
                }

                if (listaPorDatoVital[count].Pregunta8 > 9)
                {
                    ws.Range("M4:M" + (count + 4) + "").Columns().Style.Font.FontColor = XLColor.Red;
                }

                if (listaPorDatoVital[count].Pregunta9 > 9)
                {
                    ws.Range("N4:N" + (count + 4) + "").Columns().Style.Font.FontColor = XLColor.Red;
                }
            }            

            ws.Range("A3:A" + (count + 3) + "").Rows().Style.Border.LeftBorder = XLBorderStyleValues.Thick;
            ws.Range("A3:A" + (count + 3) + "").Rows().Style.Border.LeftBorderColor = XLColor.DarkBlue;

            ws.Range("O3:O" + (count + 3) + "").Rows().Style.Border.RightBorder = XLBorderStyleValues.Thick;
            ws.Range("O3:O" + (count + 3) + "").Rows().Style.Border.RightBorderColor = XLColor.Red;

            ws.Range("A" + (count + 3) + ":G" + (count + 3) + "").Cells().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            ws.Range("A" + (count + 3) + ":G" + (count + 3) + "").Cells().Style.Border.BottomBorderColor = XLColor.DarkBlue;

            ws.Range("F" + (count + 3) + ":O" + (count + 3) + "").Cells().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            ws.Range("F" + (count + 3) + ":O" + (count + 3) + "").Cells().Style.Border.BottomBorderColor = XLColor.DarkBlue;

            ws.Range("A3:G3").Cells().Style.Fill.BackgroundColor = XLColor.DarkBlue;
            ws.Range("A3:G3").Cells().Style.Font.FontColor = XLColor.White;

            ws.Range("H3:O3").Cells().Style.Fill.BackgroundColor = XLColor.Red;
            ws.Range("H3:O3").Cells().Style.Font.FontColor = XLColor.White;



            wb.SaveAs(file);
            wb.Dispose();
        }

        private void GetReportByAsesor(List<ResumenPorAsesor> listaPorUnidad, string userCode, string path)
        {
            string file = path + userCode + "_reporte Por ejecutivo.xlsx";

            int row = 0;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Reporte Por Ejecutivo");


            ws.Cell(1, 1).Value = "Total de Grabaciones Inválidas Por Ejecutivo";
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range("A1:E1").Merge();
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Cell(1, 1).Style.Font.Bold = true;

            ws.Cell(3, 1).Value = "Unidad";
            ws.Cell(3, 2).Value = "Ejecutivo";
            ws.Cell(3, 3).Value = "Total de Llamadas";
            ws.Cell(3, 4).Value = "Total Inválidas";
            ws.Cell(3, 5).Value = "Porcentaje Inválidas";


            ws.Column("A").Width = 8;
            ws.Column("B").Width = 17;
            ws.Column("C").Width = 16;
            ws.Column("D").Width = 13;
            ws.Column("E").Width = 18;

            int count = listaPorUnidad.Count - 1;
            for (int i = 0; i <= count; i++)
            {

                row = i + 4;


                if (i == 0)
                {
                    ws.Cell(row, 1).Value = listaPorUnidad[i].Unidad;
                }
                else
                {
                    if (listaPorUnidad[i - 1].Ejecutivo == "TOTAL:")
                    {
                        ws.Cell(row, 1).Value = listaPorUnidad[i].Unidad;
                    }
                }



                ws.Cell(row, 2).Value = listaPorUnidad[i].Ejecutivo;
                ws.Cell(row, 3).Value = listaPorUnidad[i].TotalLamadas;
                ws.Cell(row, 4).Value = listaPorUnidad[i].TotalInvalidas;
                ws.Cell(row, 5).Value = listaPorUnidad[i].PorcentajeInvalidas + "%";

                if (listaPorUnidad[i].PorcentajeInvalidas > 20)
                {
                    ws.Cell(row, 5).Value = listaPorUnidad[i].PorcentajeInvalidas + "%";
                    ws.Range("A" + (row) + ":E" + (row) + "").Cells().Style.Font.FontColor = XLColor.Red;
                }

                if (listaPorUnidad[i].Ejecutivo == "TOTAL:" && count != i)
                {
                    ws.Range("A" + row + ":E" + row + "").Cells().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    ws.Range("A" + row + ":E" + row + "").Cells().Style.Border.BottomBorderColor = XLColor.DarkBlue;

                    ws.Range("A" + row + ":E" + row + "").Cells().Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("A" + row + ":E" + row + "").Cells().Style.Border.TopBorderColor = XLColor.DarkBlue;

                }

                if (listaPorUnidad[i].Ejecutivo == "TOTAL:")
                {
                    ws.Cell(row, 1).Value = listaPorUnidad[i].Unidad;
                    ws.Range("A" + row + ":E" + row + "").Cells().Style.Font.Bold = true;
                    ws.Range("A" + row + ":E" + row + "").Cells().Style.Font.Bold = true;

                }
            }

            ws.Range("A3:A" + (count + 3) + "").Rows().Style.Border.LeftBorder = XLBorderStyleValues.Thick;
            ws.Range("A3:A" + (count + 3) + "").Rows().Style.Border.LeftBorderColor = XLColor.DarkBlue;

            ws.Range("E2:E" + (count + 3) + "").Rows().Style.Border.RightBorder = XLBorderStyleValues.Thick;
            ws.Range("E2:E" + (count + 3) + "").Rows().Style.Border.RightBorderColor = XLColor.DarkBlue;

            ws.Range("A" + (count + 3) + ":E" + (count + 3) + "").Cells().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            ws.Range("A" + (count + 3) + ":E" + (count + 3) + "").Cells().Style.Border.BottomBorderColor = XLColor.DarkBlue;

            ws.Range("A2:E3").Cells().Style.Fill.BackgroundColor = XLColor.DarkBlue;
            ws.Range("A3:E3").Cells().Style.Font.FontColor = XLColor.White;
            ws.Column("E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;


            //ws.Range("E4:E" + (count + 3)).Cells().DataType = XLCellValues.Number;

            wb.SaveAs(file);
            wb.Dispose();
            //HttpContext.Current.Session["reportePorAssesorPath"] = file;

        }

    }
}
