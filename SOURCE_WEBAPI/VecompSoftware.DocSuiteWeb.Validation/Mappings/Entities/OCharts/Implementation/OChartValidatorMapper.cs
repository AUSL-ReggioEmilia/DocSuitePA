using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.OCharts;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.OCharts
{
    public class OChartValidatorMapper : BaseMapper<OChart, OChartValidator>, IOChartValidatorMapper
    {
        public OChartValidatorMapper() { }

        public override OChartValidator Map(OChart entity, OChartValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Enabled = entity.Enabled;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.Imported = entity.Imported;
            entityTransformed.Description = entity.Description;
            entityTransformed.Title = entity.Title;
            #endregion

            #region [ Base ]
            entityTransformed.OChartItems = entity.OChartItems;
            #endregion


            return entityTransformed;
        }

    }
}
