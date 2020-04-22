using DSW = VecompSoftware.DocSuiteWeb.Data;
using APICommons = VecompSoftware.DocSuiteWeb.Entity.Commons;
using NHibernate;
using System;
using VecompSoftware.DocSuiteWeb.DTO.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperContactDTODSWEntity : BaseEntityMapper<ReferenceModel, DSW.ContactDTO>
    {
        protected override IQueryOver<ReferenceModel, ReferenceModel> MappingProjection(IQueryOver<ReferenceModel, ReferenceModel> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override DSW.ContactDTO TransformDTO(ReferenceModel entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare Contact se l'entità non è inizializzata");
            }

            DSW.ContactDTO dswContactDTO = new DSW.ContactDTO();
            DSW.Contact dswContact = new DSW.Contact();

            dswContact.Id = entity.EntityShortId;
            dswContact.Description = entity.Name;
            dswContact.UniqueId = Guid.Parse(entity.UniqueId);
            dswContact.ContactType = new DSW.ContactType();
            dswContact.ContactType.Id = char.Parse(entity.Type);
            dswContactDTO.Contact = dswContact;
            dswContactDTO.Id = dswContact.Id;
            dswContactDTO.Type = DSW.ContactDTO.ContactType.Address;


            return dswContactDTO;
        }
    }
}
