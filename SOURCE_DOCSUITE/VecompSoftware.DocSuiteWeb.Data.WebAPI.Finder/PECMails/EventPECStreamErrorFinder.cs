using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.PECMails
{
    public class EventPECStreamErrorFinder : BaseWebAPIFinder<ServiceBusTopicMessage, ServiceBusTopicMessage>
    {
        #region [ Constructor ]

        public EventPECStreamErrorFinder(TenantModel tenant)
         : this(new List<TenantModel>() { tenant })
        { }

        public EventPECStreamErrorFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }

        #endregion

        #region [ Properties ]

        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }
        public string CorrelationId { get; set; }

        #endregion

        #region [ Methods ]

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            string FX_GetEventPECStreamError = string.Format(
                CommonDefinition.OData.PECMailService.FX_GetEventPECStreamError,
                TopicName,
                SubscriptionName,
                CorrelationId
                );
            odataQuery = odataQuery.Function(FX_GetEventPECStreamError);
            return odataQuery;
        }

        public override void ResetDecoration()
        {
            TopicName = string.Empty;
            SubscriptionName = string.Empty;
            CorrelationId = string.Empty;
        }

        #endregion
    }
}
