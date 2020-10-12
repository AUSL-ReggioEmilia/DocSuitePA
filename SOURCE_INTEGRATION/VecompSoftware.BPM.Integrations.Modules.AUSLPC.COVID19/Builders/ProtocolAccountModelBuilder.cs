using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Models;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Builders
{
    public class ProtocolAccountModelBuilder : IBuilder
    {
        #region [ Fields ]
        private readonly IWebAPIClient _webAPIClient;
        private readonly DocumentUnit _documentUnit;
        private readonly string _fiscalCode;

        private const string PROTOCOL_RECIPIENT_FILTER = "$filter=Protocol/UniqueId eq {0} and FiscalCode eq '{1}' and ComunicationType eq 'D'";
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public ProtocolAccountModelBuilder(IWebAPIClient webAPIClient, DocumentUnit documentUnit, string fiscalCode)
        {
            _webAPIClient = webAPIClient;
            _documentUnit = documentUnit;
            _fiscalCode = fiscalCode;
        }
        #endregion

        #region [ Methods ]
        public AccountModel Build()
        {
            ICollection<ProtocolContactManual> manualContacts = _webAPIClient.GetProtocolContactManualsAsync(string.Format(PROTOCOL_RECIPIENT_FILTER, _documentUnit.UniqueId, _fiscalCode)).Result;
            if (manualContacts == null || manualContacts.Count == 0)
            {
                throw new Exception($"DocumentUnit {_documentUnit.UniqueId} has not recipient with fiscalcode {_fiscalCode}");
            }

            if (manualContacts.Count > 1)
            {
                throw new Exception("Protocol validation exception => Protocol has more than one valid recipient");
            }
            ProtocolContactManual recipient = manualContacts.Single();
            return CreateAccount(recipient);
        }

        private AccountModel CreateAccount(ProtocolContactManual protocolContactManual)
        {
            string validAccountAddress = string.IsNullOrEmpty(protocolContactManual.CertifydMail) ? protocolContactManual.EMailAddress : protocolContactManual.CertifydMail;
            return new AccountModel(protocolContactManual.Description)
            {
                FiscalCode = protocolContactManual.FiscalCode,
                Telephone = protocolContactManual.TelephoneNumber,
                EmailAddress = validAccountAddress
            };
        }
        #endregion        
    }
}
