using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Dossiers
{
    public class DossierModelMapper : BaseModelMapper<Dossier, DossierModel>, IDossierModelMapper
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public DossierModelMapper()
        {

        }
        #endregion

        #region [ Methods ]

        public override DossierModel Map(Dossier entity, DossierModel modelTransformed)
        {
            modelTransformed.UniqueId = entity.UniqueId;
            modelTransformed.Year = entity.Year;
            modelTransformed.Number = entity.Number;
            modelTransformed.Title = string.Format("{0}/{1:0000000}", entity.Year, entity.Number);
            modelTransformed.Subject = entity.Subject;
            modelTransformed.Note = entity.Note;
            modelTransformed.RegistrationDate = entity.RegistrationDate;
            modelTransformed.RegistrationUser = entity.RegistrationUser;
            modelTransformed.LastChangedDate = entity.LastChangedDate;
            modelTransformed.LastChangedUser = entity.LastChangedUser;
            modelTransformed.StartDate = entity.StartDate;
            modelTransformed.EndDate = entity.EndDate;
            modelTransformed.JsonMetadata = entity.JsonMetadata;
            modelTransformed.ContainerName = entity.Container == null ? null : entity.Container.Name;
            modelTransformed.ContainerId = entity.Container == null ? short.Parse("0") : entity.Container.EntityShortId;


            return modelTransformed;
        }
        #endregion
    }
}
