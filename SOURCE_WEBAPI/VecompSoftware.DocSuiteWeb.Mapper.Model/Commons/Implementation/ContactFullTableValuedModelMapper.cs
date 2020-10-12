using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class ContactFullTableValuedModelMapper : BaseModelMapper<ContactTableValuedModel, ContactModel>, IContactFullTableValuedModelMapper
    {
        private ContactType MapContactType(ContactTableValuedModel model)
        {
            switch(model.ContactType)
            {
                case "M":
                    {
                        return ContactType.Administration;
                    }
                case "A":
                    {
                        return ContactType.AOO;
                    }
                case "U":
                    {
                        return ContactType.AO;
                    }
                case "R":
                    {
                        return ContactType.Role;
                    }
                case "P":
                    {
                        return ContactType.Citizen;
                    }
                case "G":
                    {
                        return ContactType.Group;
                    }
                case "I":
                    {
                        return ContactType.IPA;
                    }
                case "S":
                    {
                        return ContactType.Sector;
                    }
                default:
                    {
                        return ContactType.Invalid;
                    }
            }
        }            
                     
        public override ContactModel Map(ContactTableValuedModel model, ContactModel modelTransformed)
        {
            modelTransformed.Id = model.IdContact;
            modelTransformed.EntityId = model.IdContact;
            modelTransformed.ContactType = MapContactType(model);
            modelTransformed.Description = model.Description;
            modelTransformed.Code = model.Code;
            modelTransformed.Email = model.Email;
            modelTransformed.CertifiedMail = model.CertifiedMail;
            modelTransformed.Note = model.Note;
            modelTransformed.FiscalCode = model.FiscalCode;
            modelTransformed.IncrementalFather = model.IncrementalFather;
            return modelTransformed;
        }

    }
}
