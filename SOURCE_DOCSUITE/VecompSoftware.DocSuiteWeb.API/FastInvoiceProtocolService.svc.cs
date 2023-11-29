using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using VecompSoftware.DocSuiteWeb.API.Helpers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.Helpers;
using VecompSoftware.NHibernateManager;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class FastInvoiceProtocolService : IFastInvoiceProtocolService
    {        
        #region Service Methods
        public bool IsAlive()
        {
            return true;
        }

        public string GetAccountingSectionals()
        {
            try
            {
                ContainerExtensionFacade containerExt = new ContainerExtensionFacade("ProtDB");
                var dtos = containerExt.GetAllAccountingSectionals().Select(c => new AccountingSectionalDTO(c.AccountingSectionalNumber).CopyFrom(c)).ToArray();
                return dtos.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                var response = new APIResponse<IAccountingSectionalDTO[]>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetContacts(string searchCode)
        {
            try
            {
                ContactDTO[] dtos = FacadeFactory.Instance.ContactFacade.GetContactBySearchCode(searchCode, true)
                    .Select(c => new ContactDTO() { Description = c.Description, Id = c.Id, EmailAddress = c.EmailAddress, FiscalCode = c.FiscalCode })
                    .ToArray();
                return dtos.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                var response = new APIResponse<IContactDTO[]>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetContainers()
        {
            try
            {
                var dtos = FacadeFactory.Instance.ContainerFacade.GetAll().Select(c => new ContainerDTO(c.Id).CopyFrom(c)).ToArray();
                return dtos.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                var response = new APIResponse<IContainerDTO[]>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetCategories()
        {
            try
            {
                var dtos = FacadeFactory.Instance.CategoryFacade.GetAll()
                    .OrderBy(c => c.FullCode)
                    .Select(this.GetCategoryDTO).ToArray();
                return dtos.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                var response = new APIResponse<CategoryDTO[]>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetPECMailboxes()
        {
            try
            {
                var dtos = FacadeFactory.Instance.PECMailboxFacade.GetAll().Select(m => new MailboxDTO(m.MailBoxName).CopyFrom(m)).ToArray();
                return dtos.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                var response = new APIResponse<MailboxDTO[]>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetProtocolsToSend()
        {
            try
            {
                var status = new List<int>
                {
                    (int) ProtocolStatusId.Attivo,
                    (int) ProtocolStatusId.PAInvoiceRefused,
                    (int) ProtocolStatusId.PAInvoiceSdiRefused
                };
                var dtos = FacadeFactory.Instance.ProtocolFacade.GetInvoicePaProtocolsByStatuses(status).ToArray();
                return dtos.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                var response = new APIResponse<MailboxDTO[]>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public int CountProtocolReserved(DateTime from, DateTime to)
        {
            try
            {
                return FacadeFactory.Instance.ProtocolFacade.CountProtSuspended((short)(DateTime.Now.Year - 1), from, to);
            }
            catch (Exception)
            {
                return 0;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string InsertProtocol(string protocolDTO)
        {
            try
            {
                var protocol = protocolDTO.Deserialize<ProtocolDTO>();
                protocol.IdTenantAOO = ConfigurationHelper.CurrentTenantAOOId;
                switch (protocol.Direction)
                {
                    case 1:
                        if(!(protocol.HasRecipients() || protocol.HasRecipientsManual()))
                            throw new Exception("Nessun destinatario impostato nel DTO passato.");
                        break;
                    case -1:
                        if(!(protocol.HasSenders() || protocol.HasSendersManual()))
                            throw new Exception("Nessun mittente impostato nel DTO passato.");
                        break;
                    default:
                        throw new Exception("Direction non valida");
                }

                ProtocolDTO result = ProtocolService.InsertInvoice(protocol);
                if (result.IdProtocolKind.HasValue && (short)ProtocolKind.FatturePA == result.IdProtocolKind.Value)
                {
                    MailDTO mailDTO = new MailDTO();
                    mailDTO.Subject = result.Subject;
                    mailDTO.Sender = new ContactDTO();
                    Container currentContainer = FacadeFactory.Instance.ContainerFacade.GetById(protocol.Container.Id.Value, false);
                    ContainerEnv currentContainerEnv = new ContainerEnv(DocSuiteContext.Current, ref currentContainer);
                    if (string.IsNullOrEmpty(currentContainerEnv.InvoicePAContactSDI) || !currentContainerEnv.InvoicePAMailboxSenderId.HasValue)
                    {
                        FileLogger.Warn(LogName.FileLog, string.Concat("Protocollo ", result.Number, " di fattura ", result.InvoiceNumber, " inserito correttamente con invio PEC fallito. [Parametro InvoicePAContactSDI ( ", currentContainerEnv.InvoicePAContactSDI, " ) o InvoicePAMailboxSender ( ", currentContainerEnv.InvoicePAMailboxSenderId, " ) mancante ]"));
                        throw new ApplicationException(string.Concat("Protocollo ", result.Number, " di fattura ", result.InvoiceNumber, " inserito correttamente con invio PEC fallito. [Parametro InvoicePAContactSDI ( ", currentContainerEnv.InvoicePAContactSDI, " ) o InvoicePAMailboxSender ( ", currentContainerEnv.InvoicePAMailboxSenderId, " ) mancante ]"));                        
                    }
                    mailDTO.AddRecipient(new ContactDTO(currentContainerEnv.InvoicePAContactSDI));
                    PECMailBox mailbox = FacadeFactory.Instance.PECMailboxFacade.GetById(currentContainerEnv.InvoicePAMailboxSenderId.Value);
                    mailDTO.Mailbox = new MailboxDTO(mailbox.MailBoxName).CopyFrom(mailbox);
                    mailDTO.Sender.EmailAddress = mailbox.MailBoxName;
                    mailDTO.Body = result.Subject;
                    ProtocolDTO mailProtocolDTO = result;
                    mailProtocolDTO.Annexes = new List<DocumentDTO>().ToArray();
                    mailProtocolDTO.Attachments = new List<DocumentDTO>().ToArray();
                    var inputMail = mailDTO.Serialize();
                    var email = SendProtocolMail(inputMail, mailProtocolDTO.Serialize());
                }
                return result.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                var response = new APIResponse<ProtocolDTO>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string SendMail(string mailDTO)
        {
            try
            {
                var mail = mailDTO.Deserialize<MailDTO>();

                if (mail.Sender != null)
                {
                    if (!RegexHelper.IsValidEmail(mail.Sender.EmailAddress))
                        mail.Sender.Code = mail.Sender.EmailAddress;
                }

                if (mail.HasRecipients())
                {
                    foreach (var item in mail.Recipients)
                        if (!RegexHelper.IsValidEmail(item.EmailAddress))
                            item.Code = item.EmailAddress;
                }

                if (mail.HasRecipientsCc())
                {
                    foreach (var item in mail.RecipientsCc)
                        if (!RegexHelper.IsValidEmail(item.EmailAddress))
                            item.Code = item.EmailAddress;
                }

                if (mail.HasRecipientsBcc())
                {
                    foreach (var item in mail.RecipientsBcc)
                        if (!RegexHelper.IsValidEmail(item.EmailAddress))
                            item.Code = item.EmailAddress;
                }

                var result = MailService.Send(mail, false);
                return result.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                var response = new APIResponse<MailboxDTO[]>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string SendProtocolMail(string mailDTO, string protocolDTO)
        {
            try
            {
                var mail = mailDTO.Deserialize<MailDTO>();
                var protocol = protocolDTO.Deserialize<ProtocolDTO>();
                if (!protocol.HasAnyDocument() && protocol.HasId())
                {
                    var domain = FacadeFactory.Instance.ProtocolFacade.GetById(protocol.UniqueId.Value);
                    protocol.CopyFrom(domain);
                }
                mail.CopyFrom(protocol);

                if (mail.Sender != null)
                {
                    if (!RegexHelper.IsValidEmail(mail.Sender.EmailAddress))
                        mail.Sender.Code = mail.Sender.EmailAddress;
                }

                if (mail.HasRecipients())
                {
                    foreach (var item in mail.Recipients)
                        if (!RegexHelper.IsValidEmail(item.EmailAddress))
                            item.Code = item.EmailAddress;
                }

                if (mail.HasRecipientsCc())
                {
                    foreach (var item in mail.RecipientsCc)
                        if (!RegexHelper.IsValidEmail(item.EmailAddress))
                            item.Code = item.EmailAddress;
                }

                if (mail.HasRecipientsBcc())
                {
                    foreach (var item in mail.RecipientsBcc)
                        if (!RegexHelper.IsValidEmail(item.EmailAddress))
                            item.Code = item.EmailAddress;
                }

                mail = MailService.Send(mail, false);
                mail = PairToProtocol(mail, protocol);

                return mail.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                var response = new APIResponse<MailDTO>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }
        #endregion

        #region Private Methods
        private CategoryDTO GetCategoryDTO(Category category)
        {
            var result = new CategoryDTO().CopyFrom(category);
            if (!string.IsNullOrWhiteSpace(category.FullCode))
                result.FullCode = category.FullCode;
            return result;
        }

        internal static MailDTO PairToProtocol(MailDTO mailDTO, ProtocolDTO protocolDTO)
        {
            var mailbox = (MailboxDTO)mailDTO.Mailbox;
            if (mailbox.IsPEC())
            {
                FacadeFactory.Instance.PECMailFacade.PairToProtocol(mailDTO, protocolDTO);
                return mailDTO;
            }

            var message = string.Format("Mailbox.TypeName \"{0}\" non gestito.", mailDTO.Mailbox.TypeName);
            throw new NotImplementedException(message);
        }
        #endregion
    }
}
