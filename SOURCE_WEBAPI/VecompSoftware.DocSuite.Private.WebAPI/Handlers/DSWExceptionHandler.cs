using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace VecompSoftware.DocSuite.Private.WebAPI.Handlers
{
    public class DSWExceptionHandler : ExceptionHandler
    {
        public override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            if (!ShouldHandle(context))
            {
                return Task.FromResult(0);
            }

            return HandleAsyncCore(context, cancellationToken);
        }

        public Task HandleAsyncCore(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            Handle(context);
            return Task.FromResult(0);
        }

        public override void Handle(ExceptionHandlerContext context)
        {
        }

        public override bool ShouldHandle(ExceptionHandlerContext context)
        {
            return true;
        }
    }
}
