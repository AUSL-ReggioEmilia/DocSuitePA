using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class TaskDTO : ITaskDTO
    {
        #region [ Properties ]

        public int? Id { get; set; }

        public string Code { get; set; }

        public string Title { get; set; }
        
        public string Description { get; set; }

        public int? TaskType { get; set; }
        
        public int? Status { get; set; }
        
        public DateTime? TaskDate { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<ProtocolDTO, IProtocolDTO>))]
        public IProtocolDTO[] Protocols { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<MailDTO, IMailDTO>))]
        public IMailDTO[] PECMails { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<MailDTO, IMailDTO>))]
        public IMailDTO[] POLMails { get; set; }

        #endregion

        #region [ Methods ]

        public bool HasProtocols()
        {
            return this.Protocols != null && this.Protocols.Length > 0;
        }

        public bool HasPECMails()
        {
            return this.PECMails != null && this.PECMails.Length > 0;
        }

        public bool HasPOLMails()
        {
            return this.POLMails != null && this.POLMails.Length > 0;
        }

        public TaskDTO AddProtocol(IProtocolDTO dto)
        {
            var list = this.Protocols != null ? this.Protocols.ToList() : new List<IProtocolDTO>();
            list.Add(dto);
            this.Protocols = list.ToArray();
            return this;
        }

        public TaskDTO AddPECMail(IMailDTO dto)
        {
            var list = this.Protocols != null ? this.PECMails.ToList() : new List<IMailDTO>();
            list.Add(dto);
            this.PECMails = list.ToArray();
            return this;
        }

        public TaskDTO AddPOLMail(IMailDTO dto)
        {
            var list = this.POLMails != null ? this.POLMails.ToList() : new List<IMailDTO>();
            list.Add(dto);
            this.POLMails = list.ToArray();
            return this;
        }

        #endregion
    }
}
