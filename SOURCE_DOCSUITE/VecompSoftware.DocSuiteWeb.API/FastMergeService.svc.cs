using Newtonsoft.Json;
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

    public class FastMergeService : IFastMergeService
    {
        #region [ Methods ]
        public void LogError(Exception ex)
        {
            if (ex == null)
            {
                return;
            }
            FileLogger.Error(LogName.FileLog, ex.Message, ex);
            LogError(ex.InnerException);
        }

        public bool IsAlive()
        {
            return true;
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
                LogError(ex);
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
                    .Select(c => this.GetCategoryDTO(c)).ToArray();
                return dtos.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                LogError(ex);
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
                LogError(ex);
                var response = new APIResponse<MailboxDTO[]>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetPOLMailboxes()
        {
            try
            {
                return ActionHelper.ImpersonatedAction(() =>
                {
                    var dtos = FacadeFactory.Instance.PosteOnLineAccountFacade.GetUserAccounts(ConfigurationHelper.CurrentTenantAOOId).Select(m => new MailboxDTO().CopyFrom(m)).ToArray();
                    return dtos.SerializeAsResponse();
                });
            }
            catch (Exception ex)
            {
                LogError(ex);
                var response = new APIResponse<MailboxDTO[]>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetDocumentTypes()
        {
            try
            {
                var dtos = FacadeFactory.Instance.TableDocTypeFacade.GetAll().Select(t => new TableDocTypeDTO().CopyFrom(t)).ToArray();
                return dtos.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                LogError(ex);
                var response = new APIResponse<TableDocTypeDTO[]>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetServiceCategories()
        {
            try
            {
                var dtos = new ServiceCategoryFacade().GetAll().Select(c => new ServiceCategoryDTO().CopyFrom(c)).ToArray();
                return dtos.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                LogError(ex);
                var response = new APIResponse<ServiceCategoryDTO[]>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string InsertProtocol(string protocolDTO, string taskDTO)
        {
            try
            {
                ProtocolDTO protocol = protocolDTO.Deserialize<ProtocolDTO>();
                protocol.IdTenantAOO = ConfigurationHelper.CurrentTenantAOOId;
                protocol.Direction = 1; // Sempre in uscita.
                FileLogger.Debug(LogName.FileLog, string.Concat(JsonConvert.SerializeObject(protocol, DocSuiteContext.DefaultJsonSerializerSettings).Take(1000)));
                if (protocol.HasSenders())
                {
                    foreach (var item in protocol.Senders)
                        if (!RegexHelper.IsValidEmail(item.EmailAddress))
                            item.Code = item.EmailAddress;
                }

                if (protocol.HasRecipients())
                {
                    foreach (var item in protocol.Recipients)
                        if (!RegexHelper.IsValidEmail(item.EmailAddress))
                            item.Code = item.EmailAddress;
                }

                if (protocol.HasFascicles())
                {
                    foreach (var item in protocol.Fascicles)
                        if (!RegexHelper.IsValidEmail(item.EmailAddress))
                            item.Code = item.EmailAddress;
                }

                var result = ProtocolService.Insert(protocol);

                var task = taskDTO.Deserialize<TaskDTO>();
                task.AddProtocol(result);
                task = TaskService.Update(task);

                return result.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                LogError(ex);
                var response = new APIResponse<ProtocolDTO>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string SendMail(string mailDTO, string taskDTO)
        {
            try
            {
                var mail = mailDTO.Deserialize<MailDTO>();

                if (mail.Sender != null && !((MailboxDTO)mail.Mailbox).IsPOL())
                {
                    if (!RegexHelper.IsValidEmail(mail.Sender.EmailAddress))
                    {
                        mail.Sender.Code = mail.Sender.EmailAddress;
                        mail.Sender.EmailAddress = SearchEmailAddressFromCode(mail.Sender.Code);
                    }
                }

                if (mail.HasRecipients())
                {
                    foreach (var item in mail.Recipients)
                        if (!RegexHelper.IsValidEmail(item.EmailAddress))
                        {
                            item.Code = item.EmailAddress;
                            item.EmailAddress = SearchEmailAddressFromCode(item.Code);
                        }
                }

                if (mail.HasRecipientsCc())
                {
                    foreach (var item in mail.RecipientsCc)
                        if (!RegexHelper.IsValidEmail(item.EmailAddress))
                        {
                            item.Code = item.EmailAddress;
                            item.EmailAddress = SearchEmailAddressFromCode(item.Code);
                        }
                }

                if (mail.HasRecipientsBcc())
                {
                    foreach (var item in mail.RecipientsBcc)
                        if (!RegexHelper.IsValidEmail(item.EmailAddress))
                        {
                            item.Code = item.EmailAddress;
                            item.EmailAddress = SearchEmailAddressFromCode(item.Code);
                        }
                }

                var result = MailService.Send(mail);

                var task = taskDTO.Deserialize<TaskDTO>();
                task.AddMail(mail);
                task = TaskService.Update(task);

                return result.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                LogError(ex);
                var response = new APIResponse<MailDTO>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        internal static MailDTO PairToProtocol(MailDTO mailDTO, ProtocolDTO protocolDTO)
        {
            var mailbox = (MailboxDTO)mailDTO.Mailbox;
            if (mailbox.IsPEC())
            {
                FacadeFactory.Instance.PECMailFacade.PairToProtocol(mailDTO, protocolDTO);
                return mailDTO;
            }
            else if (mailbox.IsPOL())
            {
                //Non eseguo alcuna attività in quanto l'associazione viene già fatta nel metodo di invio POL
                return mailDTO;
            }
            else if (mailbox.IsMessage())
            {
                MessageEmail savedMessage = FacadeFactory.Instance.MessageEmailFacade.GetById(int.Parse(mailDTO.Id));
                DSWMessage currentMessage = savedMessage.Message;
                Protocol protocol = FacadeFactory.Instance.ProtocolFacade.GetById(protocolDTO.UniqueId.Value);
                ProtocolMessage newProtocolMessage = new ProtocolMessage(ref protocol, ref currentMessage);
                FacadeFactory.Instance.ProtocolMessageFacade.Save(ref newProtocolMessage);
                return mailDTO;
            }

            var message = string.Format("Mailbox.TypeName \"{0}\" non gestito.", mailDTO.Mailbox.TypeName);
            throw new NotImplementedException(message);
        }

        public string SendProtocolMail(string mailDTO, string protocolDTO, string taskDTO)
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

                if (mail.Sender != null && !((MailboxDTO)mail.Mailbox).IsPOL())
                {
                    if (!RegexHelper.IsValidEmail(mail.Sender.EmailAddress))
                    {
                        mail.Sender.Code = mail.Sender.EmailAddress;
                        mail.Sender.EmailAddress = SearchEmailAddressFromCode(mail.Sender.Code);
                    }

                }

                if (mail.HasRecipients())
                {
                    foreach (var item in mail.Recipients)
                        if (!RegexHelper.IsValidEmail(item.EmailAddress))
                        {
                            item.Code = item.EmailAddress;
                            item.EmailAddress = SearchEmailAddressFromCode(item.Code);
                        }
                }

                if (mail.HasRecipientsCc())
                {
                    foreach (var item in mail.RecipientsCc)
                        if (!RegexHelper.IsValidEmail(item.EmailAddress))
                        {
                            item.Code = item.EmailAddress;
                            item.EmailAddress = SearchEmailAddressFromCode(item.Code);
                        }
                }

                if (mail.HasRecipientsBcc())
                {
                    foreach (var item in mail.RecipientsBcc)
                        if (!RegexHelper.IsValidEmail(item.EmailAddress))
                        {
                            item.Code = item.EmailAddress;
                            item.EmailAddress = SearchEmailAddressFromCode(item.Code);
                        }
                }

                mail = MailService.Send(mail, protocol);

                mail = PairToProtocol(mail, protocol);

                var task = taskDTO.Deserialize<TaskDTO>();
                task.AddMail(mail);
                task = TaskService.Update(task);

                return mail.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                LogError(ex);
                var response = new APIResponse<MailDTO>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string CreateTask(string taskDTO)
        {
            try
            {
                var dto = taskDTO.Deserialize<TaskDTO>();
                dto.TaskType = (int)TaskTypeEnum.FastProtocolSender;
                var result = TaskService.CreateTask(dto);
                return result.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                LogError(ex);
                var response = new APIResponse<TaskDTO>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string UpdateStatus(string taskDTO)
        {
            try
            {
                var dto = taskDTO.Deserialize<TaskDTO>();
                var result = TaskService.UpdateStatus(dto);
                return result.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                LogError(ex);
                var response = new APIResponse<TaskDTO>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public string GetRecentFastMergeCodes()
        {
            try
            {
                var result = FacadeFactory.Instance.TaskHeaderFacade.GetRecentFastMergeCodes(10);
                var response = new APIResponse<string[]>(result.ToArray());
                return response.Serialize();
            }
            catch (Exception ex)
            {
                LogError(ex);
                var response = new APIResponse<string[]>(ex);
                return response.Serialize();
            }
        }

        private CategoryDTO GetCategoryDTO(Category category)
        {
            var result = new CategoryDTO().CopyFrom(category);
            if (!string.IsNullOrWhiteSpace(category.FullCode))
                result.Name = string.Format("{0} {1}", category.FullCode, category.Name);
            return result;
        }

        public string GetProtocolDocument(string protocolDTO)
        {
            try
            {
                ProtocolDTO protocol = protocolDTO.Deserialize<ProtocolDTO>();
                if (!protocol.HasId())
                    throw new ArgumentNullException("Nessun Id definito per il protocollo");

                Protocol domain = FacadeFactory.Instance.ProtocolFacade.GetById(protocol.UniqueId.Value);
                ProtocolDocumentDTO dto = FacadeFactory.Instance.ProtocolFacade.GetProtocolMainDocumentPdfConverted(domain);
                APIResponse<string> response = new APIResponse<string>(dto.Serialized);
                return response.Serialize();
            }
            catch (Exception ex)
            {
                LogError(ex);
                var response = new APIResponse<TaskDTO>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }


        private string SearchEmailAddressFromCode(string code)
        {
            IList<Contact> contacts = FacadeFactory.Instance.ContactFacade.GetContactBySearchCode(code, true);
            if(contacts.Count != 1)
            {
                throw new Exception("Il contatto scelto non esiste o è ambiguo");
            }
            return contacts.Single().EmailAddress;
        }
        #endregion
    }
}
