using AmministrazioneTrasparente.Services;
using System;
using System.IO;
using System.Linq;
using System.Web;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Helpers.Web.ExtensionMethods;

namespace AmministrazioneTrasparente
{
    public class FileDocument : BaseDocument, IHttpHandler
    {
        private readonly ParameterService _parameterService = new ParameterService();

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var name = context.Request.QueryString.GetValue<string>("name");

            var path = this._parameterService.GetString("CsvFilesPath");
            var files = Directory.GetFiles(path);

            var doc = files.SingleOrDefault(x => Path.GetFileName(x).Eq(name));
            if(String.IsNullOrEmpty(doc))
                throw  new Exception("Nessun file trovato");

            ElaborateDocument(context, File.ReadAllBytes(doc), name);
        }

        #endregion
    }
}