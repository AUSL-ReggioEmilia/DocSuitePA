using log4net;
using System;
using System.ServiceModel;

namespace VecompSoftware.BiblosDS.WindowsService.Common.Helpers
{
    public static class ClientActionHelper
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]
        public static void TryCatchWithLogger<TClientChannel, TServiceType>(Action<TClientChannel> action, TimeSpan? timeout, string endpointName, ILog logger)
            where TClientChannel : ClientBase<TServiceType>
            where TServiceType : class
        {
            TClientChannel client = default(TClientChannel);
            try
            {
                if (string.IsNullOrEmpty(endpointName))
                {
                    client = Activator.CreateInstance<TClientChannel>();
                }
                else
                {
                    client = Activator.CreateInstance(typeof(TClientChannel), new object[] { endpointName }) as TClientChannel;
                }
                
                if (timeout.HasValue)
                {
                    client.InnerChannel.OperationTimeout = timeout.Value;
                }
                client.Open();
                action(client);
            }
            catch (CommunicationException cex)
            {
                logger.Error("TimerCallback -> received communication exception", cex);
                if (client != null)
                {
                    client.Abort();
                }
            }
            catch (TimeoutException tex)
            {
                logger.Error("TimerCallback -> received timeout exception", tex);
                if (client != null)
                {
                    client.Abort();
                }
            }
            catch (Exception ex)
            {
                logger.Error("TimerCallback -> error on process request", ex);
                if (client != null)
                {
                    client.Abort();
                }
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }
        #endregion
    }
}
