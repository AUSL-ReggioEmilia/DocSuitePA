using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;
using BiblosDS.WCF.StampaConforme;

namespace BiblosDS.WindowsService.WCFStampaConformeHost
{
    public partial class WCFStampaConforme : ServiceBase
    {
        public ServiceHost serviceHostStampaConforme = null;
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(WCFStampaConforme)); 

        public WCFStampaConforme()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                logger.Info("StartService");
                if (serviceHostStampaConforme != null)
                {
                    serviceHostStampaConforme.Close();
                }

                // Create a ServiceHost for the CalculatorService type and 
                // provide the base address.
                serviceHostStampaConforme = new ServiceHost(typeof(ServiceStampaConforme));

                // Open the ServiceHostBase to create listeners and start 
                // listening for messages.
                serviceHostStampaConforme.Open();
            }
            catch (Exception e)
            {
                logger.Error(e);               
            }
        }

        protected override void OnStop()
        {
            try
            {

                logger.Info("StopService");
                if (serviceHostStampaConforme != null)
                {
                    serviceHostStampaConforme.Close();
                    serviceHostStampaConforme = null;
                }
            }
            catch (Exception e)
            {
                logger.Error(e);       
            }
        }
    }
}
