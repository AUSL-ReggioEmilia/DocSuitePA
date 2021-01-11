using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.AwardBatches;
using BiblosDS.LegalExtension.AdminPortal.ServiceReferenceDocument;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Preservation.Services;
using BiblosDS.Library.Common.Utility;
using System;
using System.Linq;

namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Services.AwardBatches.Interactors
{
    public class SaveRDVXmlInteractor : IInteractor<SaveAwardBatchXMLRequestModel, SaveAwardBatchXMLResponseModel>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly DocumentsClient _documentsClient;
        private readonly PreservationService _preservationService;
        #endregion

        #region [ Constructor ]
        public SaveRDVXmlInteractor(ILogger logger)
        {
            _logger = logger;
            _documentsClient = new DocumentsClient();
            _preservationService = new PreservationService();
        }
        #endregion

        #region [ Methods ]        
        public SaveAwardBatchXMLResponseModel Process(SaveAwardBatchXMLRequestModel request)
        {
            DocumentArchive[] archives = _documentsClient.GetArchives();
            DocumentAttribute[] attributes = _documentsClient.GetAttributesDefinition(request.ArchiveName);
            AwardBatch awardBatch = _preservationService.GetAwardBatch(request.IdAwardBatch);
            if (awardBatch == null)
            {
                _logger.Warn(string.Concat("GenerateAwardBatchRDVInteractor -> award batch with id ", request.IdAwardBatch, " not found"));
                throw new Exception(string.Concat("Award batch with id ", request.IdAwardBatch, " not found"));
            }

            DocumentArchive rdvArchive = archives.SingleOrDefault(s => s.Name.Equals(request.ArchiveName, StringComparison.InvariantCultureIgnoreCase));
            if (rdvArchive == null)
            {
                _logger.Warn(string.Concat("GenerateAwardBatchRDVInteractor -> archive ", request.ArchiveName, " not found"));
                throw new Exception(string.Concat("Archive ", request.ArchiveName, " not found"));
            }
            Document chainDocument = new Document()
            {
                Archive = rdvArchive
            };
            chainDocument = _documentsClient.InsertDocumentChain(chainDocument);

            Document document = new Document
            {
                Content = new DocumentContent() { Blob = request.Content },
                Name = string.Concat(UtilityService.GetSafeFileName(awardBatch.Name), ".xml"),
                Archive = rdvArchive,
                AttributeValues = new System.ComponentModel.BindingList<DocumentAttributeValue>()
            };
            document.AttributeValues.Add(new DocumentAttributeValue()
            {
                Value = document.Name,
                Attribute = attributes.Single(f => f.Name.Equals("Filename", StringComparison.InvariantCultureIgnoreCase))
            });
            document.AttributeValues.Add(new DocumentAttributeValue()
            {
                Value = awardBatch.Name,
                Attribute = attributes.Single(f => f.Name.Equals("Descrizione", StringComparison.InvariantCultureIgnoreCase))
            });
            document.AttributeValues.Add(new DocumentAttributeValue()
            {
                Value = awardBatch.DateFrom,
                Attribute = attributes.Single(f => f.Name.Equals("DataInizio", StringComparison.InvariantCultureIgnoreCase))
            });
            document.AttributeValues.Add(new DocumentAttributeValue()
            {
                Value = awardBatch.DateTo,
                Attribute = attributes.Single(f => f.Name.Equals("DataFine", StringComparison.InvariantCultureIgnoreCase))
            });
            document.AttributeValues.Add(new DocumentAttributeValue()
            {
                Value = DateTime.UtcNow,
                Attribute = attributes.Single(f => f.Name.Equals("DataGenerazione", StringComparison.InvariantCultureIgnoreCase))
            });
            document.AttributeValues.Add(new DocumentAttributeValue()
            {
                Value = _preservationService.GetAwardBatchDocumentsCounter(awardBatch.IdAwardBatch),
                Attribute = attributes.Single(f => f.Name.Equals("NumeroDocumenti", StringComparison.InvariantCultureIgnoreCase))
            });
            document = _documentsClient.AddDocumentToChain(document, chainDocument.IdDocument, Library.Common.Enums.DocumentContentFormat.Binary);
            awardBatch.IdRDVDocument = document.IdDocument;
            _preservationService.UpdateAwardBatch(awardBatch);
            return new SaveAwardBatchXMLResponseModel() { IdDocument = document.IdDocument };
        }
        #endregion
    }
}