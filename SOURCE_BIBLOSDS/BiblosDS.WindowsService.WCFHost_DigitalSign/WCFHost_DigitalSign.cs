using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;
using BiblosDS.WCF.DigitalSign;

namespace BiblosDS.WindowsService.WCFHost_DigitalSign
{
    partial class WCFHost_DigitalSign : ServiceBase
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(WCFHost_DigitalSign));
        public ServiceHost serviceHostDigitalSign = null;

        public WCFHost_DigitalSign()
        {            
            InitializeComponent();           
        }

        protected override void OnStart(string[] args)
        {
            StartServiceDigitalSign();
        }

        protected override void OnStop()
        {
            StopServiceDigitalSign();
        }

        private void StopServiceDigitalSign()
        {
            try
            {
                logger.Error("StopServiceDigitalSign");               
                if (serviceHostDigitalSign != null)
                {
                    serviceHostDigitalSign.Close();
                    serviceHostDigitalSign = null;
                }
            }
            catch (Exception e)
            {
                logger.Error(e);               
            }
        }

        private void StartServiceDigitalSign()
        {
            try
            {
                logger.Error("StartServiceDigitalSign");               
                if (serviceHostDigitalSign != null)
                {
                    serviceHostDigitalSign.Close();
                }

                // Create a ServiceHost for the CalculatorService type and 
                // provide the base address.
                serviceHostDigitalSign = new ServiceHost(typeof(ServiceDigitalSign));

                // Open the ServiceHostBase to create listeners and start 
                // listening for messages.
                serviceHostDigitalSign.Open();
            }
            catch (Exception e)
            {
                logger.Error(e);              
            }
        }      
    }
}
