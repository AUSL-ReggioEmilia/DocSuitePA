using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.EventBuilders.Builders;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.StampaConforme;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.EventBuilders
{
    public class EventBuilder
    {
        #region [ Fields ]
        private readonly IWebAPIClient _webAPIClient;
        private readonly IDocumentClient _documentClient;
        private readonly IStampaConformeClient _stampaConformeClient;        
        private readonly IDictionary<Func<DocumentUnit, IBuilder>, Func<DocumentUnit, bool>> _builderRulesValidations;

        private string _signatureTemplate;
        #endregion

        #region [ Properties ]
        private string SignatureTemplate
        {
            get
            {
                if (string.IsNullOrEmpty(_signatureTemplate))
                {
                    _signatureTemplate = _webAPIClient.GetParameterSignatureTemplate().Result;
                }
                return _signatureTemplate;
            }
        }
        #endregion

        #region [ Constructor ]
        public EventBuilder(IWebAPIClient webAPIClient, IDocumentClient documentClient, IStampaConformeClient stampaConformeClient, ModuleConfigurationModel moduleConfiguration)
        {
            _webAPIClient = webAPIClient;            
            _documentClient = documentClient;
            _stampaConformeClient = stampaConformeClient;

            _builderRulesValidations = new Dictionary<Func<DocumentUnit, IBuilder>, Func<DocumentUnit, bool>>()
            {
                { (du) => new ProtocolEventBuilder(webAPIClient, documentClient, stampaConformeClient, SignatureTemplate, du, moduleConfiguration), 
                    (du) => du.Environment == (int)DSWEnvironmentType.Protocol },
                { (du) => new ArchiveEventBuilder(webAPIClient, documentClient, stampaConformeClient, SignatureTemplate, du, moduleConfiguration),
                    (du) => du.Environment >= 100 }
            };
        }
        #endregion

        #region [ Methods ]
        private IBuilder GetBuilder(DocumentUnit documentUnit)
        {
            return _builderRulesValidations.Where(x => x.Value(documentUnit) == true)
                .Select(s => s.Key(documentUnit)).FirstOrDefault();
        }

        public DocSuiteEvent CreateSwafEvent(DocumentUnit documentUnit)
        {
            IBuilder builder = GetBuilder(documentUnit);
            if (builder == null)
            {
                throw new Exception();
            }

            return builder.Build();
        }
        #endregion
    }
}
