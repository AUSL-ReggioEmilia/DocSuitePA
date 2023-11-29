using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.Practices.Unity.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using VecompSoftware.Commons.Interfaces.ServiceLocator;
using VecompSoftware.DocSuite.Document;
using VecompSoftware.DocSuite.Document.BiblosDS;
using VecompSoftware.DocSuite.Document.Generator.OpenXml.Word;
using VecompSoftware.DocSuite.Document.Generator.PDF;
using VecompSoftware.DocSuite.Interceptors.Behaviors.Loggers;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Configuration;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.EF;
using VecompSoftware.DocSuiteWeb.EnterpriseLogging;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.JeepServiceHosts;
using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Entity.Monitors;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.PosteWeb;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.JeepServiceHosts;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Monitors;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.PosteWeb;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Tasks;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Collaborations;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Commons;
using VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper.Model.JeepServiceHosts;
using VecompSoftware.DocSuiteWeb.Mapper.Model.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Messages;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Monitors;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Mapper.Model.PECMails;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Processes;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Securities;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper.Model.UDS;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Mapper.Workflow;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.JeepServiceHosts;
using VecompSoftware.DocSuiteWeb.Model.Entities.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Model.Entities.Monitors;
using VecompSoftware.DocSuiteWeb.Model.Entities.Parameter;
using VecompSoftware.DocSuiteWeb.Model.Entities.PECMails;
using VecompSoftware.DocSuiteWeb.Model.Entities.Processes;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders.Metadata;
using VecompSoftware.DocSuiteWeb.Model.Processes;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Security.Microsoft;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Service.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Service.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Service.Entity.JeepServiceHosts;
using VecompSoftware.DocSuiteWeb.Service.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Service.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Service.Entity.Monitors;
using VecompSoftware.DocSuiteWeb.Service.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Service.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Service.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Service.Entity.PosteWeb;
using VecompSoftware.DocSuiteWeb.Service.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Service.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Service.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Service.Entity.Tasks;
using VecompSoftware.DocSuiteWeb.Service.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Service.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Service.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Service.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Service.ServiceBus;
using VecompSoftware.DocSuiteWeb.Service.Workflow;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.Mappings;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.CQRS.Commands;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Conservations;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.JeepServiceHost;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Messages;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Monitors;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.OCharts;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Parameters;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.PECMails;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.PosteWeb;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Processes;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Resolutions;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tasks;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Templates;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenants;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows;
using VecompSoftware.DocSuiteWeb.Validation.Objects.CQRS.Commands;
using VecompSoftware.DocSuiteWeb.Validation.Objects.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Conservations;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.JeepServiceHosts;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Messages;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Monitors;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.OCharts;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Parameter;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PECMails;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PosteWeb;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Processes;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tasks;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenants;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Conservations;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Desks;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.JeepServiceHost;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Messages;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Monitors;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.OCharts;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Parameters;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.PECMails;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.PosteWeb;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Processes;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Resolutions;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Tasks;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Templates;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Tenants;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Workflows;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.ServiceBus;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;
using WorkflowManager = VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Workflows;
using VecompSoftware.DocSuiteWeb.Service.Entity.Tenders;
using VecompSoftware.DocSuiteWeb.Entity.Tenders;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Tenders;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenders;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenders;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Tenders;
using VecompSoftware.DocSuite.WebAPI.Common.Configurations;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Conservations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Conservations;
using VecompSoftware.DocSuite.Document.DocumentProxy;

namespace VecompSoftware.DocSuite.WebAPI.Common.Helpers
{
    public static class IocHelper
    {
        public static IDependencyResolver Initialize<TUnityConfig, TCurrentIdentity>(this IUnityContainer container,
            string UDSAssemblyFullName, string UDSAssemblyFileName, string serviceBusConnectionString,
            string messageConfigurationConfigPath, string customInstanceName, string passwordEncryptionKey,
            TimeSpan autoDeleteOnIdle, TimeSpan defaultMessageTimeToLive, TimeSpan lockDuration, int maxDeliveryCount)
            where TUnityConfig : ILocator
            where TCurrentIdentity : ICurrentIdentity
        {
            container.AddNewExtension<Interception>();
            IDependencyResolver resolver = new UnityHierarchicalDependencyResolver(container);

            EncryptionKey encryptionKeyInstance = new EncryptionKey { Value = WebApiConfiguration.PasswordEncryptionKey };
            container
                .RegisterInstance<IEncryptionKey>(encryptionKeyInstance, new ContainerControlledLifetimeManager())
                .RegisterType<ILogger, GlobalLogger>(new HierarchicalLifetimeManager())
                .RegisterType<ILocator, TUnityConfig>(new HierarchicalLifetimeManager())
                .RegisterType<IMapperUnitOfWork, MapperUnitOfWork>(new HierarchicalLifetimeManager())
                .RegisterType<IDSWDataContext, DSWDataContext>(new HierarchicalLifetimeManager(),
                        new InjectionProperty("UDSAssemblyFullName", UDSAssemblyFullName),
                        new InjectionProperty("UDSAssemblyFileName", UDSAssemblyFileName))
                .RegisterType<IDataUnitOfWork, DataUnitOfWork>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorService, ValidatorService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ValidationsLogging>())
                .RegisterType<IMessageConfiguration, MessageConfiguration>(new HierarchicalLifetimeManager(),
                            new InjectionConstructor(messageConfigurationConfigPath));

            container                                           
                .RegisterType<IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument>, DocumentBiblosDS>(new HierarchicalLifetimeManager());            

			ILogger logger = (ILogger)resolver.BeginScope().GetService(typeof(ILogger));

			container.RegisterType<IDocumentProxyContext>(new InjectionFactory((c) => null));
			if (!string.IsNullOrWhiteSpace(WebApiConfiguration.DocumentProxyGrpcEndpoint))
            {
				container.RegisterType<IDocumentProxyContext, DocumentProxyContext>(new HierarchicalLifetimeManager(),
						new InjectionConstructor(logger,
						WebApiConfiguration.DocumentProxyGrpcEndpoint,
						WebApiConfiguration.DocumentProxyIdentityAccount ?? string.Empty,
						WebApiConfiguration.DocumentProxyIdentityUniqueId.Value,
						WebApiConfiguration.DocumentProxyPEMCertificate ?? string.Empty));
			}

			container
                .RegisterType<IMetadataFilterFactory, MetadataFilterFactory>(new HierarchicalLifetimeManager());

            #region [ Parameters ]

            #region [ Services ]

            container
                .RegisterType<IEncryptedParameterEnvService, EncryptedParameterEnvService>(new HierarchicalLifetimeManager(),
                   new InjectionProperty("CustomInstanceName", customInstanceName),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>());

            container
                .RegisterType<IDecryptedParameterEnvService, DecryptedParameterEnvService>(new HierarchicalLifetimeManager(),
                   new InjectionProperty("CustomInstanceName", customInstanceName),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>());

            container
                .RegisterType<IParameterService, ParameterService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validation ]
            container
                .RegisterType<IParameterRuleset, ParameterRulesetDefinition>(new HierarchicalLifetimeManager());
            container
                .RegisterType<IValidator<Parameter>, ParameterValidator>(new HierarchicalLifetimeManager());
            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]

            container
                .RegisterType<IParameterEnvMapper, ParameterEnvMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<ParameterEnv, ParameterEnv>, ParameterEnvMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IParameterShortModelMapper, ParameterShortModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IMapper<string, short>, ParameterShortModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IParameterIntModelMapper, ParameterIntModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IMapper<string, int>, ParameterIntModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IParameterBoolModelMapper, ParameterBoolModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IMapper<string, bool>, ParameterBoolModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenantModelMapper, TenantModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IMapper<string, List<DocSuiteWeb.Model.Parameters.TenantModel>>, TenantModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IParameterGuidModelMapper, ParameterGuidModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IMapper<string, Guid>, ParameterGuidModelMapper>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IParameterMapper, ParameterMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<Parameter, Parameter>, ParameterMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IParameterTableValuedModelMapper, ParameterTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<ParameterTableValuedModel, ParameterModel>, ParameterTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IParameterModelMapper, ParameterModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<Parameter, ParameterModel>, ParameterModelMapper>(new HierarchicalLifetimeManager());
            #endregion

            #region [Validation]
            container
                .RegisterType<IParameterValidatorMapper, ParameterValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<Parameter, ParameterValidator>, ParameterValidatorMapper>(new HierarchicalLifetimeManager());
            #endregion

            #endregion


            #endregion

            #region [ Security ]

            #region [ Services ]

            container
                .RegisterType<ISecurity, ActiveDirectory>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<ICurrentIdentity, TCurrentIdentity>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            #endregion

            #region [ Mappers ]

            container
                .RegisterType<IDomainGroupModelMapper, DomainGroupModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainUserModelCollaborationUserModelMapper, DomainUserModelCollaborationUserModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainUserModelCollaborationSignModelMapper, DomainUserModelCollaborationSignModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainUserModelMessageContactEmailModelMapper, DomainUserModelMessageContactEmailModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainUserModelMessageContactModelMapper, DomainUserModelMessageContactModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainUserModelMapper, DomainUserModelMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #region [ Service Bus ]

            #region [ Services ]

            container
                .RegisterType<IQueueService, QueueService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<ITopicService, TopicService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IServiceBusQueueContext, ServiceBusQueueContext>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IServiceBusTopicContext, ServiceBusTopicContext>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IServiceBusConfiguration, ServiceBusConfiguration>(new HierarchicalLifetimeManager(),
                        new InjectionConstructor(serviceBusConnectionString),
                        new InjectionProperty("AutoDeleteOnIdle", autoDeleteOnIdle),
                        new InjectionProperty("DefaultMessageTimeToLive", defaultMessageTimeToLive),
                        new InjectionProperty("LockDuration", lockDuration),
                        new InjectionProperty("MaxDeliveryCount", maxDeliveryCount),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                .RegisterType<ITopicRuleset, TopicRuleset>(new HierarchicalLifetimeManager())
                .RegisterType<IQueueRuleset, QueueRuleset>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<IEvent>, EventValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ICommand>, CommandValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IEventValidator, EventValidator>(new HierarchicalLifetimeManager())
                .RegisterType<ICommandValidator, CommandValidator>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Mappers ]

            container
                .RegisterType<IServiceBusMessageMapper, ServiceBusMessageMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ICQRSMessageMapper, CQRSMessageMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IBrokeredMessageMapper, BrokeredMessageMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IServiceTopicMessageMapper, ServiceTopicMessageMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IEventValidatorMapper, EventValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ICommandValidatorMapper, CommandValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #region [ Entities ] 

            #region [ Commons ]

            #region [ Services ]
            container
                .RegisterType<ICategoryService, CategoryService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<ICategoryFascicleService, CategoryFascicleService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<ICategoryFascicleRightService, CategoryFascicleRightService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IContactService, ContactService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IContainerService, ContainerService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IContainerGroupService, ContainerGroupService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IContainerPropertyService, ContainerPropertyService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IRoleService, RoleService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IRoleGroupService, RoleGroupService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IRoleUserService, RoleUserService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<ILocationService, LocationService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<ICategorySchemaService, CategorySchemaService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IIncrementalService, IncrementalService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<ITableLogService, TableLogService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IMetadataRepositoryService, MetadataRepositoryService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                   .RegisterType<IPrivacyLevelService, PrivacyLevelService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IUserLogService, UserLogService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IContactTitleService, ContactTitleService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IContactPlaceNameService, ContactPlaceNameService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IMetadataValueService, MetadataValueService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IMetadataContactValueService, MetadataContactValueService>(new HierarchicalLifetimeManager(),
                   new Interceptor<InterfaceInterceptor>(),
                   new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]
            container
                .RegisterType<ICategoryRuleset, CategoryRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<ICategoryFascicleRuleset, CategoryFascicleRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<ICategoryFascicleRightRuleset, CategoryFascicleRightRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<IContactRuleset, ContactRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<IContainerRuleset, ContainerRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<IRoleRuleset, RoleRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<ILocationRuleset, LocationRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<IIncrementalRuleset, IncrementalRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<ITableLogRuleset, TableLogRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<IRoleUserRuleset, RoleUserRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<IMetadataRepositoryRuleset, MetadataRepositoryRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<IPrivacyLevelRuleset, PrivacyLevelRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<IUserLogRuleset, UserLogRulesetDefinition>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<Category>, CategoryValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<CategoryFascicle>, CategoryFascicleValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<CategoryFascicleRight>, CategoryFascicleRightValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<Container>, ContainerValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ContainerGroup>, ContainerGroupValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ContainerProperty>, ContainerPropertyValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<Contact>, ContactValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ContactTitle>, ContactTitleValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ContactPlaceName>, ContactPlaceNameValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<Location>, LocationValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<Role>, RoleValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<RoleGroup>, RoleGroupValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<RoleUser>, RoleUserValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<SecurityGroup>, SecurityGroupValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<SecurityUser>, SecurityUserValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<CategorySchema>, CategorySchemaValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<Incremental>, IncrementalValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<TableLog>, TableLogValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<MetadataRepository>, MetadataRepositoryValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<PrivacyLevel>, PrivacyLevelValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<UserLog>, UserLogValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<MetadataValue>, MetadataValueValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<MetadataValueContact>, MetadataValueContactValidator>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]
            container
                    .RegisterType<ILocationMapper, LocationMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<Location, Location>, LocationMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ILocationModelMapper, LocationModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<Location, LocationModel>, LocationModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ICategoryMapper, CategoryMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<Category, Category>, CategoryMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ICategoryTableValuedModelMapper, CategoryTableValuedModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<ICategoryTableValuedModel, CategoryModel>, CategoryTableValuedModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ICategoryModelMapper, CategoryModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<Category, CategoryModel>, CategoryModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ICategorySchemaMapper, CategorySchemaMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<CategorySchema, CategorySchema>, CategorySchemaMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContainerModelMapper, ContainerModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<Container, ContainerModel>, ContainerModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContainerTableValuedModelMapper, ContainerTableValuedModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<IContainerTableValuedModel, ContainerModel>, ContainerTableValuedModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContainerTableValuedEntityMapper, ContainerTableValuedEntityMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<IContainerTableValuedModel, Container>, ContainerTableValuedEntityMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ICategoryFascicleMapper, CategoryFascicleMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<CategoryFascicle, CategoryFascicle>, CategoryFascicleMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ICategoryFascicleRightMapper, CategoryFascicleRightMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<CategoryFascicleRight, CategoryFascicleRight>, CategoryFascicleRightMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContainerMapper, ContainerMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<Container, Container>, ContainerMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContainerGroupMapper, ContainerGroupMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<ContainerGroup, ContainerGroup>, ContainerGroupMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContactMapper, ContactMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<Contact, Contact>, ContactMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IRoleMapper, RoleMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<Role, Role>, RoleMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IRoleModelMapper, RoleModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<Role, RoleModel>, RoleModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IRoleGroupMapper, RoleGroupMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<RoleGroup, RoleGroup>, RoleGroupMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IRoleUserMapper, RoleUserMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<RoleUser, RoleUser>, RoleUserMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContactTitleMapper, ContactTitleMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<ContactTitle, ContactTitle>, ContactTitleMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContactPlaceNameMapper, ContactPlaceNameMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<ContactPlaceName, ContactPlaceName>, ContactPlaceNameMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IIncrementalMapper, IncrementalMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<Incremental, Incremental>, IncrementalMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IRoleUserWorkflowAuthorizationMapper, RoleUserWorkflowAuthorizationMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<RoleUser, WorkflowAuthorization>, RoleUserWorkflowAuthorizationMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ISecurityUserWorkflowAuthorizationMapper, SecurityUserWorkflowAuthorizationMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<SecurityUser, WorkflowAuthorization>, SecurityUserWorkflowAuthorizationMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ITableLogMapper, TableLogMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<TableLog, TableLog>, TableLogMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IMetadataRepositoryMapper, MetadataRepositoryMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<MetadataRepository, MetadataRepository>, MetadataRepositoryMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContactTableValuedModelMapper, ContactTableValuedModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<IContactTableValuedModel, ContactModel>, ContactTableValuedModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IRoleTableValuedModelMapper, RoleTableValuedModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<IRoleTableValuedModel, RoleModel>, RoleTableValuedModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IRoleFullTableValuedModelMapper, RoleFullTableValuedModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<RoleFullTableValuedModel, RoleModel>, RoleFullTableValuedModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContactModelMapper, ContactModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<Contact, ContactModel>, ContactModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IPrivacyLevelMapper, PrivacyLevelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<PrivacyLevel, PrivacyLevel>, PrivacyLevelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContainerPropertyMapper, ContainerPropertyMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<ContainerProperty, ContainerProperty>, ContainerPropertyMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ICategoryFullTableValuedModelMapper, CategoryFullTableValuedModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<CategoryFullTableValuedModel, CategoryModel>, CategoryFullTableValuedModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IUserLogMapper, UserLogMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<UserLog, UserLog>, UserLogMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContactFullTableValuedModelMapper, ContactFullTableValuedModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<ContactTableValuedModel, ContactModel>, ContactFullTableValuedModelMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IMetadataValueMapper, MetadataValueMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<MetadataValue, MetadataValue>, MetadataValueMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IMetadataContactValueMapper, MetadataContactValueMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IDomainMapper<MetadataValueContact, MetadataValueContact>, MetadataContactValueMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]
            container
                    .RegisterType<IContactValidatorMapper, ContactValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<Contact, ContactValidator>, ContactValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContactTitleValidatorMapper, ContactTitleValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<ContactTitle, ContactTitleValidator>, ContactTitleValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContactPlaceNameValidatorMapper, ContactPlaceNameValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<ContactPlaceName, ContactPlaceNameValidator>, ContactPlaceNameValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ICategoryValidatorMapper, CategoryValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<Category, CategoryValidator>, CategoryValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ICategoryFascicleValidatorMapper, CategoryFascicleValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<CategoryFascicle, CategoryFascicleValidator>, CategoryFascicleValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ICategoryFascicleRightValidatorMapper, CategoryFascicleRightValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<CategoryFascicleRight, CategoryFascicleRightValidator>, CategoryFascicleRightValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContainerValidatorMapper, ContainerValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<Container, ContainerValidator>, ContainerValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ILocationValidatorMapper, LocationValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<Location, LocationValidator>, LocationValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IRoleValidatorMapper, RoleValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<Role, RoleValidator>, RoleValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ISecurityGroupValidatorMapper, SecurityGroupValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<SecurityGroup, SecurityGroupValidator>, SecurityGroupValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ISecurityUserValidatorMapper, SecurityUserValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<SecurityUser, SecurityUserValidator>, SecurityUserValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ICategorySchemaValidatorMapper, CategorySchemaValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<CategorySchema, CategorySchemaValidator>, CategorySchemaValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IIncrementalValidatorMapper, IncrementalValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<Incremental, IncrementalValidator>, IncrementalValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<ITableLogValidatorMapper, TableLogValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<TableLog, TableLogValidator>, TableLogValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IMetadataRepositoryValidatorMapper, MetadataRepositoryValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<MetadataRepository, MetadataRepositoryValidator>, MetadataRepositoryValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IRoleUserValidatorMapper, RoleUserValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<RoleUser, RoleUserValidator>, RoleUserValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IPrivacyLevelValidatorMapper, PrivacyLevelValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<PrivacyLevel, PrivacyLevelValidator>, PrivacyLevelValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IContainerPropertyValidatorMapper, ContainerPropertyValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<ContainerProperty, ContainerPropertyValidator>, ContainerPropertyValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IUserLogValidatorMapper, UserLogValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<UserLog, UserLogValidator>, UserLogValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IMetadataValueValidatorMapper, MetadataValueValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<MetadataValue, MetadataValueValidator>, MetadataValueValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IMetadataContactValueValidatorMapper, MetadataContactValueValidatorMapper>(new HierarchicalLifetimeManager())
                    .RegisterType<IValidatorMapper<MetadataValueContact, MetadataValueContactValidator>, MetadataContactValueValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #region [ Desk ]

            #region [ Services ]

            container
                .RegisterType<IDeskService, DeskService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IDeskDocumentService, DeskDocumentService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                .RegisterType<IDeskRuleset, DeskRulesetDefinition>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<DeskCollaboration>, DeskCollaborationValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DeskDocumentEndorsement>, DeskDocumentEndorsementValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DeskDocument>, DeskDocumentValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DeskDocumentVersion>, DeskDocumentVersionValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DeskLog>, DeskLogValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DeskMessage>, DeskMessageValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DeskRoleUser>, DeskRoleUserValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DeskStoryBoard>, DeskStoryBoardValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<Desk>, DeskValidator>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]

            container
                .RegisterType<IDeskCollaborationMapper, DeskCollaborationMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DeskCollaboration, DeskCollaboration>, DeskCollaborationMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDeskDocumentMapper, DeskDocumentMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DeskDocument, DeskDocument>, DeskDocumentMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDeskMapper, DeskMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<Desk, Desk>, DeskMapper>(new HierarchicalLifetimeManager());
            #endregion

            #region [ Validator ]

            container
                .RegisterType<IDeskCollaborationValidatorMapper, DeskCollaborationValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DeskCollaboration, DeskCollaborationValidator>, DeskCollaborationValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDeskDocumentEndorsementValidatorMapper, DeskDocumentEndorsementValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DeskDocumentEndorsement, DeskDocumentEndorsementValidator>, DeskDocumentEndorsementValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDeskDocumentValidatorMapper, DeskDocumentValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DeskDocument, DeskDocumentValidator>, DeskDocumentValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDeskDocumentVersionValidatorMapper, DeskDocumentVersionValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DeskDocumentVersion, DeskDocumentVersionValidator>, DeskDocumentVersionValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDeskLogValidatorMapper, DeskLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DeskLog, DeskLogValidator>, DeskLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDeskMessageValidatorMapper, DeskMessageValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DeskMessage, DeskMessageValidator>, DeskMessageValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDeskRoleUserValidatorMapper, DeskRoleUserValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DeskRoleUser, DeskRoleUserValidator>, DeskRoleUserValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDeskStoryBoardValidatorMapper, DeskStoryBoardValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DeskStoryBoard, DeskStoryBoardValidator>, DeskStoryBoardValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDeskValidatorMapper, DeskValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<Desk, DeskValidator>, DeskValidatorMapper>(new HierarchicalLifetimeManager());
            #endregion

            #endregion

            #endregion

            #region [ DocumentArchives ]

            #region [ Services ]

            container
                .RegisterType<IDocumentSeriesService, DocumentSeriesService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IDocumentSeriesItemService, DocumentSeriesItemService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IDocumentSeriesItemRoleService, DocumentSeriesItemRoleService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IDocumentSeriesItemLogService, DocumentSeriesItemLogService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IDocumentSeriesConstraintService, DocumentSeriesConstraintService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IDocumentSeriesItemLinkService, DocumentSeriesItemLinkService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                .RegisterType<IDocumentSeriesRuleset, DocumentSeriesRulesetDefinition>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<DocumentSeries>, DocumentSeriesValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DocumentSeriesItemRole>, DocumentSeriesItemRoleValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DocumentSeriesItem>, DocumentSeriesItemValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DocumentSeriesItemLog>, DocumentSeriesItemLogValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DocumentSeriesConstraint>, DocumentSeriesConstraintValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DocumentSeriesItemLink>, DocumentSeriesItemLinkValidator>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]

            container
                .RegisterType<IDocumentSeriesMapper, DocumentSeriesMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentSeries, DocumentSeries>, DocumentSeriesMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentSeriesItemMapper, DocumentSeriesItemMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentSeriesItem, DocumentSeriesItem>, DocumentSeriesItemMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentSeriesItemLinkMapper, DocumentSeriesItemLinkMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentSeriesItemLink, DocumentSeriesItemLink>, DocumentSeriesItemLinkMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentSeriesItemModelMapper, DocumentSeriesItemModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentSeriesItem, DocumentSeriesItemModel>, DocumentSeriesItemModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentSeriesItemRoleMapper, DocumentSeriesItemRoleMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentSeriesItemRole, DocumentSeriesItemRole>, DocumentSeriesItemRoleMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentSeriesItemLogMapper, DocumentSeriesItemLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentSeriesItemLog, DocumentSeriesItemLog>, DocumentSeriesItemLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentSeriesConstraintMapper, DocumentSeriesConstraintMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentSeriesConstraint, DocumentSeriesConstraint>, DocumentSeriesConstraintMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IMonitoringSeriesSectionTableValuedModelMapper, MonitoringSeriesSectionTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<MonitoringSeriesSectionTableValuedModel, MonitoringSeriesSectionModel>, MonitoringSeriesSectionTableValuedModelMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]
            container
                .RegisterType<IDocumentSeriesValidatorMapper, DocumentSeriesValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DocumentSeries, DocumentSeriesValidator>, DocumentSeriesValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentSeriesItemValidatorMapper, DocumentSeriesItemValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DocumentSeriesItem, DocumentSeriesItemValidator>, DocumentSeriesItemValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentSeriesItemLinkValidatorMapper, DocumentSeriesItemLinkValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DocumentSeriesItemLink, DocumentSeriesItemLinkValidator>, DocumentSeriesItemLinkValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentSeriesItemRoleValidatorMapper, DocumentSeriesItemRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DocumentSeriesItemRole, DocumentSeriesItemRoleValidator>, DocumentSeriesItemRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentSeriesItemLogValidatorMapper, DocumentSeriesItemLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DocumentSeriesItemLog, DocumentSeriesItemLogValidator>, DocumentSeriesItemLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentSeriesConstraintValidatorMapper, DocumentSeriesConstraintValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DocumentSeriesConstraint, DocumentSeriesConstraintValidator>, DocumentSeriesConstraintValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #region [ DocumentUnits ]

            #region [ Services ]

            container
                .RegisterType<IDocumentUnitService, DocumentUnitService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IDocumentUnitRoleService, DocumentUnitRoleService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IDocumentUnitChainService, DocumentUnitChainService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IDocumentUnitContactService, DocumentUnitContactService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IDocumentUnitUserService, DocumentUnitUserService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IDocumentUnitFascicleHistoricizedCategoryService, DocumentUnitFascicleHistoricizedCategoryService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IDocumentUnitFascicleCategoryService, DocumentUnitFascicleCategoryService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                .RegisterType<IDocumentUnitRuleset, DocumentUnitRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitFascicleHistoricizedCategoryRuleset, DocumentUnitFascicleHistoricizedCategoryRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitFascicleCategoryRuleset, DocumentUnitFascicleCategoryRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DocumentUnit>, DocumentUnitValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DocumentUnitRole>, DocumentUnitRoleValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DocumentUnitChain>, DocumentUnitChainValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DocumentUnitContact>, DocumentUnitContactValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DocumentUnitFascicleHistoricizedCategory>, DocumentUnitFascicleHistoricizedCategoryValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DocumentUnitFascicleCategory>, DocumentUnitFascicleCategoryValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DocumentUnitUser>, DocumentUnitUserValidator>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]

            container
                .RegisterType<IDocumentUnitMapper, DocumentUnitMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnit, DocumentUnit>, DocumentUnitMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitRoleMapper, DocumentUnitRoleMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnitRole, DocumentUnitRole>, DocumentUnitRoleMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitChainMapper, DocumentUnitChainMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnitChain, DocumentUnitChain>, DocumentUnitChainMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitContactMapper, DocumentUnitContactMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnitContact, DocumentUnitContact>, DocumentUnitContactMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitChainTableValuedModelMapper, DocumentUnitChainTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnitTableValuedModel, DocumentUnitChainModel>, DocumentUnitChainTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitTableValuedModelMapper, DocumentUnitTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnitTableValuedModel, DocumentUnitModel>, DocumentUnitTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitUDSCommandModelMapper, DocumentUnitUDSCommandModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnit, UDSBuildModel>, DocumentUnitUDSCommandModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitRoleModelMapper, DocumentUnitRoleModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnitRole, RoleModel>, DocumentUnitRoleModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitChainModelMapper, DocumentUnitChainModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnitChain, UDSDocumentModel>, DocumentUnitChainModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitContactModelMapper, DocumentUnitContactModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnitContact, DocumentUnitContactModel>, DocumentUnitContactModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitUserMapper, DocumentUnitUserMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnitUser, DocumentUnitUser>, DocumentUnitUserMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnitUser, UserModel>, DocumentUnitUserModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitModelMapper, DocumentUnitModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnit, DocumentUnitModel>, DocumentUnitModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitFascicleHistoricizedCategoryMapper, DocumentUnitFascicleHistoricizedCategoryMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnitFascicleHistoricizedCategory, DocumentUnitFascicleHistoricizedCategory>, DocumentUnitFascicleHistoricizedCategoryMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitFascicleCategoryMapper, DocumentUnitFascicleCategoryMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnitFascicleCategory, DocumentUnitFascicleCategory>, DocumentUnitFascicleCategoryMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitCollaborationTableValueModelMapper, DocumentUnitCollaborationTableValueModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<CollaborationTableValuedModel, DocumentUnitModel>, DocumentUnitCollaborationTableValueModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DocumentUnitContact, UDSContactModel>, UDSContactModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSContactModelMapper, UDSContactModelMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                .RegisterType<IDocumentUnitValidatorMapper, DocumentUnitValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DocumentUnit, DocumentUnitValidator>, DocumentUnitValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitRoleValidatorMapper, DocumentUnitRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DocumentUnitRole, DocumentUnitRoleValidator>, DocumentUnitRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitChainValidatorMapper, DocumentUnitChainValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DocumentUnitChain, DocumentUnitChainValidator>, DocumentUnitChainValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitContactValidatorMapper, DocumentUnitContactValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DocumentUnitContact, DocumentUnitContactValidator>, DocumentUnitContactValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitUserValidatorMapper, DocumentUnitUserValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DocumentUnitUser, DocumentUnitUserValidator>, DocumentUnitUserValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DocumentUnitFascicleHistoricizedCategory, DocumentUnitFascicleHistoricizedCategoryValidator>, DocumentUnitFascicleHistoricizedCategoryValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitFascicleHistoricizedCategoryValidatorMapper, DocumentUnitFascicleHistoricizedCategoryValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DocumentUnitFascicleCategory, DocumentUnitFascicleCategoryValidator>, DocumentUnitFascicleCategoryValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDocumentUnitFascicleCategoryValidatorMapper, DocumentUnitFascicleCategoryValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #region [ Templates ]

            #region [ Services ]

            container
               .RegisterType<ITemplateCollaborationService, TemplateCollaborationService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
               .RegisterType<ITemplateCollaborationUserService, TemplateCollaborationUserService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
               .RegisterType<ITemplateCollaborationDocumentRepositoryService, TemplateCollaborationDocumentRepositoryService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
               .RegisterType<ITemplateDocumentRepositoryService, TemplateDocumentRepositoryService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
               .RegisterType<ITemplateReportService, TemplateReportService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>());
            #endregion

            #region [ Validations ]
            container
                .RegisterType<ITemplateDocumentRepositoryRuleset, TemplateDocumentRepositoryRuleset>(new HierarchicalLifetimeManager())
                .RegisterType<ITemplateCollaborationRuleset, TemplateCollaborationRuleset>(new HierarchicalLifetimeManager());
            container
                .RegisterType<IValidator<TemplateCollaboration>, TemplateCollaborationValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<TemplateCollaborationUser>, TemplateCollaborationUserValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<TemplateCollaborationDocumentRepository>, TemplateCollaborationDocumentRepositoryValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<TemplateDocumentRepository>, TemplateDocumentRepositoryValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<TemplateReport>, TemplateReportValidator>(new HierarchicalLifetimeManager());
            #endregion

            #region [ Mappers ] 

            #region [ Entities & Models ]

            container
                 .RegisterType<ITemplateCollaborationMapper, TemplateCollaborationMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<TemplateCollaboration, TemplateCollaboration>, TemplateCollaborationMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ITemplateCollaborationUserMapper, TemplateCollaborationUserMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<TemplateCollaborationUser, TemplateCollaborationUser>, TemplateCollaborationUserMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ITemplateCollaborationDocumentRepositoryMapper, TemplateCollaborationDocumentRepositoryMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<TemplateCollaborationDocumentRepository, TemplateCollaborationDocumentRepository>, TemplateCollaborationDocumentRepositoryMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ITemplateDocumentRepositoryMapper, TemplateDocumentRepositoryMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<TemplateDocumentRepository, TemplateDocumentRepository>, TemplateDocumentRepositoryMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ITemplateReportMapper, TemplateReportMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<TemplateReport, TemplateReport>, TemplateReportMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                .RegisterType<ITemplateCollaborationValidatorMapper, TemplateCollaborationValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<TemplateCollaboration, TemplateCollaborationValidator>, TemplateCollaborationValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ITemplateCollaborationUserValidatorMapper, TemplateCollaborationUserValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<TemplateCollaborationUser, TemplateCollaborationUserValidator>, TemplateCollaborationUserValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ITemplateCollaborationDocumentRepositoryValidatorMapper, TemplateCollaborationDocumentRepositoryValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<TemplateCollaborationDocumentRepository, TemplateCollaborationDocumentRepositoryValidator>, TemplateCollaborationDocumentRepositoryValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ITemplateDocumentRepositoryValidatorMapper, TemplateDocumentRepositoryValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<TemplateDocumentRepository, TemplateDocumentRepositoryValidator>, TemplateDocumentRepositoryValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ITemplateReportValidatorMapper, TemplateReportValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<TemplateReport, TemplateReportValidator>, TemplateReportValidatorMapper>(new HierarchicalLifetimeManager());
            #endregion

            #endregion

            #endregion

            #region [ Collaboration ]

            #region [ Services ]

            container
                .RegisterType<ICollaborationService, CollaborationService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<ICollaborationLogService, CollaborationLogService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
               .RegisterType<ICollaborationVersioningService, CollaborationVersioningService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
               .RegisterType<ICollaborationSignService, CollaborationSignService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
               .RegisterType<ICollaborationUserService, CollaborationUserService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
               .RegisterType<ICollaborationAggregateService, CollaborationAggregateService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
               .RegisterType<ICollaborationDraftService, CollaborationDraftService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                  .RegisterType<ICollaborationRuleset, CollaborationRuleset>(new HierarchicalLifetimeManager());

            container
                  .RegisterType<IValidator<CollaborationLog>, CollaborationLogValidator>(new HierarchicalLifetimeManager())
                  .RegisterType<IValidator<Collaboration>, CollaborationValidator>(new HierarchicalLifetimeManager())
                  .RegisterType<IValidator<CollaborationAggregate>, CollaborationAggregateValidator>(new HierarchicalLifetimeManager())
                  .RegisterType<IValidator<CollaborationUser>, CollaborationUserValidator>(new HierarchicalLifetimeManager())
                  .RegisterType<IValidator<CollaborationSign>, CollaborationSignValidator>(new HierarchicalLifetimeManager())
                  .RegisterType<IValidator<CollaborationVersioning>, CollaborationVersioningValidator>(new HierarchicalLifetimeManager());


            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]

            container
                 .RegisterType<ICollaborationDraftMapper, CollaborationDraftMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<CollaborationDraft, CollaborationDraft>, CollaborationDraftMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationLogMapper, CollaborationLogMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<CollaborationLog, CollaborationLog>, CollaborationLogMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationMapper, CollaborationMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<Collaboration, Collaboration>, CollaborationMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationAggregateMapper, CollaborationAggregateMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<CollaborationAggregate, CollaborationAggregate>, CollaborationAggregateMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationVersioningMapper, CollaborationVersioningMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<CollaborationVersioning, CollaborationVersioning>, CollaborationVersioningMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationSignMapper, CollaborationSignMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<CollaborationSign, CollaborationSign>, CollaborationSignMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationUserMapper, CollaborationUserMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<CollaborationUser, CollaborationUser>, CollaborationUserMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationModelMapper, CollaborationModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<Collaboration, CollaborationModel>, CollaborationModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationVersioningModelMapper, CollaborationVersioningModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<CollaborationVersioning, CollaborationVersioningModel>, CollaborationVersioningModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationVersioningTableValueModelMapper, CollaborationVersioningTableValueModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<CollaborationTableValuedModel, CollaborationVersioningModel>, CollaborationVersioningTableValueModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationUserModelMapper, CollaborationUserModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<CollaborationUser, CollaborationUserModel>, CollaborationUserModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationUserTableValueModelMapper, CollaborationUserTableValueModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<CollaborationTableValuedModel, CollaborationUserModel>, CollaborationUserTableValueModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationSignModelMapper, CollaborationSignModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<CollaborationSign, CollaborationSignModel>, CollaborationSignModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationSignTableValueModelMapper, CollaborationSignTableValueModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<CollaborationTableValuedModel, CollaborationSignModel>, CollaborationSignTableValueModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationTableValuedModelMapper, CollaborationTableValuedModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<CollaborationTableValuedModel, CollaborationModel>, CollaborationTableValuedModelMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                 .RegisterType<IValidator<CollaborationDraft>, CollaborationDraftValidator>(new HierarchicalLifetimeManager())
                .RegisterType<ICollaborationDraftValidatorMapper, CollaborationDraftValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationLogValidatorMapper, CollaborationLogValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<CollaborationLog, CollaborationLogValidator>, CollaborationLogValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationValidatorMapper, CollaborationValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<Collaboration, CollaborationValidator>, CollaborationValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationAggregateValidatorMapper, CollaborationAggregateValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<CollaborationAggregate, CollaborationAggregateValidator>, CollaborationAggregateValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationSignValidatorMapper, CollaborationSignValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<CollaborationSign, CollaborationSignValidator>, CollaborationSignValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationUserValidatorMapper, CollaborationUserValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<CollaborationUser, CollaborationUserValidator>, CollaborationUserValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<ICollaborationVersioningValidatorMapper, CollaborationVersioningValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<CollaborationVersioning, CollaborationVersioningValidator>, CollaborationVersioningValidatorMapper>(new HierarchicalLifetimeManager());
            #endregion

            #endregion

            #endregion

            #region [ Fascicle ]

            #region [ Services ]

            container
                .RegisterType<IFascicleService, FascicleService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IFascicleLogService, FascicleLogService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IFascicleLinkService, FascicleLinkService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IFascicleDocumentService, FascicleDocumentService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                 .RegisterType<IFascicleRoleService, FascicleRoleService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                 .RegisterType<IFascicleFolderService, FascicleFolderService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                  .RegisterType<IFascicleDocumentUnitService, FascicleDocumentUnitService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                  .RegisterType<IFascicleRuleset, FascicleRuleset>(new HierarchicalLifetimeManager());

            container
                  .RegisterType<IValidator<Fascicle>, FascicleValidator>(new HierarchicalLifetimeManager())
                  .RegisterType<IValidator<FasciclePeriod>, FasciclePeriodValidator>(new HierarchicalLifetimeManager())
                  .RegisterType<IValidator<FascicleLog>, FascicleLogValidator>(new HierarchicalLifetimeManager())
                  .RegisterType<IValidator<FascicleLink>, FascicleLinkValidator>(new HierarchicalLifetimeManager())
                  .RegisterType<IValidator<FascicleDocument>, FascicleDocumentValidator>(new HierarchicalLifetimeManager())
                  .RegisterType<IValidator<FascicleRole>, FascicleRoleValidator>(new HierarchicalLifetimeManager())
                  .RegisterType<IValidator<FascicleFolder>, FascicleFolderValidator>(new HierarchicalLifetimeManager())
                  .RegisterType<IValidator<FascicleDocumentUnit>, FascicleDocumentUnitValidator>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]

            container
                 .RegisterType<IFascicleMapper, FascicleMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<Fascicle, Fascicle>, FascicleMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IFasciclePeriodMapper, FasciclePeriodMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<FasciclePeriod, FasciclePeriod>, FasciclePeriodMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IFascicleLogMapper, FascicleLogMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<FascicleLog, FascicleLog>, FascicleLogMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IFascicleModelMapper, FascicleModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<Fascicle, FascicleModel>, FascicleModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IFascicleTableValuedModelMapper, FascicleTableValuedModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<FascicleTableValuedModel, FascicleModel>, FascicleTableValuedModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDocumentUnitTableValuedModelMapper, DocumentUnitTableValuedModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<DocumentUnitTableValuedModel, DocumentUnitModel>, DocumentUnitTableValuedModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IFascicleDocumentUnitTableValuedModelMapper, FascicleDocumentUnitTableValuedModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<FascicleDocumentUnitTableValuedModel, FascicleDocumentUnitModel>, FascicleDocumentUnitTableValuedModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IFascicleLinkMapper, FascicleLinkMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<FascicleLink, FascicleLink>, FascicleLinkMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IFascicleDocumentMapper, FascicleDocumentMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<FascicleDocument, FascicleDocument>, FascicleDocumentMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IFascicleDocumentModelMapper, FascicleDocumentModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<FascicleDocument, FascicleDocumentModel>, FascicleDocumentModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IFascicleRoleMapper, FascicleRoleMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<FascicleRole, FascicleRole>, FascicleRoleMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IFascicleFolderMapper, FascicleFolderMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<FascicleFolder, FascicleFolder>, FascicleFolderMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IFascicleFolderModelMapper, FascicleFolderModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<FascicleFolder, FascicleFolderModel>, FascicleFolderModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IFascicleFolderTableValuedModelMapper, FascicleFolderTableValuedModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<FascicleFolderTableValuedModel, FascicleFolderModel>, FascicleFolderTableValuedModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IViewableFascicleTableValuedModelMapper, ViewableFascicleTableValuedModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<ViewableFascicleTableValuedModel, FascicleModel>, ViewableFascicleTableValuedModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IFascicleDocumentUnitMapper, FascicleDocumentUnitMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<FascicleDocumentUnit, FascicleDocumentUnit>, FascicleDocumentUnitMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IFascicleDocumentUnitModelMapper, FascicleDocumentUnitModelMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IDomainMapper<FascicleDocumentUnit, FascicleDocumentUnitModel>, FascicleDocumentUnitModelMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                .RegisterType<IFascicleValidatorMapper, FascicleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<Fascicle, FascicleValidator>, FascicleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IFasciclePeriodValidatorMapper, FasciclePeriodValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<FasciclePeriod, FasciclePeriodValidator>, FasciclePeriodValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IFascicleLogValidatorMapper, FascicleLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<FascicleLog, FascicleLogValidator>, FascicleLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IFascicleLinkValidatorMapper, FascicleLinkValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<FascicleLink, FascicleLinkValidator>, FascicleLinkValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IFascicleDocumentValidatorMapper, FascicleDocumentValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<FascicleDocument, FascicleDocumentValidator>, FascicleDocumentValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IFascicleRoleValidatorMapper, FascicleRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<FascicleRole, FascicleRoleValidator>, FascicleRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IFascicleFolderValidatorMapper, FascicleFolderValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<FascicleFolder, FascicleFolderValidator>, FascicleFolderValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IFascicleDocumentUnitValidatorMapper, FascicleDocumentUnitValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<FascicleDocumentUnit, FascicleDocumentUnitValidator>>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #region [ Message ]

            #region [ Services ]

            container
                .RegisterType<IMessageService, MessageService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IMessageEmailService, MessageEmailService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IMessageContactService, MessageContactService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IMessageAttachmentService, MessageAttachmentService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IMessageContactEmailService, MessageContactEmailService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IMessageLogService, MessageLogService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                .RegisterType<IMessageRuleset, MessageRulesetDefinition>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<MessageAttachment>, MessageAttachmentValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<MessageContactEmail>, MessageContactEmailValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<MessageContact>, MessageContactValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<MessageEmail>, MessageEmailValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<MessageLog>, MessageLogValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<Message>, MessageValidator>(new HierarchicalLifetimeManager());


            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]

            container
                .RegisterType<IMessageMapper, MessageMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IMessageModelMapper, MessageModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IMessageAttachmentModelMapper, MessageAttachmentModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IMessageContactEmailModelMapper, MessageContactEmailModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IMessageContactModelMapper, MessageContactModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IMessageEmailModelMapper, MessageEmailModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<Message, Message>, MessageMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                 .RegisterType<IMessageAttachmentValidatorMapper, MessageAttachmentValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<MessageAttachment, MessageAttachmentValidator>, MessageAttachmentValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IMessageContactEmailValidatorMapper, MessageContactEmailValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<MessageContactEmail, MessageContactEmailValidator>, MessageContactEmailValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IMessageContactValidatorMapper, MessageContactValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<MessageContact, MessageContactValidator>, MessageContactValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IMessageEmailValidatorMapper, MessageEmailValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<MessageEmail, MessageEmailValidator>, MessageEmailValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IMessageLogValidatorMapper, MessageLogValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<MessageLog, MessageLogValidator>, MessageLogValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IMessageValidatorMapper, MessageValidatorMapper>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidatorMapper<Message, MessageValidator>, MessageValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #region [ OChart ]

            #region [ Services ]

            container.RegisterType<IOChartService, OChartService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                .RegisterType<IOChartRuleset, OChartRulesetDefinition>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<OChart>, OChartValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<OChartItem>, OChartItemValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<OChartItemContainer>, OChartItemContainerValidator>(new HierarchicalLifetimeManager());


            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]
            container
                .RegisterType<IOChartMapper, OChartMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<OChart, OChart>, OChartMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IOChartItemMapper, OChartItemMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<OChartItem, OChartItem>, OChartItemMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IOChartItemContainerMapper, OChartItemContainerMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<OChartItemContainer, OChartItemContainer>, OChartItemContainerMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                .RegisterType<IOChartValidatorMapper, OChartValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<OChart, OChartValidator>, OChartValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IOChartItemValidatorMapper, OChartItemValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<OChartItem, OChartItemValidator>, OChartItemValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IOChartItemContainerValidatorMapper, OChartItemContainerValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<OChartItemContainer, OChartItemContainerValidator>, OChartItemContainerValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #region [ PECMail ]

            #region [ Services ]

            container
                .RegisterType<IPECMailService, PECMailService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IPECMailLogService, PECMailLogService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IPECMailReceiptService, PECMailReceiptService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IPECMailAttachmentService, PECMailAttachmentService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IPECMailBoxService, PECMailBoxService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IPECMailBoxConfigurationService, PECMailBoxConfigurationService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                .RegisterType<IPECMailRuleset, PECMailRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailBoxRuleset, PECMailBoxRuleset>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailAttachmentRuleset, PECMailAttachmentRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailBoxConfigurationRuleset, PECMailBoxConfigurationRulesetDefinition>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<PECMail>, PECMailValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<PECMailLog>, PECMailLogValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<PECMailBox>, PECMailBoxValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<PECMailReceipt>, PECMailReceiptValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<PECMailAttachment>, PECMailAttachmentValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<PECMailBoxConfiguration>, PECMailBoxConfigurationValidator>(new HierarchicalLifetimeManager());


            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]

            container
                .RegisterType<IPECMailMapper, PECMailMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<PECMail, PECMail>, PECMailMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailBoxMapper, PECMailBoxMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<PECMailBox, PECMailBox>, PECMailBoxMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailLogMapper, PECMailLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<PECMailLog, PECMailLog>, PECMailLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailReceiptMapper, PECMailReceiptMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<PECMailReceipt, PECMailReceipt>, PECMailReceiptMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailAttachmentMapper, PECMailAttachmentMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<PECMailAttachment, PECMailAttachment>, PECMailAttachmentMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<PECMailReceipt, PECMailReceipt>, PECMailReceiptMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailBoxTableValuedModelMapper, PECMailBoxTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<PECMailBoxTableValuedModel, PECMailBoxModel>, PECMailBoxTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailBoxModelMapper, PECMailBoxModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<PECMailBox, PECMailBoxModel>, PECMailBoxModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailBoxConfigurationMapper, PECMailBoxConfigurationMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<PECMailBoxConfiguration, PECMailBoxConfiguration>, PECMailBoxConfigurationMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailBoxConfigurationTableValuedModelMapper, PECMailBoxConfigurationTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<PECMailBoxConfigurationTableValuedModel, PECMailBoxConfigurationModel>, PECMailBoxConfigurationTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailBoxConfigurationModelMapper, PECMailBoxConfigurationModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<PECMailBoxConfiguration, PECMailBoxConfigurationModel>, PECMailBoxConfigurationModelMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                .RegisterType<IPECMailValidatorMapper, PECMailValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<PECMail, PECMailValidator>, PECMailValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailReceiptValidatorMapper, PECMailReceiptValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<PECMailReceipt, PECMailReceiptValidator>, PECMailReceiptValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailBoxValidatorMapper, PECMailBoxValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<PECMailBox, PECMailBoxValidator>, PECMailBoxValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailLogValidatorMapper, PECMailLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<PECMailLog, PECMailLogValidator>, PECMailLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailAttachmentValidatorMapper, PECMailAttachmentValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<PECMailAttachment, PECMailAttachmentValidator>, PECMailAttachmentValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IPECMailBoxConfigurationValidatorMapper, PECMailBoxConfigurationValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<PECMailBoxConfiguration, PECMailBoxConfigurationValidator>, PECMailBoxConfigurationValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #region [ Protocols ]

            #region [ Services ]

            container
                .RegisterType<IProtocolService, ProtocolService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IProtocolLogService, ProtocolLogService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IProtocolJournalService, ProtocolJournalService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IProtocolTypeService, ProtocolTypeService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IProtocolRoleService, ProtocolRoleService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IProtocolRoleUserService, ProtocolRoleUserService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IProtocolContactService, ProtocolContactService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IProtocolDocumentTypeService, ProtocolDocumentTypeService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IProtocolLinkService, ProtocolLinkService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IProtocolUserService, ProtocolUserService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IProtocolContactManualService, ProtocolContactManualService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IAdvancedProtocolService, AdvancedProtocolService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());


            #endregion

            #region [ Validations ]

            container
                .RegisterType<IProtocolRuleset, ProtocolRulesetDefinition>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<Protocol>, ProtocolValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ProtocolType>, ProtocolTypeValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ProtocolLog>, ProtocolLogValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ProtocolJournal>, ProtocolJournalValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ProtocolRole>, ProtocolRoleValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ProtocolRoleUser>, ProtocolRoleUserValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ProtocolContact>, ProtocolContactValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ProtocolLink>, ProtocolLinkValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ProtocolDocumentType>, ProtocolDocumentTypeValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ProtocolUser>, ProtocolUserValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ProtocolContactManual>, ProtocolContactManualValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<AdvancedProtocol>, AdvancedProtocolValidator>(new HierarchicalLifetimeManager());


            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]

            container
                   .RegisterType<IProtocolMapper, ProtocolMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<Protocol, Protocol>, ProtocolMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IProtocolTypeMapper, ProtocolTypeMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<ProtocolType, ProtocolType>, ProtocolTypeMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IProtocolLogMapper, ProtocolLogMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<ProtocolLog, ProtocolLog>, ProtocolLogMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IProtocolJournalMapper, ProtocolJournalMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<ProtocolJournal, ProtocolJournal>, ProtocolJournalMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IProtocolRoleMapper, ProtocolRoleMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<ProtocolRole, ProtocolRole>, ProtocolRoleMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IProtocolRoleUserMapper, ProtocolRoleUserMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<ProtocolRoleUser, ProtocolRoleUser>, ProtocolRoleUserMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IProtocolContactMapper, ProtocolContactMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<ProtocolContact, ProtocolContact>, ProtocolContactMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IProtocolLinkMapper, ProtocolLinkMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<ProtocolLink, ProtocolLink>, ProtocolLinkMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IProtocolDocumentTypeMapper, ProtocolDocumentTypeMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<ProtocolDocumentType, ProtocolDocumentType>, ProtocolDocumentTypeMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IProtocolTableValuedModelMapper, ProtocolTableValuedModelMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<ProtocolTableValuedModel, ProtocolModel>, ProtocolTableValuedModelMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IProtocolContactManualTableValueModelMapper, ProtocolContactManualTableValueModelMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<ProtocolTableValuedModel, ProtocolContactManualModel>, ProtocolContactManualTableValueModelMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IProtocolContactTableValueModelMapper, ProtocolContactTableValueModelMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<ProtocolTableValuedModel, ProtocolContactModel>, ProtocolContactTableValueModelMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IProtocolTypeTableValuedModelMapper, ProtocolTypeTableValuedModelMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<ProtocolTableValuedModel, ProtocolTypeModel>, ProtocolTypeTableValuedModelMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IProtocolUserMapper, ProtocolUserMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<ProtocolUser, ProtocolUser>, ProtocolUserMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IProtocolContactManualMapper, ProtocolContactManualMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<ProtocolContactManual, ProtocolContactManual>, ProtocolContactManualMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IAdvancedProtocolMapper, AdvancedProtocolMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<AdvancedProtocol, AdvancedProtocol>, AdvancedProtocolMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IAdvancedProtocolTableValuedModelMapper, AdvacedProtocolTableValuedModelMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<AdvancedProtocolTableValuedModel, AdvancedProtocolModel>, AdvacedProtocolTableValuedModelMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IAdvancedProtocolModelMapper, AdvancedProtocolModelMapper>(new HierarchicalLifetimeManager())
                   .RegisterType<IDomainMapper<AdvancedProtocol, AdvancedProtocolModel>, AdvancedProtocolModelMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                .RegisterType<IValidatorMapper<ProtocolContactManual, ProtocolContactManualValidator>, ProtocolContactManualValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProtocolValidatorMapper, ProtocolValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<Protocol, ProtocolValidator>, ProtocolValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProtocolContactValidatorMapper, ProtocolContactValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ProtocolContact, ProtocolContactValidator>, ProtocolContactValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProtocolRoleValidatorMapper, ProtocolRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ProtocolRole, ProtocolRoleValidator>, ProtocolRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProtocolRoleUserValidatorMapper, ProtocolRoleUserValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ProtocolRoleUser, ProtocolRoleUserValidator>, ProtocolRoleUserValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProtocolTypeValidatorMapper, ProtocolTypeValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ProtocolType, ProtocolTypeValidator>, ProtocolTypeValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProtocolLogValidatorMapper, ProtocolLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ProtocolLog, ProtocolLogValidator>, ProtocolLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProtocolJournalValidatorMapper, ProtocolJournalValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ProtocolJournal, ProtocolJournalValidator>, ProtocolJournalValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProtocolLinkValidatorMapper, ProtocolLinkValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ProtocolLink, ProtocolLinkValidator>, ProtocolLinkValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProtocolDocumentTypeValidatorMapper, ProtocolDocumentTypeValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ProtocolDocumentType, ProtocolDocumentTypeValidator>, ProtocolDocumentTypeValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProtocolUserValidatorMapper, ProtocolUserValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ProtocolUser, ProtocolUserValidator>, ProtocolUserValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProtocolContactManualValidatorMapper, ProtocolContactManualValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ProtocolContactManual, ProtocolContactManualValidator>, ProtocolContactManualValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IAdvancedProtocolValidatorMapper, AdvancedProtocolValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<AdvancedProtocol, AdvancedProtocolValidator>, AdvancedProtocolValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #region [PosteOnLine]

            #region [ Services ]

            container
                .RegisterType<IPOLRequestService, POLRequestService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]
            container
                .RegisterType<IPOLRequestRuleset, POLRequestRuleset>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<PosteOnLineRequest>, POLRequestValidator>(new HierarchicalLifetimeManager());
            #endregion

            #region [ Mappers ]

            #region [ Entities & Models ]
            container
               .RegisterType<IPOLRequestMapper, POLRequestMapper>(new HierarchicalLifetimeManager())
               .RegisterType<IDomainMapper<PosteOnLineRequest, PosteOnLineRequest>, POLRequestMapper>(new HierarchicalLifetimeManager());
            #endregion

            #region [ Validators ]
            container
              .RegisterType<IPOLRequestValidatorMapper, POLRequestValidatorMapper>(new HierarchicalLifetimeManager())
              .RegisterType<IValidatorMapper<PosteOnLineRequest, POLRequestValidator>, POLRequestValidatorMapper>(new HierarchicalLifetimeManager());
            #endregion

            #endregion


            #endregion

            #region [ Resolution ]

            #region [ Services ]

            container
                .RegisterType<IResolutionService, ResolutionService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IResolutionRoleService, ResolutionRoleService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IResolutionContactService, ResolutionContactService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IFileResolutionService, FileResolutionService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IResolutionLogService, ResolutionLogService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IResolutionKindService, ResolutionKindService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IResolutionKindDocumentSeriesService, ResolutionKindDocumentSeriesService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IResolutionDocumentSeriesItemService, ResolutionDocumentSeriesItemService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>());



            #endregion

            #region [ Validations ]

            container
                .RegisterType<IResolutionRuleset, ResolutionRuleset>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<Resolution>, ResolutionValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ResolutionRole>, ResolutionRoleValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ResolutionContact>, ResolutionContactValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<FileResolution>, FileResolutionValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ResolutionLog>, ResolutionLogValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ResolutionKind>, ResolutionKindValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ResolutionKindDocumentSeries>, ResolutionKindDocumentSeriesValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ResolutionDocumentSeriesItem>, ResolutionDocumentSeriesItemValidator>(new HierarchicalLifetimeManager());



            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]

            container
                .RegisterType<IResolutionMapper, ResolutionMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<Resolution, Resolution>, ResolutionMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IFileResolutionMapper, FileResolutionMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<FileResolution, FileResolution>, FileResolutionMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IResolutionRoleMapper, ResolutionRoleMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<ResolutionRole, ResolutionRole>, ResolutionRoleMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IResolutionRoleModelMapper, ResolutionRoleModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<ResolutionRole, ResolutionRoleModel>, ResolutionRoleModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IFileResolutionModelMapper, FileResolutionModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<FileResolution, FileResolutionModel>, FileResolutionModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IResolutionModelMapper, ResolutionModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IResolutionContactMapper, ResolutionContactMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<Resolution, ResolutionModel>, ResolutionModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IResolutionLogMapper, ResolutionLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<ResolutionLog, ResolutionLog>, ResolutionLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IResolutionKindMapper, ResolutionKindMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<ResolutionKind, ResolutionKind>, ResolutionKindMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IResolutionKindDocumentSeriesMapper, ResolutionKindDocumentSeriesMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<ResolutionKindDocumentSeries, ResolutionKindDocumentSeries>, ResolutionKindDocumentSeriesMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IResolutionDocumentSeriesItemMapper, ResolutionDocumentSeriesItemMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<ResolutionDocumentSeriesItem, ResolutionDocumentSeriesItem>, ResolutionDocumentSeriesItemMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWebPublicationMapper, WebPublicationMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<WebPublication, WebPublication>, WebPublicationMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWebPublicationModelMapper, WebPublicationModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<WebPublication, WebPublicationModel>, WebPublicationModelMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                .RegisterType<IResolutionValidatorMapper, ResolutionValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<Resolution, ResolutionValidator>, ResolutionValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IFileResolutionValidatorMapper, FileResolutionValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<FileResolution, FileResolutionValidator>, FileResolutionValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IResolutionRoleValidatorMapper, ResolutionRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IResolutionContactValidatorMapper, ResolutionContactValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ResolutionRole, ResolutionRoleValidator>, ResolutionRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IResolutionLogValidatorMapper, ResolutionLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ResolutionLog, ResolutionLogValidator>, ResolutionLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IResolutionKindValidatorMapper, ResolutionKindValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ResolutionKind, ResolutionKindValidator>, ResolutionKindValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IResolutionKindDocumentSeriesValidatorMapper, ResolutionKindDocumentSeriesValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ResolutionKindDocumentSeries, ResolutionKindDocumentSeriesValidator>, ResolutionKindDocumentSeriesValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IResolutionDocumentSeriesItemValidatorMapper, ResolutionDocumentSeriesItemValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ResolutionDocumentSeriesItem, ResolutionDocumentSeriesItemValidator>, ResolutionDocumentSeriesItemValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #region [ Workflow ]

            #region [ Services ]

            container
                   .RegisterType<IWorkflowActivityLogService, WorkflowActivityLogService>(new HierarchicalLifetimeManager(),
                           new Interceptor<InterfaceInterceptor>(),
                           new InterceptionBehavior<ServicesLogging>())
                   .RegisterType<IWorkflowActivityService, WorkflowActivityService>(new HierarchicalLifetimeManager(),
                           new Interceptor<InterfaceInterceptor>(),
                           new InterceptionBehavior<ServicesLogging>())
                   .RegisterType<IWorkflowInstanceService, WorkflowInstanceService>(new HierarchicalLifetimeManager(),
                           new Interceptor<InterfaceInterceptor>(),
                           new InterceptionBehavior<ServicesLogging>())
                   .RegisterType<IWorkflowRepositoryService, WorkflowRepositoryService>(new HierarchicalLifetimeManager(),
                           new Interceptor<InterfaceInterceptor>(),
                           new InterceptionBehavior<ServicesLogging>())
                   .RegisterType<IWorkflowAuthorizationService, WorkflowAuthorizationService>(new HierarchicalLifetimeManager(),
                           new Interceptor<InterfaceInterceptor>(),
                           new InterceptionBehavior<ServicesLogging>())
                   .RegisterType<IWorkflowPropertyService, WorkflowPropertyService>(new HierarchicalLifetimeManager(),
                           new Interceptor<InterfaceInterceptor>(),
                           new InterceptionBehavior<ServicesLogging>())
                   .RegisterType<IWorkflowRoleMappingService, WorkflowRoleMappingService>(new HierarchicalLifetimeManager(),
                           new Interceptor<InterfaceInterceptor>(),
                           new InterceptionBehavior<ServicesLogging>())
                   .RegisterType<IWorkflowInstanceLogService, WorkflowInstanceLogService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                   .RegisterType<IWorkflowInstanceRoleService, WorkflowInstanceRoleService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>())
                   .RegisterType<IWorkflowEvaluationPropertyService, WorkflowEvaluationPropertyService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [Validations]

            container
                 .RegisterType<IWorkflowRuleset, WorkflowRuleset>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<WorkflowRepository>, WorkflowRepositoryValidator>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidator<WorkflowInstance>, WorkflowInstanceValidator>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidator<WorkflowActivityLog>, WorkflowActivityLogValidator>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidator<WorkflowProperty>, WorkflowPropertyValidator>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidator<WorkflowActivity>, WorkflowActivityValidator>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidator<WorkflowAuthorization>, WorkflowAuthorizationValidator>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidator<WorkflowRoleMapping>, WorkflowRoleMappingValidator>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidator<WorkflowInstanceLog>, WorkflowInstanceLogValidator>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidator<WorkflowInstanceRole>, WorkflowInstanceRoleValidator>(new HierarchicalLifetimeManager())
                 .RegisterType<IValidator<WorkflowEvaluationProperty>, WorkflowEvaluationPropertyValidator>(new HierarchicalLifetimeManager());


            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]

            container
                .RegisterType<IWorkflowInstanceMapper, WorkflowInstanceMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<WorkflowInstance, WorkflowInstance>, WorkflowInstanceMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowRepositoryMapper, WorkflowRepositoryMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<WorkflowRepository, WorkflowRepository>, WorkflowRepositoryMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowPropertyMapper, WorkflowPropertyMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<WorkflowProperty, WorkflowProperty>, WorkflowPropertyMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowActivityLogMapper, WorkflowActivityLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<WorkflowActivityLog, WorkflowActivityLog>, WorkflowActivityLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowActivityMapper, WorkflowActivityMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<WorkflowActivity, WorkflowActivity>, WorkflowActivityMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowAuthorizationMapper, WorkflowAuthorizationMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<WorkflowAuthorization, WorkflowAuthorization>, WorkflowAuthorizationMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowRoleMappingMapper, WorkflowRoleMappingMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<WorkflowRoleMapping, WorkflowRoleMapping>, WorkflowRoleMappingMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowRoleMappingCollaborationUserModelMapper, WorkflowRoleMappingCollaborationUserModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<WorkflowRoleMapping, CollaborationUserModel>, WorkflowRoleMappingCollaborationUserModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowInstanceLogMapper, WorkflowInstanceLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<WorkflowInstanceLog, WorkflowInstanceLog>, WorkflowInstanceLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowInstanceRoleMapper, WorkflowInstanceRoleMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<WorkflowInstanceRole, WorkflowInstanceRole>, WorkflowInstanceRoleMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowEvaluationPropertyMapper, WorkflowEvaluationPropertyMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<WorkflowEvaluationProperty, WorkflowEvaluationProperty>, WorkflowEvaluationPropertyMapper>(new HierarchicalLifetimeManager());


            #endregion

            #region [ Validator ]

            container
                .RegisterType<IWorkflowRepositoryValidatorMapper, WorkflowRepositoryValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<WorkflowRepository, WorkflowRepositoryValidator>, WorkflowRepositoryValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowInstanceValidatorMapper, WorkflowInstanceValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<WorkflowInstance, WorkflowInstanceValidator>, WorkflowInstanceValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowActivityLogValidatorMapper, WorkflowActivityLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<WorkflowActivityLog, WorkflowActivityLogValidator>, WorkflowActivityLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowPropertyValidatorMapper, WorkflowPropertyValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<WorkflowProperty, WorkflowPropertyValidator>, WorkflowPropertyValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowActivityValidatorMapper, WorkflowActivityValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<WorkflowActivity, WorkflowActivityValidator>, WorkflowActivityValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowAuthorizationValidatorMapper, WorkflowAuthorizationValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<WorkflowAuthorization, WorkflowAuthorizationValidator>, WorkflowAuthorizationValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowRoleMappingValidatorMapper, WorkflowRoleMappingValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<WorkflowRoleMapping, WorkflowRoleMappingValidator>, WorkflowRoleMappingValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowInstanceLogValidatorMapper, WorkflowInstanceLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<WorkflowInstanceLog, WorkflowInstanceLogValidator>, WorkflowInstanceLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowInstanceRoleValidatorMapper, WorkflowInstanceRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<WorkflowInstanceRole, WorkflowInstanceRoleValidator>, WorkflowInstanceRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IWorkflowEvaluationPropertyValidatorMapper, WorkflowEvaluationPropertyValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<WorkflowEvaluationProperty, WorkflowEvaluationPropertyValidator>, WorkflowEvaluationPropertyValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion     

            #endregion

            #endregion

            #region [ UDS ]

            #region [ Services ]

            container
                .RegisterType<IUDSRepositoryService, UDSRepositoryService>(new HierarchicalLifetimeManager(),
                           new Interceptor<InterfaceInterceptor>(),
                           new InterceptionBehavior<ServicesLogging>())
                      .RegisterType<IUDSSchemaRepositoryService, UDSSchemaRepositoryService>(new HierarchicalLifetimeManager(),
                           new Interceptor<InterfaceInterceptor>(),
                           new InterceptionBehavior<ServicesLogging>())
                      .RegisterType<IUDSTypologyService, UDSTypologyService>(new HierarchicalLifetimeManager(),
                           new Interceptor<InterfaceInterceptor>(),
                           new InterceptionBehavior<ServicesLogging>())
                      .RegisterType<IUDSLogService, UDSLogService>(new HierarchicalLifetimeManager(),
                           new Interceptor<InterfaceInterceptor>(),
                           new InterceptionBehavior<ServicesLogging>())
                      .RegisterType<IUDSRoleService, UDSRoleService>(new HierarchicalLifetimeManager(),
                           new Interceptor<InterfaceInterceptor>(),
                           new InterceptionBehavior<ServicesLogging>())
                      .RegisterType<IUDSFieldListService, UDSFieldListService>(new HierarchicalLifetimeManager(),
                           new Interceptor<InterfaceInterceptor>(),
                           new InterceptionBehavior<ServicesLogging>())
                      .RegisterType<IUDSUserService, UDSUserService>(new HierarchicalLifetimeManager(),
                           new Interceptor<InterfaceInterceptor>(),
                           new InterceptionBehavior<ServicesLogging>())
                      .RegisterType<IUDSContactService, UDSContactService>(new HierarchicalLifetimeManager(),
                          new Interceptor<InterfaceInterceptor>(),
                          new InterceptionBehavior<ServicesLogging>())
                      .RegisterType<IUDSMessageService, UDSMessageService>(new HierarchicalLifetimeManager(),
                          new Interceptor<InterfaceInterceptor>(),
                          new InterceptionBehavior<ServicesLogging>())
                      .RegisterType<IUDSDocumentUnitService, UDSDocumentUnitService>(new HierarchicalLifetimeManager(),
                           new Interceptor<InterfaceInterceptor>(),
                           new InterceptionBehavior<ServicesLogging>())
                      .RegisterType<IUDSPECMailService, UDSPECMailService>(new HierarchicalLifetimeManager(),
                          new Interceptor<InterfaceInterceptor>(),
                          new InterceptionBehavior<ServicesLogging>())
                      .RegisterType<IUDSCollaborationService, UDSCollaborationService>(new HierarchicalLifetimeManager(),
                          new Interceptor<InterfaceInterceptor>(),
                          new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                .RegisterType<IUDSRuleset, UDSRuleset>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<UDSRepository>, UDSRepositoryValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<UDSSchemaRepository>, UDSSchemaRepositoryValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<UDSTypology>, UDSTypologyValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<UDSLog>, UDSLogValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<UDSRole>, UDSRoleValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<UDSFieldList>, UDSFieldListValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<UDSUser>, UDSUserValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<UDSContact>, UDSContactValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<UDSMessage>, UDSMessageValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<UDSDocumentUnit>, UDSDocumentUnitValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<UDSPECMail>, UDSPECMailValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<UDSCollaboration>, UDSCollaborationValidator>(new HierarchicalLifetimeManager());


            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]

            container
                .RegisterType<IUDSRepositoryMapper, UDSRepositoryMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSRepository, UDSRepository>, UDSRepositoryMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSSchemaRepositoryMapper, UDSSchemaRepositoryMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSSchemaRepository, UDSSchemaRepository>, UDSSchemaRepositoryMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSRepositoryModelMapper, UDSRepositoryModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSRepository, UDSRepositoryModel>, UDSRepositoryModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSSchemaRepositoryModelMapper, UDSSchemaRepositoryModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSSchemaRepository, UDSSchemaRepositoryModel>, UDSSchemaRepositoryModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSTypologyMapper, UDSTypologyMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSTypology, UDSTypology>, UDSTypologyMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSRepositoryTableValuedModelMapper, UDSRepositoryTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSRepositoryTableValuedModel, UDSRepository>, UDSRepositoryTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSLogMapper, UDSLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSLog, UDSLog>, UDSLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSRoleMapper, UDSRoleMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSRole, UDSRole>, UDSRoleMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSFieldListMapper, UDSFieldListMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSFieldList, UDSFieldList>, UDSFieldListMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSUserMapper, UDSUserMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSUser, UDSUser>, UDSUserMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSContactMapper, UDSContactMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSContact, UDSContact>, UDSContactMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSMessageMapper, UDSMessageMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSMessage, UDSMessage>, UDSMessageMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSDocumentUnitMapper, UDSDocumentUnitMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSDocumentUnit, UDSDocumentUnit>, UDSDocumentUnitMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSUser, UDSUser>, UDSUserMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSPECMailMapper, UDSPECMailMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSPECMail, UDSPECMail>, UDSPECMailMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSCollaborationMapper, UDSCollaborationMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSCollaboration, UDSCollaboration>, UDSCollaborationMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSCollaborationEntityMapper, UDSCollaborationEntityMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSCollaborationModel, UDSCollaboration>, UDSCollaborationEntityMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSContactEntityMapper, UDSContactEntityMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSContactModel, UDSContact>, UDSContactEntityMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSDocumentUnitEntityMapper, UDSDocumentUnitEntityMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSDocumentUnitModel, UDSDocumentUnit>, UDSDocumentUnitEntityMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSMessageEntityMapper, UDSMessageEntityMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSMessageModel, UDSMessage>, UDSMessageEntityMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSPECMailEntityMapper, UDSPECMailEntityMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSPECMailModel, UDSPECMail>, UDSPECMailEntityMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSRoleEntityMapper, UDSRoleEntityMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSRoleModel, UDSRole>, UDSRoleEntityMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSUserEntityMapper, UDSUserEntityMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<UDSUserModel, UDSUser>, UDSUserEntityMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                .RegisterType<IUDSRepositoryValidatorMapper, UDSRepositoryValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<UDSRepository, UDSRepositoryValidator>, UDSRepositoryValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSSchemaRepositoryValidatorMapper, UDSSchemaRepositoryValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<UDSSchemaRepository, UDSSchemaRepositoryValidator>, UDSSchemaRepositoryValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSTypologyValidatorMapper, UDSTypologyValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<UDSTypology, UDSTypologyValidator>, UDSTypologyValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSLogValidatorMapper, UDSLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<UDSLog, UDSLogValidator>, UDSLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSRoleValidatorMapper, UDSRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<UDSRole, UDSRoleValidator>, UDSRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSFieldListValidatorMapper, UDSFieldListValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<UDSFieldList, UDSFieldListValidator>, UDSFieldListValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSUserValidatorMapper, UDSUserValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<UDSUser, UDSUserValidator>, UDSUserValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSContactValidatorMapper, UDSContactValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<UDSContact, UDSContactValidator>, UDSContactValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSMessageValidatorMapper, UDSMessageValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<UDSMessage, UDSMessageValidator>, UDSMessageValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSDocumentUnitValidatorMapper, UDSDocumentUnitValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<UDSDocumentUnit, UDSDocumentUnitValidator>, UDSDocumentUnitValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<UDSUser, UDSUserValidator>, UDSUserValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSPECMailValidatorMapper, UDSPECMailValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<UDSPECMail, UDSPECMailValidator>, UDSPECMailValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IUDSCollaborationValidatorMapper, UDSCollaborationValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<UDSCollaboration, UDSCollaborationValidator>, UDSCollaborationValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #region [ MassimarioScarto ]

            #region [ Services ]

            container
                .RegisterType<IMassimarioScartoService, MassimarioScartoService>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                  .RegisterType<IMassimarioScartoRuleset, MassimarioScartoRuleset>(new HierarchicalLifetimeManager());

            container
                  .RegisterType<IValidator<MassimarioScarto>, MassimarioScartoValidator>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]

            container
                .RegisterType<IMassimarioScartoMapper, MassimarioScartoMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<MassimarioScarto, MassimarioScarto>, MassimarioScartoMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IMassimarioScartoModelMapper, MassimarioScartoModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<MassimarioScarto, MassimarioScartoModel>, MassimarioScartoModelMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                .RegisterType<IMassimarioScartoValidatorMapper, MassimarioScartoValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<MassimarioScarto, MassimarioScartoValidator>, MassimarioScartoValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #region [ Dossiers ]

            #region [ Service ]
            container
                .RegisterType<IDossierService, DossierService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())

                 .RegisterType<IDossierLogService, DossierLogService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())

                 .RegisterType<IDossierDocumentService, DossierDocumentService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())

                .RegisterType<IDossierRoleService, DossierRoleService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())

                .RegisterType<IDossierCommentService, DossierCommentService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())

                 .RegisterType<IDossierFolderService, DossierFolderService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())

                .RegisterType<IDossierFolderRoleService, DossierFolderRoleService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())

                .RegisterType<IDossierLinkService, DossierLinkService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validation ]
            container
                .RegisterType<IDossierRuleset, DossierRuleset>(new HierarchicalLifetimeManager());
            container
                .RegisterType<IValidator<Dossier>, DossierValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DossierLog>, DossierLogValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DossierDocument>, DossierDocumentValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DossierRole>, DossierRoleValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DossierComment>, DossierCommentValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DossierFolder>, DossierFolderValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DossierFolderRole>, DossierFolderRoleValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<DossierLink>, DossierLinkValidator>(new HierarchicalLifetimeManager());
            #endregion

            #region [ Mappers ]

            #region [ Entities & Models]

            container
                .RegisterType<IDossierMapper, DossierMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<Dossier, Dossier>, DossierMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierLogMapper, DossierLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DossierLog, DossierLog>, DossierLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierDocumentMapper, DossierDocumentMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DossierDocument, DossierDocument>, DossierDocumentMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierRoleMapper, DossierRoleMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DossierRole, DossierRole>, DossierRoleMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierCommentMapper, DossierCommentMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DossierComment, DossierComment>, DossierCommentMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierFolderMapper, DossierFolderMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DossierFolder, DossierFolder>, DossierFolderMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierFolderRoleMapper, DossierFolderRoleMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DossierFolderRole, DossierFolderRole>, DossierFolderRoleMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierLinkMapper, DossierLinkMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DossierLink, DossierLink>, DossierLinkMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierModelMapper, DossierModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<Dossier, DossierModel>, DossierModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierTableValuedModelMapper, DossierTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DossierTableValuedModel, DossierModel>, DossierTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierRoleTableValuedModelMapper, DossierRoleTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DossierTableValuedModel, DossierRoleModel>, DossierRoleTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierRoleModelMapper, DossierRoleModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DossierRole, DossierRoleModel>, DossierRoleModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierFolderModelMapper, DossierFolderModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DossierFolder, DossierFolderModel>, DossierFolderModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierFolderTableValuedModelMapper, DossierFolderTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<DossierFolderTableValuedModel, DossierFolderModel>, DossierFolderTableValuedModelMapper>(new HierarchicalLifetimeManager());


            #endregion

            #region [ Validator ]

            container
                .RegisterType<IDossierValidatorMapper, DossierValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<Dossier, DossierValidator>, DossierValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierLogValidatorMapper, DossierLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DossierLog, DossierLogValidator>, DossierLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierDocumentValidatorMapper, DossierDocumentValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DossierDocument, DossierDocumentValidator>, DossierDocumentValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierRoleValidatorMapper, DossierRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DossierRole, DossierRoleValidator>, DossierRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierCommentValidatorMapper, DossierCommentValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DossierComment, DossierCommentValidator>, DossierCommentValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierFolderValidatorMapper, DossierFolderValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DossierFolder, DossierFolderValidator>, DossierFolderValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierFolderRoleValidatorMapper, DossierFolderRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DossierFolderRole, DossierFolderRoleValidator>, DossierFolderRoleValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDossierLinkValidatorMapper, DossierLinkValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<DossierLink, DossierLinkValidator>, DossierLinkValidatorMapper>(new HierarchicalLifetimeManager());
            #endregion

            #endregion

            #endregion

            #region [ Conservation ]

            #region [ Services ]

            container
                .RegisterType<IConservationService, ConservationService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                .RegisterType<IConservationRuleset, ConservationRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<Conservation>, ConservationValidator>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]

            container
                .RegisterType<IConservationMapper, ConservationMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<Conservation, Conservation>, ConservationMapper>(new HierarchicalLifetimeManager());
            container
                .RegisterType<IConservationModelMapper, ConservationModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<Conservation, ConservationModel>, ConservationModelMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                .RegisterType<IConservationValidatorMapper, ConservationValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<Conservation, ConservationValidator>, ConservationValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #region [ Monitors ]

            #region [ Service ]
            container
                .RegisterType<ITransparentAdministrationMonitorLogService, TransparentAdministrationMonitorLogService>(new HierarchicalLifetimeManager(),
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<ServicesLogging>());
            #endregion

            #region [ Validation ]
            container
                .RegisterType<ITransparentAdministrationMonitorLogRuleset, TransparentAdministrationMonitorLogRuleset>(new HierarchicalLifetimeManager());
            container
                .RegisterType<IValidator<TransparentAdministrationMonitorLog>, TransparentAdministrationMonitorLogValidator>(new HierarchicalLifetimeManager());
            #endregion

            #region [ Mappers ]

            #region [ Entities & Models ]
            container
                .RegisterType<ITransparentAdministrationMonitorLogMapper, TransparentAdministrationMonitorLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TransparentAdministrationMonitorLog, TransparentAdministrationMonitorLog>, TransparentAdministrationMonitorLogMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITransparentAdministrationMonitorLogTableValuedModelMapper, TransparentAdministrationMonitorLogTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TransparentAdministrationMonitorLogTableValuedModel, TransparentAdministrationMonitorLogModel>, TransparentAdministrationMonitorLogTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITransparentAdministrationMonitorLogModelMapper, TransparentAdministrationMonitorLogModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TransparentAdministrationMonitorLog, TransparentAdministrationMonitorLogModel>, TransparentAdministrationMonitorLogModelMapper>(new HierarchicalLifetimeManager());
            #endregion

            #region [ Validator ]
            container
                .RegisterType<ITransparentAdministrationMonitorLogValidatorMapper, TransparentAdministrationMonitorLogValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<TransparentAdministrationMonitorLog, TransparentAdministrationMonitorLogValidator>, TransparentAdministrationMonitorLogValidatorMapper>(new HierarchicalLifetimeManager());
            #endregion

            #endregion

            #endregion

            #region [ Workflow ]

            #region [ Services ]

            container
                .RegisterType<IWorkflowStartService, WorkflowStartService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IWorkflowNotifyService, WorkflowNotifyService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());
            #endregion

            #region [ Validations ]
            container
                .RegisterType<WorkflowManager.IWorkflowRuleset, WorkflowManager.WorkflowRuleset>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Mappers ]
            container
                .RegisterType<IWorkflowArgumentMapper, WorkflowArgumentMapper>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<MappingsLogging>());
            #endregion

            #endregion

            #region [ DocumentGenerator ]
            container.RegisterType<IPDFDocumentGenerator, PDFDocumentGenerator>(new HierarchicalLifetimeManager());
            container.RegisterType<IWordOpenXmlDocumentGenerator, WordOpenXmlDocumentGenerator>(new HierarchicalLifetimeManager(),
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<ServicesLogging>());
            #endregion

            #region [ JeepServiceHosts ]

            #region [ Service ]
            container
                .RegisterType<IJeepServiceHostService, JeepServiceHostService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());
            #endregion

            #region [ Validation ]
            container
                .RegisterType<IJeepServiceHostRuleset, JeepServiceHostRulesetDefinition>(new HierarchicalLifetimeManager());
            container
                .RegisterType<IValidator<JeepServiceHost>, JeepServiceHostValidator>(new HierarchicalLifetimeManager());
            #endregion

            #region [ Mappers ]

            #region [ Entities & Models ]
            container
                .RegisterType<IJeepServiceHostMapper, JeepServiceHostMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<JeepServiceHost, JeepServiceHost>, JeepServiceHostMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IJeepServiceHostTableValuedModelMapper, JeepServiceHostTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<JeepServiceHostTableValuedModel, JeepServiceHostModel>, JeepServiceHostTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IJeepServiceHostModelMapper, JeepServiceHostModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<JeepServiceHost, JeepServiceHostModel>, JeepServiceHostModelMapper>(new HierarchicalLifetimeManager());
            #endregion

            #region [ Validator ]
            container
                .RegisterType<IJeepServiceHostValidatorMapper, JeepServiceHostValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<JeepServiceHost, JeepServiceHostValidator>, JeepServiceHostValidatorMapper>(new HierarchicalLifetimeManager());
            #endregion

            #endregion

            #endregion

            #region [ Tenant ]

            #region [ Services ]

            container
                .RegisterType<ITenantService, TenantService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<ITenantConfigurationService, TenantConfigurationService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<ITenantWorkflowRepositoryService, TenantWorkflowRepositoryService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<ITenantAOOService, TenantAOOService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                .RegisterType<ITenantRuleset, TenantRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<ITenantConfigurationRuleset, TenantConfigurationRulesetDefinition>(new HierarchicalLifetimeManager())
                .RegisterType<ITenantWorkflowRepositoryRuleset, TenantWorkflowRepositoryRuleset>(new HierarchicalLifetimeManager())
                .RegisterType<ITenantAOORuleset, TenantAOORuleset>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<Tenant>, TenantValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<TenantConfiguration>, TenantConfigurationValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<PECMailLog>, PECMailLogValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<PECMailBox>, PECMailBoxValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<PECMailReceipt>, PECMailReceiptValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<PECMailAttachment>, PECMailAttachmentValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<PECMailBoxConfiguration>, PECMailBoxConfigurationValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<TenantWorkflowRepository>, TenantWorkflowRepositoryValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<TenantAOO>, TenantAOOValidator>(new HierarchicalLifetimeManager());


            #endregion

            #region [ Mappers ]

            #region [ Entites & Models ]

            container
                .RegisterType<ITenantMapper, TenantMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<Tenant, Tenant>, TenantMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenantConfigurationMapper, TenantConfigurationMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TenantConfiguration, TenantConfiguration>, TenantConfigurationMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenantConfigurationTableValuedModelMapper, TenantConfigurationTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TenantConfigurationTableValuedModel, TenantConfigurationModel>, TenantConfigurationTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenantConfigurationModelMapper, TenantConfigurationModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TenantConfiguration, TenantConfigurationModel>, TenantConfigurationModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenantTableValuedModelMapper, TenantTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TenantTableValuedModel, Tenant>, TenantTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenantWorkflowRepositoryMapper, TenantWorkflowRepositoryMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TenantWorkflowRepository, TenantWorkflowRepository>, TenantWorkflowRepositoryMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenantAOOMapper, TenantAOOMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TenantAOO, TenantAOO>, TenantAOOMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenantAOOModelMapper, TenantAOOModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TenantAOO, TenantAOOModel>, TenantAOOModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenantAOOTableValuedModelMapper, TenantAOOTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<ITenantAOOTableValuedModel, TenantAOOModel>, TenantAOOTableValuedModelMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                .RegisterType<ITenantValidatorMapper, TenantValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<Tenant, TenantValidator>, TenantValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenantConfigurationValidatorMapper, TenantConfigurationValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<TenantConfiguration, TenantConfigurationValidator>, TenantConfigurationValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenantWorkflowRepositoryValidatorMapper, TenantWorkflowRepositoryValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<TenantWorkflowRepository, TenantWorkflowRepositoryValidator>, TenantWorkflowRepositoryValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenantAOOValidatorMapper, TenantAOOValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<TenantAOO, TenantAOOValidator>, TenantAOOValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #region [ Mappers ]
            #region [ Entities & Models ]
            #endregion
            #endregion

            #region [ Processes ]

            #region [ Services ]

            container
                .RegisterType<IProcessService, ProcessService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IProcessFascicleTemplateService, ProcessFascicleTemplateService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<IProcessFascicleWorkflowRepositoryService, ProcessFascicleWorkflowRepositoryService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                .RegisterType<IProcessRuleset, ProcessRuleset>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<Process>, ProcessValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ProcessFascicleTemplate>, ProcessFascicleTemplateValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<ProcessFascicleWorkflowRepository>, ProcessFascicleWorkflowRepositoryValidator>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Mappers ]

            #region [ Entities & Models ]

            container
                .RegisterType<IProcessMapper, ProcessMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<Process, Process>, ProcessMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProcessFascicleTemplateMapper, ProcessFascicleTemplateMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<ProcessFascicleTemplate, ProcessFascicleTemplate>, ProcessFascicleTemplateMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProcessFascicleWorkflowRepositoryMapper, ProcessFascicleWorkflowRepositoryMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<ProcessFascicleWorkflowRepository, ProcessFascicleWorkflowRepository>, ProcessFascicleWorkflowRepositoryMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProcessFascicleTemplateModelMapper, ProcessFascicleTemplateModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<ProcessFascicleTemplate, ProcessFascicleTemplateModel>, ProcessFascicleTemplateModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProcessTableValuedModelMapper, ProcessTableValuedModelMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<ProcessTableValuedModel, ProcessModel>, ProcessTableValuedModelMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                .RegisterType<IValidatorMapper<Process, ProcessValidator>, ProcessValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProcessValidatorMapper, ProcessValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ProcessFascicleTemplate, ProcessFascicleTemplateValidator>, ProcessFascicleTemplateValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProcessFascicleTemplateValidatorMapper, ProcessFascicleTemplateValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<ProcessFascicleWorkflowRepository, ProcessFascicleWorkflowRepositoryValidator>, ProcessFascicleWorkflowRepositoryValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IProcessFascicleWorkflowRepositoryValidatorMapper, ProcessFascicleWorkflowRepositoryValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #region [ Tasks ]

            #region [ Services ]

            container
                .RegisterType<ITaskHeaderService, TaskHeaderService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<ITaskHeaderProtocolService, TaskHeaderProtocolService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<ITaskDetailService, TaskDetailService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                .RegisterType<ITaskHeaderRuleset, TaskHeaderRuleset>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<TaskHeader>, TaskHeaderValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<TaskHeaderProtocol>, TaskHeaderProtocolValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<TaskDetail>, TaskDetailValidator>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Mappers ]

            #region [ Entities & Models ]

            container
                .RegisterType<ITaskHeaderMapper, TaskHeaderMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TaskHeader, TaskHeader>, TaskHeaderMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITaskHeaderProtocolMapper, TaskHeaderProtocolMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TaskHeaderProtocol, TaskHeaderProtocol>, TaskHeaderProtocolMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITaskDetailMapper, TaskDetailMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TaskDetail, TaskDetail>, TaskDetailMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                .RegisterType<IValidatorMapper<TaskHeader, TaskHeaderValidator>, TaskHeaderValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITaskHeaderValidatorMapper, TaskHeaderValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<TaskHeaderProtocol, TaskHeaderProtocolValidator>, TaskHeaderProtocolValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITaskHeaderProtocolValidatorMapper, TaskHeaderProtocolValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<TaskDetail, TaskDetailValidator>, TaskDetailValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITaskDetailValidatorMapper, TaskDetailValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #region [ Tenders ]

            #region [ Services ]

            container
                .RegisterType<ITenderHeaderService, TenderHeaderService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<ITenderLotService, TenderLotService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>())
                .RegisterType<ITenderLotPaymentService, TenderLotPaymentService>(new HierarchicalLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ServicesLogging>());

            #endregion

            #region [ Validations ]

            container
                .RegisterType<ITenderRuleset, TenderRuleset>(new HierarchicalLifetimeManager());

            container
                .RegisterType<IValidator<TenderHeader>, TenderHeaderValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<TenderLot>, TenderLotValidator>(new HierarchicalLifetimeManager())
                .RegisterType<IValidator<TenderLotPayment>, TenderLotPaymentValidator>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Mappers ]

            #region [ Entities & Models ]

            container
                .RegisterType<ITenderHeaderMapper, TenderHeaderMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TenderHeader, TenderHeader>, TenderHeaderMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenderLotMapper, TenderLotMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TenderLot, TenderLot>, TenderLotMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenderLotPaymentMapper, TenderLotPaymentMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IDomainMapper<TenderLotPayment, TenderLotPayment>, TenderLotPaymentMapper>(new HierarchicalLifetimeManager());

            #endregion

            #region [ Validator ]

            container
                .RegisterType<IValidatorMapper<TenderHeader, TenderHeaderValidator>, TenderHeaderValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenderHeaderValidatorMapper, TenderHeaderValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<TenderLot, TenderLotValidator>, TenderLotValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenderLotValidatorMapper, TenderLotValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<IValidatorMapper<TenderLotPayment, TenderLotPaymentValidator>, TenderLotPaymentValidatorMapper>(new HierarchicalLifetimeManager())
                .RegisterType<ITenderLotPaymentValidatorMapper, TenderLotPaymentValidatorMapper>(new HierarchicalLifetimeManager());

            #endregion

            #endregion

            #endregion

            #endregion

            return resolver;
        }
    }
}
