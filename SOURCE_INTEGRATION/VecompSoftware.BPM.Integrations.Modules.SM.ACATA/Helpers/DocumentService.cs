using System;
using System.Collections.Generic;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Adaptors;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.DocumentService;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Services;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Helpers
{
    public class DocumentContentService
    {
        #region [ Fields ]
        private readonly AcarisService _acarisService;
        private readonly RepositoryService.ObjectIdType _repositoryId;
        private readonly BackOfficeService.PrincipalIdType _principalId;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ILogger _logger;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(Execution));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public DocumentContentService(AcarisService acarisService, RepositoryService.ObjectIdType repositoryId, BackOfficeService.PrincipalIdType principalId, ILogger logger)
        {
            _acarisService = acarisService;
            _repositoryId = repositoryId;
            _principalId = principalId;
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        public DocumentoFisicoIRC CreatePhysicalDocument(string fileName, string extension, byte[] contentStream, enumMimeTypeType mimeType, enumStreamId contentType)
        {
            DocumentoFisicoIRC document = new DocumentoFisicoIRC();
            DocumentoFisicoPropertiesType pysicalDocumentPropertyType = new DocumentoFisicoPropertiesType
            {
                descrizione = $"documento {fileName}.{extension}",
                dataMemorizzazione = DateTime.Now
            };
            document.propertiesDocumentoFisico = pysicalDocumentPropertyType;

            // caso semplice: documento fisico con un solo contenuto fisico (file) associato
            List<ContenutoFisicoIRC> content = new List<ContenutoFisicoIRC>
            {
                // creazione contenuto fisico e relativo stream dati
                CreatePysicalContentIRC(fileName, contentStream, mimeType, contentType)
            };
            document.contenutiFisici = content.ToArray();
            return document;
        }
        public IdentificazioneTrasformazione[] TransformDocumentPlaceholder(OfficialBookService.ObjectIdType classificationId, OfficialBookService.ObjectIdType registrationId, DocumentoFisicoIRC[] documents, int statusEfficacyId, int physicalDocTypeId, int documentCompositionId)
        {
            IdentificazioneTrasformazione[] result = null;
            try
            {
                InfoRichiestaTrasformazione requestTransformationInfo = new InfoRichiestaTrasformazione()
                {
                    diventaElettronico = false,
                    multiplo = false,
                    rimandareOperazioneSbustamento = true,
                    statoDiEfficaciaId = statusEfficacyId,
                    tipoDocFisicoId = physicalDocTypeId,
                    composizioneId = documentCompositionId
                };

                result = _acarisService.TransformDocumentPlaceholderInElectronicDocument(_repositoryId, _principalId, classificationId.ConvertToDocumentService(), registrationId.ConvertToDocumentService(), requestTransformationInfo, documents);
                _logger.WriteInfo(new LogMessage($"SM.ACATA -> Successfully attached documents."), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"SM.ACATA -> Errore in transformazione del document {classificationId.value}"), ex, LogCategories);
                throw;
            }
            return result;
        }
        #endregion

        #region [ Helpers ]
        private ContenutoFisicoIRC CreatePysicalContentIRC(string fileName, byte[] stream, enumMimeTypeType mimeType, enumStreamId contentType)
        {
            ContenutoFisicoIRC content = new ContenutoFisicoIRC();

            acarisContentStreamType contentStream = new acarisContentStreamType()
            {
                filename = fileName,
                mimeType = mimeType,
                mimeTypeSpecified = true,
                streamMTOM = stream
            };
            content.stream = contentStream;
            content.tipo = contentType; //main content; RENDITION_ENGINE = allegato_xml; RENDITION_DOCUMENT - allegato_xml; SIGNATURE = signature_detached; TIMESTAMP = signature_detached; 

            ContenutoFisicoPropertiesType physicalContentPropertyType = new ContenutoFisicoPropertiesType() { sbustamento = false };
            content.propertiesContenutoFisico = physicalContentPropertyType;
            return content;
        }
        #endregion
    }
}
