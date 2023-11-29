using Microsoft.Practices.Unity;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.ServiceBus.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Common.Configuration;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Common.Test;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.EF;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.EF.Test.FakeRepositories;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Collaborations;
using VecompSoftware.Services.Command;

namespace VecompSoftware.DocSuiteWeb.Service.EF.Test
{
    public class Identity : ICurrentIdentity
    {
        public string FullUserName => "";

        public string Account => "";

        public string Domain => "";

        public bool IsServiceAccount => true;
    }

    [TestClass]
    public class UnitTestProtocolService
    {
        #region [ Fields ]

        private bool _disposed = false;
        private IDisposable _shimContext;
        private IUnityContainer _unityContainer;
        #endregion

        #region [ Constructor ]

        public UnitTestProtocolService() { }
        #endregion

        #region [ Dispose ]

        ~UnitTestProtocolService()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _shimContext.Dispose();
                    _unityContainer.Dispose();
                }

                _shimContext = null;
                _disposed = true;
            }
        }
        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _shimContext = ShimsContext.Create();
            _unityContainer = new UnityContainer();
            _unityContainer.RegisterType<IDataUnitOfWork, DataUnitOfWork>();
            _unityContainer.RegisterType<IDSWDataContext, DSWDataContext>();
            _unityContainer.RegisterType<ILogger, FakeLogger>();
            _unityContainer.RegisterType<ICurrentIdentity, Identity>();
            _unityContainer.RegisterType<ICQRSMessageMapper, CQRSMessageMapper>();
            _unityContainer.RegisterType<IServiceBusMessageMapper, ServiceBusMessageMapper>();
            _unityContainer.RegisterType<IMessageConfiguration, MessageConfiguration>(new HierarchicalLifetimeManager(),
                            new InjectionConstructor(@"C:\_w\WebAPI\VecompSoftware.DocSuite.Private.WebAPI\App_Data\ConfigurationFiles\MessageConfiguration.json"));
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Protocol"),
        TestCategory("CQRSCOmmand")]
        public async Task TestMethod_CQRS_HasValues()
        {
            IDataUnitOfWork unitOfWork = _unityContainer.Resolve<IDataUnitOfWork>();
            IDSWDataContext dataContext = _unityContainer.Resolve<IDSWDataContext>();
            ICQRSMessageMapper cqrsMapper = _unityContainer.Resolve<ICQRSMessageMapper>();
            IServiceBusMessageMapper mapper_to_brokered = _unityContainer.Resolve<IServiceBusMessageMapper>();
            Protocol protocol = unitOfWork.Repository<Protocol>().GetByUniqueIdWithRoleAndContact(Guid.Parse("5ED5F64E-FA8E-4658-B8A2-2C3C0E817C1F")).FirstOrDefault();
            IList<CategoryFascicle> categoryFascicles = protocol.Category.CategoryFascicles.Where(f => f.DSWEnvironment == (int)DSWEnvironmentType.Protocol || f.DSWEnvironment == 0).ToList();
            CategoryFascicle categoryFascicle = categoryFascicles.FirstOrDefault();
            IIdentityContext identity = new IdentityContext(protocol.LastChangedUser);
            CommandCreateProtocol command = new CommandCreateProtocol("", Guid.NewGuid(), protocol.TenantAOO.UniqueId, null, null, null, identity, protocol, categoryFascicle, null);
            ServiceBusMessage message = cqrsMapper.Map(command, new ServiceBusMessage());
            BrokeredMessage requestMessage = mapper_to_brokered.Map(message, new BrokeredMessage());
            string messageContent = requestMessage.GetBody<string>();
            CommandCreateProtocol ret = JsonConvert.DeserializeObject<CommandCreateProtocol>(messageContent, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });
            Assert.IsNotNull(ret);
            Assert.IsNotNull(ret.ContentType.ContentTypeValue.Container);
        }


    }
}
