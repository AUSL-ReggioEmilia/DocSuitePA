using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Formatter;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Conservations;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Report;
using VecompSoftware.DocSuiteWeb.Report.Templates.Protocol;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Facade.Report
{
    public class ProtocolReport : ReportBase<Protocol>
    {
        private ConservationFinder _conservationFinder;

        private FacadeFactory _factory;

        public ProtocolReport(TenantModel tenantModel) : base(ReportType.Rdlc) 
        {
            _conservationFinder = new ConservationFinder(tenantModel);
        }

        protected FacadeFactory Factory
        {
            get { return _factory ?? (_factory = new FacadeFactory("ProtDB")); }
        }

        private ProtocolDataSet _protocolDataSet;

        protected override IList<Protocol> Items
        {
            get
            {
                return base.Items.ToList();
            }
        }

        protected override void DataBind()
        {
            FileLogger.Debug(LoggerName, "ProtocolReport.DataBind() - Caricamento dati di protocollo.");
            _protocolDataSet = new ProtocolDataSet();
            foreach (var protocol in Items)
            {
                var cat = Factory.ProtocolFacade.GetCategory(protocol);

                var row = _protocolDataSet.ProtocolDataTable.NewProtocolDataTableRow();
                row.Protocol_ID = string.Format("{0:0000}/{1:000000}", protocol.Year, protocol.Number);
                row.Protocol_Year = protocol.Year;
                row.Protocol_Number = protocol.Number;
                row.Protocol_Signature = Factory.ProtocolFacade.GenerateSignature(protocol, protocol.RegistrationDate.DateTime, new ProtocolSignatureInfo { DocumentType = ProtocolDocumentType.Main });

                row.Protocol_Object = protocol.ProtocolObject;

                if (protocol.RegistrationDate != null)
                {
                    row.Protocol_RegistrationDate = protocol.RegistrationDate.LocalDateTime;
                    row.Protocol_FullNumber = ProtocolFacade.ProtocolFullNumber(protocol.Year, protocol.Number) + "\r\n" + string.Format("{0:dd/MM/yyyy}", protocol.RegistrationDate);
                }
                row.Protocol_RegistrationUser = protocol.RegistrationUser;

                // CLASSIFICATORE
                row.Category_ID = cat.Id;
                row.Category_Description = Factory.ProtocolFacade.GetCategoryDescription(protocol);
                row.Category_Code = Factory.ProtocolFacade.GetCategoryCode(protocol);
                row.Category_FullCode = cat.FullCodeDotted;


                // CONTENITORE
                if (protocol.Container != null)
                {
                    row.Container_ID = protocol.Container.Id;
                    row.Container_Description = protocol.Container.Name;
                }

                row.Protocol_DocumentCode = protocol.DocumentCode;

                if (protocol.DocumentDate != null)
                {
                    row.Protocol_DocumentDate = protocol.DocumentDate.Value;
                }
                if (protocol.DocumentProtocol != null)
                {
                    row.Protocol_DocumentProtocol = protocol.DocumentProtocol;
                    //Protocollo del mittente già formattato
                    row.Protocol_POrigin = string.Format("{0} {1}", protocol.DocumentProtocol.Replace("|", @"/"), protocol.DocumentDate.HasValue ? protocol.DocumentDate.Value.ToString("dd/MM/yyyy") : "");
                }

                if (protocol.DocumentType != null)
                {
                    row.DocumentType_ID = protocol.DocumentType.Id;
                    row.DocumentType_Description = protocol.DocumentType.Description;
                }
                if (protocol.Status != null) row.Protocol_Advanced_Status = protocol.Status.Description;

                if (protocol.IdStatus.HasValue)
                {
                    row.Protocol_StatusId = protocol.IdStatus.Value.ToString(CultureInfo.InvariantCulture);
                    row.Protocol_StatusDescription = ProtocolFacade.GetStatusDescription(protocol.IdStatus.Value);
                    //Annullamento
                    if (protocol.IdStatus.Value == (int)ProtocolStatusId.Annullato)
                    {
                        row.Protocol_Cancellation = string.Format("Estremi Annullamento: {0}", StringHelper.ReplaceCrLf(protocol.LastChangedReason, ""));
                    }
                }

                if (protocol.Type != null)
                {
                    row.Protocol_TypeId = protocol.Type.Id.ToString(CultureInfo.InvariantCulture);
                    row.Protocol_TypeName = ProtocolTypeFacade.CalcolaTipoProtocolloChar(protocol.Type.Id);
                    row.Protocol_TypeDescription = ProtocolTypeFacade.CalcolaTipoProtocollo(protocol.Type.Id);
                }

                row.Protocol_ServiceCategory = protocol.ServiceCategory;
                row.Protocol_Note = protocol.Note;
                if (protocol.Location != null) row.Protocol_Location = protocol.Location.Name;
                if (protocol.AdvanceProtocol != null) row.Protocol_Proposer = protocol.AdvanceProtocol.Subject;
                row.Protocol_LastChangedDate = protocol.LastChangedDate.DefaultString();
                row.Protocol_LastChangedUser = protocol.LastChangedUser;
                if (DocSuiteContext.Current.ProtocolEnv.ConservationEnabled)
                {
                    WebAPIDto<Conservation> conservation = WebAPIImpersonatorFacade.ImpersonateFinder(_conservationFinder,
                    (impersonationType, finder) =>
                    {
                        _conservationFinder.ResetDecoration();
                        _conservationFinder.EnablePaging = false;
                        _conservationFinder.UniqueId = protocol.UniqueId;
                        return _conservationFinder.DoSearch().FirstOrDefault();
                    });

                    string parerDescription = "Non analizzato";
                    if (conservation != null)
                    {
                        row.Protocol_ParerStatusCode = (int)conservation.Entity.Status;
                        parerDescription = ConservationHelper.StatusDescription[conservation.Entity.Status];
                    }
                    row.Protocol_ParerStatusDescription = parerDescription;
                }

                //Verifico se ci sono settori autorizzati
                row.Protocol_hasRoles = (protocol.Roles.Count > 0).ToString();

                _protocolDataSet.ProtocolDataTable.Rows.Add(row);

                //Mittenti
                foreach (var contact in Factory.ProtocolFacade.GetSenders(protocol))
                {
                    _protocolDataSet.ContactDataTable.Rows.Add(ContactDataBind(contact, row.Protocol_ID, true));
                }

                //Destinatari
                foreach (var contact in Factory.ProtocolFacade.GetRecipients(protocol))
                {
                    _protocolDataSet.ContactDataTable.Rows.Add(ContactDataBind(contact, row.Protocol_ID, false));
                }

                //TODO: inserire il datatable nelle ProcotolDataSet e fare il bind

                //Settori
                foreach (var protocolRole in protocol.Roles)
                {
                    var roleRow = _protocolDataSet.RoleDataTable.NewRoleDataTableRow();
                    roleRow.Protocol_ID = row.Protocol_ID;
                    roleRow.Role_ID = protocolRole.Role.Id;
                    roleRow.Role_Name = protocolRole.Role.Name;
                    _protocolDataSet.RoleDataTable.Rows.Add(roleRow);
                }

                foreach (var protocolEmail in protocol.OutgoingPecMailsActive)
                {
                    var mailrow = _protocolDataSet.PECMailOutgoingDataTable.NewPECMailOutgoingDataTableRow();
                    mailrow.Protocol_ID = row.Protocol_ID;
                    mailrow.IDPECMail = protocolEmail.Id;
                    mailrow.MailSubject = protocolEmail.MailSubject;
                    mailrow.MailDate = protocolEmail.MailDate.HasValue ? protocolEmail.MailDate.Value : DateTime.MinValue;
                    _protocolDataSet.PECMailOutgoingDataTable.Rows.Add(mailrow);
                }

                foreach (var message in protocol.Messages)
                foreach (var mail in message.Message.Emails)    
                {

                    var mailrow = _protocolDataSet.MessageEmailDataTable.NewMessageEmailDataTableRow();
                    mailrow.Protocol_ID = row.Protocol_ID;
                    mailrow.IDMessageEmail = mail.Id;
                    mailrow.Subject = mail.Subject;
                    mailrow.SentDate = mail.SentDate.HasValue ? mail.SentDate.Value : DateTime.MinValue;
                    _protocolDataSet.MessageEmailDataTable.Rows.Add(mailrow);
                }
            }

            DataSource = _protocolDataSet;
        }

        private DataRow ContactDataBind(ContactDTO contact, string protocolId, bool isSender)
        {
            var contactRow = _protocolDataSet.ContactDataTable.NewContactDataTableRow();
            contactRow.Protocol_ID = protocolId;
            contactRow.Contact_IsSender = isSender;
            contactRow.Contact_ID = contact.Id;
            if (contact.Contact != null)
            {
                contactRow.Contact_Description = contact.Contact.Description;
                contactRow.Contact_FullIncrementalPath = contact.Contact.FullIncrementalPath;
                var formattedDescription = string.Empty;
                ContactFacade.FormatContacts(contact.Contact, ref formattedDescription);
                contactRow.Contact_FormattedDescription = formattedDescription;
                contactRow.Contact_BirthDate = string.Format("{0:dd/MM/yyyy}", contact.Contact.BirthDate);
                contactRow.Contact_FullDescription = contact.Contact.FullDescription;
                contactRow.Contact_LastName = contact.Contact.LastName;
                contactRow.Contact_FirstName = contact.Contact.FirstName;
                if (contact.Contact.Address != null)
                {
                    contactRow.Contact_Località = string.Format("{0} {1}{2}", contact.Contact.Address.ZipCode,
                        contact.Contact.Address.City, !string.IsNullOrEmpty(contact.Contact.Address.CityCode) ? string.Format(" ({0})", contact.Contact.Address.CityCode) : string.Empty);
                    contactRow.Contact_Indirizzo = string.Format("{0}{1}{2}",
                                                                 (contact.Contact.Address.PlaceName != null
                                                                      ? contact.Contact.Address.PlaceName.Description + " "
                                                                      : string.Empty),
                                                                 contact.Contact.Address.Address,
                                                                 !string.IsNullOrEmpty(contact.Contact.Address.CivicNumber)
                                                                      ? string.Format(", {0}", contact.Contact.Address.CivicNumber)
                                                                      : string.Empty);
                    contactRow.Contact_Provincia = contact.Contact.Address.CityCode;
                    contactRow.Contact_Citta = contact.Contact.Address.City;
                    contactRow.Contact_CAP = contact.Contact.Address.ZipCode;
                }
            }
            return contactRow;
        }
    }
}
