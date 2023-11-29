using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuite.Public.Core.Models.Customs.AUSL_RE.BandiDiGara;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.UDS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Repository;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.CustomModules
{
    [LogCategory(LogCategoryDefinition.ODATAAPI)]
    [EnableQuery]
    [AllowAnonymous]
    public class AUSLRE_CommittenteMenuController : ODataController
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories = null;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly Guid _instanceId;
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(AUSLRE_BandiDiGaraMenuController));
                }
                return _logCategories;
            }
        }

        protected string Username { get; }

        protected string Domain { get; }
        #endregion

        #region [ Constructor ]
        public AUSLRE_CommittenteMenuController(IDataUnitOfWork unitOfWork, ILogger logger)
            : base()
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _instanceId = Guid.NewGuid();
        }
        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult GetMenu()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IEnumerable<Guid> menuIds = _unitOfWork.Repository<UDSFieldList>().ExecuteModelFunction<Guid>(CommonDefinition.SQL_FX_Get_UDS_T_Committente_MenuValue);
                List<MenuModel> levels = new List<MenuModel>();
                foreach (Guid menuId in menuIds)
                {
                    List<UDSFieldListTableValuedModel> mainNode = _unitOfWork.Repository<UDSFieldList>()
                    .ExecuteModelFunction<UDSFieldListTableValuedModel>(CommonDefinition.SQL_FX_UDSFieldList_GetChildrenByParent,
                             new QueryParameter(CommonDefinition.SQL_Param_UDSFieldList_IdUDSFieldList, menuId)).ToList();

                    ICollection<MenuModel> menu = mainNode.Select(x => new MenuModel() { Name = x.Name, UniqueId = x.UniqueId }).ToList();

                    foreach (UDSFieldListTableValuedModel item in mainNode)
                    {
                        List<UDSFieldListTableValuedModel> children = _unitOfWork.Repository<UDSFieldList>().ExecuteModelFunction<UDSFieldListTableValuedModel>(CommonDefinition.SQL_FX_UDSFieldList_GetChildrenByParent,
                            new QueryParameter(CommonDefinition.SQL_Param_UDSFieldList_IdUDSFieldList, item.UniqueId)).ToList();
                        ICollection<MenuModel> subMenu = children.Select(x => new MenuModel() { Name = x.Name, UniqueId = x.UniqueId }).ToList();
                        menu.FirstOrDefault(x => x.UniqueId == item.UniqueId).SubMenu = subMenu;
                    }
                    levels.AddRange(menu);
                }

                return Ok(levels);
            }, _logger, _logCategories);
        }
        #endregion
    }
}