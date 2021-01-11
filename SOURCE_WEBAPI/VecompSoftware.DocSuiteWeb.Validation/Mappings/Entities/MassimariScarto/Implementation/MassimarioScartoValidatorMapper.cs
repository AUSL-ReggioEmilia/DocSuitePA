using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.MassimariScarto;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.MassimariScarto
{
    public class MassimarioScartoValidatorMapper : BaseMapper<MassimarioScarto, MassimarioScartoValidator>, IMassimarioScartoValidatorMapper
    {
        public MassimarioScartoValidatorMapper() { }

        public override MassimarioScartoValidator Map(MassimarioScarto entity, MassimarioScartoValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Code = entity.Code;
            entityTransformed.FullCode = entity.FullCode;
            entityTransformed.ConservationPeriod = entity.ConservationPeriod;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.MassimarioScartoLevel = entity.MassimarioScartoLevel;
            entityTransformed.MassimarioScartoPath = entity.MassimarioScartoPath;
            entityTransformed.MassimarioScartoParentPath = entity.MassimarioScartoParentPath;
            entityTransformed.Name = entity.Name;
            entityTransformed.Note = entity.Note;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.Status = entity.Status;
            entityTransformed.FakeInsertId = entity.FakeInsertId;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }

    }
}
