using System;
using VecompSoftware.JeepService.LogConservation.Models;

namespace VecompSoftware.JeepService.LogConservation.Services
{
    public class ServiceFactory
    {        
        public static IService GetService(string entityName, string loggerName, LogConservationParameters parameters, Func<bool> cancelRequest)
        {
            IService instance = null;
            switch (entityName)
            {
                case LogEntityNameDefinition.PROTOCOLLOG_NAME:
                    instance = new ProtocolLogConservationService(loggerName, parameters, cancelRequest);
                    break;
                case LogEntityNameDefinition.DOCUMENTSERIESITEMLOG_NAME:
                    instance = new DocumentSeriesItemLogConservationService(loggerName, parameters, cancelRequest);
                    break;
                case LogEntityNameDefinition.UDSLOG_NAME:
                    instance = new UDSLogConservationService(loggerName, parameters, cancelRequest);
                    break;
                case LogEntityNameDefinition.FASCICLELOG_NAME:
                    instance = new FascicleLogConservationService(loggerName, parameters, cancelRequest);
                    break;
                case LogEntityNameDefinition.DOSSIERLOG_NAME:
                    instance = new DossierLogConservationService(loggerName, parameters, cancelRequest);
                    break;
                case LogEntityNameDefinition.PECMAILLOG_NAME:
                    instance = new PECMailLogConservationService(loggerName, parameters, cancelRequest);
                    break;
                case LogEntityNameDefinition.TABLELOG_NAME:
                    instance = new TableLogConservationService(loggerName, parameters, cancelRequest);
                    break;
            }
            return instance;
        }
    }
}
