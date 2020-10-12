using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.Helpers;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.StampaConforme;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Tasks;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.EventBuilders.Builders
{
    public class ProtocolEventBuilder : IBuilder
    {
        #region [ Fields ]
        private readonly DocumentUnit _documentUnit;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IDocumentClient _documentClient;
        private readonly IStampaConformeClient _stampaConformeClient;
        private readonly string _signatureTemplate;

        private const string PROTOCOL_FILTER = "$filter=UniqueId eq {0}&$expand=AdvancedProtocol";
        private const string PROTOCOL_RECIPIENT_FILTER = "$filter=Protocol/UniqueId eq {0} and FiscalCode ne '' and TelephoneNumber ne '' and ComunicationType eq 'D'";
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public ProtocolEventBuilder(IWebAPIClient webAPIClient, IDocumentClient documentClient, 
            IStampaConformeClient stampaConformeClient, string signatureTemplate, DocumentUnit documentUnit, ModuleConfigurationModel moduleConfiguration)
        {
            _webAPIClient = webAPIClient;
            _documentClient = documentClient;
            _stampaConformeClient = stampaConformeClient;
            _signatureTemplate = signatureTemplate;
            _documentUnit = documentUnit;
        }
        #endregion

        #region [ Methods ]
        public DocSuiteEvent Build()
        {
            Protocol protocol = _webAPIClient.GetProtocolAsync(string.Format(PROTOCOL_FILTER, _documentUnit.UniqueId)).Result.Single();
            DocSuiteEvent @event = new DocSuiteEvent()
            {
                WorkflowReferenceId = _documentUnit.IdWorkflowActivity,
                EventDate = _documentUnit.RegistrationDate,
                EventModel = new DocSuiteModel()
                {
                    UniqueId = _documentUnit.UniqueId,
                    Title = string.Format(DocSuiteModel.PROTOCOL_TITLE_FORMAT, _documentUnit.Year, _documentUnit.Number),
                    Year = _documentUnit.Year,
                    Number = _documentUnit.Number,
                    ModelStatus = DocSuiteStatus.Activated,
                    ModelType = DocSuiteType.Protocol
                }
            };

            ProtocolContactManual recipient = GetRecipient();
            @event.EventModel.CustomProperties = GetEventProperties(protocol, recipient);
            return @event;
        }

        private ProtocolContactManual GetRecipient()
        {
            ICollection<ProtocolContactManual> recipients = _webAPIClient.GetProtocolContactManualsAsync(string.Format(PROTOCOL_RECIPIENT_FILTER, _documentUnit.UniqueId)).Result;
            if (recipients.Count > 1)
            {
                throw new Exception("Protocol validation exception => Protocol has more than one valid recipient");
            }
            return recipients.Single();
        }

        private IDictionary<string, string> GetEventProperties(Protocol protocol, ProtocolContactManual recipient)
        {
            IDictionary<string, string> customProperties = new Dictionary<string, string>()
            {
                { SWAFEventPropertyNames.SWAF_EVENT_TYPE, "NewDocument" },
                { SWAFEventPropertyNames.SWAF_DOCUMENT_TYPE, protocol.AdvancedProtocol.ServiceCategory },
                { SWAFEventPropertyNames.FISCAL_CODE, recipient.FiscalCode },
                { SWAFEventPropertyNames.NAME, recipient.GetFirstname() },
                { SWAFEventPropertyNames.SURNAME, recipient.GetSurname() },
                { SWAFEventPropertyNames.BORNDATE, recipient.GetBirthDateFormatted() },
                { SWAFEventPropertyNames.MOBILE, recipient.TelephoneNumber },
                { SWAFEventPropertyNames.EMAIL, recipient.GetValidAddress() },
                { SWAFEventPropertyNames.ADDRESS, recipient.Address },
                { SWAFEventPropertyNames.CIVIC_NUMBER, recipient.CivicNumber },
                { SWAFEventPropertyNames.ZIP_CODE, recipient.ZipCode },
                { SWAFEventPropertyNames.CITY, recipient.City },
                { SWAFEventPropertyNames.COUNTRY, recipient.CityCode }
            };

            ArchiveDocument mainDocument = GetMainDocument();
            byte[] mainDocumentContent = _documentClient.GetDocumentContentByIdAsync(mainDocument.IdDocument).Result.Blob;
            string signature = StampaConformeClient.GetSignature(_signatureTemplate, mainDocument.Metadata[_documentClient.ATTRIBUTE_SIGNATURE] as string);
            byte[] convertedMainDocumentContent = _stampaConformeClient.ConvertToPDFAAsync(mainDocumentContent, Path.GetExtension(mainDocument.Name), signature).Result;

            customProperties.Add(SWAFEventPropertyNames.DOCUMENT_STREAM, Convert.ToBase64String(convertedMainDocumentContent));
            customProperties.Add(SWAFEventPropertyNames.FILENAME, $"{mainDocument.Name}.pdf");

            TaskHeaderProtocol taskHeaderProtocol = _webAPIClient.GetTaskHeaderProtocolAsync(protocol).Result;
            if (taskHeaderProtocol != null)
            {
                customProperties.Add(SWAFEventPropertyNames.HEADER_ID, taskHeaderProtocol.TaskHeader.EntityId.ToString());
            }
            return customProperties;
        }

        private ArchiveDocument GetMainDocument()
        {
            ICollection<DocumentUnitChain> documentChains = _webAPIClient.GetDocumentUnitChainsAsync(_documentUnit.UniqueId).Result;
            DocumentUnitChain mainDocument = documentChains.Single(x => x.ChainType == ChainType.MainChain);
            return _documentClient.GetChildrenAsync(mainDocument.IdArchiveChain).Result.Single();
        }
        #endregion        
    }
}
