using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Dossiers
{
    public class DossierTableValuedModelMapper : BaseModelMapper<DossierTableValuedModel, DossierModel>, IDossierTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public DossierTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override DossierModel Map(DossierTableValuedModel model, DossierModel modelTransformed)
        {
            modelTransformed.UniqueId = model.IdDossier;
            modelTransformed.Year = model.Year;
            modelTransformed.Number = model.Number;
            modelTransformed.Title = string.Format("{0}/{1:0000000}", model.Year, model.Number);
            modelTransformed.Subject = model.Subject;
            modelTransformed.RegistrationDate = model.RegistrationDate;
            modelTransformed.StartDate = model.StartDate;
            modelTransformed.EndDate = model.EndDate;
            modelTransformed.ContainerId = model.Container_Id;
            modelTransformed.ContainerName = model.Container_Name;

            return modelTransformed;
        }

        public override ICollection<DossierModel> MapCollection(ICollection<DossierTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<DossierModel>();
            }

            List<DossierModel> modelsTransformed = new List<DossierModel>();
            DossierModel modelTransformed = null;
            ICollection<DossierTableValuedModel> dossierRoles;
            ICollection<DossierTableValuedModel> dossierContacts;
            foreach (IGrouping<Guid, DossierTableValuedModel> dossierLookup in model.ToLookup(x => x.IdDossier))
            {
                modelTransformed = Map(dossierLookup.First(), new DossierModel());
                dossierRoles = dossierLookup.ToLookup(x => x.Role_IdRole)
                    .Select(f => f.First())
                    .ToList();
                dossierContacts = dossierLookup.ToLookup(x => x.Contact_Incremental)
                    .Select(f => f.First())
                    .ToList();

                modelTransformed.Roles = _mapperUnitOfWork.Repository<IDomainMapper<DossierTableValuedModel, DossierRoleModel>>().MapCollection(dossierRoles);
                modelTransformed.Contacts = _mapperUnitOfWork.Repository<IDomainMapper<IContactTableValuedModel, ContactModel>>().MapCollection(dossierContacts);

                modelsTransformed.Add(modelTransformed);
            }
            return modelsTransformed;
        }
    }
}
