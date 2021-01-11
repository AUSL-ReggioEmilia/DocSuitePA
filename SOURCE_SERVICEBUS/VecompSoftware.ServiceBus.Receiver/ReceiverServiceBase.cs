using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.WebAPI;

namespace VecompSoftware.ServiceBus.Receiver
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class ReceiverServiceBase : IDisposable
    {
        private bool disposedValue = false;
        private readonly IListenerMessage _listener = null;
        private readonly ILogger _logger = null;
        private readonly DirectoryInfo _directoryInfo = null;
        protected static IEnumerable<LogCategory> _logCategories = null;
        protected static Type _type_iListenerMessage = typeof(IListenerMessage);
        protected static Type _type_biblosClient = typeof(BiblosDS.BiblosClient);
        protected static Type _type_stampaConformeClient = typeof(StampaConforme.StampaConformeClient);
        protected static Type _type_serviceBusClient = typeof(ServiceBus.ServiceBusClient);

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ReceiverServiceBase));
                }
                return _logCategories;
            }
        }
        #endregion

        public ReceiverServiceBase(ILogger logger)
        {
            try
            {
                _logger = logger;
                _logger.WriteInfo(new LogMessage("Init Vecomsoftware.ServiceBus.Receiver"), LogCategories);

                _logger.WriteInfo(new LogMessage("Create new Message Factory"), LogCategories);
                MessagingFactory factory = MessagingFactory.CreateFromConnectionString(ReceiverConfiguration.QueueConnectionString);
                _logger.WriteInfo(new LogMessage(string.Concat("Create new Message Receiver -> ", ReceiverConfiguration.QueueName)), LogCategories);
                MessageReceiver receiver = factory.CreateMessageReceiver(ReceiverConfiguration.QueueName, ReceiveMode.PeekLock);

                _logger.WriteInfo(new LogMessage("Start listening message"), LogCategories);
                AppDomain listenerDomain = Thread.GetDomain();
                listenerDomain.AssemblyResolve += ListenerDomain_AssemblyResolve;
                IWebAPIClient webApiClient = new WebAPIClient(logger, ReceiverConfiguration.AddressesJsonConfigWebAPI);

                _directoryInfo = new DirectoryInfo(Path.Combine(listenerDomain.BaseDirectory, "Listener"));
                _logger.WriteDebug(new LogMessage(string.Concat("DirectoryInfo: ", _directoryInfo.FullName, " (", _directoryInfo.EnumerateFiles().Count(), ")")), LogCategories);
                _logger.WriteDebug(new LogMessage(string.Concat("Searching ... ", ReceiverConfiguration.ListenerAssemblyFullName)), LogCategories);


                Assembly externalAssembly = listenerDomain.Load(File.ReadAllBytes(_directoryInfo.EnumerateFiles()
                    .Single(f => f.Name.Equals(ReceiverConfiguration.ListenerAssemblyFullName)).FullName));

                Type currentListener = externalAssembly.GetTypes().Single(f => _type_iListenerMessage.IsAssignableFrom(f));
                bool containsBiblosDocumentClient = currentListener.GetConstructors().Single().GetParameters().Any(f => f.ParameterType.Equals(_type_biblosClient));
                bool containsStampaConformeClient = currentListener.GetConstructors().Single().GetParameters().Any(f => f.ParameterType.Equals(_type_stampaConformeClient));
                bool containsServiceBusClient = currentListener.GetConstructors().Single().GetParameters().Any(f => f.ParameterType.Equals(_type_serviceBusClient));
                List<object> constructorParameters = new List<object>
                {
                    receiver,
                    _logger,
                    webApiClient
                };
                if (containsBiblosDocumentClient)
                {
                    constructorParameters.Add(new BiblosDS.BiblosClient());
                }
                if (containsStampaConformeClient)
                {
                    constructorParameters.Add(new StampaConforme.StampaConformeClient());
                }
                if (containsServiceBusClient)
                {
                    constructorParameters.Add(new ServiceBus.ServiceBusClient(logger));
                }
                _listener = (IListenerMessage)Activator.CreateInstance(currentListener, constructorParameters.ToArray());
            }
            catch (ReflectionTypeLoadException r_ex)
            {
                foreach (Exception ex in r_ex.LoaderExceptions)
                {
                    _logger.WriteError(ex, LogCategories);
                }
                throw r_ex;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        public async Task StartAsync()
        {
            await _listener.StartListeningAsync(true, 1);
        }
        private Assembly ListenerDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                AppDomain listenerDomain = Thread.GetDomain();
                Assembly externalAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(f => f.FullName.Equals(args.Name));
                if (externalAssembly == null)
                {
                    AssemblyName requestedAssemblyName = new AssemblyName(args.Name);
                    string requestedAssembly = Path.Combine(Path.Combine(listenerDomain.BaseDirectory, "Listener"), string.Concat(requestedAssemblyName.Name, ".dll"));
                    _logger.WriteInfo(new LogMessage(string.Concat("loading dynamic dll ", args.Name, " -> ", requestedAssembly)), LogCategories);
                    externalAssembly = Assembly.LoadFrom(requestedAssembly);
                }
                return externalAssembly;
            }
            catch (Exception ex)
            {
                if (!(ex.Message.Contains("VecompSoftware.Helpers.UDS.XmlSerializers.dll") || ex.Message.Contains("System.IO.FileSystem.Primitives.dll") ||
                    ex.Message.Contains("System.IO.FileSystem.dll") || ex.Message.Contains(".resources.dll") || !(ex.Message.Contains("VecompSoftware.Helpers.XmlSerializers.dll"))))
                {
                    _logger.WriteError(ex, LogCategories);
                    throw ex;
                }
                return null;
            }
        }

        #region [ IDisposable ]

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _logger.WriteInfo(new LogMessage("Disposing listener"), LogCategories);
                    if (_listener != null)
                    {
                        _listener.CloseListeningAsync().Wait();
                    }

                    _logger.WriteInfo(new LogMessage("listening is now disposed"), LogCategories);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
