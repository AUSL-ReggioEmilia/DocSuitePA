using AmministrazioneTrasparente.Services;
using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.Xsl;
using VecompSoftware.Helpers.Web.ExtensionMethods;
using VecompSoftware.Services.Logging;

namespace AmministrazioneTrasparente
{
    public class AvcpIndex : BaseDocument, IHttpHandler
    {
        public const string LoggerName = "Application";

        public void WriteError(Exception ex)
        {
            if (ex == null)
            {
                return;
            }
            WriteError(ex.InnerException);
            FileLogger.Error(LoggerName, ex.Message, ex);
        }

        private readonly ParameterService _parameterService = new ParameterService();
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                int idSeries;
                if (!int.TryParse(ConfigurationManager.AppSettings["AVCPDocumentSeriesId"], out idSeries))
                {
                    idSeries = context.Request.QueryString.GetValue<int>("idSeries");
                };
                //Di default se non viene passato nessun anno, viene preso l'anno corrente
                int year = context.Request.QueryString.GetValueOrDefault<int>("year", DateTime.Today.Year);

                bool avcpOnlyPublished = this._parameterService.GetBoolean("AVCPCheckIsPublish");

                var impersonatingUser = ConfigurationManager.AppSettings["ImpersonatingUser"];
                var urlMask = ConfigurationManager.AppSettings["AVCP.UrlMask"];
                var urlFile = string.Format(ConfigurationManager.AppSettings["AVCP.UrlFileMask"], idSeries, year);
                var titolo = ConfigurationManager.AppSettings["AVCP.Titolo"];
                var ente = ConfigurationManager.AppSettings["AVCP.EntePubblicatore"];

                var doc = Client.GetIndex(idSeries, impersonatingUser, urlFile, titolo, titolo, ente, string.Empty, urlMask, year, avcpOnlyPublished);

                var enc = new System.Text.UTF8Encoding();
                byte[] dBytes;

                dBytes = enc.GetBytes(doc);
                ElaborateDocument(context, dBytes, "indicedataset.xml");

                //TODO: Da rivedere
                //XslCompiledTransform xslTransform = new XslCompiledTransform();
                //XsltSettings settings = new XsltSettings(true, true);
                //xslTransform.Load(HttpContext.Current.Server.MapPath("~/Templates/AvcpIndex.xslt"), settings, new XmlUrlResolver());

                //StringWriter writer = new StringWriter();
                //string templateUrl = HttpContext.Current.Server.MapPath("~/Templates/AvcpIndex.xslt");
                //XmlDocument xmlDoc = new XmlDocument();
                //xmlDoc.LoadXml(doc);
                //xmlDoc.DocumentElement.PrependChild(xmlDoc.CreateProcessingInstruction("xml-stylesheet", string.Format("type='text/xsl' href='{0}'", templateUrl)));
                //using (XmlReader reader = XmlReader.Create(new MemoryStream(enc.GetBytes(doc))))
                //{

                //    //xslTransform.Transform(reader, null, writer);
                //}            

                //ElaborateDocument(context, dBytes, "indicedataset.xml");

                //dBytes = enc.GetBytes(writer.ToString());            
            }
            catch(Exception ex)
            {
                WriteError(ex);
                throw;
            }
        }
    }
}