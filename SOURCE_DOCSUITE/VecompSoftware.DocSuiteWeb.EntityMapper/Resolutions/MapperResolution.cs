using System;
using System.Linq;
using NHibernate;
using DSW = VecompSoftware.DocSuiteWeb.Data;
using APIResolution = VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Resolutions
{
    public class MapperResolution : BaseEntityMapper<DSW.Resolution, APIResolution.Resolution>
    {
        #region [ Fields ]

        private readonly MapperCategoryEntity _categoryMapper;
        private readonly MapperContainerEntity _containerMapper;
        private readonly MapperFileResolution _fileResolutionMapper;
        private readonly MapperResolutionRole _resolutionRoleMapper;
        private readonly MapperResolutionKind _resolutionKindMapper;
        private readonly MapperResolutionContact _resolutionContactMapper;

        #endregion

        #region [ Constructor ]

        public MapperResolution() : base()
        {
            _categoryMapper = new MapperCategoryEntity();
            _containerMapper = new MapperContainerEntity();
            _fileResolutionMapper = new MapperFileResolution();
            _resolutionRoleMapper = new MapperResolutionRole();
            _resolutionKindMapper = new MapperResolutionKind();
            _resolutionContactMapper = new MapperResolutionContact();
        }
        #endregion

        #region [ Methods ]

        protected override IQueryOver<DSW.Resolution, DSW.Resolution> MappingProjection(IQueryOver<DSW.Resolution, DSW.Resolution> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APIResolution.Resolution TransformDTO(DSW.Resolution entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare Resolution se l'entità non è inizializzata");
            }

            APIResolution.Resolution apiResolution = new APIResolution.Resolution()
            {
                EntityId = entity.Id,
                UniqueId = entity.UniqueId,
                AdoptionDate = entity.AdoptionDate,
                AdoptionUser = entity.AdoptionUser,
                AlternativeAssignee = entity.AlternativeAssignee,
                AlternativeManager = entity.AlternativeManager,
                AlternativeProposer = entity.AlternativeProposer,
                AlternativeRecipient = entity.AlternativeRecipient,
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
                InclusiveNumber = entity.InclusiveNumber,
                Object = entity.ResolutionObject,
                WaitDate = entity.WaitDate,
                WaitUser = entity.WaitUser,
                WarningDate = entity.WarningDate,
                WarningUser = entity.WarningUser,
                WorkflowType = entity.WorkflowType,
                Year = entity.Year,
                IdType = (byte)entity.Type.Id,
                LastChangedDate = entity.LastChangedDate,
                LastChangedUser = entity.LastChangedUser,
                Amount = entity.Amount,
                WebPublicationDate = entity.WebPublicationDate,
                Status = (APIResolution.ResolutionStatus)(entity.Status != null ? entity.Status.Id : -1),
                RegistrationDate = entity.AdoptionDate.HasValue ? entity.AdoptionDate.Value : entity.ProposeDate.HasValue ? entity.ProposeDate.Value : DateTimeOffset.UtcNow,
                Category = _categoryMapper.MappingDTO(entity.SubCategory != null ? entity.SubCategory : entity.Category),
                Container = _containerMapper.MappingDTO(entity.Container),
                RegistrationUser = entity.AdoptionDate.HasValue ? entity.AdoptionUser : entity.ProposeUser,
                ResolutionKind = entity.ResolutionKind != null ? _resolutionKindMapper.MappingDTO(entity.ResolutionKind) : null,
                FileResolution = _fileResolutionMapper.MappingDTO(entity.File),
                ResolutionRoles = entity.ResolutionRoles.Select(r => _resolutionRoleMapper.MappingDTO(r)).ToList(),
                ResolutionContacts = entity.ResolutionContacts.Where(f => f.Contact != null).Select(r => _resolutionContactMapper.MappingDTO(r)).ToList()
            };
            return apiResolution;
        }

        #endregion
    }
}
