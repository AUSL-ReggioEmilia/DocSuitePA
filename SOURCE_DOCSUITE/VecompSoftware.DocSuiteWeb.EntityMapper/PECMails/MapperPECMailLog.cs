using System;
using NHibernate;
using NHibernate.Transform;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.PECMails
{
    public class MapperPECMailLog : BaseEntityMapper<PECMailLog, PECMailLogHeader>
    {
         #region [ Constructor ]

        public MapperPECMailLog()
            : base()
        {
        }

        #endregion

        #region [ Methods ]

        protected override PECMailLogHeader TransformDTO(PECMailLog entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare PECMailBoxUser se l'entità non è inizializzata");

            PECMailLogHeader dto = new PECMailLogHeader();
            dto.Id = entity.Id;
            dto.Description = entity.Description;
            dto.Type = entity.Type;
            dto.Date = entity.Date;
            dto.SystemComputer = entity.SystemComputer;
            dto.SystemUser = entity.SystemUser;
            dto.MailId = entity.Mail.Id;
            dto.MailSubject = entity.Mail.MailSubject;

            return dto;
        }

        protected override IQueryOver<PECMailLog, PECMailLog> MappingProjection(IQueryOver<PECMailLog, PECMailLog> queryOver)
        {
            PECMail pec = null;
            PECMailLogHeader header = null;

            queryOver
                .JoinAlias(o => o.Mail, () => pec)
                .SelectList(list => list
                                    .Select(x => x.Id).WithAlias(() => header.Id)
                                    .Select(x => x.Description).WithAlias(() => header.Description)
                                    .Select(x => x.Type).WithAlias(() => header.Type)
                                    .Select(x => x.Date).WithAlias(() => header.Date)
                                    .Select(x => x.SystemComputer).WithAlias(() => header.SystemComputer)
                                    .Select(x => x.SystemUser).WithAlias(() => header.SystemUser)
                                    .Select(x => pec.Id).WithAlias(() => header.MailId)
                                    .Select(x => pec.MailSubject).WithAlias(() => header.MailSubject)
                                    )
                .TransformUsing(Transformers.AliasToBean<PECMailLogHeader>());
            return queryOver;
        }

        #endregion
    }
}
