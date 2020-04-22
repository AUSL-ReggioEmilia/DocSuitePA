using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Resolutions
{
    public class ResolutionModelMapper : BaseModelMapper<Resolution, ResolutionModel>, IResolutionModelMapper
    {
        #region [ Fields ]
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion

        #region [ Properties ]
        public FileResolution FileResolution { get; set; }

        public IEnumerable<ResolutionRole> ResolutionRoles { get; set; }
        #endregion

        #region [ Constructor ]
        public ResolutionModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        #endregion

        #region [ Methods ]
        public override ResolutionModel Map(Resolution entity, ResolutionModel modelTransformed)
        {
            modelTransformed.IdResolution = entity.EntityId;
            modelTransformed.Number = entity.Number;
            modelTransformed.Year = entity.Year;
            modelTransformed.Subject = entity.Object;
            modelTransformed.ServiceNumber = entity.ServiceNumber;
            modelTransformed.AdoptionDate = entity.AdoptionDate;
            modelTransformed.AlternativeAssignee = entity.AlternativeAssignee;
            modelTransformed.AlternativeManager = entity.AlternativeManager;
            modelTransformed.AlternativeProposer = entity.AlternativeProposer;
            modelTransformed.AlternativeRecipient = entity.AlternativeRecipient;
            modelTransformed.ConfirmDate = entity.ConfirmDate;
            modelTransformed.EffectivenessDate = entity.EffectivenessDate;
            modelTransformed.LeaveDate = entity.LeaveDate;
            modelTransformed.Number = entity.Number;
            modelTransformed.ProposeDate = entity.ProposeDate;
            modelTransformed.PublishingDate = entity.PublishingDate;
            modelTransformed.ResponseDate = entity.ResponseDate;
            modelTransformed.WaitDate = entity.WaitDate;
            modelTransformed.WarningDate = entity.WarningDate;
            modelTransformed.WorkflowType = entity.WorkflowType;
            modelTransformed.ProposeUser = entity.ProposeUser;
            modelTransformed.LeaveUser = entity.LeaveUser;
            modelTransformed.EffectivenessUser = entity.EffectivenessUser;
            modelTransformed.ResponseUser = entity.ResponseUser;
            modelTransformed.WaitUser = entity.WaitUser;
            modelTransformed.ConfirmUser = entity.ConfirmUser;
            modelTransformed.WarningUser = entity.WarningUser;
            modelTransformed.PublishingUser = entity.PublishingUser;
            modelTransformed.AdoptionUser = entity.AdoptionUser;
            modelTransformed.UniqueId = entity.UniqueId;
            modelTransformed.RegistrationUser = entity.AdoptionUser;
            modelTransformed.LastChangedUser = entity.LastChangedUser;
            modelTransformed.Category = _mapperUnitOfWork.Repository<IDomainMapper<Category, CategoryModel>>().Map(entity.Category, new CategoryModel());
            modelTransformed.Container = _mapperUnitOfWork.Repository<IDomainMapper<Container, ContainerModel>>().Map(entity.Container, new ContainerModel());

            if (FileResolution != null)
            {
                modelTransformed.FileResolution = _mapperUnitOfWork.Repository<IDomainMapper<FileResolution, FileResolutionModel>>().Map(FileResolution, new FileResolutionModel());
            }
            if (ResolutionRoles != null && ResolutionRoles.Any())
            {
                modelTransformed.ResolutionRoles = _mapperUnitOfWork.Repository<IDomainMapper<ResolutionRole, ResolutionRoleModel>>().MapCollection(ResolutionRoles);
            }
            return modelTransformed;
        }

        #endregion
    }
}
