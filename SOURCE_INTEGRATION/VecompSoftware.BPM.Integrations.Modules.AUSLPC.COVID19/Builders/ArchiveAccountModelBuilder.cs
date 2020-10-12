using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Models;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Builders
{
    public class ArchiveAccountModelBuilder : IBuilder
    {
        #region [ Fields ]
        private readonly IWebAPIClient _webAPIClient;
        private readonly DocumentUnit _documentUnit;
        private readonly string _fiscalCode;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public ArchiveAccountModelBuilder(IWebAPIClient webAPIClient, DocumentUnit documentUnit, string fiscalCode)
        {
            _webAPIClient = webAPIClient;
            _documentUnit = documentUnit;
            _fiscalCode = fiscalCode;
        }
        #endregion

        #region [ Methods ]
        public AccountModel Build()
        {
            ICollection<UDSContact> contacts = _webAPIClient.GetUDSContacts(_documentUnit.UniqueId).Result;
            if (contacts == null || contacts.Count(x => !string.IsNullOrEmpty(x.ContactManual)) == 0)
            {
                throw new Exception($"DocumentUnit {_documentUnit.UniqueId} has not contacts to process");
            }

            ContactManualHeaderModel contact = contacts.Where(x => !string.IsNullOrEmpty(x.ContactManual))
                .Select(s => JsonConvert.DeserializeObject<ContactManualHeaderModel>(s.ContactManual))
                .Where(x => x.Contact.FiscalCode == _fiscalCode).SingleOrDefault();
            if (contact == null)
            {
                throw new Exception($"DocumentUnit {_documentUnit.UniqueId} has not contact with fiscal code {_fiscalCode}");
            }

            return CreateAccount(contact);
        }

        private AccountModel CreateAccount(ContactManualHeaderModel contact)
        {
            string validAccountAddress = string.IsNullOrEmpty(contact.Contact.CertifiedMail) ? contact.Contact.EmailAddress : contact.Contact.CertifiedMail;
            return new AccountModel(contact.Contact.Description)
            {
                FiscalCode = contact.Contact.FiscalCode,
                Telephone = contact.Contact.TelephoneNumber,
                EmailAddress = validAccountAddress
            };
        }
        #endregion        
    }
}
