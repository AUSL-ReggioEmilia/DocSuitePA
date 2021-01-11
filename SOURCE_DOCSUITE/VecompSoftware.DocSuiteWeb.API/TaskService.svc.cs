using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.NHibernateManager;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class TaskService : ITaskService
    {
        public bool IsAlive()
        {
            return true;
        }

        internal static TaskDTO CreateTask(TaskDTO dto)
        {
            var header = FacadeFactory.Instance.TaskHeaderFacade.CreateHeader(dto);
            var result = new TaskDTO().CopyFrom(header);
            return result;
        }

        public string CreateTask(string taskDTO)
        {
            try
            {
                var dto = taskDTO.Deserialize<TaskDTO>();
                var result = CreateTask(dto);
                return result.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                var response = new APIResponse<TaskDTO>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        internal static TaskDTO UpdateStatus(TaskDTO dto)
        {
            var header = FacadeFactory.Instance.TaskHeaderFacade.GetById(dto.Id.Value);
            header.Status = (TaskStatusEnum)dto.Status;
            if (header.POLRequests == null && header.PECMails == null)
            {
                header.SendingProcessStatus = TaskHeaderSendingProcessStatus.Complete;
                header.SendedStatus = TaskHeaderSendedStatus.Successfully;
            }
            FacadeFactory.Instance.TaskHeaderFacade.Update(ref header);
            var result = new TaskDTO().CopyFrom(header);
            return result;
        }

        public string UpdateStatus(string taskDTO)
        {
            try
            {
                var dto = taskDTO.Deserialize<TaskDTO>();
                var result = UpdateStatus(dto);
                return result.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                var response = new APIResponse<TaskDTO>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        internal static TaskDTO Update(TaskDTO dto)
        {
            var header = FacadeFactory.Instance.TaskHeaderFacade.GetById(dto.Id.Value);

            if (dto.HasProtocols())
                dto.Protocols.ToList().ForEach(f => header.AddProtocol(FacadeFactory.Instance.ProtocolFacade.GetById(f.UniqueId.Value)));
            if (dto.HasPECMails())
                dto.PECMails.ToList().ForEach(f => header.AddPECMail(FacadeFactory.Instance.PECMailFacade.GetById(int.Parse(f.Id))));
            if (dto.HasPOLMails())
                dto.POLMails.ToList().ForEach(f => header.AddPOLRequest(FacadeFactory.Instance.PosteOnLineRequestFacade.GetById(Guid.Parse(f.Id))));

            FacadeFactory.Instance.TaskHeaderFacade.Update(ref header);

            var result = dto.CopyFrom(header);
            return result;
        }

        internal static void AddProtocol(TaskDTO taskDTO, ProtocolDTO protocolDTO)
        {
            var header = FacadeFactory.Instance.TaskHeaderFacade.GetById(taskDTO.Id.Value);
            var protocol = new Protocol();
            protocol.Id = protocolDTO.UniqueId.Value;

            header.AddProtocol(protocol);
            FacadeFactory.Instance.TaskHeaderFacade.Update(ref header);
        }

        internal static void AddPECMail(TaskDTO taskDTO, MailDTO mailDTO)
        {
            var header = FacadeFactory.Instance.TaskHeaderFacade.GetById(taskDTO.Id.Value);
            var pecMail = new PECMail();
            pecMail.Id = int.Parse(mailDTO.Id);

            header.AddPECMail(pecMail);
            FacadeFactory.Instance.TaskHeaderFacade.Update(ref header);
        }

        internal static void AddPOLRequest(TaskDTO taskDTO, MailDTO mailDTO)
        {
            var header = FacadeFactory.Instance.TaskHeaderFacade.GetById(taskDTO.Id.Value);
            var polRequest = new POLRequest();
            polRequest.Id = new Guid(mailDTO.Id);

            header.AddPOLRequest(polRequest);
            FacadeFactory.Instance.TaskHeaderFacade.Update(ref header);
        }

    }
}
