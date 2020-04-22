using System;
using System.Configuration;
using System.IO;
using System.Web;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Helpers.Web.ExtensionMethods;
using VecompSoftware.Services.Logging;

namespace AmministrazioneTrasparente
{
    public class Document : BaseDocument, IHttpHandler
    {
        #region IHttpHandler Members
        public const string LoggerName = "Application";

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var idSeriesItem = context.Request.QueryString.GetValue<int>("idSeriesItem");
            var idDoc = context.Request.QueryString.GetValue<Guid>("idDoc");
            var idSeries = context.Request.QueryString.GetValue<int>("idSeries");
            var ext = context.Request.QueryString.GetValue<string>("ext");

            FileLogger.Info(LoggerName, string.Concat("Processo documento con Id: ", idDoc, " - Serie documentale: ", idSeriesItem, " - con estensione: ", ext));

            var xml = String.Empty;

            //Gestisco ANAC
            int idCorruptionSeries;
            if (int.TryParse(ConfigurationManager.AppSettings["CorruzioneDocumentSeriesId"], out idCorruptionSeries))
            {
                if (idSeries == idCorruptionSeries)
                {
                    if (ext.Eq(FileHelper.XLS) || ext.Eq(FileHelper.XLSX))
                    {
                        xml = Client.GetMainDocument(idSeriesItem, idDoc, false);
                    }
                }
            };            

            if (String.IsNullOrEmpty(xml))
            {
                xml = SignatureEnable ? Client.GetMainDocumentWithSignature(idSeriesItem, idDoc, SignatureTemplate) : Client.GetMainDocument(idSeriesItem, idDoc, true);
            }            

            ElaborateDocument(context, xml);
        }

        #endregion
    }
}
