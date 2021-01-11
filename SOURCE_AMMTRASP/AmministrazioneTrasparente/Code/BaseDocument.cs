using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Web;
using AmministrazioneTrasparente.Code;
using AmministrazioneTrasparente.WSSeries;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.ExtensionMethods;
using AmministrazioneTrasparente.Services;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;

namespace AmministrazioneTrasparente
{
    public class BaseDocument
    {
        private readonly ParameterService _parameterService;
        private const string Endpointname = "WSSeriesEndpoint";
        private static WSSeriesClient _client;

        public BaseDocument()
        {
            this._parameterService = new ParameterService();
        }

        public static WSSeriesClient Client
        {
            get
            {
                if (_client != null) return _client;

                var endpointAddress = ConfigurationManager.AppSettings["WebServiceURI"];
                _client = new WSSeriesClient(Endpointname, new EndpointAddress(endpointAddress));
                return _client;
            }
        }        

        public bool SignatureEnable
        {
            get { return this._parameterService.GetBoolean("SignatureEnable"); }
        }

        public string SignatureTemplate
        {
            get { return this._parameterService.GetString("SignatureTemplate"); }
        }

        public int? ArchiveRestriction
        {
            get
            {
                string idArchive = this._parameterService.GetString("ArchiveRestriction");
                if (string.IsNullOrEmpty(idArchive))
                    return null;

                int.TryParse(idArchive, out int val);
                return val;
            }
        }

        public void ElaborateDocument(HttpContext context, string xml)
        {
            var result = SerializationHelper.SerializeFromString<DocResultWSO>(xml);
            var stream = Convert.FromBase64String(result.Docs[0].Stream);

            ElaborateDocument(context, stream, result.Docs[0].Name);
        }

        public void ElaborateDocument(HttpContext context, byte[] stream, string name)
        {
            context.Response.Clear();            
            if (Path.GetExtension(name).Eq(".xml"))
            {
                context.Response.ContentType = "text/xml";
                context.Response.ContentEncoding = Encoding.UTF8;
            }
            else if (Path.GetExtension(name).Eq(".html"))
            {
                context.Response.ContentType = "text/html";
                context.Response.ContentEncoding = Encoding.UTF8;
            }
            else
            {
                context.Response.ContentType = "application/octet-stream";
                context.Response.AddHeader("Content-Disposition", String.Concat("attachment", "; filename=", name));
            }
                                   
            context.Response.AddHeader("Content-Length", stream.Length.ToString(CultureInfo.InvariantCulture));
            context.Response.BinaryWrite(stream);

            context.ApplicationInstance.CompleteRequest();
        }
    }
}