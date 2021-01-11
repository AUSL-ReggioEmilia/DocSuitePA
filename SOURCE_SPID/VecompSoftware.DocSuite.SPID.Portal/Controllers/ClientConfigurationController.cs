using Microsoft.AspNetCore.Mvc;
using VecompSoftware.DocSuite.SPID.Portal.Models;
using Microsoft.Extensions.Options;

namespace VecompSoftware.DocSuite.SPID.Portal.Controllers
{
    [Route("api/[controller]")]
    public class ClientConfigurationController : Controller
    {
        ClientConfiguration clientConfig;
        public ClientConfigurationController(IOptions<ClientConfiguration> clientConfigOptions)
        {
            clientConfig = clientConfigOptions?.Value;
        }

        [HttpGet()]
        public ClientConfiguration Get()
        {
            return clientConfig;
        }
    }
}