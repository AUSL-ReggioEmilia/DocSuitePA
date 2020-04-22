using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BiblosDS.Library.Common.Objects;
using System.ServiceModel;
using BiblosDS.Library.Common.Objects.Response;

namespace BiblosDS.WCF.WCFServices
{
    public class PreservationServiceInstances : ServiceReferencePreservation.IServicePreservationCallback
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(PreservationServiceInstances));

        public PreservationInfoResponse ExecutePreservation(PreservationTask task)
        {
            PreservationInfoResponse result = null;
            InstanceContext context = new InstanceContext(this);
            using (var client = new ServiceReferencePreservation.ServicePreservationClient(context, "ServicePreservation"))
            {
                result = client.CreatePreservationByTask(task);
            }
            return result;
        }

        public void Pulse(string source, string message, int progressPercentage)
        {
            logger.InfoFormat("source:{0}, message:{1}, progressPercentage:{2}", source, message, progressPercentage);
        }
    }
}