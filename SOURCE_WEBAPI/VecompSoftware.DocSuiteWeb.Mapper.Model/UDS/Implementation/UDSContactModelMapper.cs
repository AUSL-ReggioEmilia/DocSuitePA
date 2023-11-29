using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.UDS
{
    public class UDSContactModelMapper : BaseModelMapper<DocumentUnitContact, UDSContactModel>, IUDSContactModelMapper
    {
        #region [ Methods ]
        public override UDSContactModel Map(DocumentUnitContact model, UDSContactModel modelTransformed)
        {
            modelTransformed.UniqueId = model.UniqueId;
            modelTransformed.RelationType = UDSRelationType.Contact;
            modelTransformed.IdContact = model.Contact?.EntityId ?? 0;
            modelTransformed.ContactLabel = model.ContactLabel;
            modelTransformed.ContactManual = model.ContactManual;
            modelTransformed.RegistrationDate = modelTransformed.RegistrationDate;
            modelTransformed.RegistrationUser = modelTransformed.RegistrationUser;

            if (model.DocumentUnit != null)
            {
                modelTransformed.Environment = model.DocumentUnit.Environment;
                modelTransformed.IdUDS = model.DocumentUnit.UniqueId;
            }

            return modelTransformed;
        }

        public override ICollection<UDSContactModel> MapCollection(ICollection<DocumentUnitContact> entities)
        {
            if (entities == null)
            {
                return new List<UDSContactModel>();
            }

            List<UDSContactModel> entitiesTransformed = new List<UDSContactModel>();
            UDSContactModel entityTransformed = null;
            foreach (IGrouping<Guid, DocumentUnitContact> udLookup in entities.ToLookup(x => x.UniqueId))
            {
                entityTransformed = Map(udLookup.First(), new UDSContactModel());
                entitiesTransformed.Add(entityTransformed);
            }

            return entitiesTransformed;
        }
        #endregion
    }
}
