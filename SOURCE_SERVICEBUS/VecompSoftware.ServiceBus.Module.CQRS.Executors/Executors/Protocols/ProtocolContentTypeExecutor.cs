using System;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.Core.Command.CQRS.Events.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Commands.Entities.Protocols;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Protocols
{
    public class ProtocolContentTypeExecutor : BaseCommonExecutor, IProtocolContentTypeExecutor, IDocumentUnitEntity
    {
        #region [ Fields ]
        private readonly IWebAPIClient _webApiClient;
        private readonly BiblosDS.BiblosClient _biblosClient;
        private readonly ILogger _logger;
        #endregion

        #region [ Properties ]
        public Guid? CollaborationUniqueId { get; set; }
        public int? CollaborationId { get; set; }
        public string CollaborationTemplateName { get; set; }
        #endregion

        #region [ Constructor ]
        public ProtocolContentTypeExecutor(ILogger logger, IWebAPIClient webApiClient, BiblosClient biblosClient, ServiceBus.ServiceBusClient serviceBusClient)
            : base(logger, webApiClient, biblosClient, serviceBusClient)
        {
            _logger = logger;
            _webApiClient = webApiClient;
            _biblosClient = biblosClient;
        }
        #endregion

        #region [ Methods ]

        internal override IEvent CreateInsertEvent(ICommandCQRSCreate command, DocumentUnit documentUnit = null)
        {
            IEvent evt = null;
            Protocol protocol;
            try
            {
                protocol = ((ICommandCreateProtocol)command).ContentType.ContentTypeValue;

                CollaborationUniqueId = null;
                if (command.CustomProperties.Where(x => x.Key == CustomPropertyName.COLLABORATION_UNIQUE_ID).Any() && Guid.TryParse(command.CustomProperties.Where(x => x.Key == CustomPropertyName.COLLABORATION_UNIQUE_ID).FirstOrDefault().Value.ToString(), out Guid guidResult))
                {
                    CollaborationUniqueId = guidResult;
                }
                CollaborationId = null;
                if (command.CustomProperties.Where(x => x.Key == CustomPropertyName.COLLABORATION_ID).Any() && int.TryParse(command.CustomProperties.Where(x => x.Key == CustomPropertyName.COLLABORATION_ID).FirstOrDefault().Value.ToString(), out int intResult))
                {
                    CollaborationId = intResult;
                }
                CollaborationTemplateName = string.Empty;
                if (command.CustomProperties.Where(x => x.Key == CustomPropertyName.COLLABORATION_TEMPLATE_NAME).Any())
                {
                    CollaborationTemplateName = command.CustomProperties.Where(x => x.Key == CustomPropertyName.COLLABORATION_TEMPLATE_NAME).FirstOrDefault().Value.ToString();
                }

                evt = new EventCreateProtocol(command.TenantName, command.TenantId, command.TenantAOOId, CollaborationUniqueId, CollaborationId, CollaborationTemplateName, command.Identity, protocol, ((ICommandCQRSFascicolable)command).CategoryFascicle, documentUnit);

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("Protocol, CreateInsertEvent Error: ", command.GetType())), ex, LogCategories);
                throw ex;
            }

            return evt;
        }

        internal override IEvent CreateUpdateEvent(ICommandCQRSUpdate command, DocumentUnit documentUnit = null)
        {
            IEvent evt = null;
            try
            {
                Protocol protocol;

                protocol = ((ICommandUpdateProtocol)command).ContentType.ContentTypeValue;

                CollaborationUniqueId = null;
                if (command.CustomProperties.Any(x => x.Key == CustomPropertyName.COLLABORATION_UNIQUE_ID) && Guid.TryParse(command.CustomProperties.Single(x => x.Key == CustomPropertyName.COLLABORATION_UNIQUE_ID).Value.ToString(), out Guid guidResult))
                {
                    CollaborationUniqueId = guidResult;
                }
                CollaborationId = null;
                if (command.CustomProperties.Any(x => x.Key == CustomPropertyName.COLLABORATION_ID) && int.TryParse(command.CustomProperties.Single(x => x.Key == CustomPropertyName.COLLABORATION_ID).Value.ToString(), out int intResult))
                {
                    CollaborationId = intResult;
                }

                CollaborationTemplateName = string.Empty;
                if (command.CustomProperties.Any(x => x.Key == CustomPropertyName.COLLABORATION_TEMPLATE_NAME))
                {
                    CollaborationTemplateName = command.CustomProperties.Single(x => x.Key == CustomPropertyName.COLLABORATION_TEMPLATE_NAME).Value.ToString();
                }

                if (protocol.IdStatus == 0)
                {
                    evt = new EventUpdateProtocol(command.TenantName, command.TenantId, command.TenantAOOId, CollaborationUniqueId, CollaborationId, CollaborationTemplateName, command.Identity, protocol, ((ICommandCQRSFascicolable)command).CategoryFascicle, documentUnit);
                }

                if (protocol.IdStatus != 0)
                {
                    evt = new EventCancelProtocol(command.TenantName, command.TenantId, command.TenantAOOId, CollaborationUniqueId, CollaborationId, CollaborationTemplateName, command.Identity, protocol, ((ICommandCQRSFascicolable)command).CategoryFascicle, documentUnit);
                }

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("Protocol, CreateUpdateEvent Error: ", command.GetType())), ex, LogCategories);
                throw ex;
            }

            return evt;
        }

        internal override async Task<DocumentUnit> MappingInsertAsync(IContentBase entity, IIdentityContext identity)
        {
            DocumentUnit documentUnit = new DocumentUnit();

            try
            {
                Protocol protocol = (Protocol)entity;

                #region [ Base ]

                documentUnit.EntityId = 0;
                documentUnit.Environment = (int)DSWEnvironmentType.Protocol;
                documentUnit.LastChangedDate = null;
                documentUnit.LastChangedUser = null;
                documentUnit.Number = protocol.Number;
                documentUnit.RegistrationDate = protocol.RegistrationDate;
                documentUnit.RegistrationUser = protocol.RegistrationUser;
                documentUnit.Subject = protocol.Object;
                documentUnit.Title = string.Concat(protocol.Year, "/", protocol.Number.ToString("0000000"));
                documentUnit.UniqueId = protocol.UniqueId;
                documentUnit.Year = protocol.Year;
                documentUnit.DocumentUnitName = "Protocollo";
                documentUnit.Status = protocol.IdStatus == 0 ? DocumentUnitStatus.Active : DocumentUnitStatus.Inactive;
                #endregion

                #region [ Navigation Properties ]

                documentUnit.UDSRepository = null;
                documentUnit.Category = protocol.Category;
                documentUnit.Container = protocol.Container;
                documentUnit.Fascicle = await _webApiClient.GetFascicleAsync(protocol.UniqueId);

                foreach (ProtocolRole item in protocol.ProtocolRoles)
                {
                    documentUnit.DocumentUnitRoles.Add(new DocumentUnitRole()
                    {
                        UniqueIdRole = item.Role.UniqueId,
                        RegistrationDate = DateTimeOffset.UtcNow,
                        RegistrationUser = identity.User,
                        AuthorizationRoleType = GetRoleType(item.Type)
                    });
                }

                foreach (ProtocolUser user in protocol.ProtocolUsers)
                {
                    documentUnit.DocumentUnitUsers.Add(new DocumentUnitUser()
                    {
                        UniqueId = user.UniqueId,
                        Account = user.Account,
                        RegistrationDate = DateTimeOffset.UtcNow,
                        RegistrationUser = identity.User,
                        AuthorizationType = AuthorizationRoleType.Accounted
                    });
                }
                if (protocol.IdAnnexed.HasValue && protocol.IdAnnexed.Value != Guid.Empty)
                {
                    AddDocumentUnitChain(documentUnit, protocol.IdAnnexed.Value, ChainType.AnnexedChain, identity, protocol.Container.ProtLocation.ProtocolArchive);
                }

                if (protocol.IdAttachments.HasValue && protocol.IdAttachments.Value != 0)
                {
                    string attachmentArchiveName = protocol.Container.ProtLocation.ProtocolArchive;
                    if (protocol.AttachLocation != null && protocol.Container.ProtLocation.EntityShortId != protocol.AttachLocation.EntityShortId)
                    {
                        attachmentArchiveName = protocol.AttachLocation?.ProtocolArchive;
                    }
                    AddDocumentUnitChain(documentUnit, _biblosClient.Document.GetDocumentId(attachmentArchiveName, protocol.IdAttachments.Value), ChainType.AttachmentsChain, identity, attachmentArchiveName);
                }

                if (protocol.IdDocument.HasValue && protocol.IdDocument.Value != 0)
                {
                    Guid chainId = _biblosClient.Document.GetDocumentId(protocol.Container.ProtLocation.ProtocolArchive, protocol.IdDocument.Value);
                    BiblosDS.BiblosDS.Document document = _biblosClient.Document.GetDocumentChildren(chainId).FirstOrDefault();
                    AddDocumentUnitChain(documentUnit, chainId, ChainType.MainChain, identity, protocol.Container.ProtLocation.ProtocolArchive, documentName: document?.Name);
                }

                if (protocol.DematerialisationChainId.HasValue && protocol.DematerialisationChainId != Guid.Empty)
                {
                    AddDocumentUnitChain(documentUnit, protocol.DematerialisationChainId.Value, ChainType.DematerialisationChain, identity, protocol.Container.ProtLocation.ProtocolArchive);
                }
                #endregion

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("Protocol, MappingInsertAsync Error: "), ex, LogCategories);
                throw ex;
            }


            return documentUnit;
        }

        internal override async Task<DocumentUnit> MappingUpdateAsync(IContentBase entity, DocumentUnit documentUnit, IIdentityContext identity)
        {
            try
            {
                Protocol protocol = (Protocol)entity;

                #region [ Base ]

                documentUnit.LastChangedDate = protocol.LastChangedDate;
                documentUnit.LastChangedUser = protocol.LastChangedUser;
                documentUnit.Subject = protocol.Object;
                documentUnit.Title = documentUnit.Title;
                documentUnit.Status = protocol.IdStatus == 0 ? DocumentUnitStatus.Active : DocumentUnitStatus.Inactive;
                #endregion

                #region [ Navigation Properties ]
                if (documentUnit.Category.UniqueId != protocol.Category.UniqueId)
                {
                    documentUnit.Category = protocol.Category;
                }

                if (documentUnit.Container.UniqueId != protocol.Container.UniqueId)
                {
                    documentUnit.Container = protocol.Container;
                }

                documentUnit.Fascicle = await _webApiClient.GetFascicleAsync(protocol.UniqueId);

                if (protocol.ProtocolRoles == null || !protocol.ProtocolRoles.Any())
                {
                    documentUnit.DocumentUnitRoles.Clear();
                }

                if (protocol.ProtocolRoles != null)
                {
                    //Aggiorno i settori della documentunit esistenti con il type che arriva dai settori del protocollo
                    foreach (DocumentUnitRole docRole in documentUnit.DocumentUnitRoles.Where(t => protocol.ProtocolRoles.Any(x => x.Role.UniqueId == t.UniqueIdRole)).ToList())
                    {
                        if (protocol.ProtocolRoles.Any(r => r.Role.UniqueId == docRole.UniqueIdRole))
                        {
                            docRole.AuthorizationRoleType = GetRoleType(protocol.ProtocolRoles.First(r => r.Role.UniqueId == docRole.UniqueIdRole).Type);
                        }
                    }

                    foreach (ProtocolRole item in protocol.ProtocolRoles.Where(t => !documentUnit.DocumentUnitRoles.Any(x => x.UniqueIdRole == t.Role.UniqueId)).ToList())
                    {
                        documentUnit.DocumentUnitRoles.Add(new DocumentUnitRole()
                        {
                            UniqueIdRole = item.Role.UniqueId,
                            RegistrationDate = DateTimeOffset.UtcNow,
                            RegistrationUser = item.RegistrationUser,
                            AuthorizationRoleType = GetRoleType(item.Type)
                        });
                    }

                    foreach (DocumentUnitRole item in documentUnit.DocumentUnitRoles.Where(t => !protocol.ProtocolRoles.Any(x => x.Role.UniqueId == t.UniqueIdRole)).ToList())
                    {
                        documentUnit.DocumentUnitRoles.Remove(item);
                    }
                }

                if (protocol.ProtocolUsers != null)
                {
                    foreach (ProtocolUser item in protocol.ProtocolUsers.Where(t => !documentUnit.DocumentUnitUsers.Any(x => x.Account == t.Account)).ToList())
                    {
                        documentUnit.DocumentUnitUsers.Add(new DocumentUnitUser()
                        {
                            UniqueId = item.UniqueId,
                            Account = item.Account,
                            RegistrationDate = DateTimeOffset.UtcNow,
                            RegistrationUser = item.RegistrationUser,
                            AuthorizationType = AuthorizationRoleType.Accounted
                        });
                    }

                    foreach (DocumentUnitUser item in documentUnit.DocumentUnitUsers.Where(t => !protocol.ProtocolUsers.Any(x => x.Account == t.Account)).ToList())
                    {
                        documentUnit.DocumentUnitUsers.Remove(item);
                    }
                }


                if (protocol.IdAnnexed.HasValue && protocol.IdAnnexed.Value != Guid.Empty)
                {
                    DocumentUnitChain annexed = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.AnnexedChain).FirstOrDefault();
                    if (annexed != null)
                    {
                        if (annexed.IdArchiveChain != protocol.IdAnnexed.Value)
                        {
                            documentUnit.DocumentUnitChains.Remove(annexed);
                            AddDocumentUnitChain(documentUnit, protocol.IdAnnexed.Value, ChainType.AnnexedChain, identity, protocol.Container.ProtLocation.ProtocolArchive);
                        }
                    }
                    else
                    {
                        AddDocumentUnitChain(documentUnit, protocol.IdAnnexed.Value, ChainType.AnnexedChain, identity, protocol.Container.ProtLocation.ProtocolArchive);
                    }
                }
                else
                {
                    foreach (DocumentUnitChain chain in documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.AnnexedChain).ToList())
                    {
                        documentUnit.DocumentUnitChains.Remove(chain);
                    }
                }

                if (protocol.DematerialisationChainId.HasValue && protocol.DematerialisationChainId.Value != Guid.Empty)
                {
                    DocumentUnitChain dematerialisation = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.DematerialisationChain).FirstOrDefault();
                    if (dematerialisation != null)
                    {
                        if (dematerialisation.IdArchiveChain != protocol.DematerialisationChainId.Value)
                        {
                            documentUnit.DocumentUnitChains.Remove(dematerialisation);
                            AddDocumentUnitChain(documentUnit, protocol.DematerialisationChainId.Value, ChainType.DematerialisationChain, identity, protocol.Container.ProtLocation.ProtocolArchive);
                        }
                    }
                    else
                    {
                        AddDocumentUnitChain(documentUnit, protocol.DematerialisationChainId.Value, ChainType.DematerialisationChain, identity, protocol.Container.ProtLocation.ProtocolArchive);
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
                _logger.WriteError(new LogMessage("Protocol, MappingUpdateAsync Error: "), ex, LogCategories);
                throw ex;
            }


            return documentUnit;
        }

        #endregion
    }
}
