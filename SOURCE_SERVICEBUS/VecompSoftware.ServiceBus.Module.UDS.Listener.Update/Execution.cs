using System;
using System.Collections.Generic;
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
using VecompSoftware.ServiceBus.Module.UDS.Storage.Smo;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands.Models.UDS;

namespace VecompSoftware.ServiceBus.Module.UDS.Listener.Update
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class Execution : UDSBaseExecution<ICommandUpdateUDS>
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
        public override async Task ExecuteAsync(ICommandUpdateUDS command)
        {
            _logger.WriteInfo(new LogMessage(string.Concat(command.CommandName, " is arrived")), LogCategories);
            _logger.WriteInfo(new LogMessage("starting update storage"), LogCategories);
            UDSStorageFacade storage = new UDSStorageFacade(_logger, command.ContentType.ContentTypeValue.XMLContent, CurrentUDSSchemaRepository.SchemaXML, _administrationClient, DBSchema);
            UDSEntity uds = await UpdateStorageAsync(storage);

            _logger.WriteInfo(new LogMessage(string.Concat(command.ContentType.Id, " model evaluating ... ")), LogCategories);
            await UpdateTableAndCodeAsync(uds);
            _logger.WriteInfo(new LogMessage("Tables and codes aligned"), LogCategories);

            await UpdateArchiveAsync(uds, storage);
            _logger.WriteInfo(new LogMessage("Archive updated"), LogCategories);

            command.ContentType.ContentTypeValue.XMLContent = storage.UDS.SerializeToXml();
            if (storage.UDS.Model.RequiredRevisionUDSRepository)
            {
                await UpdateUDSRepositoryAsync(command.ContentType.ContentTypeValue);
                _logger.WriteInfo(new LogMessage("Repository updated"), LogCategories);
            }
            else
            {
                await UpdateUDSRepositoryAsync(command.ContentType.ContentTypeValue, false);
                _logger.WriteInfo(new LogMessage("Confirmed repository updated and draft repository deleted"), LogCategories);
            }
            _logger.WriteInfo(new LogMessage("message completed."), LogCategories);
        }

        /// <summary>
        /// Aggiorno la UDSRepository attiva in questo momento.
        /// Aggiongo la repository presente nel comando
        /// Associo l'ultimo schema
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task UpdateUDSRepositoryAsync(UDSBuildModel model, bool requiredRevisionUDSRepository = true)
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

                    UDSRepository currentRepository = await GetCurrentUDSRepositoryAsync(model.UDSRepository.Name);
                    UDSRepository newRepository = await GetUDSRepositoryAsync(model.UDSRepository.Id);
                    _logger.WriteInfo(new LogMessage($"Last valid Repository is {CurrentUDSSchemaRepository.UniqueId}"), LogCategories);

                    if (requiredRevisionUDSRepository)
                    {
                        currentRepository.ExpiredDate = model.UDSRepository.ActiveDate;
                        newRepository.ActiveDate = model.UDSRepository.ActiveDate.Value;
                        newRepository.ExpiredDate = null;
                        newRepository.LastChangedDate = DateTimeOffset.UtcNow;
                        newRepository.ModuleXML = model.XMLContent;
                        newRepository.Name = model.UDSRepository.Name;
                        newRepository.SchemaRepository = CurrentUDSSchemaRepository;
                        newRepository.Version = ++currentRepository.Version;
                        newRepository.Status = DocSuiteWeb.Entity.UDS.UDSRepositoryStatus.Confirmed;
                        newRepository.SequenceCurrentYear = currentRepository.SequenceCurrentYear;
                        newRepository.SequenceCurrentNumber = currentRepository.SequenceCurrentNumber;
                        currentRepository = await UpdateUDSRepositoryAsync(currentRepository);
                        _logger.WriteInfo(new LogMessage($"Repository {currentRepository.UniqueId}/{currentRepository.Name}/{newRepository.Version} has expired"), LogCategories);
                        newRepository = await UpdateUDSRepositoryAsync(newRepository);
                        _logger.WriteInfo(new LogMessage($"Repository {newRepository.UniqueId}/{newRepository.Name}/{newRepository.Version} has been successfully confirmed."), LogCategories);
                    }
                    else
                    {
                        currentRepository.ModuleXML = model.XMLContent;
                        currentRepository = await UpdateUDSRepositoryAsync(currentRepository);
                        _logger.WriteInfo(new LogMessage($"Repository {newRepository.UniqueId}/{newRepository.Name}/{newRepository.Version} has been successfully updated."), LogCategories);
                        newRepository = await DeleteUDSRepositoryAsync(newRepository);
                        if (newRepository != null)
                        {
                            _logger.WriteInfo(new LogMessage($"Draft repository {newRepository.UniqueId}/{newRepository.Name}/{newRepository.Version} has been deleted"), LogCategories);
                        }
                        else
                        {
                            _logger.WriteWarning(new LogMessage($"Error occoured during deleting draft repository {newRepository.UniqueId}/{newRepository.Name}/{newRepository.Version}"), LogCategories);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        private async Task<bool> UpdateTableAndCodeAsync(UDSEntity uds)
        {
            try
            {
                if (uds != null)
                {
                    CreateUDSNamespace codeGeneration = new CreateUDSNamespace(_logger, uds,
                        SolutionPath, ProjectName, DBSchema);
                    _logger.WriteInfo(new LogMessage("Update UDS entities"), LogCategories);
                    if (await codeGeneration.ExecuteAsync())
                    {
                        _logger.WriteInfo(new LogMessage("Creating uds controller"), LogCategories);
                        CreateUDSControllerNamespace controllerGeneration = new CreateUDSControllerNamespace(_logger, uds,
                            SolutionPath, ProjectName);
                        if (await controllerGeneration.ExecuteAsync())
                        {
                            _logger.WriteInfo(new LogMessage("Building code ... "), LogCategories);
                            CompilerBuilder builder = new CompilerBuilder(_logger, CompilationLoggerPath,
                                controllerGeneration.ProjectFilePath, OutputDLLPath);
                            if (await builder.BuildAsync())
                            {
                                _logger.WriteInfo(new LogMessage("Build successful"), LogCategories);
                                InstallUDS();
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
        }

        private async Task<UDSEntity> UpdateStorageAsync(UDSStorageFacade storage)
        {
            UDSEntity entity = null;
            MetadataEntity metadatas = new MetadataEntity(_logger);
            try
            {
                //verifica esistenza tabella
                using (SmoContext smo = new SmoContext(ConnectionString, DBSchema))
                {
                    if (!smo.TableExist(storage.Builder.UDSTableName))
                    {
                        _logger.WriteError(new LogMessage(string.Concat(storage.Builder.UDSTableName, " not exist")), LogCategories);
                        throw new Exception(string.Concat(storage.Builder.UDSTableName, " - UDS structures not found"));
                    }
                }

                await storage.UpdateStorageAsync(ConnectionString);
                _logger.WriteInfo(new LogMessage("update storage completed"), LogCategories);
                entity = metadatas.LoadMetadata(storage);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw ex;
            }
            return entity;
        }


        private async Task UpdateArchiveAsync(UDSEntity uds, UDSStorageFacade udsStorageFacade, bool isMainDocument = false)
        {
            _logger.WriteInfo(new LogMessage("starting update archive"), LogCategories);

            if (udsStorageFacade.UDS.Model.Documents != null)
            {
                IList<BiblosDS.BiblosDSManagement.Archive> archives = _administrationClient.GetArchives().ToList();
                string miscellaneousArchiveName = string.Concat("Archive_UDS_Miscellaneous_", udsStorageFacade.UDS.Model.Title);
                BiblosDS.BiblosDSManagement.Archive miscellaneousArchive = new BiblosDS.BiblosDSManagement.Archive
                {
                    Name = miscellaneousArchiveName
                };

                if (udsStorageFacade.UDS.Model.Documents.Document != null)
                {
                    BiblosDS.BiblosDSManagement.Archive udsArchive = archives.SingleOrDefault(a => a.Name.Equals(udsStorageFacade.UDS.Model.Documents.Document.BiblosArchive));
                    if (udsArchive == null)
                    {
                        if (udsStorageFacade.UDS.Model.Documents.Document.CreateBiblosArchive)
                        {
                            await CreateBiblosDSArchiveAsync(uds, udsStorageFacade, true);
                            _logger.WriteInfo(new LogMessage("Archive created successfully"), LogCategories);
                        }
                        else
                        {
                            _logger.WriteError(new LogMessage(string.Format("Archive {0} to update not found", udsStorageFacade.UDS.Model.Documents.Document.BiblosArchive)), LogCategories);
                        }
                    }
                    else
                    {
                        BiblosDS.BiblosDSManagement.Attribute[] attributes = _administrationClient.GetAttributesFromArchive(udsArchive.IdArchive);
                        Task<BiblosDS.BiblosDSManagement.AttributeMode[]> task_attributeModes = _administrationClient.GetAttributeModesAsync();
                        BiblosDS.BiblosDSManagement.AttributeMode[] attributeModes = await task_attributeModes;
                        BiblosDS.BiblosDSManagement.AttributeMode attributeMode_ReadOnly = attributeModes.Single(f => f.IdMode == 0);
                        BiblosDS.BiblosDSManagement.AttributeMode attributeMode_ModifyAlways = attributeModes.Single(f => f.IdMode == 3);
                        BiblosDS.BiblosDSManagement.AttributeMode attributeMode_ModifyNotArchived = attributeModes.Single(f => f.IdMode == 2);

                        Metadata[] addedMetadata = uds.MetaData.Where(m => !attributes.Any(a => a.Name.Equals(m.PropertyName))).ToArray();

                        string[] requiredAttributes = { AttributeHelper.AttributeName_Signature, AttributeHelper.AttributeName_Filename, AttributeHelper.AttributeName_UDSSubject,
                            AttributeHelper.AttributeName_UDSYear, AttributeHelper.AttributeName_UDSNumber, AttributeHelper.AttributeName_Date};
                        BiblosDS.BiblosDSManagement.Attribute[] dynamicAttributes = attributes.Where(a => !requiredAttributes.Any(r => r.Equals(a.Name))).ToArray();
                        BiblosDS.BiblosDSManagement.Attribute[] deprecatedAttribute = dynamicAttributes.Where(a => !uds.MetaData.Any(m => m.PropertyName.Equals(a.Name))).ToArray();

                        //creo i nuovi attributi aggiunti all'archivio
                        foreach (Metadata metadata in addedMetadata)
                        {
                            BiblosDS.BiblosDSManagement.Attribute attribute = attributes.FirstOrDefault(a => a.AttributeGroup.GroupType == BiblosDS.BiblosDSManagement.AttributeGroupType.Chain);
                            BiblosDS.BiblosDSManagement.AttributeGroup attributeGroup_chain = attribute.AttributeGroup;
                            await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_chain, attributeMode_ModifyAlways,
                                    metadata.PropertyName, metadata.BiblosPropertyType.FullName, metadata.Required);
                        }
                        //disabilito gli attributi tolti dall'archivio
                        foreach (BiblosDS.BiblosDSManagement.Attribute item in deprecatedAttribute)
                        {
                            item.IsRequired = false;
                            item.IsVisible = false;
                            item.IsVisibleForUser = false;
                            item.Mode = attributeMode_ReadOnly;
                            _logger.WriteInfo(new LogMessage(string.Concat("Updating biblosds attribute ", item.Name)), LogCategories);
                            await _administrationClient.UpdateAttributeAsync(item);
                        }
                    }

                }

                if (udsStorageFacade.UDS.Model.Documents.DocumentAttachment != null && udsStorageFacade.UDS.Model.Documents.DocumentAttachment.CreateBiblosArchive)
                {
                    BiblosDS.BiblosDSManagement.Archive udsAttachmentArchive = archives.SingleOrDefault(a => a.Name.Equals(udsStorageFacade.UDS.Model.Documents.DocumentAttachment.BiblosArchive));

                    if (udsAttachmentArchive == null)
                    {
                        udsStorageFacade.UDS.Model.Documents.DocumentAttachment.BiblosArchive = miscellaneousArchiveName;
                        if (!archives.Any(a => a.Name.Equals(miscellaneousArchiveName)))
                        {
                            await CreateBiblosDSArchiveAsync(uds, udsStorageFacade);
                            archives.Add(miscellaneousArchive);
                            _logger.WriteInfo(new LogMessage("Archive Miscellaneous created successfully"), LogCategories);
                        }
                    }
                }

                if (udsStorageFacade.UDS.Model.Documents.DocumentAnnexed != null && udsStorageFacade.UDS.Model.Documents.DocumentAnnexed.CreateBiblosArchive)
                {
                    BiblosDS.BiblosDSManagement.Archive udsAnnexedArchive = archives.SingleOrDefault(a => a.Name.Equals(udsStorageFacade.UDS.Model.Documents.DocumentAnnexed.BiblosArchive));

                    if (udsAnnexedArchive == null)
                    {
                        udsStorageFacade.UDS.Model.Documents.DocumentAnnexed.BiblosArchive = miscellaneousArchiveName;
                        if (!archives.Any(a => a.Name.Equals(miscellaneousArchiveName)))
                        {
                            await CreateBiblosDSArchiveAsync(uds, udsStorageFacade);
                            archives.Add(miscellaneousArchive);
                            _logger.WriteInfo(new LogMessage("Archive Miscellaneous created successfully"), LogCategories);
                        }
                    }
                }

                if ((udsStorageFacade.UDS.Model.Documents.Document != null && udsStorageFacade.UDS.Model.Documents.Document.DematerialisationEnabled) ||
                    (udsStorageFacade.UDS.Model.Documents.DocumentAttachment != null && udsStorageFacade.UDS.Model.Documents.DocumentAttachment.DematerialisationEnabled) ||
                    (udsStorageFacade.UDS.Model.Documents.DocumentAnnexed != null && udsStorageFacade.UDS.Model.Documents.DocumentAnnexed.DematerialisationEnabled))
                {
                    if (!archives.Any(a => a.Name.Equals(miscellaneousArchiveName)))
                    {
                        await CreateBiblosDSArchiveAsync(uds, udsStorageFacade);
                        archives.Add(miscellaneousArchive);
                        _logger.WriteInfo(new LogMessage("Archive Miscellaneous created successfully"), LogCategories);
                    }

                    if (udsStorageFacade.UDS.Model.Documents.DocumentDematerialisation == null || !udsStorageFacade.UDS.Model.Documents.DocumentDematerialisation.Instances.Any())
                    {
                        udsStorageFacade.AddDocumentDematerialisation(udsStorageFacade.UDS.Model.Documents, miscellaneousArchiveName);
                    }
                }
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

                    await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_default, attributeMode_ReadOnly, "Signature", "System.String", true);
                    await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_default, attributeMode_ReadOnly, "Filename", "System.String", true);
                    await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_default, attributeMode_ModifyAlways, AttributeHelper.AttributeName_PrivacyLevel, "System.Int64", false);
                    await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_default, attributeMode_ModifyAlways, AttributeHelper.AttributeName_SecureDocumentId, "System.String", false);

                    if (isMainDocument)
                    {
                        await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_chain, attributeMode_ModifyNotArchived, "Subject", "System.String", false);
                        await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_chain, attributeMode_ReadOnly, "Year", "System.Int64", true);
                        await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_chain, attributeMode_ReadOnly, "Number", "System.Int64", true);
                        await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_chain, attributeMode_ReadOnly, "Date", "System.DateTime", true, isMainDate: true);

                        foreach (Metadata metadata in uds.MetaData)
                        {
                            await udsStorageFacade.GenerateBiblosDSAttribute(udsArchive, attributeGroup_chain,
                                metadata.Required ? attributeMode_ReadOnly : attributeMode_ModifyNotArchived,
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


        #endregion
    }
}
