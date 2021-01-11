using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.DSS;
using VecompSoftware.Services.DSS.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService
{
    public class DocumentoSanitarioTypeParser : DocumentParserBase<DocumentoSanitarioType>
    {
        #region [ Fields ]
        private readonly RESTService _dssService;
        #endregion

        #region [ Constructors ]

        public DocumentoSanitarioTypeParser(BDSImporterParameters parameters)
            : base(parameters)
        {
            _dssService = new RESTService(parameters.SignatureValidationBaseUrl);
        }

        #endregion

        #region [ Methods ]

        private void SetSignInfo(FileDocumentInfo document, string key, string xpath, string name)
        {
            if (!document.Attributes.ContainsKey(key))
                document.Attributes.Add(key, null);

            if (!string.IsNullOrEmpty(document.Attributes[key]))
            {
                FileLogger.Info(LoggerName, "Attributo già valorizzato: " + key);
                return;
            }

            FileLogger.Info(LoggerName, "Valore mancante per attributo: " + key);
            var signInfo = GetSignInfo(document);
            var value = signInfo.SelectNodes(xpath)[0].Attributes[name].Value;
            FileLogger.Info(LoggerName, "Valore recuperato da SignInfo: " + value);
            document.Attributes[key] = value;
        }

        private void EvaluateBySignInfo(FileDocumentInfo document)
        {
            if (!document.Extension.Equals(".p7m"))
                return;

            SetSignInfo(document, "DataOraReferto", "Document/Signature/Details", "Date"); // Data e ora di firma.
            SetSignInfo(document, "MedicoRefertante", "Document/Signature/Subject", "NAME"); // Cognome e nome firmatario.
            SetSignInfo(document, "MedicoRefertanteDescrizione", "Document/Signature/Subject", "FC"); // Codice fiscale firmatario.
        }

        protected override IList<FileDocumentInfo> GetDocumentInfos(DocumentoSanitarioType deserialized)
        {
            var documentPath = Path.Combine(ParserInputFolder, deserialized.Componente.Filename);
            if (!string.IsNullOrEmpty(Parameters.ExtensionWhiteList))
            {
                var fileName = Path.GetFileName(documentPath).ToLowerInvariant();
                var whiteList = Parameters.ExtensionWhiteList.ToLowerInvariant().Split('|');
                if (!whiteList.Any(e => documentPath.EndsWith(e)))
                    throw new InvalidOperationException("Estensione non valida per: " + documentPath);
            }

            var document = new FileDocumentInfo(new FileInfo(documentPath));

            if (Parameters.SignatureValidationEnabled)
            {
                ValidateResultModel validateResult = _dssService.Validate(document.Name, document.Stream);
                if (!validateResult.IsValid)
                {
                    throw new SignatureDocumentValidationException(document.FileInfo.FullName, validateResult.ErrorMessages, validateResult.ToRetry);
                }
            }

            var attributes = new List<IDictionary<string, string>> { GetPropertiesDictionary(deserialized.Chiave) };
            if (deserialized.Componente != null)
                attributes.Add(GetPropertiesDictionary(deserialized.Componente));
            if (deserialized.Dati != null)
                attributes.Add(GetPropertiesDictionary(deserialized.Dati));
            if (deserialized.Revisione != null)
                attributes.Add(GetPropertiesDictionary(deserialized.Revisione));

            var result = new Dictionary<string, string>();
            foreach (var item in attributes)
                result = result
                    .Concat(item.Where(a => !result.ContainsKey(a.Key)))
                    .ToDictionary(a => a.Key, a => a.Value);

#if DEBUG
            result["Numero"] = Guid.NewGuid().ToString("N");
            FileLogger.Info(LoggerName, "#if DEBUG: modificato attributo Numero in: " + result["Numero"]);

            var flattened = result.Select(d => string.Format("{0}: {1}", d.Key, d.Value));
            var output = string.Join("\r\n", flattened);
#endif

            document.AddAttributes(result);
            EvaluateBySignInfo(document);

            return new List<FileDocumentInfo> { document };
        }
        protected override void SaveToBiblos(IList<FileDocumentInfo> documents, DocumentoSanitarioType deserialized)
        {
            var archiveName = Parameters.BiblosArchive;
            if (string.IsNullOrEmpty(archiveName))
                archiveName = deserialized.Chiave.Archivio;

            foreach (var item in documents)
                item.ArchiveInBiblos(Parameters.BiblosServerName, archiveName);
        }

        #endregion

    }
}
