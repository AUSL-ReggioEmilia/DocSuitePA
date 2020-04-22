using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;
using BiblosDS.WCF.StampaConforme.Converter;
using System.IO;

namespace BiblosDS.WindowsService.StampaConforme.Office.Converter
{
    partial class OfficeConverterService : ServiceBase
    {
        public ServiceHost serviceHostStampaConforme = null;
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(OfficeConverterService)); 

        public OfficeConverterService()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();
        }


        protected override void OnStart(string[] args)
        {
            try
            {
                Write("Start");
                logger.Info("StartService");
                if (serviceHostStampaConforme != null)
                {
                    serviceHostStampaConforme.Close();
                }

                // Create a ServiceHost for the CalculatorService type and 
                // provide the base address.
                serviceHostStampaConforme = new ServiceHost(typeof(StampaConformeConverter));

                // Open the ServiceHostBase to create listeners and start 
                // listening for messages.
                serviceHostStampaConforme.Open();
            }
            catch (Exception e)
            {
                Write(e.ToString());
                logger.Error(e);
            }
        }
        public void ConsoleOnStart()
        {
            OnStart(null);
        }

        protected override void OnStop()
        {
            try
            {
                Write("Stop");
                logger.Info("StopService");
                if (serviceHostStampaConforme != null)
                {
                    serviceHostStampaConforme.Close();
                    serviceHostStampaConforme = null;
                }
            }
            catch (Exception e)
            {
                Write(e.ToString());
                logger.Error(e);
            }
        }
        public void ConsoleOnStop()
        {
            OnStop();
        }

        public static void Write(string msg)
        {
            try
            {
                if (!EventLog.SourceExists("Application"))
                    EventLog.CreateEventSource("Application", "BiblosDS2010");
                EventLog.WriteEntry("Application", msg, EventLogEntryType.Warning, 234);
            }
            catch (Exception ex)
            {               
            }
        }
    }
}
