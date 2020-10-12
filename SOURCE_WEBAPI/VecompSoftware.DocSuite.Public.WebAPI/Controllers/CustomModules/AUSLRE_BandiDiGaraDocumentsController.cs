using System;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.CustomModules
{
    [AllowAnonymous]
    public class AUSLRE_BandiDiGaraDocumentsController : ApiController
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
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(AUSLRE_BandiDiGaraArchivesController));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public AUSLRE_BandiDiGaraDocumentsController(IDataUnitOfWork unitOfWork, ILogger logger)
            : base()
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _instanceId = Guid.NewGuid();
        }
        #endregion

        #region [ Methods ]
        [HttpGet]
        public HttpResponseMessage GetDocument(Guid guid)
        {
            using (var client = new WebClient())
            {
                var content = client.DownloadData("https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf");
                using (var stream = new MemoryStream(content))
                {
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(stream.ToArray())
                    };

                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "title_replaced_by_client.pdf"
                    };

                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    return result;
                }
            }
        }
        #endregion
    }
}