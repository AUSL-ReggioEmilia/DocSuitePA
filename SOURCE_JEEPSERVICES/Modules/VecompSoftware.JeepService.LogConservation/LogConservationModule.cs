using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VecompSoftware.JeepService.Common;
using VecompSoftware.JeepService.LogConservation.Models;
using VecompSoftware.JeepService.LogConservation.Services;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.LogConservation
{
    public class LogConservationModule : JeepModuleBase<LogConservationParameters>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        private IDictionary<string, IService> Services { get; set; }
        #endregion

        #region [ Constructor ]
        public LogConservationModule()
            : base()
        {
            Services = new Dictionary<string, IService>();
        }
        #endregion

        #region [ Methods ]
        private bool CancelRequest()
        {
            return Cancel;
        }

        private void SendNotificationMessage(string message)
        {
            SendMessage(message);
        }

        public override void Initialize(List<Parameter> parameters)
        {
            base.Initialize(parameters);
            if (Parameters.LogConservationLocation == null)
            {
                throw new Exception("Nessuna location definita per la conservazione dei log di DocSuite.");
            }

            InitializeServices();
        }

        public override void SingleWork()
        {
            if (Services == null || Services.Count == 0)
            {
                return;
            }

            ConservateResultModel resultModel;
            foreach (KeyValuePair<string, IService> service in Services)
            {
                if (Cancel)
                {
                    return;
                }

                FileLogger.Info(Name, string.Concat(service.Key, " - init conservate process"));
                resultModel = service.Value.DoConservate();
                if (resultModel.HasError)
                {
                    StringBuilder builder = new StringBuilder();
                    resultModel.Errors.ToList().ForEach(f => builder.AppendLine(f));
                    SendMessage(builder.ToString());
                }
                FileLogger.Info(Name, string.Concat(service.Key, " - end conservate process"));
            }
        }

        private void InitializeServices()
        {
            IEnumerable<PropertyInfo> properties = typeof(LogConservationParameters).GetProperties().Where(prop => prop.IsDefined(typeof(LogServiceAttribute), false));
            foreach (PropertyInfo prop in properties.Where(x => (bool)x.GetValue(Parameters, null) == true))
            {
                InitializeService(prop.GetCustomAttribute<LogServiceAttribute>().Name);
            }
        }

        private void InitializeService(string serviceName)
        {
            FileLogger.Debug(Name, string.Concat("Instantiate service for ", serviceName));
            IService service = ServiceFactory.GetService(serviceName, Name, Parameters, CancelRequest);
            if (service == null)
            {
                FileLogger.Debug(Name, string.Concat("Service for ", serviceName, " not found"));
                return;
            }
            FileLogger.Debug(Name, string.Concat("Service for ", serviceName, " instantiated"));
            Services.Add(serviceName, service);
        }
        #endregion
    }
}
