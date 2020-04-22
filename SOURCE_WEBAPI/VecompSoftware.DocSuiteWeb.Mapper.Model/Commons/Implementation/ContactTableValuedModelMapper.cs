using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class ContactTableValuedModelMapper : BaseModelMapper<IContactTableValuedModel, ContactModel>, IContactTableValuedModelMapper
    {
        public override ContactModel Map(IContactTableValuedModel model, ContactModel modelTransformed)
        {
            modelTransformed = null;
            if (model.Contact_Incremental.HasValue)
            {
                modelTransformed = new ContactModel
                {
                    Id = model.Contact_Incremental,
                    Description = model.Contact_Description
                };
            }

            return modelTransformed;
        }

    }
}
