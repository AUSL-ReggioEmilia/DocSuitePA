using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.StampaConforme;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.EventBuilders.Builders
{
    public class ArchiveEventBuilder : IBuilder
    {
        #region [ Fields ]
        private readonly DocumentUnit _documentUnit;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IDocumentClient _documentClient;
        private readonly IStampaConformeClient _stampaConformeClient;
        private readonly string _signatureTemplate;
        private readonly ModuleConfigurationModel _moduleConfiguration;

        private const string EVENT_TITLE = "Archivio {0} {1}";
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public ArchiveEventBuilder(IWebAPIClient webAPIClient, IDocumentClient documentClient,
            IStampaConformeClient stampaConformeClient, string signatureTemplate, DocumentUnit documentUnit, ModuleConfigurationModel moduleConfiguration)
        {
            _webAPIClient = webAPIClient;
            _documentClient = documentClient;
            _stampaConformeClient = stampaConformeClient;
            _signatureTemplate = signatureTemplate;
            _documentUnit = documentUnit;
            _moduleConfiguration = moduleConfiguration;
        }
        #endregion

        #region [ Methods ]
        public DocSuiteEvent Build()
        {
            Dictionary<int, Guid> documents = new Dictionary<int, Guid>();
            string controllerName = VecompSoftware.Helpers.UDS.Utils.GetWebAPIControllerName(_documentUnit.DocumentUnitName);
            IDictionary<string, object> uds_metadatas = _webAPIClient.GetUDS(controllerName, _documentUnit.UniqueId, documents).Result;
            DocSuiteEvent @event = new DocSuiteEvent()
            {
                WorkflowReferenceId = _documentUnit.IdWorkflowActivity,
                EventDate = _documentUnit.RegistrationDate,
                EventModel = new DocSuiteModel()
                {
                    UniqueId = _documentUnit.UniqueId,
                    Title = string.Format(EVENT_TITLE, _documentUnit.DocumentUnitName, _documentUnit.Title),
                    Year = _documentUnit.Year,
                    Number = _documentUnit.Number,
                    ModelStatus = DocSuiteStatus.Activated,
                    ModelType = DocSuiteType.UDS
                }
            };

            UDSContact contact = GetContact();
            @event.EventModel.CustomProperties = GetEventProperties(uds_metadatas, contact);
            return @event;
        }

        private UDSContact GetContact()
        {
            ICollection<UDSContact> contacts = _webAPIClient.GetUDSContacts(_documentUnit.UniqueId).Result;
            ICollection<UDSContact> swafContacts = contacts.Where(x => x.ContactType == (int)UDSContactType.Manual && !string.IsNullOrEmpty(x.ContactManual) && x.ContactLabel == _moduleConfiguration.ArchiveContactSectionLabel).ToList();
            if (swafContacts.Count == 0)
            {
                throw new Exception($"Archive validation exception => Archive has not contacts for section {_moduleConfiguration.ArchiveContactSectionLabel}");
            }
            if (swafContacts.Count > 1)
            {
                throw new Exception($"Archive validation exception => Archive has more than one valid contact for section {_moduleConfiguration.ArchiveContactSectionLabel}");
            }
            return swafContacts.Single();
        }

        private IDictionary<string, string> GetEventProperties(IDictionary<string, object> uds_metadatas, UDSContact contact)
        {
            KeyValuePair<string, object> documentTypeMetadata = uds_metadatas.FirstOrDefault(x => x.Key == _moduleConfiguration.ArchiveDocumentTypeMetadata);
            ContactManualHeaderModel manualModel = JsonConvert.DeserializeObject<ContactManualHeaderModel>(contact.ContactManual);

            IDictionary<string, string> customProperties = new Dictionary<string, string>()
            {
                { SWAFEventPropertyNames.SWAF_EVENT_TYPE, "NewDocument" },
                { SWAFEventPropertyNames.SWAF_DOCUMENT_TYPE, documentTypeMetadata.Value == null ? string.Empty : documentTypeMetadata.Value.ToString()  },
                { SWAFEventPropertyNames.FISCAL_CODE, manualModel.Contact.FiscalCode },
                { SWAFEventPropertyNames.NAME, manualModel.Contact.GetFirstname() },
                { SWAFEventPropertyNames.SURNAME, manualModel.Contact.GetSurname() },
                { SWAFEventPropertyNames.BORNDATE, manualModel.Contact.GetBirthDateFormatted() },
                { SWAFEventPropertyNames.MOBILE, manualModel.Contact.TelephoneNumber },
                { SWAFEventPropertyNames.EMAIL, manualModel.Contact.GetValidAddress() },
                { SWAFEventPropertyNames.ADDRESS, manualModel.Contact.Address.Address },
                { SWAFEventPropertyNames.CIVIC_NUMBER, manualModel.Contact.Address.CivicNumber },
                { SWAFEventPropertyNames.ZIP_CODE, manualModel.Contact.Address.ZipCode },
                { SWAFEventPropertyNames.CITY, manualModel.Contact.Address.City },
                { SWAFEventPropertyNames.COUNTRY, manualModel.Contact.Address.CityCode }
            };

            ArchiveDocument mainDocument = GetMainDocument();
            byte[] mainDocumentContent = _documentClient.GetDocumentContentByIdAsync(mainDocument.IdDocument).Result.Blob;
            string signature = StampaConformeClient.GetSignature(_signatureTemplate, mainDocument.Metadata[_documentClient.ATTRIBUTE_SIGNATURE] as string);
            byte[] convertedMainDocumentContent = _stampaConformeClient.ConvertToPDFAAsync(mainDocumentContent, Path.GetExtension(mainDocument.Name), signature).Result;

            customProperties.Add(SWAFEventPropertyNames.DOCUMENT_STREAM, Convert.ToBase64String(convertedMainDocumentContent));
            customProperties.Add(SWAFEventPropertyNames.FILENAME, $"{mainDocument.Name}.pdf");

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
