using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuite.Public.Core.Models.Customs.AUSL_RE.BandiDiGara;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.CustomModules
{
    [LogCategory(LogCategoryDefinition.ODATAAPI)]
    [EnableQuery]
    [AllowAnonymous]
    public class AUSLRE_BandiDiGaraMenuController : ODataController
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
        public AUSLRE_BandiDiGaraMenuController(IDataUnitOfWork unitOfWork, ILogger logger)
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
                ICollection<MenuModel> menu = new List<MenuModel>
                {
                    new MenuModel
                    {
                        Name = "Concorsi - Selezioni - Procedure comparative - Incarichi di funzione",
                        SubMenu = new List<MenuModel>()
                        {
                            new MenuModel
                            {
                                Name = "Concorsi, selezioni, avvisi mobilita"
                            },
                            new MenuModel
                            {
                                Name = "Bando Child 2"
                            },
                        }
                    },
                    new MenuModel
                    {
                        Name = "Incarichi ad esperti e consulenti dell\'Azienda \'USL",
                        SubMenu = new List<MenuModel>()
                        {
                            new MenuModel
                            {
                                Name = "Bando Child 3"
                            }
                        }
                    },
                    new MenuModel
                    {
                        Name = "Incarichi personale convenzionato",
                        SubMenu = new List<MenuModel>()
                        {
                            new MenuModel
                            {
                                Name = "Bando Child 4"
                            }
                        }
                    }
                };

                return Ok(menu);
            }, _logger, _logCategories);
        }
        #endregion
    }
}