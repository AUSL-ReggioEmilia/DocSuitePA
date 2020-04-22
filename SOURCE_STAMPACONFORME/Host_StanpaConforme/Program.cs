using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace Host_StanpaConforme
{
    class Program
    {
        static void Main(string[] args)
        {
            WCFHost host = new WCFHost();
            host.OnStart();
            Console.WriteLine("Started..");
            Console.ReadLine();
            host.OnStop();
        }
    }


    class WCFHost
    {
        Timer _GetAliveTimer;        
        public ServiceHost serviceHostStampaConforme = null;
        
        public WCFHost()
        {


            _GetAliveTimer = new Timer(new TimerCallback(GetAliveTimer_Tick));
            _GetAliveTimer.Change(50000, 50000);

            //serviceHostDocument.Faulted += new EventHandler(serviceHostDocument_Faulted);
            //for (int iChannel = 0 ; iChannel < serviceHostDocument.ChannelDispatchers.Count ; iChannel++)
            //    serviceHostDocument.ChannelDispatchers[iChannel].Faulted += new EventHandler(serviceHostDocumentChannel_Faulted);
        }

        void serviceHostDocumentChannel_Faulted(object sender, EventArgs e)
        {
            //Logging.WriteLogEvent(LoggingSource.BiblosDS_WindowsService_WCFHost,
            //    "WCFHost.serviceHostDocumentChannel_Faulted",
            //    "WCF Channel of ServiceDocument report an fault",
            //    LoggingOperationType.BiblosDS_General,
            //    LoggingLevel.BiblosDS_Managed_Error);
        }

        void serviceHostDocument_Faulted(object sender, EventArgs e)
        {
            //Logging.WriteLogEvent(LoggingSource.BiblosDS_WindowsService_WCFHost,
            //    "WCFHost.serviceHostDocumentChannel_Faulted",
            //    "WCFHost of ServiceDocument report an fault",
            //    LoggingOperationType.BiblosDS_General,
            //    LoggingLevel.BiblosDS_Managed_Error);
        }

        private void GetAliveTimer_Tick(object State)
        {
            _GetAliveTimer.Change(Timeout.Infinite, Timeout.Infinite);            
        }

        #region StopStart WCF Services


        private void StartServiceStampaConforme()
        {
            try
            {

                if (serviceHostStampaConforme != null)
                {
                    serviceHostStampaConforme.Close();
                }

                // Create a ServiceHost for the CalculatorService type and 
                // provide the base address.
                serviceHostStampaConforme = new ServiceHost(typeof(BiblosDS.WCF.StampaConforme.ServiceStampaConforme));

                // Open the ServiceHostBase to create listeners and start 
                // listening for messages.
                serviceHostStampaConforme.Open();
            }
            catch (Exception e)
            {

            }
        }

        private void StopServiceStampaConforme()
        {
            try
            {

                if (serviceHostStampaConforme != null)
                {
                    serviceHostStampaConforme.Close();
                    serviceHostStampaConforme = null;
                }
            }
            catch (Exception e)
            {

            }
        }


        #endregion

        public void OnStart()
        {
            StartServiceStampaConforme();

        }

        public void OnStop()
        {


            StopServiceStampaConforme();

        }
    }
}
