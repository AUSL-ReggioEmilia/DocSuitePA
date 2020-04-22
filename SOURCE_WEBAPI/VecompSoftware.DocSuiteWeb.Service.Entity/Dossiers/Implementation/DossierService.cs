using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers
{
    public class DossierService : BaseDossierService<Dossier>, IDossierService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]

        public DossierService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IDossierRuleset dossierRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, dossierRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]

        protected override Dossier BeforeCreate(Dossier entity)
        {
            _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entity, null, DossierLogType.Insert, string.Concat("Nuovo dossier '", entity.Subject, "'"), CurrentDomainUser.Account));

            if (entity.Contacts != null)
            {
                HashSet<Contact> contacts = new HashSet<Contact>();
                Contact contact = null;
                foreach (Contact item in entity.Contacts)
                {
                    contact = _unitOfWork.Repository<Contact>().Find(item.EntityId);
                    if (contact != null)
                    {
                        contacts.Add(contact);
                    }
                }
                entity.Contacts = contacts;
            }

            if (entity.MetadataRepository != null)
            {
                entity.MetadataRepository = _unitOfWork.Repository<MetadataRepository>().Find(entity.MetadataRepository.UniqueId);
            }

            if (entity.Container != null)
            {
                entity.Container = _unitOfWork.Repository<Container>().Find(entity.Container.EntityShortId);
            }

            if (entity.DossierRoles != null)
            {
                HashSet<DossierRole> dossierRoles = new HashSet<DossierRole>();
                foreach (DossierRole item in entity.DossierRoles)
                {
                    dossierRoles.Add(CreateDossierRole(item, entity));
                }
                entity.DossierRoles = dossierRoles.Where(f => f != null).ToList();
                _unitOfWork.Repository<DossierRole>().InsertRange(entity.DossierRoles);
            }

            if (entity.DossierDocuments != null && entity.DossierDocuments.Count > 0)
            {
                _unitOfWork.Repository<DossierDocument>().InsertRange(entity.DossierDocuments);
            }

            entity.Year = Convert.ToInt16(DateTime.UtcNow.Year);
            entity.Number = -1;
            int latestNumber = _unitOfWork.Repository<Dossier>().GetLatestNumber(entity.Year);
            entity.Number = ++latestNumber;

            DossierFolder rootNode = new DossierFolder
            {
                UniqueId = entity.UniqueId,
                Dossier = entity,
                Name = string.Format("Dossier {0}/{1:0000000}", entity.Year, entity.Number),
                Status = DossierFolderStatus.InProgress,
            };

            _unitOfWork.Repository<DossierFolder>().Insert(rootNode);
            _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entity, rootNode, DossierLogType.FolderInsert, string.Concat("Creata cartella radice '", rootNode.Name, "'"), CurrentDomainUser.Account));

            return base.BeforeCreate(entity);
        }

        private DossierRole CreateDossierRole(DossierRole entity, Dossier dossier)
        {
            DossierRole dossierRole = null;
            if (entity.Role == null)
            {
                return dossierRole;
            }

            Role role = null;
            role = _unitOfWork.Repository<Role>().Find(entity.Role.EntityShortId);

            if (role != null)
            {
                dossierRole = new DossierRole
                {
                    Role = role,
                    Dossier = dossier,
                    AuthorizationRoleType = AuthorizationRoleType.Responsible,
                    IsMaster = true,
                    Status = DossierRoleStatus.Active
                };
                _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(dossier, null, DossierLogType.Authorize,
                    string.Concat("Autorizzato il dossier al settore '", role.Name, "' (", role.EntityShortId, ") responsabile (Responsible)"), CurrentDomainUser.Account));
            }

            return dossierRole;
        }

        protected override IQueryFluent<Dossier> SetEntityIncludeOnUpdate(IQueryFluent<Dossier> query)
        {
            query.Include(d => d.Contacts)
                 .Include(d => d.DossierDocuments);
            return query;
        }

        protected override Dossier BeforeUpdate(Dossier entity, Dossier entityTransformed)
        {
            if (entity.Contacts != null)
            {
                foreach (Contact item in entityTransformed.Contacts.Where(f => !entity.Contacts.Any(c => c.EntityId == f.EntityId)).ToList())
                {
                    entityTransformed.Contacts.Remove(item);
                    _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed, null, DossierLogType.Modify, string.Concat("Rimosso il contatto '", item.Description, "'"), CurrentDomainUser.Account));
                }
                foreach (Contact item in entity.Contacts.Where(f => !entityTransformed.Contacts.Any(c => c.EntityId == f.EntityId)))
                {
                    entityTransformed.Contacts.Add(_unitOfWork.Repository<Contact>().Find(item.EntityId));
                    _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed, null, DossierLogType.Modify, string.Concat("Aggiunto il contatto '", item.Description, "'"), CurrentDomainUser.Account));
                }
            }

            if (entity.DossierDocuments != null && entity.DossierDocuments.Any())
            {
                foreach (DossierDocument item in entity.DossierDocuments.Where(x => !entityTransformed.DossierDocuments.Any(f => f.UniqueId == x.UniqueId)))
                {
                    _unitOfWork.Repository<DossierDocument>().Insert(item);
                    entityTransformed.DossierDocuments.Add(item);
                    _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed, null, DossierLogType.DocumentInsert, string.Concat("Inserimento documento con id catena ", item.IdArchiveChain), CurrentDomainUser.Account));
                }
            }
            if (entity.Container != null)
            {
                entityTransformed.Container = _unitOfWork.Repository<Container>().Find(entity.Container.EntityShortId);
            }

            if (entity.DossierRoles != null)
            {
                foreach (DossierRole item in entityTransformed.DossierRoles.Where(f => !entity.DossierRoles.Any(c => c.EntityId == f.EntityId)).ToList())
                {
                    entityTransformed.DossierRoles.Remove(item);
                    _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed, null, DossierLogType.Authorize,
                        string.Concat("Rimossa autorizzazione '", item.AuthorizationRoleType.ToString(), "' del settore '", item.Role.Name, "' (", item.Role.EntityShortId, ")"), CurrentDomainUser.Account));
                }
                DossierRole newDossierRole = null;
                foreach (DossierRole item in entity.DossierRoles.Where(f => !entityTransformed.DossierRoles.Any(c => c.EntityId == f.EntityId)))
                {
                    newDossierRole = CreateDossierRole(item, entityTransformed);
                    if (newDossierRole != null)
                    {
                        entityTransformed.DossierRoles.Add(newDossierRole);
                    }
                }
            }

            _unitOfWork.Repository<DossierLog>().Insert(BaseDossierService<DossierLog>.CreateLog(entityTransformed, null, entity.EndDate.HasValue ? DossierLogType.Close : DossierLogType.Modify,
                string.Concat(entity.EndDate.HasValue ? "Chiusura" : "Modifica", " del Dossier"), CurrentDomainUser.Account));

            return base.BeforeUpdate(entity, entityTransformed);
        }

        #endregion
    }
}