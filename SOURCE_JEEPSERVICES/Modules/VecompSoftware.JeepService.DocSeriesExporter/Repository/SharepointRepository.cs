using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.JeepService.DocSeriesExporter.CsvHelper;
using VecompSoftware.JeepService.DocSeriesExporter.Sp2007CopyReference;
using VecompSoftware.JeepService.DocSeriesExporter.Sp2007ListsReference;

namespace VecompSoftware.JeepService.DocSeriesExporter.Repository
{
    public class SharepointRepository : BaseRepository, IExportRepository
    {
        #region [ Fields ]
        private readonly DocSeriesExporterParameters _parameters;
        #endregion

        #region [ Constructor ]
        public SharepointRepository(DocSeriesExporterParameters parameters)
            :base(parameters)
        {
            this._parameters = parameters;
        }
        #endregion

        #region [ Methods ]

        public SeriesToCsvFields GetHeaderDynamicFields()
        {
            var seriesToCsv = new SeriesToCsvFields();
            foreach (var data in ArchiveAttribute)
            {
                var dict = new KeyValuePair<string, object>(data.Description, null);

                seriesToCsv.DynamicData.Add(dict);
            }
            return seriesToCsv;
        }

        public void StartExport(int id, bool isSubSection = false)
        {            
            var streamData = new MemoryStream();

            CsvWriter csvWriter;
            string fullPath;
            if (isSubSection)
            {
                csvWriter = new CsvWriter(streamData, ";", new List<string>() { "IdSeries", "IdSubsection" });
            }
            else
            {
                csvWriter = new CsvWriter(streamData, ";", new List<string>() { "IdSeries" });
            }

            csvWriter.WriteHeader(GetHeaderDynamicFields());

            foreach (var field in CsvFields)
            {
                csvWriter.WriteLine(field);
            }

            if (isSubSection)
            {
                fullPath = string.Format("Export_Subsection_{0}.csv", id);
            }
            else
            {
                fullPath = string.Format("Export_{0}.csv", id);
            }

            SendToSharePoint(fullPath, streamData);
            streamData.Dispose();
        }

        #region [ Upload to Sharepoint ]
        //Codice recuperato da tool creato da Fabbri
        private void SendToSharePoint(string fileName, MemoryStream stream)
        {
            // Verifica se esiste già
            var itemId = FindFileInList(fileName);
            if (itemId >= 0)
            {
                // Elimina il file già esistente
                DeleteFileFromList(fileName, itemId);
            }

            // Copia il nuovo file
            CopyFileToList(fileName, stream);
        }

        private int FindFileInList(string fileName)
        {
            var listService = new Lists();

            if (this._parameters.ImpersonateUser == "1")
            {
                var cred = new System.Net.NetworkCredential(this._parameters.SpUser, this._parameters.SpPassword, this._parameters.SpDomain);
                listService.Credentials = cred;
            }
            else
            {
                listService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }

            var xmlDocForGet = new XmlDocument();

            var ndQuery = xmlDocForGet.CreateNode(XmlNodeType.Element, "Query", "");
            var ndViewFields = xmlDocForGet.CreateNode(XmlNodeType.Element, "ViewFields", "");
            var ndQueryOptions = xmlDocForGet.CreateNode(XmlNodeType.Element, "QueryOptions", "");

            ndQueryOptions.InnerXml =
                "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>" +
                "<DateInUtc>TRUE</DateInUtc>";

            ndViewFields.InnerXml = "<FieldRef Name='ID' /><FieldRef Name='Title'/>";
            var queryInnerXml = "<Where><Eq><FieldRef Name='FileLeafRef'/><Value Type='Text'>" + fileName +
                                "</Value></Eq></Where>";

            ndQuery.InnerXml = queryInnerXml;

            var ndListItems = listService.GetListItems(this._parameters.SpDocumentLibrary, null, ndQuery, ndViewFields, null,
                ndQueryOptions, null);

            var itemId = -1;
            if (ndListItems == null) return -1;

            try
            {
                var xmlAttributeCollection = ndListItems.ChildNodes[1].ChildNodes[1].Attributes;
                if (xmlAttributeCollection != null)
                {
                    var item = xmlAttributeCollection["ows_ID"].Value;
                    if (item != string.Empty)
                    {
                        itemId = int.Parse(item);
                    }
                }
            }
            catch (Exception)
            {
                return -1;
            }
            return itemId;
        }

        private XmlNode DeleteFileFromList(string fileName, int itemId)
        {
            try
            {
                var listService = new Lists();

                if (this._parameters.ImpersonateUser == "1")
                {
                    var cred = new System.Net.NetworkCredential(this._parameters.SpUser, this._parameters.SpPassword, this._parameters.SpDomain);
                    listService.Credentials = cred;
                }
                else
                {
                    listService.Credentials = System.Net.CredentialCache.DefaultCredentials;
                }

                var strBatch = @"<Method Cmd=""Delete"" ID=""1""><Field Name=""ID"">" + itemId + @"</Field><Field Name=""FileRef"">" + Uri.EscapeUriString(this._parameters.SpSiteCollection + "/" + this._parameters.SpDocumentLibrary + "/" + fileName) + @"</Field></Method>";

                var xmlDoc = new XmlDocument();
                var elBatch = xmlDoc.CreateElement("Batch");

                elBatch.SetAttribute("OnError", "Continue");
                elBatch.SetAttribute("ListVersion", "1");
                elBatch.InnerXml = strBatch;

                var ndReturn = listService.UpdateListItems(this._parameters.SpDocumentLibrary, elBatch);
                return ndReturn;
            }
            catch (Exception)
            {
                return new XmlDocument();
            }
        }

        private void CopyFileToList(string fileName, MemoryStream stream)
        {
            var copyService = new Copy();

            var bytBytes = stream.ToArray();
            var sFileCompletePath = this._parameters.SpSiteCollection + "/" + this._parameters.SpDocumentLibrary + "/" + fileName;

            try
            {
                var fields = new List<FieldInformation>();

                CopyResult[] result;
                if (this._parameters.ImpersonateUser == "1")
                {
                    var cred = new System.Net.NetworkCredential(this._parameters.SpUser, this._parameters.SpPassword, this._parameters.SpDomain);
                    copyService.Credentials = cred;
                }
                else
                {
                    copyService.Credentials = System.Net.CredentialCache.DefaultCredentials;
                }

                string[] destinationUrls = { Uri.EscapeUriString(sFileCompletePath) };

                copyService.CopyIntoItems(sFileCompletePath, destinationUrls, fields.ToArray(), bytBytes, out result);
                if (result.First().ErrorCode.ToString() != "Success")
                {
                    throw new Exception("Errore: " + result.First().ErrorMessage);
                }
            }
            catch
            {
            }
        }
        #endregion

        #endregion
    }
}
