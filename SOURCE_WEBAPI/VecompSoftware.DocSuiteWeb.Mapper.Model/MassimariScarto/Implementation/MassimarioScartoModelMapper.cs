using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Model.Entities.MassimariScarto;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.MassimariScarto
{
    public class MassimarioScartoModelMapper : BaseModelMapper<MassimarioScarto, MassimarioScartoModel>, IMassimarioScartoModelMapper
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public MassimarioScartoModelMapper()
        {

        }
        #endregion

        #region [ Methods ]
        public static DocSuiteWeb.Model.Entities.MassimariScarto.MassimarioScartoStatus StatusConverter(Entity.MassimariScarto.MassimarioScartoStatus status)
        {
            switch (status)
            {
                case Entity.MassimariScarto.MassimarioScartoStatus.Active:
                    return DocSuiteWeb.Model.Entities.MassimariScarto.MassimarioScartoStatus.Active;
                case Entity.MassimariScarto.MassimarioScartoStatus.LogicalDelete:
                    return DocSuiteWeb.Model.Entities.MassimariScarto.MassimarioScartoStatus.LogicalDelete;
                default:
                    return DocSuiteWeb.Model.Entities.MassimariScarto.MassimarioScartoStatus.Active;
            }
        }

        public override MassimarioScartoModel Map(MassimarioScarto entity, MassimarioScartoModel modelTransformed)
        {
            modelTransformed.UniqueId = entity.UniqueId;
            modelTransformed.Code = entity.Code;
            modelTransformed.FullCode = entity.FullCode;
            modelTransformed.ConservationPeriod = entity.ConservationPeriod;
            modelTransformed.EndDate = entity.EndDate;
            modelTransformed.LastChangedDate = entity.LastChangedDate;
            modelTransformed.LastChangedUser = entity.LastChangedUser;
            modelTransformed.MassimarioScartoLevel = entity.MassimarioScartoLevel;
            modelTransformed.MassimarioScartoPath = entity.MassimarioScartoPath;
            modelTransformed.MassimarioScartoParentPath = entity.MassimarioScartoParentPath;
            modelTransformed.Name = entity.Name;
            modelTransformed.Note = entity.Note;
            modelTransformed.FakeInsertId = entity.FakeInsertId;
            modelTransformed.RegistrationDate = entity.RegistrationDate;
            modelTransformed.RegistrationUser = entity.RegistrationUser;
            modelTransformed.StartDate = entity.StartDate;
            modelTransformed.Status = StatusConverter(entity.Status);
            return modelTransformed;
        }
        #endregion
    }
}
