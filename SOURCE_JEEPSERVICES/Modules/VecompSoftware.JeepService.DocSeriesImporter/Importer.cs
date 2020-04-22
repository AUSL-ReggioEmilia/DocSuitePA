using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Data.OleDb;
using System.Linq;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Services.Logging;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.DocSuiteWeb.Data;

using DocSuiteWebAPI = VecompSoftware.DocSuiteWeb.API;
using System.Text;

namespace VecompSoftware.JeepService.DocSeriesImporter
{
    public class Importer
    {
        private static readonly string[] _basicFields = {
            "Subject",
            "Subsection",
            "DOC_Main",
            "DOC_Annexed",
            "DOC_UnpublishedAnnexed"
        };

        private string LoggerName;
        private string dropFolder;
        private string doneFolderName;
        private bool deleteDoc_Main;
        private bool deleteDoc_Annexed;
        private bool deleteDoc_Unpublished;
        private string errorFolder;
        private int maxTimes_reWorkError;

        public Importer(string loggerName, string dropFolder, string doneFolder,string error, bool deleteMain, bool deleteAnn, bool deleteUnpub,int max)
        {
            this.LoggerName = loggerName;
            this.dropFolder = dropFolder;
            this.doneFolderName = doneFolder;
            this.deleteDoc_Main = deleteMain;
            this.deleteDoc_Annexed = deleteAnn;
            this.deleteDoc_Unpublished = deleteUnpub;
            this.errorFolder = error;
            this.maxTimes_reWorkError = max;
        }

        public void DownloadFile(TaskHeader task, string dropFolder)
        {
            DocSuiteWebAPI.DocumentSeriesItemDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<DocSuiteWebAPI.DocumentSeriesItemDTO>(task.Code);
            FileLogger.Info(this.LoggerName, String.Format("Download file '{0}'", dto.Document.FullName));

            if (!Helpers.FileHelper.MatchExtension(dto.Document.FullName, Helpers.FileHelper.XLS))
                throw new DocSuiteException("Il documento non è " + Helpers.FileHelper.XLS);

#if LOCALTEST
#warning LOCALTEST - Rename dei file
      dto.Document.FullName = Path.Combine(@"c:\temp\docseries", Path.GetFileName(dto.Document.FullName));
#endif

            //copia nel process folder e crea file info se non esiste
            string publishDate = dto.PublishingDate.HasValue == true ? dto.PublishingDate.Value.ToString("yyyyMMdd") : DateTime.Today.ToString("yyyyMMdd");
            string outFile = String.Format("{0}-{1}", publishDate, Path.GetFileName(dto.Document.FullName));
            outFile = Path.Combine(dropFolder, outFile);

            if (File.Exists(outFile))
                File.Delete(outFile);

            File.Copy(dto.Document.FullName, outFile);

            string infoFullName = String.Format("{0}-{1}.xml", publishDate, Path.GetFileNameWithoutExtension(dto.Document.FullName));
            string infoFilename = Path.Combine(dropFolder, infoFullName);

            if (!File.Exists(infoFilename))
            {
                TaskInfo info = new TaskInfo
                {
                    documentFilename = outFile,
                    taskId = task.Id,
                    taskCode = task.Code
                };

                info.SaveAs(infoFilename);
            }
        }


        public bool ProcessFile(string fileName, ref int rowImported, ref int totalRow)
        {
            bool res = true;

            TaskInfo tInfo = TaskInfo.Load(fileName);
            TaskHeader task = FacadeFactory.Instance.TaskHeaderFacade.GetById(tInfo.taskId);
            DocSuiteWebAPI.DocumentSeriesItemDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<DocSuiteWebAPI.DocumentSeriesItemDTO>(tInfo.taskCode);
            FileLogger.Info(this.LoggerName, String.Format("Elaborazione file '{0}'", fileName));

            TaskDetail taskDetail = new TaskDetail() { DetailType = DetailTypeEnum.Info, Title = String.Format("Elaborazione file '{0}'", fileName), TaskHeader = task };
            FacadeFactory.Instance.TaskDetailFacade.Save(ref taskDetail);

            //informazioni sull'archivio
            DocumentSeries docSeries = FacadeFactory.Instance.DocumentSeriesFacade.GetById(dto.IdDocumentSeries.Value);
            IList<DocumentSeriesSubsection> docSeriesSubsections = FacadeFactory.Instance.DocumentSeriesSubsectionFacade.GetAll();

            ArchiveInfo archInfo = GetSelectedArchiveInfo(docSeries);

            List<string> columns = new List<string>(_basicFields);
            columns.AddRange(archInfo.VisibleChainAttributes.Select(p => p.Name));

            string excelVers = "";
            if (Path.GetExtension(tInfo.documentFilename).ToLower() == ".xls")
                excelVers = "Excel 8.0";

            if (Path.GetExtension(tInfo.documentFilename).ToLower() == ".xlsx")
                excelVers = "Excel 12.0";

            rowImported = 0;


            List<List<string>> righeErrore = new List<List<string>>();

            string connStr = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"{0}\";Extended Properties=\"{1};HDR=Yes;IMEX=1;\"", tInfo.documentFilename, excelVers);
            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                conn.Open();
                string sheetName = conn.GetSchema("Tables").Rows[0]["TABLE_NAME"].ToString();

                //Gestione del file excel
                IDbCommand command = conn.CreateCommand();
                command.CommandText = string.Format("SELECT {0} FROM  [{1}]", string.Format("[{0}]", string.Join("], [", columns)), sheetName);
                command.CommandType = CommandType.Text;

                using (IDataReader dr = command.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        dr.Close();
                        taskDetail = new TaskDetail() { DetailType = DetailTypeEnum.ErrorType, Title = "Foglio excel vuoto.", TaskHeader = task };
                        FacadeFactory.Instance.TaskDetailFacade.Save(ref taskDetail);

                        throw new DocSuiteException("Foglio excel vuoto.");
                    }

                    //Verifica delle colonne
                    DataTable dt = dr.GetSchemaTable();
                    if (dt.Select("ColumnName = 'Subject'").Length < 1)
                    {
                        taskDetail = new TaskDetail() { DetailType = DetailTypeEnum.ErrorType, Title = "Colonna [Subject] mancante.", TaskHeader = task };
                        FacadeFactory.Instance.TaskDetailFacade.Save(ref taskDetail);
                        throw new DocSuiteException("Colonna [Subject] mancante.");
                    }
                    foreach (ArchiveAttribute attribute in archInfo.VisibleChainAttributes)
                    {
                        int columnIndex = dt.Select(string.Format("ColumnName = '{0}'", attribute.Name)).Length;
                        if (columnIndex < 1 & attribute.Required)
                        {
                            taskDetail = new TaskDetail() { DetailType = DetailTypeEnum.ErrorType, Title = string.Format("Colonna obbligatoria [{0}] mancante.", attribute.Name), TaskHeader = task };
                            FacadeFactory.Instance.TaskDetailFacade.Save(ref taskDetail);
                            throw new DocSuiteException(string.Format("Colonna obbligatoria [{0}] mancante.", attribute.Name));
                        }
                    }

                    int currentRow = 0;

                    //import
                    while (true)
                    {
                        string rowId = string.Empty;
                        string message = string.Empty;
                        string subSection = string.Empty;

                        try
                        {
                            rowId = dr["Subject"].ToString();

                            if (tInfo.IsRowProcessed(rowId))
                            {
                                message = string.Format("Riga [{0}]: saltata poiché è già stato processato un elemento con campo oggetto '{1}'", currentRow + 2, rowId);
                                FileLogger.Info(LoggerName, message);
                                taskDetail = new TaskDetail() { DetailType = DetailTypeEnum.Info, Title = message, TaskHeader = task };
                                FacadeFactory.Instance.TaskDetailFacade.Save(ref taskDetail);
                                //next
                                if (!dr.Read())
                                    break;
                                continue;
                            }

                            // Catena Biblos per la gestione dei Metadati
                            BiblosChainInfo chain = new BiblosChainInfo();
                            List<DocumentInfo> mainDocs = new List<DocumentInfo>();
                            if (dt.Select("ColumnName = 'DOC_Main'").Length == 1)
                            {
                                mainDocs = ParseDocumentString(dr["DOC_Main"].ToString());
                                chain.AddDocuments(mainDocs);

                            }

                            // Recupero i metadati da EXCEL e li salvo nella catena
                            foreach (ArchiveAttribute attribute in archInfo.VisibleChainAttributes)
                            {
                                if (dt.Select(string.Format("ColumnName = '{0}'", attribute.Name)).Length < 1)
                                {
                                    continue;
                                }
                                FileLogger.Debug(LoggerName, attribute.Name);
                                chain.AddAttribute(attribute.Name, string.Format(attribute.Format, dr[attribute.Name]));
                            }

                            // DocumentSeriesItem da salvare
                            DocumentSeriesItem item = new DocumentSeriesItem();

                            if (dt.Select("ColumnName = 'Subsection'").Length > 0)
                            {
                                subSection = dr["Subsection"].ToString();
                                if (!subSection.IsNullOrEmpty())
                                {
                                    DocumentSeriesSubsection subSectionToAdd = docSeriesSubsections.FirstOrDefault(x => x.Description == subSection);

                                    if (subSectionToAdd != null)
                                    {
                                        item.DocumentSeriesSubsection = subSectionToAdd;
                                    }
                                    else
                                    {
                                        message = string.Format("Sottosezione [{0}] della serie documentale [{1}] non caricata perchè non presente sul database", subSection, docSeries.Name);
                                        FileLogger.Info(LoggerName, message);
                                    }
                                }
                            }

                            item.DocumentSeries = docSeries;
                            item.Subject = dr["Subject"].ToString();

                            // Recupero e salvo i dati di classificazione
                            Category selectedCategory = FacadeFactory.Instance.CategoryFacade.GetById(dto.Category.Id.Value);
                            Category root = selectedCategory.Root;
                            if (selectedCategory.Equals(root))
                            {
                                item.Category = selectedCategory;
                            }
                            else
                            {
                                item.Category = root;
                                item.SubCategory = selectedCategory;
                            }

                            // Recupero e salvo l'evenatuale data di pubblicazione
                            if (dto.PublishingDate.HasValue)
                            {
                                item.PublishingDate = dto.PublishingDate.Value;
                            }

                            // Imposto la STATUS desiderato
                            DocumentSeriesItemStatus status = (DocumentSeriesItemStatus)dto.Status;

                            List<DocumentInfo> annexed = new List<DocumentInfo>();
                            if (dt.Select("ColumnName = 'DOC_Annexed'").Length == 1)
                            {
                                annexed.AddRange(ParseDocumentString(dr["DOC_Annexed"].ToString()));
                            }

                            List<DocumentInfo> unpublished = new List<DocumentInfo>();
                            if (dt.Select("ColumnName = 'DOC_UnpublishedAnnexed'").Length == 1)
                            {
                                unpublished.AddRange(ParseDocumentString(dr["DOC_UnpublishedAnnexed"].ToString()));
                            }

                            // Salvo l'Item in DB
                            FacadeFactory.Instance.DocumentSeriesItemFacade.SaveDocumentSeriesItem(item, chain, annexed, unpublished, status, "Registrazione importata da documento EXCEL.");

                            //A questo punto sono certo che la procedura ha salvato in BiblosDS i documenti,
                            //li rimuovo dalla sorgente.

                            RemoveProcessedFiles(mainDocs, annexed, unpublished);

                            message = string.Format("Riga [{0}]: Ok", currentRow + 2);
                            FileLogger.Info(LoggerName, message);

                            taskDetail = new TaskDetail() { DetailType = DetailTypeEnum.Info, Title = message, TaskHeader = task };
                            FacadeFactory.Instance.TaskDetailFacade.Save(ref taskDetail);

                            tInfo.AddRowInfo(rowId, message, RowInfo.RowStatus.Processed);

                            // Incremento il counter
                            rowImported += 1;
                        }
                        catch (Exception ex)
                        {
                            int count = dr.FieldCount;

                            //genero la lista da inserire nella lista di errori
                            List<string> errorValues = new List<string>();
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < count; i++)
                            {
                                errorValues.Add(dr.GetValue(i).ToString());
                            }
                            righeErrore.Add(errorValues);



                            FileLogger.Debug(LoggerName, "QRY su EXEL: " + command.CommandText);

                            message = string.Format("Riga [{0}]: Errore - {1}", currentRow + 2, ex.Message);
                            FileLogger.Error(LoggerName, message, ex);

                            taskDetail = new TaskDetail() { DetailType = DetailTypeEnum.ErrorType, Title = message, ErrorDescription = ex.StackTrace, TaskHeader = task };
                            FacadeFactory.Instance.TaskDetailFacade.Save(ref taskDetail);

                            tInfo.AddRowInfo(rowId, message, RowInfo.RowStatus.Error);
                            res = false;
                        }
                        finally
                        {
                            currentRow += 1;
                        }
                        //next
                        if (!dr.Read())
                            break;
                    }
                    totalRow = currentRow;
                }
                // chiudo la conn oledb
                conn.Close();
            }
            return res;
        }

        /// <summary>
        /// Metodo dove viene creata la query di create table dell'excel, mediante uno string builder.
        /// </summary>
        /// <param name="columns">Colonne del file che è stato processato dal JS</param>
        /// <returns>stringa di query</returns>
        private string BuildCreateTableCommand(List<string> columns)
        {
            // Get the type look-up tables.
            StringBuilder sb = new StringBuilder();

            // Check for null data set.
            if (columns.Count <= 0)
                return null;

            // Start the command build.
            sb.AppendFormat("CREATE TABLE [{0}] (", "Recovery");

            // Build column names and types.
            foreach (string col in columns)
            {
                string type = "String";
                string strToInsert = String.Empty;
                strToInsert = col.ToString(); //.Replace("'", "''");
                sb.AppendFormat("[{0}] {1},", strToInsert.Replace(' ', '_'), type);
            }

            sb = sb.Replace(',', ')', sb.ToString().LastIndexOf(','), 1);
            return sb.ToString();
        }

        /// <summary>
        /// Metodo dove viene creata la query di insert dentro la tabella dell'excel del file di recovery.
        /// </summary>
        /// <param name="columns">Colonne del file che è stato processato dal JS</param>
        /// <param name="errors">Lista contente lista degli errori che cilati mi permettono di formare la query</param>
        /// <returns>stringa di query</returns>
        private string BuildInsertCommand(List<string> columns, List<string> errors)
        {
            StringBuilder sb = new StringBuilder();

            // Remove whitespace.
            sb.AppendFormat("INSERT INTO [{0}$](", "Recovery");
            foreach (string col in columns)
                sb.AppendFormat("[{0}],", col.Replace(' ', '_'));

            sb = sb.Replace(',', ')', sb.ToString().LastIndexOf(','), 1);

            // Write values.
            sb.Append("VALUES (");

            int indexRow = 0;
            foreach (string colError in errors)
            {
                indexRow++;
                if (indexRow != errors.Count)
                    sb.AppendFormat("'{0}',", colError);
                else
                    sb.AppendFormat("'{0}')", colError);
            }

            return sb.ToString();
        }


        private void RemoveProcessedFiles(IEnumerable<DocumentInfo> main, IEnumerable<DocumentInfo> annexed, IEnumerable<DocumentInfo> unpublished)
        {
            if (!main.Any() && !annexed.Any() && !unpublished.Any())
                return;

            if (deleteDoc_Main)
            {
                foreach (FileDocumentInfo document in main)
                {
                    File.Delete(document.FileInfo.FullName);
                }
            }

            if (deleteDoc_Annexed)
            {
                foreach (FileDocumentInfo document in annexed)
                {
                    File.Delete(document.FileInfo.FullName);
                }
            }

            if (deleteDoc_Unpublished)
            {
                foreach (FileDocumentInfo document in unpublished)
                {
                    File.Delete(document.FileInfo.FullName);
                }
            }
        }

        private List<DocumentInfo> ParseDocumentString(string docs)
        {
            List<DocumentInfo> tor = new List<DocumentInfo>();
            foreach (string token in docs.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (File.Exists(token))
                {
                    tor.Add(new FileDocumentInfo(new FileInfo(token)));
                }
                else
                {
                    throw new DocSuiteException(string.Format("File [{0}] non trovato.", token));
                }
            }
            return tor;
        }

        private ArchiveInfo GetSelectedArchiveInfo(DocumentSeries docSeries)
        {
            if (docSeries != null)
            {
                return DocumentSeriesFacade.GetArchiveInfo(docSeries);
            }
            return null;
        }

    }
}
