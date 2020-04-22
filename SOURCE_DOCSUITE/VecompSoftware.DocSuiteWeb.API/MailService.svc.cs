using System;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.NHibernateManager;


namespace VecompSoftware.DocSuiteWeb.API
{
    public class MailService : IMailService
    {
        public bool IsAlive()
        {
            return true;
        }

        public string Send(string mailDTO)
        {
            try
            {
                var dto = mailDTO.Deserialize<MailDTO>();
                var result = Send(dto);
                return result.SerializeAsResponse();
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

        internal static MailDTO Send(MailDTO dto, bool sendPdf = true)
        {
            return Send(dto, null, sendPdf);
        }

        internal static MailDTO Send(MailDTO dto, ProtocolDTO protocolDTO, bool sendPdf = true)
        {
            MailboxDTO mailbox = (MailboxDTO)dto.Mailbox;
            if (mailbox.IsPEC())
            {
                dto.Id = FacadeFactory.Instance.PECMailFacade.SendMail(dto, sendPdf).ToString();
                return dto;
            }
            else
            {
                if (mailbox.IsPOL())
                {
                    if (protocolDTO == null)
                    {
                        throw new ArgumentNullException("protDTO", "L'invio tramite Poste WEB è possibile solamente se è presente un protocollo da associare");
                    }

                    dto.Id = FacadeFactory.Instance.PosteOnLineRequestFacade.SendLettera(dto, protocolDTO).ToString();
                    return dto;
                }
                else
                {
                    if (mailbox.IsMessage())
                    {
                        dto.Id = FacadeFactory.Instance.MessageEmailFacade.CreateEmailMessage(dto, sendPdf).Id.ToString();
                        return dto;
                    }
                }
            }

            var message = string.Format("Mailbox.TypeName \"{0}\" non gestito.", dto.Mailbox.TypeName);
            throw new NotImplementedException(message);
        }

    }
}
