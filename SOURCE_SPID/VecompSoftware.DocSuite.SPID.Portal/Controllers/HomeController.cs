using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VecompSoftware.DocSuite.SPID.AuthEngine.Models;
using VecompSoftware.DocSuite.SPID.Common.Helpers.Actions;
using Microsoft.Extensions.Logging;
using VecompSoftware.DocSuite.SPID.Common.Logging;

namespace VecompSoftware.DocSuite.SPID.Portal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        public HomeController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(LogCategories.PORTAL);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return LocalRedirect("~/pages/500");
        }

        [HttpPost]
        public IActionResult AuthenticationCallback([FromForm]bool authenticated)
        {
            return ActionHelper.TryCatchWithLoggerGeneric<IActionResult>(() =>
            {
                if (!authenticated)
                {
                    return LocalRedirect("~/pages/login?authFailed=true");
                }

                return LocalRedirect("~/home");
            }, _logger);
        }
    }
}
