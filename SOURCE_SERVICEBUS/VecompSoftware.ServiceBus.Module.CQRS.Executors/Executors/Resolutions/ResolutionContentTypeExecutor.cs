using System;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Core.Command.CQRS.Events.Models.Resolutions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Resolutions;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Resolutions
{
    public class ResolutionContentTypeExecutor : BaseCommonExecutor, IResolutionContentTypeExecutor, IDocumentUnitEntity
    {
        #region [ Fields ]
        private readonly IWebAPIClient _webAPIClient;
        private readonly BiblosClient _biblosClient;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]
        public ResolutionContentTypeExecutor(ILogger logger, IWebAPIClient webApiClient, BiblosClient biblosClient, ServiceBus.ServiceBusClient serviceBusClient)
            : base(logger, webApiClient, biblosClient, serviceBusClient)
        {
            _webAPIClient = webApiClient;
            _biblosClient = biblosClient;
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        internal override IEvent CreateInsertEvent(ICommandCQRSCreate command, DocumentUnit documentUnit = null)
        {
            IEvent evt = null;
            ResolutionModel resolution;
            try
            {
                resolution = ((ICommandCreateResolution)command).ContentType.ContentTypeValue;
                evt = new EventCreateResolution(command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, resolution, ((ICommandCQRSFascicolable)command).CategoryFascicle, documentUnit);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("Resolution, CreateInsertEvent Error: ", command.GetType())), ex, LogCategories);
                throw ex;
            }

            return evt;
        }

        internal override IEvent CreateUpdateEvent(ICommandCQRSUpdate command, DocumentUnit documentUnit = null)
        {
            IEvent evt = null;
            ResolutionModel resolution;
            try
            {
                resolution = ((ICommandUpdateResolution)command).ContentType.ContentTypeValue;
                evt = new EventUpdateResolution(command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, resolution, ((ICommandCQRSFascicolable)command).CategoryFascicle, documentUnit);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("Resolution, CreateUpdateEvent Error: ", command.GetType())), ex, LogCategories);
                throw ex;
            }

            return evt;
        }

        internal override async Task<DocumentUnit> MappingInsertAsync(IContentBase entity, IIdentityContext identity)
        {
            DocumentUnit documentUnit = new DocumentUnit();

            try
            {
                ResolutionModel resolutionModel = (ResolutionModel)entity;

                #region [ Base ]

                documentUnit.EntityId = resolutionModel.IdResolution.HasValue ? resolutionModel.IdResolution.Value : 0;
                documentUnit.Environment = (int)DocSuiteWeb.Entity.Commons.DSWEnvironmentType.Resolution;
                documentUnit.LastChangedDate = null;
                documentUnit.LastChangedUser = null;
                documentUnit.Number = resolutionModel.Number.Value;
                documentUnit.RegistrationDate = resolutionModel.AdoptionDate.Value;
                documentUnit.RegistrationUser = resolutionModel.RegistrationUser;
                documentUnit.Subject = resolutionModel.Subject;
                documentUnit.Title = string.Concat(resolutionModel.Year.Value, "/", string.IsNullOrEmpty(resolutionModel.ServiceNumber) ? resolutionModel.Number.Value.ToString("0000000") : resolutionModel.ServiceNumber);
                documentUnit.UniqueId = resolutionModel.UniqueId;
                documentUnit.Year = resolutionModel.Year.Value;
                documentUnit.DocumentUnitName = resolutionModel.DocumentUnitName;
                documentUnit.Status = DocumentUnitStatus.Active;
                #endregion

                #region [ Navigation Properties ]

                documentUnit.UDSRepository = null;
                documentUnit.Category = MapCategoryModel(new Category(), resolutionModel.Category);
                documentUnit.Container = MapContainerModel(new Container(), resolutionModel.Container);
                documentUnit.Fascicle = await _webAPIClient.GetFascicleAsync(resolutionModel.UniqueId);

                foreach (ResolutionRoleModel item in resolutionModel.ResolutionRoles)
                {
                    documentUnit.DocumentUnitRoles.Add(new DocumentUnitRole()
                    {
                        UniqueIdRole = item.Role.UniqueId.Value,
                        RegistrationDate = DateTimeOffset.UtcNow,
                        RegistrationUser = identity.User,
                        AuthorizationRoleType = GetRoleType(string.Empty)
                    });
                }

                if (resolutionModel.FileResolution.IdAnnexes.HasValue && resolutionModel.FileResolution.IdAnnexes.Value != Guid.Empty)
                {
                    AddResolutionDocumentUnitChain(documentUnit, resolutionModel, resolutionModel.FileResolution.IdAnnexes.Value, ChainType.AnnexedChain, identity);
                }

                if (resolutionModel.FileResolution.IdResolutionFile.HasValue && resolutionModel.FileResolution.IdResolutionFile.Value != 0)
                {
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdResolutionFile.Value);
                    AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.MainChain, identity);
                }

                if (resolutionModel.FileResolution.IdAttachments.HasValue && resolutionModel.FileResolution.IdAttachments.Value != 0)
                {
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdAttachments.Value);
                    AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.AttachmentsChain, identity);
                }

                if (resolutionModel.FileResolution.IdControllerFile.HasValue && resolutionModel.FileResolution.IdControllerFile.Value != 0)
                {
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdControllerFile.Value);
                    AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.ControllerChain, identity);
                }

                if (resolutionModel.FileResolution.IdFrontespizio.HasValue && resolutionModel.FileResolution.IdFrontespizio.Value != 0)
                {
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdFrontespizio.Value);
                    AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.FrontespizioChain, identity);
                }

                if (resolutionModel.FileResolution.IdAssumedProposal.HasValue && resolutionModel.FileResolution.IdAssumedProposal.Value != 0)
                {
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdAssumedProposal.Value);
                    AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.AssumedProposalChain, identity);
                }

                if (resolutionModel.FileResolution.IdPrivacyAttachments.HasValue && resolutionModel.FileResolution.IdPrivacyAttachments.Value != 0)
                {
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdPrivacyAttachments.Value);
                    AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.PrivacyAttachmentChain, identity);
                }

                if (resolutionModel.FileResolution.IdFrontalinoRitiro.HasValue && resolutionModel.FileResolution.IdFrontalinoRitiro.Value != 0)
                {
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdFrontalinoRitiro.Value);
                    AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.FrontalinoRitiroChain, identity);
                }

                if (resolutionModel.FileResolution.IdPrivacyPublicationDocument.HasValue && resolutionModel.FileResolution.IdPrivacyPublicationDocument.Value != 0)
                {
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdPrivacyPublicationDocument.Value);
                    AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.PrivacyPublicationDocumentChain, identity);
                }

                if (resolutionModel.FileResolution.IdUltimaPaginaFile.HasValue && resolutionModel.FileResolution.IdUltimaPaginaFile.Value != 0)
                {
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdUltimaPaginaFile.Value);
                    AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.UltimaPaginaChain, identity);
                }

                if (resolutionModel.FileResolution.IdSupervisoryBoardFile.HasValue && resolutionModel.FileResolution.IdSupervisoryBoardFile.Value != 0)
                {
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdSupervisoryBoardFile.Value);
                    AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.SupervisoryBoardChain, identity);
                }

                if (resolutionModel.FileResolution.IdMainDocumentsOmissis.HasValue && resolutionModel.FileResolution.IdMainDocumentsOmissis.Value != Guid.Empty)
                {
                    AddResolutionDocumentUnitChain(documentUnit, resolutionModel, resolutionModel.FileResolution.IdMainDocumentsOmissis.Value, ChainType.MainOmissisChain, identity);
                }

                if (resolutionModel.FileResolution.IdAttachmentsOmissis.HasValue && resolutionModel.FileResolution.IdAttachmentsOmissis.Value != Guid.Empty)
                {
                    AddResolutionDocumentUnitChain(documentUnit, resolutionModel, resolutionModel.FileResolution.IdAttachmentsOmissis.Value, ChainType.AttachmentOmissisChain, identity);
                }

                if (resolutionModel.FileResolution.DematerialisationChainId.HasValue && resolutionModel.FileResolution.DematerialisationChainId.Value != Guid.Empty)
                {
                    AddResolutionDocumentUnitChain(documentUnit, resolutionModel, resolutionModel.FileResolution.DematerialisationChainId.Value, ChainType.DematerialisationChain, identity);
                }

                #endregion
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("Resolution, MappingInsertAsync Error: "), ex, LogCategories);
                throw ex;
            }


            return documentUnit;
        }

        internal override async Task<DocumentUnit> MappingUpdateAsync(IContentBase entity, DocumentUnit documentUnit, IIdentityContext identity)
        {
            try
            {
                ResolutionModel resolutionModel = (ResolutionModel)entity;

                #region [ Base ]

                documentUnit.Subject = resolutionModel.Subject;
                documentUnit.Title = documentUnit.Title;
                documentUnit.LastChangedDate = resolutionModel.LastChangedDate;
                documentUnit.LastChangedUser = resolutionModel.LastChangedUser;

                #endregion

                #region [ Navigation Properties ]
                if (resolutionModel.Category.UniqueId.HasValue && resolutionModel.Category.UniqueId.Value != Guid.Empty && documentUnit.Category.UniqueId != resolutionModel.Category.UniqueId)
                {
                    documentUnit.Category = MapCategoryModel(new Category(), resolutionModel.Category);
                }

                if (resolutionModel.Container.UniqueId.HasValue && resolutionModel.Container.UniqueId.Value != Guid.Empty && documentUnit.Container.UniqueId != resolutionModel.Container.UniqueId)
                {
                    documentUnit.Container = MapContainerModel(new Container(), resolutionModel.Container);
                }

                documentUnit.Fascicle = await _webAPIClient.GetFascicleAsync(resolutionModel.UniqueId);

                if (resolutionModel.ResolutionRoles == null || !resolutionModel.ResolutionRoles.Any())
                {
                    documentUnit.DocumentUnitRoles.Clear();
                }
                if (resolutionModel.ResolutionRoles != null)
                {
                    foreach (ResolutionRoleModel item in resolutionModel.ResolutionRoles.Where(t => !documentUnit.DocumentUnitRoles.Any(x => x.UniqueIdRole == t.Role.UniqueId)).ToList())
                    {
                        documentUnit.DocumentUnitRoles.Add(new DocumentUnitRole()
                        {
                            UniqueIdRole = item.Role.UniqueId.Value,
                            RegistrationDate = DateTimeOffset.UtcNow,
                            RegistrationUser = identity.User,
                            AuthorizationRoleType = GetRoleType(string.Empty)
                        });
                    }

                    foreach (DocumentUnitRole item in documentUnit.DocumentUnitRoles.Where(t => !resolutionModel.ResolutionRoles.Any(x => x.Role.UniqueId == t.UniqueIdRole)).ToList())
                    {
                        documentUnit.DocumentUnitRoles.Remove(item);
                    }
                }

                //Annessi
                if (resolutionModel.FileResolution.IdAnnexes.HasValue && resolutionModel.FileResolution.IdAnnexes.Value != Guid.Empty)
                {
                    DocumentUnitChain annexed = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.AnnexedChain).FirstOrDefault();
                    if (annexed != null)
                    {
                        if (annexed.IdArchiveChain != resolutionModel.FileResolution.IdAnnexes.Value)
                        {
                            documentUnit.DocumentUnitChains.Remove(annexed);
                            AddResolutionDocumentUnitChain(documentUnit, resolutionModel, resolutionModel.FileResolution.IdAnnexes.Value, ChainType.AnnexedChain, identity);
                        }
                    }
                    else
                    {
                        AddResolutionDocumentUnitChain(documentUnit, resolutionModel, resolutionModel.FileResolution.IdAnnexes.Value, ChainType.AnnexedChain, identity);
                    }
                }
                else
                {
                    foreach (DocumentUnitChain chain in documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.AnnexedChain).ToList())
                    {
                        documentUnit.DocumentUnitChains.Remove(chain);
                    }
                }

                //Allegati
                if (resolutionModel.FileResolution.IdAttachments.HasValue && resolutionModel.FileResolution.IdAttachments.Value != 0)
                {
                    DocumentUnitChain attachment = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.AttachmentsChain).FirstOrDefault();
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdAttachments.Value);
                    if (attachment != null)
                    {
                        if (attachment.IdArchiveChain != chainId)
                        {
                            documentUnit.DocumentUnitChains.Remove(attachment);
                            AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.AttachmentsChain, identity);
                        }
                    }
                    else
                    {
                        AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.AttachmentsChain, identity);
                    }
                }


                //AttachmentOmissis
                if (resolutionModel.FileResolution.IdAttachmentsOmissis.HasValue && resolutionModel.FileResolution.IdAttachmentsOmissis.Value != Guid.Empty)
                {
                    DocumentUnitChain attachmentOmissis = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.AttachmentOmissisChain).FirstOrDefault();
                    if (attachmentOmissis != null)
                    {
                        if (attachmentOmissis.IdArchiveChain != resolutionModel.FileResolution.IdAttachmentsOmissis.Value)
                        {
                            documentUnit.DocumentUnitChains.Remove(attachmentOmissis);
                            AddResolutionDocumentUnitChain(documentUnit, resolutionModel, resolutionModel.FileResolution.IdAttachmentsOmissis.Value, ChainType.AttachmentOmissisChain, identity);
                        }
                    }
                    else
                    {
                        AddResolutionDocumentUnitChain(documentUnit, resolutionModel, resolutionModel.FileResolution.IdAttachmentsOmissis.Value, ChainType.AttachmentOmissisChain, identity);
                    }
                }


                //ResolutionFile
                if (resolutionModel.FileResolution.IdResolutionFile.HasValue && resolutionModel.FileResolution.IdResolutionFile.Value != 0)
                {
                    DocumentUnitChain resolutionFile = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.MainChain).FirstOrDefault();
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdResolutionFile.Value);
                    if (resolutionFile != null)
                    {
                        if (resolutionFile.IdArchiveChain != chainId)
                        {
                            documentUnit.DocumentUnitChains.Remove(resolutionFile);
                            AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.MainChain, identity);
                        }
                    }
                    else
                    {
                        AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.MainChain, identity);
                    }
                }

                //Frontespizio
                if (resolutionModel.FileResolution.IdFrontespizio.HasValue && resolutionModel.FileResolution.IdFrontespizio.Value != 0)
                {
                    DocumentUnitChain frontespizio = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.FrontespizioChain).FirstOrDefault();
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdFrontespizio.Value);
                    if (frontespizio != null)
                    {
                        if (frontespizio.IdArchiveChain != chainId)
                        {
                            documentUnit.DocumentUnitChains.Remove(frontespizio);
                            AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.FrontespizioChain, identity);
                        }
                    }
                    else
                    {
                        AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.FrontespizioChain, identity);
                    }
                }

                //ControllerFile
                if (resolutionModel.FileResolution.IdControllerFile.HasValue && resolutionModel.FileResolution.IdControllerFile.Value != 0)
                {
                    DocumentUnitChain controllerFile = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.ControllerChain).FirstOrDefault();
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdControllerFile.Value);
                    if (controllerFile != null)
                    {
                        if (controllerFile.IdArchiveChain != chainId)
                        {
                            documentUnit.DocumentUnitChains.Remove(controllerFile);
                            AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.ControllerChain, identity);
                        }
                    }
                    else
                    {
                        AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.ControllerChain, identity);
                    }
                }

                //FrontalinoRitiro
                if (resolutionModel.FileResolution.IdFrontalinoRitiro.HasValue && resolutionModel.FileResolution.IdFrontalinoRitiro.Value != 0)
                {
                    DocumentUnitChain frontalinoRitiro = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.FrontalinoRitiroChain).FirstOrDefault();
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdFrontalinoRitiro.Value);
                    if (frontalinoRitiro != null)
                    {
                        if (frontalinoRitiro.IdArchiveChain != chainId)
                        {
                            documentUnit.DocumentUnitChains.Remove(frontalinoRitiro);
                            AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.FrontalinoRitiroChain, identity);
                        }
                    }
                    else
                    {
                        AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.FrontalinoRitiroChain, identity);
                    }
                }

                //MainDocumentOmissis
                if (resolutionModel.FileResolution.IdMainDocumentsOmissis.HasValue && resolutionModel.FileResolution.IdMainDocumentsOmissis.Value != Guid.Empty)
                {
                    DocumentUnitChain mainDocumentOmissis = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.MainOmissisChain).FirstOrDefault();
                    if (mainDocumentOmissis != null)
                    {
                        if (mainDocumentOmissis.IdArchiveChain != resolutionModel.FileResolution.IdMainDocumentsOmissis.Value)
                        {
                            documentUnit.DocumentUnitChains.Remove(mainDocumentOmissis);
                            AddResolutionDocumentUnitChain(documentUnit, resolutionModel, resolutionModel.FileResolution.IdMainDocumentsOmissis.Value, ChainType.MainOmissisChain, identity);
                        }
                    }
                    else
                    {
                        AddResolutionDocumentUnitChain(documentUnit, resolutionModel, resolutionModel.FileResolution.IdMainDocumentsOmissis.Value, ChainType.MainOmissisChain, identity);
                    }
                }

                //PrivacyAttachments
                if (resolutionModel.FileResolution.IdPrivacyAttachments.HasValue && resolutionModel.FileResolution.IdPrivacyAttachments.Value != 0)
                {
                    DocumentUnitChain privacyAttachemnt = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.PrivacyAttachmentChain).FirstOrDefault();
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdPrivacyAttachments.Value);
                    if (privacyAttachemnt != null)
                    {
                        if (privacyAttachemnt.IdArchiveChain != chainId)
                        {
                            documentUnit.DocumentUnitChains.Remove(privacyAttachemnt);
                            AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.PrivacyAttachmentChain, identity);
                        }
                    }
                    else
                    {
                        AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.PrivacyAttachmentChain, identity);
                    }
                }

                //PrivacyPublicationDocument
                if (resolutionModel.FileResolution.IdPrivacyPublicationDocument.HasValue && resolutionModel.FileResolution.IdPrivacyPublicationDocument.Value != 0)
                {
                    DocumentUnitChain privacyPublicationAttachemnt = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.PrivacyPublicationDocumentChain).FirstOrDefault();
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdPrivacyAttachments.Value);
                    if (privacyPublicationAttachemnt != null)
                    {
                        if (privacyPublicationAttachemnt.IdArchiveChain != chainId)
                        {
                            documentUnit.DocumentUnitChains.Remove(privacyPublicationAttachemnt);
                            AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.PrivacyPublicationDocumentChain, identity);
                        }
                    }
                    else
                    {
                        AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.PrivacyPublicationDocumentChain, identity);
                    }
                }

                //SupervisoryBoard
                if (resolutionModel.FileResolution.IdSupervisoryBoardFile.HasValue && resolutionModel.FileResolution.IdSupervisoryBoardFile.Value != 0)
                {
                    DocumentUnitChain supervisoryBoard = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.SupervisoryBoardChain).FirstOrDefault();
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdSupervisoryBoardFile.Value);
                    if (supervisoryBoard != null)
                    {
                        if (supervisoryBoard.IdArchiveChain != chainId)
                        {
                            documentUnit.DocumentUnitChains.Remove(supervisoryBoard);
                            AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.SupervisoryBoardChain, identity);
                        }
                    }
                    else
                    {
                        AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.SupervisoryBoardChain, identity);
                    }
                }

                //UltimaPagina
                if (resolutionModel.FileResolution.IdUltimaPaginaFile.HasValue && resolutionModel.FileResolution.IdUltimaPaginaFile.Value != 0)
                {
                    DocumentUnitChain lastPage = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.UltimaPaginaChain).FirstOrDefault();
                    Guid chainId = _biblosClient.Document.GetDocumentId(resolutionModel.Container.ReslLocation.ResolutionArchive, resolutionModel.FileResolution.IdUltimaPaginaFile.Value);
                    if (lastPage != null)
                    {
                        if (lastPage.IdArchiveChain != chainId)
                        {
                            documentUnit.DocumentUnitChains.Remove(lastPage);
                            AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.UltimaPaginaChain, identity);
                        }
                    }
                    else
                    {
                        AddResolutionDocumentUnitChain(documentUnit, resolutionModel, chainId, ChainType.UltimaPaginaChain, identity);
                    }
                }

                //Dematerialisation
                if (resolutionModel.FileResolution.DematerialisationChainId.HasValue && resolutionModel.FileResolution.DematerialisationChainId.Value != Guid.Empty)
                {
                    DocumentUnitChain dematerialisation = documentUnit.DocumentUnitChains.Where(t => t.ChainType == ChainType.DematerialisationChain).FirstOrDefault();
                    if (dematerialisation != null)
                    {
                        if (dematerialisation.IdArchiveChain != resolutionModel.FileResolution.DematerialisationChainId.Value)
                        {
                            documentUnit.DocumentUnitChains.Remove(dematerialisation);
                            AddResolutionDocumentUnitChain(documentUnit, resolutionModel, resolutionModel.FileResolution.DematerialisationChainId.Value, ChainType.DematerialisationChain, identity);
                        }
                    }
                    else
                    {
                        AddResolutionDocumentUnitChain(documentUnit, resolutionModel, resolutionModel.FileResolution.DematerialisationChainId.Value, ChainType.DematerialisationChain, identity);
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
                _logger.WriteError(new LogMessage("Resolution, MappingUpdateAsync Error: "), ex, LogCategories);
                throw ex;
            }

            return documentUnit;
        }

        internal void AddResolutionDocumentUnitChain(DocumentUnit documentUnit, ResolutionModel resolution, Guid chainId, ChainType chainType, IIdentityContext identity)
        {
            string documentName = string.Empty;
            if (chainType == ChainType.MainChain)
            {
                BiblosDS.BiblosDS.Document document = _biblosClient.Document.GetDocumentChildren(chainId).FirstOrDefault();
                documentName = document != null ? document.Name : string.Empty;
            }

            AddDocumentUnitChain(documentUnit, chainId, chainType, identity, resolution.Container.ReslLocation.ResolutionArchive, documentName);
        }

        private Category MapCategoryModel(Category category, CategoryModel categoryModel)
        {
            category.EntityShortId = Convert.ToInt16(categoryModel.IdCategory.Value);
            category.UniqueId = categoryModel.UniqueId.Value;
            return category;
        }

        private Container MapContainerModel(Container container, ContainerModel containerModel)
        {
            container.EntityShortId = Convert.ToInt16(containerModel.IdContainer.Value);
            container.UniqueId = containerModel.UniqueId.Value;
            return container;
        }

        #endregion

    }
}
