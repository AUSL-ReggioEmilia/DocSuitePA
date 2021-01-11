using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using VecompSoftware.DocSuite.SPID.AccessoAtti.Models;
using VecompSoftware.DocSuite.SPID.Common.Helpers.Actions;
using VecompSoftware.DocSuite.SPID.Common.Logging;

namespace VecompSoftware.SPID.AccessoAtti.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        public HomeController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(LogCategories.APPLICATION);
        }

        public IActionResult Index()
        {
            ViewData["UserLogged"] = TempData["UserLogged"];
            return View();
        }

        public IActionResult ExternalAuth(string jwt_token)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = handler.ReadJwtToken(jwt_token);
                UserModel userModel = new UserModel(jwtToken);
                TempData["UserLogged"] = JsonConvert.SerializeObject(userModel);
                return RedirectToAction("Index");
            }, _logger);            
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return LocalRedirect("~/error");
        }
    }
}
