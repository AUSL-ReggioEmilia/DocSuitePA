using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Fascicles
{
    public class FascicleDocumentUnitTableValuedModelMapper : BaseModelMapper<FascicleDocumentUnitTableValuedModel, FascicleDocumentUnitModel>, IFascicleDocumentUnitTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        public FascicleDocumentUnitTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override FascicleDocumentUnitModel Map(FascicleDocumentUnitTableValuedModel model, FascicleDocumentUnitModel modelTransformed)
        {
            modelTransformed.UniqueId = model.UniqueId;
            modelTransformed.ReferenceType = model.ReferenceType;
            modelTransformed.SequenceNumber = model.SequenceNumber;

            DocumentUnitTableValuedModel documentUnitTableValuedModel = new DocumentUnitTableValuedModel
            {
                UniqueId = model.DocumentUnit_IdDocumentUnit.Value,
                Environment = model.DocumentUnit_Environment,
                DocumentUnitName = model.DocumentUnit_DocumentUnitName,
                Year = model.DocumentUnit_Year,
                Number = model.DocumentUnit_Number,
                Title = model.DocumentUnit_Title,
                RegistrationDate = model.DocumentUnit_RegistrationDate,
                RegistrationUser = model.DocumentUnit_RegistrationUser,
                Subject = model.DocumentUnit_Subject,
                DocumentUnitChain_DocumentName = null,
                IdUDSRepository = model.DocumentUnit_IdUDSRepository,
                IdFascicle = model.DocumentUnit_IdFascicle,
                EntityId = model.DocumentUnit_EntityId,
                Category_IdCategory = model.Category_IdCategory,
                Category_Name = model.Category_Name,
                Container_IdContainer = model.Container_IdContainer,
                Container_Name = model.Container_Name,
                TenantAOO_IdTenantAOO = model.TenantAOO_IdTenantAOO,
                TenantAOO_Name = model.TenantAOO_Name
            };
            modelTransformed.DocumentUnit = _mapperUnitOfWork.Repository<IDomainMapper<DocumentUnitTableValuedModel, DocumentUnitModel>>().Map(documentUnitTableValuedModel, new DocumentUnitModel());

            return modelTransformed;
        }

        public override ICollection<FascicleDocumentUnitModel> MapCollection(ICollection<FascicleDocumentUnitTableValuedModel> entities)
        {
            if (entities == null)
            {
                return new List<FascicleDocumentUnitModel>();
            }

            List<FascicleDocumentUnitModel> entitiesTransformed = new List<FascicleDocumentUnitModel>();
            FascicleDocumentUnitModel entityTransformed = null;
            foreach (IGrouping<Guid, FascicleDocumentUnitTableValuedModel> udLookup in entities.ToLookup(x => x.UniqueId))
            {
                entityTransformed = Map(udLookup.First(), new FascicleDocumentUnitModel());
                entitiesTransformed.Add(entityTransformed);
            }

            return entitiesTransformed;
        }

    }
}
