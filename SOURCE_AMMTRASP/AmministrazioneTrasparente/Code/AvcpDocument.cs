using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.Web.ExtensionMethods;
using VecompSoftware.Services.Logging;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;

namespace AmministrazioneTrasparente
{
    public class AvcpDocument : BaseDocument, IHttpHandler
    {
        public const string LoggerName = "Application";

        public void WriteError(Exception ex)
        {
            if  (ex == null)
            {
                return;
            }
            WriteError(ex.InnerException);
            FileLogger.Error(LoggerName, ex.Message, ex);
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                int idSeriesItem = context.Request.QueryString.GetValue<int>("idSeriesItem");
                string serialized = Client.Consultation(idSeriesItem, true, false, false, false);
                DocumentSeriesItemWSO itemWso = SerializationHelper.SerializeFromString<DocumentSeriesItemWSO>(serialized);

                DocWSO doc = itemWso.MainDocs[0];

                string xml = Client.GetMainDocument(idSeriesItem, doc.Id, false);
                DocResultWSO result = SerializationHelper.SerializeFromString<DocResultWSO>(xml);

                byte[] bDocument;
                using (MemoryStream docStream = new MemoryStream(Convert.FromBase64String(result.Docs[0].Stream)))
                {
                    XmlDocument docXml = new XmlDocument();
                    string xmlEncoded = string.Empty;
                    try
                    {
                        xmlEncoded = Encoding.Unicode.GetString(Convert.FromBase64String(result.Docs[0].Stream));
                        docXml.LoadXml(xmlEncoded);
                    }
                    catch (Exception ex)
                    {
                        FileLogger.Warn(LoggerName, string.Concat("Errore in deserializzazione XML per la serie con Id ", idSeriesItem," tramite encoding Unicode. Ignorare il messaggio, si procede alla deserializzazione tramite encoding UTF8."), ex);
                        xmlEncoded = Encoding.UTF8.GetString(Convert.FromBase64String(result.Docs[0].Stream));
                        docXml.LoadXml(xmlEncoded);
                    }                    
                    bDocument = Encoding.UTF8.GetBytes(docXml.OuterXml);
                }

                ElaborateDocument(context, bDocument, doc.Name);
            }
            catch (Exception ex)
            {
                WriteError(ex);
                throw;
            }
        }
    }
}