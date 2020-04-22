using System;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.Module.UDS.Roslyn;
using VecompSoftware.ServiceBus.Module.UDS.Roslyn.Generators;
using VecompSoftware.ServiceBus.Module.UDS.Roslyn.Generators.Classes;
using VecompSoftware.ServiceBus.Module.UDS.Roslyn.Generators.Controllers;
using VecompSoftware.ServiceBus.Module.UDS.Storage;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands.Models.UDS;

namespace VecompSoftware.ServiceBus.Module.UDS.Listener.Create
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class Execution : UDSBaseExecution<ICommandCreateUDS>
    {
        #region [ Fields ]

        private readonly ILogger _logger;

        private readonly BiblosDS.BiblosDSManagement.AdministrationClient _administrationClient;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public Execution(ILogger logger, IWebAPIClient webApiClient, BiblosDS.BiblosClient biblosClient)
            : base(logger, webApiClient)
        {
            _logger = logger;
            _administrationClient = biblosClient.Administration;
        }
        #endregion

        #region [ Methods ]
        public override async Task ExecuteAsync(ICommandCreateUDS command)
        {
            _logger.WriteInfo(new LogMessage(string.Concat(command.CommandName, " is arrived")), LogCategories);
            try
            {
                UDSStorageFacade udsStorageFacade = new UDSStorageFacade(_logger, command.ContentType.ContentTypeValue.XMLContent, CurrentUDSSchemaRepository.SchemaXML, _administrationClient, DBSchema);
                _logger.WriteInfo(new LogMessage("Starting create storage"), LogCategories);
                await udsStorageFacade.CreateStorageAsync(ConnectionString, async (smo) =>
                {
                    _logger.WriteInfo(new LogMessage("Create storage completed"), LogCategories);
                    bool res = true;
                    try
                    {
                        res &= smo.TableExist(udsStorageFacade.Builder.UDSTableName);
                        if (res)
                        {
                            _logger.WriteInfo(new LogMessage(string.Concat(udsStorageFacade.Builder.UDSTableName, " has been successfully created")), LogCategories);
                        }

                        res &= smo.TableExist(udsStorageFacade.Builder.UDSDocumentsTableName);
                        if (res)
                        {
                            _logger.WriteInfo(new LogMessage(string.Concat(udsStorageFacade.Builder.UDSDocumentsTableName, " has been successfully created")), LogCategories);
                        }

                        if (!res)
                        {
                            throw new InvalidOperationException("Error in creation table. Detect miss tables.");
                        }
                        MetadataEntity metadatas = new MetadataEntity(_logger);
                        UDSEntity udsEntity = metadatas.LoadMetadata(udsStorageFacade);
                        bool codeGenerated = await GenerateUDSCodeAsync(udsEntity);
                        if (codeGenerated)
                        {
                            _logger.WriteInfo(new LogMessage("UDS libraries has been successfully installed."), LogCategories);
                        }
                        else
                        {
                            _logger.WriteError(new LogMessage("Occour error in UDS libraries building. Process manual UDS Migrations"), LogCategories);
                        }
                        if (udsStorageFacade.UDS.Model.Documents != null)
                        {
                            if (udsStorageFacade.UDS.Model.Documents.Document != null && udsStorageFacade.UDS.Model.Documents.Document.CreateBiblosArchive)
                            {
                                await CreateBiblosDSArchiveAsync(udsEntity, udsStorageFacade, true);
                                udsStorageFacade.UDS.Model.Documents.Document.CreateBiblosArchive = false;
                                _logger.WriteInfo(new LogMessage("Archive created successfully"), LogCategories);
                            }

                            if ((udsStorageFacade.UDS.Model.Documents.DocumentAttachment != null && udsStorageFacade.UDS.Model.Documents.DocumentAttachment.CreateBiblosArchive) ||
                                (udsStorageFacade.UDS.Model.Documents.DocumentAnnexed != null && udsStorageFacade.UDS.Model.Documents.DocumentAnnexed.CreateBiblosArchive) ||
                                (udsStorageFacade.UDS.Model.Documents.Document != null && udsStorageFacade.UDS.Model.Documents.Document.DematerialisationEnabled) ||
                                (udsStorageFacade.UDS.Model.Documents.DocumentAttachment != null && udsStorageFacade.UDS.Model.Documents.DocumentAttachment.DematerialisationEnabled) ||
                                (udsStorageFacade.UDS.Model.Documents.DocumentAnnexed != null && udsStorageFacade.UDS.Model.Documents.DocumentAnnexed.DematerialisationEnabled))
                            {
                                await CreateBiblosDSArchiveAsync(udsEntity, udsStorageFacade);
                                if (udsStorageFacade.UDS.Model.Documents.DocumentAttachment != null)
                                {
                                    udsStorageFacade.UDS.Model.Documents.DocumentAttachment.CreateBiblosArchive = false;
                                }
                                if (udsStorageFacade.UDS.Model.Documents.DocumentAnnexed != null)
                                {
                                    udsStorageFacade.UDS.Model.Documents.DocumentAnnexed.CreateBiblosArchive = false;
                                }
                                _logger.WriteInfo(new LogMessage("Archive Miscellaneous created successfully"), LogCategories);
                            }
                            command.ContentType.ContentTypeValue.XMLContent = udsStorageFacade.UDS.SerializeToXml();
                        }

                        if (udsStorageFacade.UDS.Model.Container.CreateContainer)
                        {
                            _logger.WriteInfo(new LogMessage(string.Concat("Creating Container ", udsStorageFacade.UDS.Model.Title, " with admin SecurityUser ", command.Identity.User)), LogCategories);
                            DocSuiteWeb.Entity.Commons.Container container = CreateContainerFromArchive(udsStorageFacade.UDS.Model.Title, command.Identity.User);
                            container = await CreateConteainerAsync(container);
                            udsStorageFacade.UDS.Model.Container.IdContainer = container.EntityShortId.ToString();
                            udsStorageFacade.UDS.Model.Container.CreateContainer = false;
                            command.ContentType.ContentTypeValue.XMLContent = udsStorageFacade.UDS.SerializeToXml();
                            _logger.WriteInfo(new LogMessage("DocSuite Container created successfully"), LogCategories);
                        }

                        _logger.WriteInfo(new LogMessage("UDS storage configured successfully"), LogCategories);
                        await SaveUDSRepositoryAsync(command.ContentType.ContentTypeValue, udsStorageFacade.UDS.Model.Container.IdContainer);
                        _logger.WriteInfo(new LogMessage("UDS entity saved successfully"), LogCategories);
                        return true;
                    }
                    catch (AggregateException aex)
                    {
                        foreach (Exception ex in aex.Flatten().InnerExceptions)
                        {
                            _logger.WriteError(ex, LogCategories);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(ex, LogCategories);
                    }
                    return false;

                });

            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        private async Task SaveUDSRepositoryAsync(UDSBuildModel model, string idContainer)
        {
            try
            {
                if (model.UDSRepository != null)
                {
                    if (!model.UDSRepository.ActiveDate.HasValue)
                    {
                        _logger.WriteError(new LogMessage("Active date is empty"), LogCategories);
                        throw new ArgumentNullException("Active date is empty");
                    }

                    UDSRepository udsRepository = new UDSRepository(model.UDSRepository.Id);
                    UDSSchemaRepository lastSchemaRepository = CurrentUDSSchemaRepository;

                    _logger.WriteInfo(new LogMessage(string.Concat("Last valid Repository : ", lastSchemaRepository.UniqueId)), LogCategories);

                    udsRepository.ActiveDate = model.UDSRepository.ActiveDate.Value;
                    udsRepository.ExpiredDate = null;
                    udsRepository.LastChangedDate = DateTimeOffset.UtcNow;
                    udsRepository.Version = 1;
                    udsRepository.ModuleXML = model.XMLContent;
                    udsRepository.Name = model.UDSRepository.Name;
                    udsRepository.SchemaRepository = lastSchemaRepository;
                    udsRepository.SequenceCurrentYear = (short)model.UDSRepository.ActiveDate.Value.Year;
                    udsRepository.Alias = model.UDSRepository.Alias;
                    udsRepository.DSWEnvironment = model.UDSRepository.DSWEnvironment;
                    udsRepository.Status = DocSuiteWeb.Entity.UDS.UDSRepositoryStatus.Confirmed;
                    udsRepository.Container = new DocSuiteWeb.Entity.Commons.Container() { EntityShortId = Convert.ToInt16(idContainer) };

                    udsRepository = await UpdateUDSRepositoryAsync(udsRepository);

                    _logger.WriteInfo(new LogMessage(string.Concat("Repository is updated: ", udsRepository.UniqueId)), LogCategories);
                }

            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        private async Task CreateBiblosDSArchiveAsync(UDSEntity uds, UDSStorageFacade udsStorageFacade, bool isMainDocument = false)
        {
            try
            {
                await udsStorageFacade.GenerateStorageAsync(async (udsArchive, attributeGroups, attributeModes) =>
                {
                    BiblosDS.BiblosDSManagement.AttributeMode attributeMode_ReadOnly = attributeModes.Single(f => f.IdMode == 0);
                    BiblosDS.BiblosDSManagement.AttributeMode attributeMode_ModifyAlways = attributeModes.Single(f => f.IdMode == 3);
                    BiblosDS.BiblosDSManagement.AttributeMode attributeMode_ModifyNotArchived = attributeModes.Single(f => f.IdMode == 2);

                    BiblosDS.BiblosDSManagement.AttributeGroup attributeGroup_chain = attributeGroups.SingleOrDefault(a => a.GroupType.Equals(BiblosDS.BiblosDSManagement.AttributeGroupType.Chain));
                    BiblosDS.BiblosDSManagement.AttributeGroup attributeGroup_default = attributeGroups.SingleOrDefault(a => a.GroupType.Equals(BiblosDS.BiblosDSManagement.AttributeGroupType.Undefined));

                    await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_default, attributeMode_ReadOnly, AttributeHelper.AttributeName_Signature, "System.String", true);
                    await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_default, attributeMode_ReadOnly, AttributeHelper.AttributeName_Filename, "System.String", true);
                    await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_default, attributeMode_ModifyAlways, AttributeHelper.AttributeName_PrivacyLevel, "System.Int64", false);
                    await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_default, attributeMode_ModifyAlways, AttributeHelper.AttributeName_SecureDocumentId, "System.String", false);

                    if (isMainDocument)
                    {
                        await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_chain, attributeMode_ModifyAlways, AttributeHelper.AttributeName_UDSSubject, "System.String", false);
                        await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_chain, attributeMode_ModifyAlways, AttributeHelper.AttributeName_UDSYear, "System.Int64", true);
                        await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_chain, attributeMode_ModifyAlways, AttributeHelper.AttributeName_UDSNumber, "System.Int64", true);
                        await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_chain, attributeMode_ModifyAlways, AttributeHelper.AttributeName_Date, "System.DateTime", true, isMainDate: true);

                        foreach (Metadata metadata in uds.MetaData)
                        {
                            await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_chain, attributeMode_ModifyAlways,
                                metadata.PropertyName, metadata.BiblosPropertyType.FullName, metadata.Required);
                        }
                    }
                }, BiblosDS_Storage_MainPath, BiblosDS_Storage_StorageType, isMainDocument);

            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }


        private async Task<bool> GenerateUDSCodeAsync(UDSEntity uds)
        {
            try
            {
                CreateUDSNamespace codeGeneration = new CreateUDSNamespace(_logger, uds,
                    SolutionPath, ProjectName, DBSchema);
                _logger.WriteInfo(new LogMessage("Creating uds entities"), LogCategories);
                if (await codeGeneration.ExecuteAsync())
                {
                    _logger.WriteInfo(new LogMessage("Creating uds controller"), LogCategories);
                    CreateUDSControllerNamespace controllerGeneration = new CreateUDSControllerNamespace(_logger, uds,
                        SolutionPath, ProjectName);
                    if (await controllerGeneration.ExecuteAsync())
                    {
                        _logger.WriteInfo(new LogMessage("Building code"), LogCategories);
                        CompilerBuilder builder = new CompilerBuilder(_logger, CompilationLoggerPath,
                            controllerGeneration.ProjectFilePath, OutputDLLPath);
                        if (await builder.BuildAsync())
                        {
                            _logger.WriteInfo(new LogMessage("Build successful"), LogCategories);
                            return InstallUDS();
                        }
                    }
                }
                _logger.WriteError(new LogMessage("Roslyn generate error in codes generation"), LogCategories);
                return false;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                return false;
            }
        }


        #endregion
    }
}
