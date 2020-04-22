using log4net;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.BiblosDS.Model.CQRS;
using VecompSoftware.BiblosDS.WindowsService.WebAPI.Receivers;

namespace VecompSoftware.BiblosDS.WindowsService.WebAPI.Controllers
{
    public class CQRSController : ApiController
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CQRSController));
        private readonly IReceiverMediator _receiverMediator;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public CQRSController()
        {
            _receiverMediator = new ReceiverMediator();
        }
        #endregion

        #region [ Methods ]
        [HttpPost]
        public IHttpActionResult Post([FromBody]CommandModel message)
        {
            if (message == null)
            {
                _logger.Error("Received message is not in the correct format");
                return BadRequest("Received message is not in the correct format");
            }

            _receiverMediator.Send(message).ConfigureAwait(false);
            return Ok();
        }
        #endregion        
    }
}
