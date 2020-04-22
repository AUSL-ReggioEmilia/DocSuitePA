using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.OCharts;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.OCharts
{
    public class OChartItemValidatorMapper : BaseMapper<OChartItem, OChartItemValidator>, IOChartItemValidatorMapper
    {
        public OChartItemValidatorMapper() { }

        public override OChartItemValidator Map(OChartItem entity, OChartItemValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Enabled = entity.Enabled;
            entityTransformed.Acronym = entity.Acronym;
            entityTransformed.Code = entity.Code;
            entityTransformed.FullCode = entity.FullCode;
            entityTransformed.Imported = entity.Imported;
            entityTransformed.Email = entity.Email;
            entityTransformed.Description = entity.Description;
            entityTransformed.Title = entity.Title;
            #endregion

            #region [ Base ]
            entityTransformed.OChart = entity.OChart;
            entityTransformed.Parent = entity.Parent;
            entityTransformed.Children = entity.Children;
            entityTransformed.Contacts = entity.Contacts;
            entityTransformed.OChartItemContainers = entity.OChartItemContainers;
            entityTransformed.Mailboxes = entity.Mailboxes;
            entityTransformed.Roles = entity.Roles;
            #endregion


            return entityTransformed;
        }

    }
}
