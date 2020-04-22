using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;
using System.Linq;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Resolutions
{
    public class MapperResolutionModel : BaseEntityMapper<Resolution, ResolutionModel>
    {
        #region [ Fields ]

        private readonly MapperCategoryModel _categoryMapper;
        private readonly MapperContainerModel _containerMapper;
        private readonly MapperFileResolutionModel _fileResolutionMapper;
        private readonly MapperResolutionRoleModel _resolutionRoleMapper;

        #endregion

        #region [ Constructor ]

        public MapperResolutionModel() : base()
        {
            _categoryMapper = new MapperCategoryModel();
            _containerMapper = new MapperContainerModel();
            _fileResolutionMapper = new MapperFileResolutionModel();
            _resolutionRoleMapper = new MapperResolutionRoleModel();
        }
        #endregion

        #region [ Methods ]

        protected override IQueryOver<Resolution, Resolution> MappingProjection(IQueryOver<Resolution, Resolution> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override ResolutionModel TransformDTO(Resolution entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare Resolution se l'entità non è inizializzata");

            ResolutionModel model = new ResolutionModel(entity.Id)
            {
                UniqueId = entity.UniqueId,
                AdoptionDate = entity.AdoptionDate,
                AdoptionUser = entity.AdoptionUser,
                AlternativeAssignee = entity.AlternativeAssignee,
                AlternativeManager = entity.AlternativeManager,
                AlternativeProposer = entity.AlternativeProposer,
                AlternativeRecipient = entity.AlternativeRecipient,
                Category = entity.SubCategory == null ? _categoryMapper.MappingDTO(entity.Category) : _categoryMapper.MappingDTO(entity.SubCategory),
                Container = _containerMapper.MappingDTO(entity.Container),
                ConfirmDate = entity.ConfirmDate,
                ConfirmUser = entity.ConfirmUser,
                EffectivenessDate = entity.EffectivenessDate,
                EffectivenessUser = entity.EffectivenessUser,
                LeaveDate = entity.LeaveDate,
                LeaveUser = entity.Leaveuser,
                Number = entity.Number,
                ProposeDate = entity.ProposeDate,
                ProposeUser = entity.ProposeUser,
                PublishingDate = entity.PublishingDate,
                PublishingUser = entity.PublishingUser,
                ResponseDate = entity.ResponseDate,
                ResponseUser = entity.ResponseUser,
                ServiceNumber = entity.ServiceNumber,
                Subject = entity.ResolutionObject,
                WaitDate = entity.WaitDate,
                WaitUser = entity.WaitUser,
                WarningDate = entity.WarningDate,
                WarningUser = entity.WarningUser,
                WorkflowType = entity.WorkflowType,
                Year = entity.Year,
                LastChangedDate = entity.LastChangedDate,
                LastChangedUser = entity.LastChangedUser,
                RegistrationUser = entity.AdoptionUser,
                FileResolution = _fileResolutionMapper.MappingDTO(entity.File),
                ResolutionRoles = entity.ResolutionRoles.Select(r => _resolutionRoleMapper.MappingDTO(r)).ToList()
            };

            return model;
        }

        #endregion
    }
}
