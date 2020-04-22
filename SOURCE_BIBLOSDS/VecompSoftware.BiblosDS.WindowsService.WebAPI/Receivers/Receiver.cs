using System.Threading.Tasks;
using VecompSoftware.BiblosDS.Model.CQRS;

namespace VecompSoftware.BiblosDS.WindowsService.WebAPI.Receivers
{
    public abstract class Receiver : IReceiver
    {
        #region [ Fields ]
        private readonly IReceiverMediator _mediator;
        #endregion

        #region [ Properties ]
        protected IReceiverMediator Mediator => _mediator;
        #endregion

        #region [ Constructor ]
        public Receiver(IReceiverMediator mediator)
        {
            _mediator = mediator;
        }
        #endregion

        #region [ Methods ]
        public abstract Task Execute(CommandModel commandModel);
        #endregion        
    }
}
