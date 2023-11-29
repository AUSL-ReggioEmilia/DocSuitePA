using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.UDS;
using VecompSoftware.DocSuiteWeb.DTO.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.EntityMapper.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Command.CQRS.Commands.Models.UDS;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
{
    public class UDSRepositoryFacade : BaseProtocolFacade<UDSRepository, Guid, UDSRepositoryDao>
    {
        #region [ Fields ]

        private readonly string _userName;
        private UDSSchemaRepositoryFacade _udsSchemaRepositoryfacade;

        #endregion [ Fields ]

        #region [ Properties ]

        protected UDSSchemaRepositoryFacade UDSSchemaRepositoryFacade
        {
            get
            {
                if (this._udsSchemaRepositoryfacade == null)
                    this._udsSchemaRepositoryfacade = new UDSSchemaRepositoryFacade(this._userName);

                return this._udsSchemaRepositoryfacade;
            }
        }

        #endregion [ Properties ]

        #region [ Constructor ]

        public UDSRepositoryFacade(string userName) : base()
        {
            _userName = userName;
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        public IList<UDSRepository> GetActiveRepositories()
        {
            return _dao.GetActiveRepositories();
        }

        public IList<UDSRepository> GetActiveRepositories(string name)
        {
            return _dao.GetActiveRepositories(name);
        }

        public UDSRepository GetMaxVersionByName(string name)
        {
            return _dao.GetMaxVersionByName(name);
        }


        public UDSRepository GetByDSWEnvironment(int DSWEnvironment)
        {
            return _dao.GetByDSWEnvironment(DSWEnvironment);
        }

        public IList<UDSRepository> GetByPecEnabled(string name)
        {
            return _dao.GetByPecEnabled(name);
        }

        public IList<UDSRepository> GetByPecAnnexedEnabled(string name)
        {
            return _dao.GetByPecAnnexedEnabled(name);
        }

        public IList<UDSRepository> GetFascicolable()
        {
            return _dao.GetFascicolableRepositories();
        }

        public IList<UDSRepository> GetProtocollable()
        {
            return _dao.GetProtocollableRepositories();
        }

        public bool HasProtocollableRepositories()
        {
            return _dao.HasProtocollableRepositories();
        }

        public Guid SendCommandInsertData(Guid idRepository, Guid correlationId, UDSModel model, Guid? idFascicle)
        {
            CommandFacade<ICommandInsertUDSData> commandFacade = new CommandFacade<ICommandInsertUDSData>();
            IdentityContext identity = new IdentityContext(DocSuiteContext.Current.User.FullUserName);
            UDSBuildModel commandModel = CreateUDSCommandModel(idRepository, null, model, idFascicle);

            ICommandInsertUDSData commandInsert = new CommandInsertUDSData(correlationId, CurrentTenant.TenantName, CurrentTenant.UniqueId, CurrentTenant.TenantAOO.UniqueId, identity, commandModel);
            commandFacade.Push(commandInsert);
            return commandInsert.Id;
        }

        public Guid SendCommandUpdateData(Guid idRepository, Guid idUDS, Guid correlationId, UDSModel model)
        {
            CommandFacade<ICommandUpdateUDSData> commandFacade = new CommandFacade<ICommandUpdateUDSData>();
            IdentityContext identity = new IdentityContext(DocSuiteContext.Current.User.FullUserName);
            UDSBuildModel commandModel = CreateUDSCommandModel(idRepository, idUDS, model);

            ICommandUpdateUDSData commandUpdate = new CommandUpdateUDSData(correlationId, CurrentTenant.TenantName, CurrentTenant.UniqueId, CurrentTenant.TenantAOO.UniqueId, identity, commandModel);
            commandFacade.Push(commandUpdate);
            return commandUpdate.Id;
        }

        public Guid SendCommandDeleteData(Guid idRepository, Guid idUDS, Guid correlationId, UDSModel model, string cancelMotivation)
        {
            CommandFacade<ICommandDeleteUDSData> commandFacade = new CommandFacade<ICommandDeleteUDSData>();
            IdentityContext identity = new IdentityContext(DocSuiteContext.Current.User.FullUserName);

            UDSBuildModel commandModel = CreateUDSCommandModel(idRepository, idUDS, model);
            commandModel.CancelMotivation = cancelMotivation;
            ICommandDeleteUDSData commandCancel = new CommandDeleteUDSData(correlationId, CurrentTenant.TenantName, CurrentTenant.UniqueId, CurrentTenant.TenantAOO.UniqueId, identity, commandModel);
            commandFacade.Push(commandCancel);
            return commandCancel.Id;
        }

        private UDSBuildModel CreateUDSCommandModel(Guid idRepository, Guid? idUDS, UDSModel model, Guid? idFascicle = null)
        {
            UDSRepositoryModelMapper repositoryModelMapper = new UDSRepositoryModelMapper();
            UDSBuildModel commandModel = new UDSBuildModel(model.SerializeToXml());
            UDSRepository repository = GetById(idRepository);

            idUDS = idUDS ?? Guid.Parse(model.Model.UDSId);

            if (model.Model.Collaborations != null && model.Model.Collaborations.Instances != null)
            {
                foreach (CollaborationInstance item in model.Model.Collaborations.Instances)
                {
                    commandModel.WorkflowActions.Add(new WorkflowActionDocumentUnitLinkModel(
                        new DocumentUnitModel()
                        {
                            UniqueId = idUDS.Value,
                            Environment = repository.DSWEnvironment
                        },
                        new DocumentUnitModel()
                        {
                            UniqueId = Guid.Parse(item.UniqueId),
                            EntityId = item.IdCollaboration,
                            Environment = (int)DSWEnvironmentType.Collaboration
                        }));
                }
            }

            if (idFascicle.HasValue)
            {
                commandModel.WorkflowActions.Add(new WorkflowActionFascicleModel(
                    new FascicleModel { UniqueId = idFascicle.Value },
                    new DocumentUnitModel { UniqueId = idUDS.Value, Environment = (int)DSWEnvironmentType.UDS },
                    null));
            }

            commandModel.UDSRepository = repositoryModelMapper.MappingDTO(repository);
            commandModel.UniqueId = idUDS.Value;
            commandModel.RegistrationUser = DocSuiteContext.Current.User.FullUserName;

            return commandModel;
        }

        /// <summary>
        /// Ritorna un nuovo oggetto ProtocolInitializer per la protocollazione automatica di una UDS
        /// </summary>
        public ProtocolInitializer GetProtocolInitializer(UDSDto dto)
        {
            ProtocolInitializer protInitializer = new ProtocolInitializer();
            // Oggetto
            protInitializer.Subject = dto.UDSModel.Model.Subject.Value;
            if (dto.UDSModel.Model.ProtocolDirectionSpecified)
            {
                switch (dto.UDSModel.Model.ProtocolDirection)
                {
                    case ProtocolDirectionType.None:
                        break;
                    case ProtocolDirectionType.In:
                        {
                            protInitializer.ProtocolType = -1;
                            break;
                        }
                    case ProtocolDirectionType.Out:
                        {
                            protInitializer.ProtocolType = 1;
                            break;
                        }
                    case ProtocolDirectionType.InternalOffice:
                        {
                            protInitializer.ProtocolType = 0;
                            break;
                        }
                    default:
                        break;
                }
            }

            // Classificazione
            if (dto.UDSModel.Model.Category.DefaultEnabled)
            {
                int idCategory = int.Parse(dto.UDSModel.Model.Category.IdCategory);
                protInitializer.Category = FacadeFactory.Instance.CategoryFacade.GetById(idCategory);
            }

            // Gestione documenti        
            if (dto.UDSModel.Model.Documents != null)
            {
                // Documento principale
                ICollection<BiblosDocumentInfo> mainDocuments = FillUDSDocuments(dto.UDSModel.Model.Documents.Document);
                if (mainDocuments.Count > 0)
                {
                    protInitializer.MainDocument = mainDocuments.FirstOrDefault();
                }

                // Allegati
                ICollection<BiblosDocumentInfo> attachments = FillUDSDocuments(dto.UDSModel.Model.Documents.DocumentAttachment);
                if (attachments.Any())
                {
                    protInitializer.Attachments = attachments.Cast<DocumentInfo>().ToList();
                }

                // Annessi
                ICollection<BiblosDocumentInfo> annexed = FillUDSDocuments(dto.UDSModel.Model.Documents.DocumentAnnexed);
                if (annexed.Any())
                {
                    protInitializer.Annexed = annexed.Cast<DocumentInfo>().ToList();
                }
            }

            // Contatti
            protInitializer.Senders = new List<ContactDTO>();
            protInitializer.Recipients = new List<ContactDTO>();
            if (dto.UDSModel.Model.Contacts != null)
            {
                foreach (Contacts contact in dto.UDSModel.Model.Contacts)
                {
                    ICollection<ContactDTO> contactDtos = FillUDSContacts(contact.ContactInstances).Concat(FillUDSContacts(contact.ContactManualInstances)).ToList();
                    if (contact.ContactType.Equals(Helpers.UDS.ContactType.Sender))
                    {
                        protInitializer.Senders.AddRange(contactDtos);
                    }
                    else
                    {
                        protInitializer.Recipients.AddRange(contactDtos);
                    }
                }
            }

            // Settori
            if (dto.UDSModel.Model.Authorizations != null && dto.UDSModel.Model.Authorizations.Instances != null)
            {
                IList<Data.Role> roles = new List<Data.Role>();
                foreach (AuthorizationInstance auth in dto.UDSModel.Model.Authorizations.Instances)
                {
                    Data.Role role = FacadeFactory.Instance.RoleFacade.GetById(auth.IdAuthorization);
                    roles.Add(role);
                }

                protInitializer.Roles = roles;
            }

            return protInitializer;
        }

        private IList<ContactDTO> FillUDSContacts(ContactInstance[] items)
        {
            IList<ContactDTO> dtos = new List<ContactDTO>();
            if (items == null)
                return new List<ContactDTO>();

            foreach (ContactInstance instance in items)
            {
                ContactDTO dto = new ContactDTO();
                dto.Id = instance.IdContact;
                dto.Contact = FacadeFactory.Instance.ContactFacade.GetById(dto.Id);
                dto.Type = ContactDTO.ContactType.Address;
                dtos.Add(dto);
            }
            return dtos;
        }

        private IList<ContactDTO> FillUDSContacts(ContactManualInstance[] items)
        {
            IList<ContactDTO> dtos = new List<ContactDTO>();
            if (items == null)
            {
                return new List<ContactDTO>();
            }

            foreach (ContactManualInstance instance in items.Where(f => !string.IsNullOrEmpty(f.ContactDescription)))
            {
                ContactDTO dto = JsonConvert.DeserializeObject<ContactDTO>(instance.ContactDescription);
                dtos.Add(dto);
            }
            return dtos;
        }

        private ICollection<BiblosDocumentInfo> FillUDSDocuments(Helpers.UDS.Document document)
        {
            if (document == null || document.Instances == null)
            {
                return new List<BiblosDocumentInfo>();
            }

            IList<BiblosDocumentInfo> docInfos = new List<BiblosDocumentInfo>();
            foreach (DocumentInstance instance in document.Instances)
            {
                IList<BiblosDocumentInfo> bibDocs = BiblosDocumentInfo.GetDocuments(Guid.Parse(instance.StoredChainId));
                foreach (BiblosDocumentInfo doc in bibDocs)
                {
                    docInfos.Add(doc);
                }
            }

            return docInfos.ToList();
        }
        #endregion
    }
}
