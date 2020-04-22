using Microsoft.AspNet.OData.Batch;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace VecompSoftware.DocSuite.Private.WebAPI.Handlers
{
    public class DSWODataBatchHandler : DefaultODataBatchHandler
    {
        public DSWODataBatchHandler(HttpServer httpServer)
            : base(httpServer)
        {

        }

        //
        // Summary:
        //     Executes the OData batch requests.
        //
        // Parameters:
        //   requests:
        //     The collection of OData batch requests.
        //
        //   cancellationToken:
        //     The token to monitor for cancellation requests.
        //
        // Returns:
        //     A collection of System.Web.OData.Batch.ODataBatchResponseItem for the batch requests.
        public override Task<IList<ODataBatchResponseItem>> ExecuteRequestMessagesAsync(IEnumerable<ODataBatchRequestItem> requests, CancellationToken cancellationToken)
        {
            return base.ExecuteRequestMessagesAsync(requests, cancellationToken);
        }
        //
        // Summary:
        //     Converts the incoming OData batch request into a collection of request messages.
        //
        // Parameters:
        //   request:
        //     The request containing the batch request messages.
        //
        //   cancellationToken:
        //     The token to monitor for cancellation requests.
        //
        // Returns:
        //     A collection of System.Web.OData.Batch.ODataBatchRequestItem.
        public override Task<IList<ODataBatchRequestItem>> ParseBatchRequestsAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.ParseBatchRequestsAsync(request, cancellationToken);
        }

        public override Task<HttpResponseMessage> ProcessBatchAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.ProcessBatchAsync(request, cancellationToken);
        }

    }
}
