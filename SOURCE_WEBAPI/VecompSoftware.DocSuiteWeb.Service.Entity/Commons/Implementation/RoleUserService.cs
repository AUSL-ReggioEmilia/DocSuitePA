using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Finder.Templates;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Commons
{
    public class RoleUserService : BaseService<RoleUser>, IRoleUserService
    {
        #region [ Fields ]

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public RoleUserService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IRoleRuleset roleRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, roleRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override IQueryFluent<RoleUser> SetEntityIncludeOnUpdate(IQueryFluent<RoleUser> query)
        {
            query.Include(f => f.Role);


            return query;
        }

        protected override RoleUser BeforeUpdate(RoleUser entity, RoleUser entityTransformed)
        {
            if (CurrentUpdateActionType.HasValue && CurrentUpdateActionType.Value == UpdateActionType.RoleUserTemplateCollaborationInvalid)
            {
                if (string.Equals(entityTransformed.Type, "D") || string.Equals(entityTransformed.Type, "V"))
                {
                    if (entity.Role != null)
                    {
                        entityTransformed.Role = _unitOfWork.Repository<Role>().Find(entity.Role.IdRoleTenant);
                    }

                    IQueryable<TemplateCollaborationUser> invalidUsers = _unitOfWork.Repository<TemplateCollaborationUser>().GetTemplatesByUser(entityTransformed.Account, entityTransformed.Role.IdRoleTenant);

                    if (invalidUsers != null && invalidUsers.Count() > 0)
                    {
                        TemplateCollaboration template;

                        foreach (TemplateCollaborationUser user in invalidUsers)
                        {
                            template = user.TemplateCollaboration;
                            template.Status = TemplateCollaborationStatus.NotActive;
                            user.IsValid = false;
                            _unitOfWork.Repository<TemplateCollaboration>().Update(template);
                            _unitOfWork.Repository<TemplateCollaborationUser>().Update(user);
                        }
                    }
                }
            }
            return base.BeforeUpdate(entity, entityTransformed);
        }

        #endregion

    }
}