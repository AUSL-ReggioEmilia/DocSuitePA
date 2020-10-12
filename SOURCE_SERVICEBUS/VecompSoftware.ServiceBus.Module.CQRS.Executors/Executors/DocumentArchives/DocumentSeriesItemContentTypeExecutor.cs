using System;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Core.Command.CQRS.Events.Entities.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Commands.Entities.DocumentArchives;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.DocumentArchives
{
    public class DocumentSeriesItemContentTypeExecutor : BaseCommonExecutor, IDocumentSeriesItemContentTypeExecutor, IDocumentUnitEntity
    {
        #region [ Fields ]
        private readonly IWebAPIClient _webApiClient;
        private readonly BiblosClient _biblosClient;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]
        public DocumentSeriesItemContentTypeExecutor(ILogger logger, IWebAPIClient webApiClient, BiblosClient biblosClient, ServiceBus.ServiceBusClient serviceBusClient)
            : base(logger, webApiClient, biblosClient, serviceBusClient)
        {
            _logger = logger;
            _webApiClient = webApiClient;
            _biblosClient = biblosClient;
        }
        #endregion

        internal override IEvent CreateInsertEvent(ICommandCQRSCreate command, DocumentUnit documentUnit = null)
        {
            IEvent evt = null;
            try
            {
                DocumentSeriesItem documentSeriesItem = ((ICommandCreateDocumentSeriesItem)command).ContentType.ContentTypeValue;
                evt = new EventCreateDocumentSeriesItem(command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, documentSeriesItem, ((ICommandCQRSFascicolable)command).CategoryFascicle, documentUnit);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("UDS, CreateInsertEvent Error: ", command.GetType())), ex, LogCategories);
                throw ex;
            }

            return evt;
        }

        internal override IEvent CreateUpdateEvent(ICommandCQRSUpdate command, DocumentUnit documentUnit = null)
        {
            IEvent evt = null;
            try
            {
                DocumentSeriesItem documentSeriesItem = ((ICommandUpdateDocumentSeriesItem)command).ContentType.ContentTypeValue;
                evt = new EventUpdateDocumentSeriesItem(command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, documentSeriesItem, ((ICommandCQRSFascicolable)command).CategoryFascicle, documentUnit);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("DocumentSeriesItem, CreateUpdateEvent Error: ", command.GetType())), ex, LogCategories);
                throw ex;
            }

            return evt;
        }

        internal override async Task<DocumentUnit> MappingInsertAsync(IContentBase entity, IIdentityContext identity)
        {
            DocumentUnit documentUnit = new DocumentUnit();
            try
            {
                DocumentSeriesItem documentSeriesItem = (DocumentSeriesItem)entity;

                #region [ Base ]

                documentUnit.EntityId = documentSeriesItem.EntityId;
                documentUnit.Environment = (int)DocSuiteWeb.Entity.Commons.DSWEnvironmentType.DocumentSeries;
                documentUnit.LastChangedDate = null;
                documentUnit.LastChangedUser = null;
                documentUnit.Number = documentSeriesItem.Number.Value;
                documentUnit.RegistrationDate = documentSeriesItem.RegistrationDate;
                documentUnit.RegistrationUser = documentSeriesItem.RegistrationUser;
                documentUnit.Subject = documentSeriesItem.Subject;
                documentUnit.Title = string.Concat(documentSeriesItem.Year.Value, "/", documentSeriesItem.Number.Value.ToString("0000000"));
                documentUnit.UniqueId = documentSeriesItem.UniqueId;
                documentUnit.Year = (short)(documentSeriesItem.Year.Value);
                documentUnit.DocumentUnitName = documentSeriesItem.DocumentSeries.Name;
                documentUnit.Status = documentSeriesItem.Status == DocumentSeriesItemStatus.Active ? DocumentUnitStatus.Active : DocumentUnitStatus.Inactive;
                #endregion

                #region [ Navigation Properties ]

                documentUnit.UDSRepository = null;
                documentUnit.Category = documentSeriesItem.Category;
                documentUnit.Container = documentSeriesItem.DocumentSeries.Container;
                documentUnit.Fascicle = await _webApiClient.GetFascicleAsync(documentSeriesItem.UniqueId);

                foreach (DocumentSeriesItemRole item in documentSeriesItem.DocumentSeriesItemRoles)
                {
                    documentUnit.DocumentUnitRoles.Add(new DocumentUnitRole()
                    {
                        UniqueIdRole = item.Role.UniqueId,
                        RegistrationDate = DateTimeOffset.UtcNow,
                        RegistrationUser = identity.User,
                        AuthorizationRoleType = GetRoleType(string.Empty)
                    });
                }

                if (documentSeriesItem.IdAnnexed.HasValue && documentSeriesItem.IdAnnexed.Value != Guid.Empty)
                {
                    AddDocumentUnitChain(documentUnit, documentSeriesItem.IdAnnexed.Value, ChainType.AnnexedChain, identity, documentSeriesItem.DocumentSeries.Container.DocumentSeriesAnnexedLocation.ProtocolArchive);
                }

                if (documentSeriesItem.IdUnpublishedAnnexed.HasValue && documentSeriesItem.IdUnpublishedAnnexed.Value != Guid.Empty)
                {
                    AddDocumentUnitChain(documentUnit, documentSeriesItem.IdUnpublishedAnnexed.Value, ChainType.UnpublishedAnnexedChain, identity, documentSeriesItem.DocumentSeries.Container.DocumentSeriesUnpublishedAnnexedLocation.ProtocolArchive);
                }

                if (documentSeriesItem.IdMain.HasValue && documentSeriesItem.IdMain.Value != Guid.Empty)
                {
                    BiblosDS.BiblosDS.Document document = _biblosClient.Document.GetDocumentChildren(documentSeriesItem.IdMain.Value).FirstOrDefault();
                    AddDocumentUnitChain(documentUnit, documentSeriesItem.IdMain.Value, ChainType.MainChain, identity, documentSeriesItem.DocumentSeries.Container.DocumentSeriesLocation.ProtocolArchive, document != null ? document.Name : null);
                }

                if (documentSeriesItem.DematerialisationChainId.HasValue && documentSeriesItem.DematerialisationChainId.Value != Guid.Empty)
                {
                    AddDocumentUnitChain(documentUnit, documentSeriesItem.DematerialisationChainId.Value, ChainType.DematerialisationChain, identity, documentSeriesItem.DocumentSeries.Container.DocumentSeriesAnnexedLocation.ProtocolArchive);
                }

                #endregion
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("DocumentSeriesItem, MappingInsertAsync Error: "), ex, LogCategories);
                throw ex;
            }

            return documentUnit;
        }

        internal override async Task<DocumentUnit> MappingUpdateAsync(IContentBase entity, DocumentUnit documentUnit, IIdentityContext identity)
        {
            try
            {
                DocumentSeriesItem documentSeriesItem = (DocumentSeriesItem)entity;

                #region [ Base ]

                documentUnit.LastChangedDate = documentSeriesItem.LastChangedDate;
                documentUnit.LastChangedUser = documentSeriesItem.LastChangedUser;
                documentUnit.Subject = documentSeriesItem.Subject;
                documentUnit.Title = documentUnit.Title;
                documentUnit.Status = documentSeriesItem.Status == DocumentSeriesItemStatus.Active ? DocumentUnitStatus.Active : DocumentUnitStatus.Inactive;

                #endregion

                #region [ Navigation Properties ]

                if (documentUnit.Category.UniqueId != documentSeriesItem.Category.UniqueId)
                {
                    documentUnit.Category = documentSeriesItem.Category;
                }

                if (documentUnit.Container.UniqueId != documentSeriesItem.DocumentSeries.Container.UniqueId)
                {
                    documentUnit.Container = documentSeriesItem.DocumentSeries.Container;
                }

                documentUnit.Fascicle = await _webApiClient.GetFascicleAsync(documentSeriesItem.UniqueId);

                if (documentSeriesItem.DocumentSeriesItemRoles == null || !documentSeriesItem.DocumentSeriesItemRoles.Any())
                {
                    documentSeriesItem.DocumentSeriesItemRoles.Clear();
                }
                if (documentSeriesItem.DocumentSeriesItemRoles != null)
                {
                    foreach (DocumentSeriesItemRole item in documentSeriesItem.DocumentSeriesItemRoles.Where(t => !documentUnit.DocumentUnitRoles.Any(x => x.UniqueIdRole == t.Role.UniqueId)).ToList())
                    {
                        documentUnit.DocumentUnitRoles.Add(new DocumentUnitRole()
                        {
                            UniqueIdRole = item.Role.UniqueId,
                            RegistrationDate = DateTimeOffset.UtcNow,
                            RegistrationUser = identity.User,
                            AuthorizationRoleType = GetRoleType(string.Empty)
                        });
                    }

                    foreach (DocumentUnitRole item in documentUnit.DocumentUnitRoles.Where(t => !documentSeriesItem.DocumentSeriesItemRoles.Any(x => x.Role.UniqueId == t.UniqueIdRole)).ToList())
                    {
                        documentUnit.DocumentUnitRoles.Remove(item);
                    }
                }


                if (documentSeriesItem.IdAnnexed.HasValue && documentSeriesItem.IdAnnexed.Value != Guid.Empty)
                {
                    DocumentUnitChain annexed = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.AnnexedChain).FirstOrDefault();
                    if (annexed != null)
                    {
                        if (annexed.IdArchiveChain != documentSeriesItem.IdAnnexed.Value)
                        {
                            documentUnit.DocumentUnitChains.Remove(annexed);
                            AddDocumentUnitChain(documentUnit, documentSeriesItem.IdAnnexed.Value, ChainType.AnnexedChain, identity, documentSeriesItem.DocumentSeries.Container.DocumentSeriesAnnexedLocation.ProtocolArchive);
                        }
                    }
                    else
                    {
                        AddDocumentUnitChain(documentUnit, documentSeriesItem.IdAnnexed.Value, ChainType.AnnexedChain, identity, documentSeriesItem.DocumentSeries.Container.DocumentSeriesAnnexedLocation.ProtocolArchive);
                    }
                }
                else
                {
                    foreach (DocumentUnitChain chain in documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.AnnexedChain).ToList())
                    {
                        documentUnit.DocumentUnitChains.Remove(chain);
                    }
                }

                if (documentSeriesItem.IdUnpublishedAnnexed.HasValue && documentSeriesItem.IdUnpublishedAnnexed.Value != Guid.Empty)
                {
                    DocumentUnitChain unpublishedAnnexed = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.UnpublishedAnnexedChain).FirstOrDefault();
                    if (unpublishedAnnexed != null)
                    {
                        if (unpublishedAnnexed.IdArchiveChain != documentSeriesItem.IdUnpublishedAnnexed.Value)
                        {
                            documentUnit.DocumentUnitChains.Remove(unpublishedAnnexed);
                            AddDocumentUnitChain(documentUnit, documentSeriesItem.IdUnpublishedAnnexed.Value, ChainType.UnpublishedAnnexedChain, identity, documentSeriesItem.DocumentSeries.Container.DocumentSeriesUnpublishedAnnexedLocation.ProtocolArchive);
                        }
                    }
                    else
                    {
                        AddDocumentUnitChain(documentUnit, documentSeriesItem.IdUnpublishedAnnexed.Value, ChainType.UnpublishedAnnexedChain, identity, documentSeriesItem.DocumentSeries.Container.DocumentSeriesUnpublishedAnnexedLocation.ProtocolArchive);
                    }
                }
                else
                {
                    foreach (DocumentUnitChain chain in documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.UnpublishedAnnexedChain).ToList())
                    {
                        documentUnit.DocumentUnitChains.Remove(chain);
                    }
                }

                if (documentSeriesItem.IdMain.HasValue && documentSeriesItem.IdMain.Value != Guid.Empty)
                {
                    DocumentUnitChain main = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.MainChain).FirstOrDefault();
                    if (main != null)
                    {
                        if (main.IdArchiveChain != documentSeriesItem.IdMain.Value)
                        {
                            documentUnit.DocumentUnitChains.Remove(main);
                            BiblosDS.BiblosDS.Document document = _biblosClient.Document.GetDocumentChildren(documentSeriesItem.IdMain.Value).FirstOrDefault();
                            AddDocumentUnitChain(documentUnit, documentSeriesItem.IdMain.Value, ChainType.MainChain, identity, documentSeriesItem.DocumentSeries.Container.DocumentSeriesLocation.ProtocolArchive, document != null ? document.Name : null);
                        }
                    }
                    else
                    {
                        BiblosDS.BiblosDS.Document document = _biblosClient.Document.GetDocumentChildren(documentSeriesItem.IdMain.Value).FirstOrDefault();
                        AddDocumentUnitChain(documentUnit, documentSeriesItem.IdMain.Value, ChainType.MainChain, identity, documentSeriesItem.DocumentSeries.Container.DocumentSeriesLocation.ProtocolArchive, document != null ? document.Name : null);
                    }
                }
                else
                {
                    foreach (DocumentUnitChain chain in documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.MainChain).ToList())
                    {
                        documentUnit.DocumentUnitChains.Remove(chain);
                    }
                }

                if (documentSeriesItem.DematerialisationChainId.HasValue && documentSeriesItem.DematerialisationChainId.Value != Guid.Empty)
                {
                    DocumentUnitChain dematerialisation = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.DematerialisationChain).FirstOrDefault();
                    if (dematerialisation != null)
                    {
                        if (dematerialisation.IdArchiveChain != documentSeriesItem.DematerialisationChainId.Value)
                        {
                            documentUnit.DocumentUnitChains.Remove(dematerialisation);
                            AddDocumentUnitChain(documentUnit, documentSeriesItem.DematerialisationChainId.Value, ChainType.DematerialisationChain, identity, documentSeriesItem.DocumentSeries.Container.DocumentSeriesAnnexedLocation.ProtocolArchive);
                        }
                    }
                    else
                    {
                        AddDocumentUnitChain(documentUnit, documentSeriesItem.DematerialisationChainId.Value, ChainType.DematerialisationChain, identity, documentSeriesItem.DocumentSeries.Container.DocumentSeriesAnnexedLocation.ProtocolArchive);
                    }
                }
                else
                {
                    foreach (DocumentUnitChain chain in documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.DematerialisationChain).ToList())
                    {
                        documentUnit.DocumentUnitChains.Remove(chain);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("DocumentSeriesItem, MappingUpdateAsync Error: "), ex, LogCategories);
                throw ex;
            }

            return documentUnit;
        }

    }
}
