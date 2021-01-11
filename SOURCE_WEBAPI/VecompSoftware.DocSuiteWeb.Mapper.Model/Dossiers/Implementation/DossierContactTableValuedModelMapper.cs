using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Dossiers
{
    public class DossierContactTableValuedModelMapper : BaseModelMapper<DossierTableValuedModel, ContactModel>, IDossierContactTableValuedModelMapper
    {
        public override ContactModel Map(DossierTableValuedModel model, ContactModel modelTransformed)
        {
            modelTransformed.Id = model.Contact_Incremental;
            modelTransformed.EntityId = model.Contact_Incremental;
            modelTransformed.Description = model.Container_Name;

            return modelTransformed;
        }
    }
}
