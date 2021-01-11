using System;
using System.IO;
using System.Web;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Helpers.Web.ExtensionMethods;

namespace AmministrazioneTrasparente
{
    public class Annexed : BaseDocument, IHttpHandler
    {

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

            var xml = String.Empty;
            var serialized = Client.GetDocumentSeries(idSeries, false, ArchiveRestriction);
            var series = SerializationHelper.SerializeFromString<DocumentSeriesWSO>(serialized);
            //Gestisco ANAC
            if (series.Name.Eq("corruzione"))
            {
                if (ext.Eq(FileHelper.XLS) || ext.Eq(FileHelper.XLSX))
                    xml = Client.GetAnnexed(idSeriesItem, idDoc, false);
            }

            if (String.IsNullOrEmpty(xml))
            {
                xml = SignatureEnable ? Client.GetAnnexedWithSignature(idSeriesItem, idDoc, SignatureTemplate) : Client.GetAnnexed(idSeriesItem, idDoc, true);   
            }            

            ElaborateDocument(context, xml);
        }
    }
}