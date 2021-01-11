using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.DTO.PECMails;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.PECMails
{
    public class MapperPECMailBoxUser: BaseEntityMapper<PECMailBoxUser, PECMailBoxUserDto>
    {
        #region [ Constructor ]

        public MapperPECMailBoxUser()
            : base()
        {
        }

        #endregion

        #region [ Methods ]

        protected override PECMailBoxUserDto TransformDTO(PECMailBoxUser entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare PECMailBoxUser se l'entità non è inizializzata");

            PECMailBoxUserDto dto = new PECMailBoxUserDto();
            dto.Id = entity.Id;
            dto.Account = entity.AccountName;
            dto.SecurityAccount = entity.SecurityUser.Account;
            dto.SecurityId = entity.SecurityUser.Id;
            dto.MailBoxId = entity.PECMailBox.Id;
            dto.MailBoxName = entity.PECMailBox.MailBoxName;

            return dto;
        }

        protected override IQueryOver<PECMailBoxUser, PECMailBoxUser> MappingProjection(
            IQueryOver<PECMailBoxUser, PECMailBoxUser> queryOver)
        {
            PECMailBoxUserDto mailBoxDto = null;
            SecurityUsers securityUsers = null;
            PECMailBox pecMailBox = null;

            queryOver
                .SelectList(list => list
                    // Mappatura degli oggetti Desk
                    .Select(x => x.Id).WithAlias(() => mailBoxDto.Id)
                    .Select(x => x.AccountName).WithAlias(() => mailBoxDto.Account)
                    .Select(() => securityUsers.Account).WithAlias(() => mailBoxDto.SecurityAccount)
                    .Select(() => securityUsers.Id).WithAlias(() => mailBoxDto.SecurityId)
                    .Select(() => pecMailBox.Id).WithAlias(() => mailBoxDto.MailBoxId)
                    .Select(() => pecMailBox.MailBoxName).WithAlias(() => mailBoxDto.MailBoxName));

            return queryOver;
        }

        #endregion
    }
}
