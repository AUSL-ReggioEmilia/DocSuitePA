using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.DTO.PECMails;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.PECMails
{
    public class MapperPECMailBox : BaseEntityMapper<PECMailBox, PECMailBoxDto>
    {
        #region [ Constructor ]

        public MapperPECMailBox() : base()
        {
        }

        #endregion

        #region [ Methods ]

        protected override PECMailBoxDto TransformDTO(PECMailBox entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare PECMailBox se l'entità non è inizializzata");

            PECMailBoxDto dto = new PECMailBoxDto();
            dto.Id = entity.Id;
            dto.Name = entity.MailBoxName;

            return dto;
        }

        protected override IQueryOver<PECMailBox, PECMailBox> MappingProjection(
            IQueryOver<PECMailBox, PECMailBox> queryOver)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
