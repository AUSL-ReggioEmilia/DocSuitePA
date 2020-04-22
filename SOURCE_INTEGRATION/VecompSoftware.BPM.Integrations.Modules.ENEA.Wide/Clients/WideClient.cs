using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.Configurations;
using VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.Extensions;
using VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.InserisciAllegatiService;
using VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.Models;
using VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.ProtocollaService;
using VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.TipoDocumentoService;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using BiblosDocument = VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService.Document;
using Category = VecompSoftware.DocSuiteWeb.Entity.Commons.Category;
using DocumentUnit = VecompSoftware.DocSuiteWeb.Entity.DocumentUnits.DocumentUnit;

namespace VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.Clients
{
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class WideClient
    {
        #region [ Fields ]
        private const string TIPO_DOC_CHIAVE = "TUTTI";
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IWebAPIClient _webAPIClient;
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(WideClient));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]

        public WideClient(IWebAPIClient webAPIClient, ILogger logger)
        {
            _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
            _webAPIClient = webAPIClient;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        public inserisciAllegatiResponse InserimentoAllegati(List<BiblosDocument> documents, Tipologia_Type tipologia, string widePath, string wideProtocolNummber)
        {
            inserisciAllegatiResponse inserimentoResponse = null;
            using (InserisciAllegatiServicePortTypeClient allegatiSvc = CreateAllegatiServiceClient())
            {
                Tipologia_Type[] tipologies = documents.Select(_ => tipologia).ToArray();
                string[] names = documents.Select(x => Convert.ToBase64String(Encoding.Default.GetBytes(x.Name))).ToArray();
                inserisciAllegatiRequest protRequest = new inserisciAllegatiRequest()
                {
                    NumeroProtocollo = wideProtocolNummber,
                    TipoAllegato = TipoAllegato_Type.P,
                    Tipologia = tipologies,
                    Descrizione = names,
                    NomeFile = documents.Select(x => x.Name).ToArray(),
                    PathFile = $"{_moduleConfiguration.WideRelativePath}/{widePath}",
                    Titolo = names,
                    utente = _moduleConfiguration.WideUsername
                };

                _logger.WriteDebug(new LogMessage($"Inserting protocol into WIDE {wideProtocolNummber} ..."), LogCategories);
                _logger.WriteDebug(new LogMessage(JsonConvert.SerializeObject(protRequest)), LogCategories);
                inserimentoResponse = allegatiSvc.InserimentoAllegati(protRequest);
            }

            if (inserimentoResponse == null)
            {
                throw new NullReferenceException($"ENEA.ProtocolWide -> ProtocolloService returned null response");
            }

            if (inserimentoResponse.esito != InserisciAllegatiService.Esito_Type.Item0)
            {
                throw new Exception($"ENEA.ProtocolWide -> TipoDocService returned with the following error description \'{inserimentoResponse.DescrizioneErrore}\'");
            }

            _logger.WriteInfo(new LogMessage("ENEA.ProtocolWide -> InserimentoAllegatiAsync returned successfully"), LogCategories);

            return inserimentoResponse;
        }

        public async Task<protDocumentoResponse> ProtocolloRegistrazioneAsync(DocumentUnit documentUnit, string tipoDocumento, Guid udsId, Registro_Type registro_Type)
        {
            protDocumentoResponse protocolRegistration = null;
            using (ProtocollaServicePortTypeClient protocollaSvc = CreateProtocollaServiceClient())
            {
                string fullIncrementalPath = documentUnit.Category?.FullIncrementalPath ?? string.Empty;
                List<string> categoryIds = fullIncrementalPath.Split('|').ToList();

                string classificationLevel1 = await GetCategoryNameAtIndexAsync(categoryIds, 0).ConfigureAwait(false);
                string classificationLevel2 = await GetCategoryNameAtIndexAsync(categoryIds, 1).ConfigureAwait(false);
                string classificationLevel3 = await GetCategoryNameAtIndexAsync(categoryIds, 2).ConfigureAwait(false);
                string classificationLevel4 = await GetCategoryNameAtIndexAsync(categoryIds, 3).ConfigureAwait(false);

                List<DocSuiteWeb.Entity.Commons.Category> categories = documentUnit.Category.Categories.ToList();
                ICollection<UDSContact> udsContacts = await _webAPIClient.GetUDSContacts(udsId).ConfigureAwait(false);

                string senderNames = GetUserSenderNames(udsContacts);
                string recipientNames = GetUserRecipientNames(udsContacts);

                protDocumentoRequest protRequest = new protDocumentoRequest
                {
                    Registro = registro_Type,
                    Oggetto = Convert.ToBase64String(Encoding.Default.GetBytes(documentUnit.Subject.Truncate(256))),
                    Mittente = Convert.ToBase64String(Encoding.Default.GetBytes(string.IsNullOrEmpty(senderNames) ? "ENEA" : senderNames.Truncate(256))),
                    Destinatari = Convert.ToBase64String(Encoding.Default.GetBytes(recipientNames.Truncate(256))),
                    TipoDocumento = tipoDocumento,
                    OriginalePresso = null,//"ENEA",
                    utente = _moduleConfiguration.WideUsername,
                    Livello1Classificazione = classificationLevel1,
                    Livello2Classificazione = string.IsNullOrEmpty(classificationLevel2) ? null : classificationLevel2,
                    Livello3Classificazione = string.IsNullOrEmpty(classificationLevel3) ? null : classificationLevel3,
                    Livello4Classificazione = string.IsNullOrEmpty(classificationLevel4) ? null : classificationLevel4
                };
                _logger.WriteDebug(new LogMessage(JsonConvert.SerializeObject(protRequest)), LogCategories);
                protocolRegistration = protocollaSvc.ProtocolloRegistrazione(protRequest);
            }

            if (protocolRegistration == null)
            {
                throw new NullReferenceException($"ENEA.ProtocolWide -> ProtocolloService returned null response");
            }

            if (protocolRegistration.esito != ProtocollaService.Esito_Type.Item0)
            {
                throw new Exception($"ENEA.ProtocolWide -> TipoDocService returned with the following error description \'{protocolRegistration.DescrizioneErrore}\'");
            }

            _logger.WriteInfo(new LogMessage($"ENEA.ProtocolWide -> ProtocolloRegistrazione returned successfully WideProtocolNumber: {protocolRegistration.NumeroProtocollo}"), LogCategories);

            return protocolRegistration;
        }

        private async Task<string> GetCategoryNameAtIndexAsync(List<string> categoryIds, int index)
        {
            if (categoryIds == null)
            {
                return string.Empty;
            }

            if (categoryIds.Count > index)
            {
                int shortId = 0;
                bool parseResult = int.TryParse(categoryIds[index], out shortId);
                if (parseResult)
                {
                    Category cat = await _webAPIClient.GetCategoryAsync(shortId).ConfigureAwait(false);

                    return cat?.Name ?? string.Empty;
                }
            }

            return string.Empty;
        }

        public TipoDocDocumentoResponse TipoDocDocumento()
        {
            TipoDocDocumentoResponse tipoDocResponse = null;

            using (TipoDocServicePortTypeClient tipoDocSvc = CreateTipoDocWebServiceClient())
            {
                TipoDocDocumentoRequest tipoRequest = new TipoDocDocumentoRequest
                {
                    Chiave = TIPO_DOC_CHIAVE
                };

                tipoDocResponse = tipoDocSvc.TipoDocDocumento(tipoRequest);
            }

            if (tipoDocResponse == null)
            {
                throw new NullReferenceException($"ENEA.ProtocolWide -> TipoDocService returned null response.");
            }

            if (tipoDocResponse.esito != TipoDocumentoService.Esito_Type.Item0)
            {
                throw new Exception($"ENEA.ProtocolWide -> TipoDocService returned with the following error description \'{tipoDocResponse.DescrizioneErrore}\'");
            }

            _logger.WriteInfo(new LogMessage("ENEA.ProtocolWide -> TipoDocDocumento returned successfully"), LogCategories);

            return tipoDocResponse;
        }

        private InserisciAllegatiServicePortTypeClient CreateAllegatiServiceClient()
        {
            BasicHttpBinding binding = new BasicHttpBinding { Name = "InserisciAllegatiServicePort" };
            EndpointAddress endpoint = new EndpointAddress($"{_moduleConfiguration.WideWebServiceUrl}/InserisciAllegatiWebService");
            return new InserisciAllegatiServicePortTypeClient(binding, endpoint);
        }

        private TipoDocServicePortTypeClient CreateTipoDocWebServiceClient()
        {
            BasicHttpBinding binding = new BasicHttpBinding { Name = "InserisciAllegatiServicePort" };

            EndpointAddress endpoint = new EndpointAddress($"{_moduleConfiguration.WideWebServiceUrl}/TipoDocWebService");
            return new TipoDocServicePortTypeClient(binding, endpoint);
        }

        private ProtocollaServicePortTypeClient CreateProtocollaServiceClient()
        {
            BasicHttpBinding binding = new BasicHttpBinding { Name = "InserisciAllegatiServicePort" };
            EndpointAddress endpoint = new EndpointAddress($"{_moduleConfiguration.WideWebServiceUrl}/ProtocolWebService");

            return new ProtocollaServicePortTypeClient(binding, endpoint);
        }

        private string GetUserRecipientNames(ICollection<UDSContact> udsContacts)
        {

            List<string> contactDescriptions = udsContacts
                    .Where(f => f.ContactLabel == "Destinatario" && f.Relation != null)
                    .Select(f => f.Relation.Description)
                    .ToList();

            contactDescriptions.AddRange(
                   udsContacts.Where(f => f.ContactLabel == "Destinatario" && !string.IsNullOrEmpty(f.ContactManual))
                              .Select(f => JsonConvert.DeserializeObject<ContactManualHeaderModel>(f.ContactManual).Contact.Description)
                );

            return string.Join(";", contactDescriptions);
        }

        private string GetUserSenderNames(ICollection<UDSContact> udsContacts)
        {
            List<string> contactDescriptions = udsContacts
                    .Where(f => f.ContactLabel == "Mittenti" && f.Relation != null)
                    .Select(f => f.Relation.Description)
                    .ToList();

            contactDescriptions.AddRange(
                   udsContacts.Where(f => f.ContactLabel == "Mittenti" && !string.IsNullOrEmpty(f.ContactManual))
                              .Select(f => JsonConvert.DeserializeObject<ContactManualHeaderModel>(f.ContactManual).Contact.Description)
                );

            return string.Join(";", contactDescriptions);
        }

        #endregion
    }
}
