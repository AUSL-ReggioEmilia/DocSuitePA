using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using VecompSoftware.Helpers;

namespace VecompSoftware.DocSuiteWeb.Gui.WebComponent
{

    public class ExportHelper
    {
        /// <summary>
        /// Esporta il contenuto presente nella tabella HTML.
        /// Viene utilizzato l'oggetto PAGE per creare la risposta
        /// </summary>
        /// <param name="control"></param>
        /// <param name="page"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        private static void ExportToContentType(Control control, Page page, string fileName, string contentType)
        {
            page.Response.Clear();
            page.Response.Buffer = true;
            page.Response.ContentType = contentType;
            page.Response.ContentEncoding = Encoding.UTF8;
            page.Response.BinaryWrite(Encoding.UTF8.GetPreamble());
            //UTF-8
            if (!string.IsNullOrEmpty(fileName))
            {
                page.Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", fileName));
            }
            page.Response.Charset = "UTF-8";
            page.EnableViewState = false;
            StringWriter oStringWriter = new StringWriter();
            HtmlTextWriter oHtmlTextWriter = new HtmlTextWriter(oStringWriter);
            control.RenderControl(oHtmlTextWriter);
            page.Response.Write(oStringWriter.ToString());
            page.Response.Flush();
            page.Response.SuppressContent = true;
        }

        /// <summary>
        /// Setto il content per i file EXCEL type e esporto i dati presenti nella tabella
        /// </summary>
        /// <param name="control"></param>
        /// <param name="page"></param>
        /// <param name="fileName"></param>
        public static void ExportToExcel(Control control, Page page, string fileName = "")
        {
            //export to excel
            string contentType = "application/msexcel";
            ExportToContentType(control, page, fileName, contentType);
        }

        /// <summary>
        /// Setto il content per i file WORD type e esporto i dati presenti nella tabella
        /// </summary>
        /// <param name="control"></param>
        /// <param name="page"></param>
        /// <param name="fileName"></param>
        public static void ExportToWord(Control control, Page page, string fileName = "")
        {
            //export to excel
            string contentType = "application/msword";
            ExportToContentType(control, page, fileName, contentType);
        }

        /// <summary>
        /// Creazione della tabella di esportazione a partire dalle colonne e dagli item presenti nella griglia.
        /// </summary>
        /// <param name="gridColumns"></param>
        /// <param name="gridItems"></param>
        /// <param name="border"></param>
        /// <returns></returns>
        public static Table TableFromGrid(GridColumnCollection gridColumns, GridDataItemCollection gridItems, bool border = true)
        {
            Table table = new Table();
            table.Style.Add("border-collapse", "collapse");

            List<KeyValuePair<int, List<string>>> columns = new List<KeyValuePair<int, List<string>>>();
            //Recupero estraggo le informazioni delle colonne presenti nella griglia
            double width = 0;
            int fanta = 0;
            foreach (GridColumn col in gridColumns)
            {
                if (col.Visible && col.Display && col.UniqueName != "ClientSelectColumn")
                {
                    List<string> list = new List<string>();
                    list.Add(col.HeaderText);
                    list.Add(col.ColumnType);
                    list.Add(col.HeaderStyle.Width.Value.ToString());
                    KeyValuePair<int, List<string>> column = new KeyValuePair<int, List<string>>(fanta, list);
                    columns.Add(column);
                    width += col.HeaderStyle.Width.Value;
                }
                fanta = fanta + 1;
            }
            
            //Creo l'intestazione
            TableRow rowHeader = new TableRow();
            foreach (KeyValuePair<int, List<string>> item in columns)
            {
                TableCell cellStyle = new TableCell();
                double wPerc = (Convert.ToDouble(item.Value[2]) / width) * 100;
                cellStyle.Width = Unit.Percentage(wPerc);
                cellStyle.Font.Bold = true;
                cellStyle.BorderStyle = BorderStyle.Solid;
                cellStyle.BorderWidth = Unit.Pixel(1);
                cellStyle.Text = item.Value[0];
                rowHeader.Cells.Add(cellStyle);                 
            }
            table.Rows.Add(rowHeader);   
            
            //Creo i righe contenenti i dati
            GridDataItemCollection items = gridItems;
            foreach (GridDataItem item in items)
            {
                TableRow rowData = new TableRow();
                foreach (KeyValuePair<int, List<string>> x in columns)
                {
                    if (x.Key >= 0 && x.Key <= item.Cells.Count)
                    {
                        TableCell cellStyle = new TableCell();
                        cellStyle.BorderStyle = BorderStyle.Solid;
                        cellStyle.BorderWidth = Unit.Pixel(1);
                        // Più 2 per evitare le due colonne iniziali --> calcolo al volo l'offset per eliminare le prime colonne.
                        int offset = Math.Abs(columns.Count - item.Cells.Count);
                        cellStyle.Text = CellToString(item.Cells[x.Key + offset]);
                        rowData.Cells.Add(cellStyle);
                    }
                }
                table.Rows.Add(rowData);
            }
            return table;           
        }

        /// <summary>
        /// Estrazione del testo dalla cella
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static string CellToString(TableCell cell)
        {   
            string retval = string.Empty;
            
            if (cell.Controls.Count > 1)
            {
                switch (cell.Controls[1].GetType().ToString())
                {
                    case "System.Web.UI.WebControls.Image":
                    case "System.Web.UI.WebControls.ImageButton":
                        retval = ImageToString(ReflectionHelper.GetProperty(cell.Controls[1], "ImageUrl").ToString());
                        break;
                    default:
                        retval = ReflectionHelper.GetProperty(cell.Controls[1], "Text").ToString();
                        break;
                }
            }
            else if (cell.Controls != null && cell.Controls.Count > 0 && string.Compare(cell.Controls[0].GetType().ToString(), "System.Web.UI.DataBoundLiteralControl", true).Equals(0))
            {
                retval = ((DataBoundLiteralControl)cell.Controls[0]).Text;
            }
            else
            {
                retval = cell.Text;
            }
            
            return retval;
        }

        /// <summary>
        /// Converte il nome dell'immagine in testo da inserire nella tabella di esportazione
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ImageToString(string path)
        {
            string sRet = string.Empty;
            string filename = System.IO.Path.GetFileNameWithoutExtension(path);

            switch (filename.ToUpper())
            {
                case "MAIL16_U":
                case "MAIL32_U":
                    sRet = "Uscita";
                    break;
                case "MAIL16_I":
                case "MAIL32_I":
                    sRet = "Ingresso";
                    break;
                case "MAIL16_IU":
                case "MAIL32_IU":
                    sRet = "Ingresso/Uscita";
                    break;
                case "DOCMANNULLA":
                    sRet = "Pratica Annullata";
                    break;
                case "DOCMARCHIVIA":
                    sRet = "Pratica Archiviata";
                    break;
                case "DOCMCHIUSURA":
                    sRet = "Pratica Chiusura";
                    break;
                case "DOCMRIAPERTURA":
                    sRet = "Pratica Riapertura";
                    break;
                case "ATTOANNULLATO":
                    sRet = "Atto Annullato";
                    break;
                case "DELIBERAANNULLATA":
                    sRet = "Delibera Annullata";
                    break;
                //Cartella DocSuite
                case "ATTO16":
                case "ATTO32":
                case "RESOLUTION16":
                    sRet = "Atto";
                    break;
                case "COLLEGAMENTO16":
                case "COLLEGAMENTO32":
                    sRet = "Collegamento";
                    break;
                case "DELIBERA16":
                case "DELIBERA32":
                    sRet = "Delibera";
                    break;
                case "FASCICOLI16":
                case "FASCICOLI32":
                    sRet = "Fascicoli";
                    break;
                case "FASCICOLOCLOSED16":
                case "FASCICOLOCLOSED32":
                    sRet = "Fascicolo Chiuso";
                    break;
                case "PRATICA16":
                case "PRATICA32":
                    sRet = "Pratica";
                    break;
                case "PRATICHE16":
                case "PRATICHE32":
                    sRet = "Pratiche";
                    break;
                //Cartella File
                case "ACROBAT16":
                case "ACROBAT32":
                    sRet = "PDF";
                    break;
                case "ALLEGATI16":
                case "ALLEGATI32":
                    sRet = "Allegati";
                    break;
                case "ATTI16":
                case "ATTI32":
                    sRet = "Atti";
                    break;
                case "CHECKOUT16":
                case "CHECKOUT32":
                    sRet = "CheckOut";
                    break;
                case "DIGITALSIGN16":
                case "DIGITALSIGN32":
                    sRet = "Firmato";
                    break;
                case "EXCEL16":
                case "EXCEL32":
                    sRet = "Excel";
                    break;
                case "EXPLORERHTM16":
                case "EXPLORERHTM32":
                    sRet = "HTML";
                    break;
                case "EXPLORERMHT16":
                case "EXPLORERMHT32":
                    sRet = "MHT";
                    break;
                case "FASCICOLO16":
                case "FASCICOLO32":
                    sRet = "Fascicolo";
                    break;
                case "IMAGE16":
                case "IMAGE32":
                    sRet = "Immagine";
                    break;
                case "MAIL16":
                case "MAIL32":
                    sRet = "Mail";
                    break;
                case "NONE16":
                case "NONE32":
                    sRet = string.Empty;
                    break;
                case "NOTEPAD16":
                case "NOTEPAD32":
                    sRet = "Testo/ASCII";
                    break;
                case "POWERPOINT16":
                case "POWERPOINT32":
                    sRet = "PowerPoint";
                    break;
                case "PROTOCOLLO16":
                case "PROTOCOLLO32":
                    sRet = "Protocollo";
                    break;
                case "VERSION16":
                case "VERSION32":
                    sRet = "Versione";
                    break;
                case "WINWORD16":
                case "WINWORD32":
                    sRet = "Word";
                    break;
                case "XML16":
                case "XML32":
                    sRet = "XML";
                    break;
                case "FASCICLE_PROCEDURE":
                    sRet = "Fascicolo di procedimento";
                    break;
                default:
                    sRet = filename;
                    break;
            }

            return sRet;
        }
    }
}