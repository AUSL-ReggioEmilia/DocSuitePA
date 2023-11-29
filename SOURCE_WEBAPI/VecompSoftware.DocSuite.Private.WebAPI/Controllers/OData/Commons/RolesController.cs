using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Mapper;
using System.Linq;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Commons
{
    public class RolesController : BaseODataController<Role, IRoleService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        #endregion

        #region [ Constructor ]

        public RolesController(IRoleService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security, IMapperUnitOfWork mapperUnitOfWork)
            : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult FindRoles(ODataQueryOptions<Role> options, [FromODataUri]RoleFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<RoleFullTableValuedModel> roles = _unitOfWork.Repository<Role>().FindRoles(Username, Domain, finder.Name, finder.UniqueId, finder.ParentId, finder.ServiceCode,
                    finder.IdTenantAOO, finder.Environment, finder.LoadOnlyRoot, finder.LoadOnlyMy, finder.LoadAlsoParent, finder.RoleTypology, finder.IdCategory, finder.IdDossierFolder);
                ICollection<RoleModel> results = _mapperUnitOfWork.Repository<IDomainMapper<RoleFullTableValuedModel, RoleModel>>().MapCollection(roles);

                // build tree model only if loading parents with children
                if (finder.LoadOnlyRoot.HasValue && finder.LoadOnlyRoot.Value == false)
                {
                    ICollection<RoleModel> roleModelsWithChildren = BindChildNodesToParents(results.ToList());
                    return Ok(roleModelsWithChildren);
                }

                return Ok(results);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult CountFindRoles(ODataQueryOptions<Role> options, [FromODataUri]RoleFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                int rolesCount = _unitOfWork.Repository<Role>().CountFindRoles(Username, Domain, finder.Name, finder.UniqueId, finder.ParentId, finder.ServiceCode,
                    finder.IdTenantAOO, finder.Environment, finder.LoadOnlyRoot, finder.LoadOnlyMy, finder.LoadAlsoParent, finder.RoleTypology, finder.IdCategory, finder.IdDossierFolder);
                return Ok(rolesCount);
            }, _logger, LogCategories);
        }

        private List<RoleModel> BindChildNodesToParents(List<RoleModel> allRoleModels)
        {
            allRoleModels.ForEach(roleModelNode =>
                    roleModelNode.Children = allRoleModels.Where(roleModel => roleModel.IdRoleFather.HasValue && roleModel.IdRoleFather.Value == roleModelNode.IdRole).ToList());
            return allRoleModels.Where(roleModel => !roleModel.IdRoleFather.HasValue).ToList();
        }

        [HttpGet]
        public IHttpActionResult HasCategoryFascicleRole(ODataQueryOptions<Role> options, short idCategory)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<Role>().HasCategoryFascicleRole($"{Domain}\\{Username}", idCategory);
                return Ok(result);
            }, _logger, LogCategories);
        }

        #endregion
    }
}
