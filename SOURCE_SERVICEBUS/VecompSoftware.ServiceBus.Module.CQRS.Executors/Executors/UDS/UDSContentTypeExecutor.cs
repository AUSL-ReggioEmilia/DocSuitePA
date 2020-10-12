using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.Core.Command.CQRS.Events.Models.UDS;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.UDS
{
    public class UDSContentTypeExecutor : BaseCommonExecutor, IUDSContentTypeExecutor, IDocumentUnitEntity
    {
        #region [ Fields ]
        private readonly IWebAPIClient _webApiClient;
        private readonly BiblosClient _biblosClient;
        private readonly ILogger _logger;
        #endregion

        #region [ Properties ]

        public Guid? CollaborationUniqueId { get; set; }
        public int? CollaborationId { get; set; }
        public string CollaborationTemplateName { get; set; }

        #endregion

        #region [ Constructor ]
        public UDSContentTypeExecutor(ILogger logger, IWebAPIClient webApiClient, BiblosClient biblosClient, ServiceBus.ServiceBusClient serviceBusClient)
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
            UDSBuildModel udsBuildModel;
            try
            {
                udsBuildModel = ((ICommandCQRSCreateUDSData)command).ContentType.ContentTypeValue;

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
                evt = new EventCQRSCreateUDSData(command.TenantName, CollaborationUniqueId, CollaborationId, CollaborationTemplateName, command.TenantId, command.TenantAOOId, command.Identity, udsBuildModel, ((ICommandCQRSFascicolable)command).CategoryFascicle, documentUnit);
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
            UDSBuildModel udsBuildModel;
            try
            {
                udsBuildModel = ((ICommandCQRSUpdateUDSData)command).ContentType.ContentTypeValue;

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

                evt = new EventCQRSUpdateUDSData(command.TenantName, CollaborationUniqueId, CollaborationId, CollaborationTemplateName, command.TenantId, command.TenantAOOId, command.Identity, udsBuildModel, ((ICommandCQRSFascicolable)command).CategoryFascicle, documentUnit);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("UDS, CreateUpdateEvent Error: ", command.GetType())), ex, LogCategories);
                throw ex;
            }

            return evt;
        }

        internal override async Task<DocumentUnit> MappingInsertAsync(IContentBase entity, IIdentityContext identity)
        {
            DocumentUnit documentUnit = new DocumentUnit();

            try
            {
                UDSBuildModel udsBuildModel = (UDSBuildModel)entity;
                #region [ Base ]

                documentUnit.EntityId = 0;
                documentUnit.UniqueId = udsBuildModel.UniqueId;
                documentUnit.Environment = udsBuildModel.UDSRepository.DSWEnvironment;
                documentUnit.Year = udsBuildModel.Year.Value;
                documentUnit.Number = udsBuildModel.Number.Value;
                documentUnit.RegistrationDate = udsBuildModel.RegistrationDate.Value;
                documentUnit.RegistrationUser = udsBuildModel.RegistrationUser;
                documentUnit.LastChangedDate = null;
                documentUnit.LastChangedUser = null;
                documentUnit.Subject = udsBuildModel.Subject;
                documentUnit.Title = string.Concat(udsBuildModel.Year.Value, "/", udsBuildModel.Number.Value.ToString("0000000"));
                documentUnit.DocumentUnitName = udsBuildModel.Title;
                documentUnit.Status = DocumentUnitStatus.Active;
                #endregion

                #region [ Navigation Properties ]

                //TODO: Category, Container e Authorizations dovrebbero arrivare già completi di UniqueId. Va modificata l'UDS nella DSW.
                documentUnit.UDSRepository = MapUDSRepositoryModel(new UDSRepository(), udsBuildModel.UDSRepository);
                Category category = await _webApiClient.GetCategoryAsync(udsBuildModel.Category.IdCategory.Value);
                if (category.UniqueId != Guid.Empty)
                {
                    documentUnit.Category = category;
                }

                Container container = await _webApiClient.GetContainerAsync(udsBuildModel.Container.IdContainer.Value);
                if (container.UniqueId != Guid.Empty)
                {
                    documentUnit.Container = container;
                }

                documentUnit.Fascicle = await _webApiClient.GetFascicleAsync(udsBuildModel.UniqueId);

                foreach (RoleModel item in udsBuildModel.Roles)
                {
                    Role role = await _webApiClient.GetRoleAsync(item);
                    documentUnit.DocumentUnitRoles.Add(new DocumentUnitRole()
                    {
                        UniqueIdRole = role.UniqueId,
                        RoleLabel = item.RoleLabel,
                        RegistrationDate = DateTimeOffset.UtcNow,
                        RegistrationUser = identity.User,
                        AuthorizationRoleType = GetRoleType(string.Empty)
                    });
                }
                foreach (UserModel item in udsBuildModel.Users)
                {
                    documentUnit.DocumentUnitUsers.Add(new DocumentUnitUser()
                    {
                        Account = item.Account,
                        RegistrationDate = DateTimeOffset.UtcNow,
                        RegistrationUser = identity.User,
                        AuthorizationType = AuthorizationRoleType.Accounted
                    });
                }

                foreach (UDSDocumentModel document in udsBuildModel.Documents)
                {
                    switch (document.DocumentType)
                    {
                        case UDSDocumentType.Document:
                            AddUDSDocumentUnitChain(documentUnit, document, ChainType.MainChain, identity);
                            break;
                        case UDSDocumentType.DocumentAttachment:
                            AddUDSDocumentUnitChain(documentUnit, document, ChainType.AttachmentsChain, identity);
                            break;
                        case UDSDocumentType.DocumentAnnexed:
                            AddUDSDocumentUnitChain(documentUnit, document, ChainType.AnnexedChain, identity);
                            break;
                        case UDSDocumentType.DocumentDematerialisation:
                            AddUDSDocumentUnitChain(documentUnit, document, ChainType.DematerialisationChain, identity);
                            break;
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("UDS, MappingInsertAsync Error: "), ex, LogCategories);
                throw ex;
            }

            return documentUnit;
        }

        internal override async Task<DocumentUnit> MappingUpdateAsync(IContentBase entity, DocumentUnit documentUnit, IIdentityContext identity)
        {
            try
            {
                UDSBuildModel udsBuildModel = (UDSBuildModel)entity;

                #region [ Base ]

                documentUnit.LastChangedDate = udsBuildModel.LastChangedDate;
                documentUnit.LastChangedUser = udsBuildModel.LastChangedUser;
                documentUnit.Subject = udsBuildModel.Subject;
                documentUnit.Title = documentUnit.Title;
                documentUnit.Status = DocumentUnitStatus.Active;

                #endregion

                #region [ Navigation Properties ]

                Category category = await _webApiClient.GetCategoryAsync(udsBuildModel.Category.IdCategory.Value);
                if (category.UniqueId != Guid.Empty && documentUnit.Category.UniqueId != udsBuildModel.Category.UniqueId)
                {
                    documentUnit.Category = category;
                }

                Container container = await _webApiClient.GetContainerAsync(udsBuildModel.Container.IdContainer.Value);
                if (container.UniqueId != Guid.Empty && documentUnit.Container.UniqueId != udsBuildModel.Container.UniqueId)
                {
                    documentUnit.Container = container;
                }

                documentUnit.Fascicle = await _webApiClient.GetFascicleAsync(udsBuildModel.UniqueId);

                if (udsBuildModel.Roles == null || !udsBuildModel.Roles.Any())
                {
                    documentUnit.DocumentUnitRoles.Clear();
                }

                if (udsBuildModel.Roles != null)
                {
                    IList<Role> roles = new List<Role>();
                    foreach (RoleModel item in udsBuildModel.Roles)
                    {
                        roles.Add(await _webApiClient.GetRoleAsync(item));
                    }

                    foreach (Role item in roles.Where(t => !documentUnit.DocumentUnitRoles.Any(x => x.UniqueIdRole == t.UniqueId)))
                    {
                        documentUnit.DocumentUnitRoles.Add(new DocumentUnitRole()
                        {
                            UniqueIdRole = item.UniqueId,
                            RoleLabel = udsBuildModel.Roles.SingleOrDefault(r => r.UniqueId.HasValue ? r.UniqueId.Value.Equals(item.UniqueId) : r.IdRole.Equals(item.EntityShortId))?.RoleLabel,
                            RegistrationDate = DateTimeOffset.UtcNow,
                            RegistrationUser = identity.User,
                            AuthorizationRoleType = GetRoleType(string.Empty)
                        });
                    }

                    foreach (DocumentUnitRole item in documentUnit.DocumentUnitRoles.Where(t => !roles.Any(x => x.UniqueId == t.UniqueIdRole)).ToList())
                    {
                        documentUnit.DocumentUnitRoles.Remove(item);
                    }
                }

                if (udsBuildModel.Users == null || !udsBuildModel.Users.Any())
                {
                    documentUnit.DocumentUnitUsers.Clear();
                }

                if (udsBuildModel.Users != null)
                {
                    IList<DocumentUnitUser> users = new List<DocumentUnitUser>();
                    foreach (UserModel item in udsBuildModel.Users)
                    {
                        if (item.Account != null && !documentUnit.DocumentUnitUsers.Where(x => x.Account != item.Account).Any())
                        {
                            users.Add(new DocumentUnitUser()
                            {
                                Account = item.Account,
                                RegistrationDate = DateTimeOffset.UtcNow,
                                RegistrationUser = identity.User,
                                AuthorizationType = AuthorizationRoleType.Accounted
                            });
                        }

                    }
                    foreach (DocumentUnitUser item in users.Where(t => !documentUnit.DocumentUnitUsers.Any(x => x.Account == t.Account)))
                    {
                        documentUnit.DocumentUnitUsers.Add(new DocumentUnitUser()
                        {
                            Account = item.Account,
                            RegistrationDate = DateTimeOffset.UtcNow,
                            RegistrationUser = identity.User,
                            AuthorizationType = AuthorizationRoleType.Accounted
                        });
                    }

                    foreach (DocumentUnitUser item in documentUnit.DocumentUnitUsers.Where(t => !users.Any(x => x.Account == t.Account)).ToList())
                    {
                        documentUnit.DocumentUnitUsers.Remove(item);
                    }
                }

                if (udsBuildModel.Documents.Count == 0)
                {
                    documentUnit.DocumentUnitChains.Clear();
                }

                if (udsBuildModel.Documents.Count > 0)
                {
                    //Elimino le chain che non esistono più
                    foreach (DocumentUnitChain item in documentUnit.DocumentUnitChains.Where(t => !udsBuildModel.Documents.Any(x => x.IdChain == t.IdArchiveChain)).ToList())
                    {
                        documentUnit.DocumentUnitChains.Remove(item);
                    }

                    foreach (UDSDocumentModel document in udsBuildModel.Documents)
                    {
                        switch (document.DocumentType)
                        {
                            case UDSDocumentType.Document:
                                {
                                    DocumentUnitChain mainDocument = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.MainChain).FirstOrDefault();
                                    if (mainDocument != null)
                                    {
                                        if (document.IdChain != Guid.Empty)
                                        {
                                            if (mainDocument.IdArchiveChain != document.IdChain || (mainDocument.ChainType == ChainType.MainChain && !mainDocument.DocumentName.Equals(document.DocumentName)))
                                            {
                                                documentUnit.DocumentUnitChains.Remove(mainDocument);
                                                AddUDSDocumentUnitChain(documentUnit, document, ChainType.MainChain, identity);
                                            }
                                        }
                                        else
                                        {
                                            documentUnit.DocumentUnitChains.Remove(mainDocument);
                                        }
                                    }
                                    else
                                    {
                                        AddUDSDocumentUnitChain(documentUnit, document, ChainType.MainChain, identity);
                                    }
                                    break;
                                }
                            case UDSDocumentType.DocumentAttachment:
                                {
                                    DocumentUnitChain attachment = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.AttachmentsChain).FirstOrDefault();
                                    if (attachment != null)
                                    {
                                        if (document.IdChain != Guid.Empty)
                                        {
                                            if (attachment.IdArchiveChain != document.IdChain)
                                            {
                                                documentUnit.DocumentUnitChains.Remove(attachment);
                                                AddUDSDocumentUnitChain(documentUnit, document, ChainType.AttachmentsChain, identity);
                                            }
                                        }
                                        else
                                        {
                                            documentUnit.DocumentUnitChains.Remove(attachment);
                                        }
                                    }
                                    else
                                    {
                                        AddUDSDocumentUnitChain(documentUnit, document, ChainType.AttachmentsChain, identity);
                                    }
                                    break;
                                }
                            case UDSDocumentType.DocumentAnnexed:
                                {
                                    DocumentUnitChain annexed = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.AnnexedChain).FirstOrDefault();
                                    if (annexed != null)
                                    {
                                        if (document.IdChain != Guid.Empty)
                                        {
                                            if (annexed.IdArchiveChain != document.IdChain)
                                            {
                                                documentUnit.DocumentUnitChains.Remove(annexed);
                                                AddUDSDocumentUnitChain(documentUnit, document, ChainType.AnnexedChain, identity);
                                            }
                                        }
                                        else
                                        {
                                            documentUnit.DocumentUnitChains.Remove(annexed);
                                        }
                                    }
                                    else
                                    {
                                        AddUDSDocumentUnitChain(documentUnit, document, ChainType.AnnexedChain, identity);
                                    }
                                    break;

                                }
                            case UDSDocumentType.DocumentDematerialisation:
                                {
                                    DocumentUnitChain dematerialisation = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.DematerialisationChain).FirstOrDefault();
                                    if (dematerialisation != null)
                                    {
                                        if (document.IdChain != Guid.Empty)
                                        {
                                            if (dematerialisation.IdArchiveChain != document.IdChain)
                                            {
                                                documentUnit.DocumentUnitChains.Remove(dematerialisation);
                                                AddUDSDocumentUnitChain(documentUnit, document, ChainType.DematerialisationChain, identity);
                                            }
                                        }
                                        else
                                        {
                                            documentUnit.DocumentUnitChains.Remove(dematerialisation);
                                        }
                                    }
                                    else
                                    {
                                        AddUDSDocumentUnitChain(documentUnit, document, ChainType.DematerialisationChain, identity);
                                    }
                                    break;
                                }
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("UDS, MappingUpdateAsync Error: "), ex, LogCategories);
                throw ex;
            }

            return documentUnit;
        }


        private UDSRepository MapUDSRepositoryModel(UDSRepository udsRepository, UDSRepositoryModel udsRepositoryModel)
        {
            udsRepository.UniqueId = udsRepositoryModel.Id;
            udsRepository.DSWEnvironment = udsRepositoryModel.DSWEnvironment;
            udsRepository.Name = udsRepository.Name;
            udsRepository.Status = udsRepository.Status;
            return udsRepository;
        }

        public void AddUDSDocumentUnitChain(DocumentUnit documentUnit, UDSDocumentModel udsDocument, ChainType chainType, IIdentityContext identity)
        {
            string documentName = string.Empty;

            BiblosDS.BiblosDS.Document document = _biblosClient.Document.GetDocumentChildren(udsDocument.IdChain).FirstOrDefault();

            if (document != null)
            {
                AddDocumentUnitChain(documentUnit, udsDocument.IdChain, chainType, identity, document.Archive.Name, chainType == ChainType.MainChain ? document.Name : null, udsDocument.DocumentLabel);
            }
        }

        #endregion

    }
}
