using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VecompSoftware.DocSuite.SPID.Portal.Models;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.Logging;
using VecompSoftware.DocSuite.SPID.Common.Logging;
using VecompSoftware.DocSuite.SPID.Common.Helpers.Actions;
using VecompSoftware.DocSuite.SPID.Model.Common;
using VecompSoftware.DocSuite.SPID.JWT;
using System.IdentityModel.Tokens.Jwt;
using VecompSoftware.DocSuite.SPID.DataProtection;
using VecompSoftware.DocSuite.SPID.Model.Auth;
using System.Security.Claims;

namespace VecompSoftware.DocSuite.SPID.Portal.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger _logger;
        private readonly IJwtService _jwtService;
        private readonly IDataProtectionService _dataProtectionService;

        public ApplicationController(ILoggerFactory logger, IHostingEnvironment hostingEnvironment, IJwtService jwtService, IDataProtectionService dataProtectionService)
        {
            _logger = logger.CreateLogger(LogCategories.PORTAL);
            _hostingEnvironment = hostingEnvironment;
            _jwtService = jwtService;
            _dataProtectionService = dataProtectionService;
        }

        private ICollection<ApplicationModel> Applications =>
            JsonConvert.DeserializeObject<ICollection<ApplicationModel>>(System.IO.File.ReadAllText(Path.Combine(_hostingEnvironment.ContentRootPath, "applications.json")));

        [HttpGet]        
        public IActionResult GetAll()
        {
            return ActionHelper.TryCatchWithLoggerGeneric<IActionResult>(() =>
            {
                ODataResult<ApplicationModel> results = new ODataResult<ApplicationModel>
                {
                    value = Applications.OrderBy(o => o.Name).ToList()
                };
                return Ok(results);
            }, _logger);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            return ActionHelper.TryCatchWithLoggerGeneric<IActionResult>(() =>
            {
                ApplicationModel selectedApplication = Applications.FirstOrDefault(x => x.Id == id);                
                if (selectedApplication == null)
                {
                    _logger.LogWarning("GetById -> {0} not found", id);
                    return NotFound();
                }

                string claimId = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
                string currentToken = _jwtService.Clone(_dataProtectionService.Protect(claimId), true);
                if (string.IsNullOrEmpty(currentToken))
                {
                    return Unauthorized();
                }

                selectedApplication.Url = $"{selectedApplication.Url}?jwt_token={currentToken}";
                ODataResult<ApplicationModel> results = new ODataResult<ApplicationModel>
                {
                    value = new List<ApplicationModel>() { selectedApplication }
                };
                return Ok(results);
            }, _logger);
        }
    }
}