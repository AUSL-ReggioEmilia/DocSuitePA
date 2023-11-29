using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.UDS;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper.Model.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Services.Command.CQRS.Commands.Models.UDS;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.UDS
{
    public class UDSRepositoryFacade : FacadeWebAPIBase<UDSRepository, UDSRepositoryDao>
    {
        #region [ Fields ]
        private UDSRepositoryFinder _currentRepositoryFinder;
        private UDSFieldListFinder _currentFieldListFinder;
        private UDSFieldListFacade _currentFieldListFacade;
        #endregion [ Fields ]

        #region [ Properties ]
        private UDSRepositoryFinder CurrentRepositoryFinder
        {
            get
            {
                if (_currentRepositoryFinder == null)
                {
                    _currentRepositoryFinder = new UDSRepositoryFinder(DocSuiteContext.Current.Tenants);
                }
                return _currentRepositoryFinder;
            }
        }

        private UDSFieldListFinder CurrentFieldListFinder
        {
            get
            {
                if (_currentFieldListFinder == null)
                {
                    _currentFieldListFinder = new UDSFieldListFinder(DocSuiteContext.Current.Tenants);
                }
                return _currentFieldListFinder;
            }
        }

        private UDSFieldListFacade CurrentFieldListFacade
        {
            get
            {
                return _currentFieldListFacade;
            }
        }

        #endregion [ Properties ]

        #region [ Constructor ]
        public UDSRepositoryFacade(ICollection<TenantModel> model, Tenant currentTenant)
            : base(model.Select(s => new WebAPITenantConfiguration<UDSRepository, UDSRepositoryDao>(s)).ToList(), currentTenant)
        {
            _currentFieldListFacade = new UDSFieldListFacade(model, currentTenant);
        }
        #endregion [ Constructor ]

        #region [ Methods ]
        public Guid SaveRepository(UDSModel model, DateTimeOffset activeDate, Guid idRepository, bool publish)
        {
            Guid draftIdSaved = SaveDraftRepository(model, idRepository);
            if (publish)
            {
                //Se l'ID della bozza salvata è uguale all'idRepository in ingresso significa che lo schema deve essere modificato
                ConfirmRepository(model, activeDate, draftIdSaved);
            }
            return draftIdSaved;
        }

        private void ConfirmRepository(UDSModel model, DateTimeOffset activeDate, Guid idRepository)
        {
            if (idRepository.Equals(Guid.Empty))
            {
                throw new ArgumentNullException("idRepository");
            }

            UDSSchemaRepositoryModelMapper repositoryschemaModelMapper = new UDSSchemaRepositoryModelMapper();
            UDSRepositoryModelMapper repositoryModelMapper = new UDSRepositoryModelMapper(repositoryschemaModelMapper);
            UDSBuildModel commandModel = new UDSBuildModel(model.SerializeToXml());
            IdentityContext identity = new IdentityContext(DocSuiteContext.Current.User.FullUserName);
            string tenantName = DocSuiteContext.Current.ProtocolEnv.CorporateAcronym;
            Guid tenantId = DocSuiteContext.Current.CurrentTenant.TenantId;

            WebAPIDto<UDSRepository> resultDto = null;
            WebAPIImpersonatorFacade.ImpersonateFinder(CurrentRepositoryFinder, (impersonationType, finder) =>
            {
                finder.UniqueId = idRepository;
                finder.EnablePaging = false;
                finder.ExpandProperties = true;
                finder.ActionType = UDSRepositoryFinderActionType.FindElement;
                resultDto = finder.DoSearch().FirstOrDefault();
                finder.ResetDecoration();
            });

            UDSRepository repository = resultDto.Entity;
            commandModel.UDSRepository = repositoryModelMapper.Map(repository, new UDSRepositoryModel());
            commandModel.ActiveDate = activeDate;
            commandModel.UniqueId = repository.UniqueId;
            commandModel.RegistrationUser = repository.RegistrationUser;

            if (repository.Version > 0)
            {
                ICommandUpdateUDS commandUpdate = new CommandUpdateUDS(CurrentTenant.TenantName, CurrentTenant.UniqueId, CurrentTenant.TenantAOO.UniqueId, identity, commandModel);
                CommandFacade<ICommandUpdateUDS> commandUpdateFacade = new CommandFacade<ICommandUpdateUDS>();
                commandUpdateFacade.Push(commandUpdate);
            }
            else
            {
                ICommandCreateUDS commandInsert = new CommandCreateUDS(CurrentTenant.TenantName, CurrentTenant.UniqueId, CurrentTenant.TenantAOO.UniqueId, identity, commandModel);
                CommandFacade<ICommandCreateUDS> commandCreateFacade = new CommandFacade<ICommandCreateUDS>();
                commandCreateFacade.Push(commandInsert);
            }
        }

        /// <summary>
        /// Salva o modifica una Bozza in UDSRepositories
        /// </summary>
        /// <returns>Il medesimo IdRepository del parametro idRepository se l'oggetto si trova in stato Bozza, altrimenti viene creato un nuovo ID</returns>
        private Guid SaveDraftRepository(UDSModel model, Guid idRepository)
        {
            UDSRepository repository = null;
            UDSRepository savedRepository = null;
            if (!idRepository.Equals(Guid.Empty))
            {
                WebAPIImpersonatorFacade.ImpersonateFinder(CurrentRepositoryFinder, (impersonationType, finder) =>
                {
                    //Se il repository recuperato è in stato Bozza allora procedo alla modifica del medesimo oggetto,
                    //viceversa creo sempre una nuova Bozza.
                    finder.UniqueId = idRepository;
                    finder.EnablePaging = false;
                    finder.ExpandProperties = true;
                    finder.ActionType = UDSRepositoryFinderActionType.FindElement;
                    WebAPIDto<UDSRepository> resultDto = finder.DoSearch().FirstOrDefault();
                    finder.ResetDecoration();
                    if (resultDto != null && resultDto.Entity != null)
                    {
                        savedRepository = resultDto.Entity;
                        repository = (savedRepository.Status.Equals(Entity.UDS.UDSRepositoryStatus.Draft)) ? savedRepository : null;
                    }
                });
            }

            short idContainer = -1;
            if (repository == null)
            {
                repository = new UDSRepository()
                {
                    ModuleXML = model.SerializeToXml(),
                    Name = model.Model.Title,
                    Status = Entity.UDS.UDSRepositoryStatus.Draft,
                    Version = idRepository.Equals(Guid.Empty) ? (short)0 : savedRepository.Version,
                    Alias = model.Model.Alias
                };

                repository.Container = null;
                if (repository.Version > 0)
                {
                    model.Model.Container.IdContainer = savedRepository.Container.EntityShortId.ToString();
                    model.Model.Title = savedRepository.Name;
                    model.Model.Alias = savedRepository.Alias;
                    model.Model.Container.CreateContainer = false;
                    if (model.Model.Documents != null && model.Model.Documents.Document != null)
                    {
                        model.Model.Documents.Document.CreateBiblosArchive = false;
                    }
                    if (model.Model.Documents != null && model.Model.Documents.DocumentDematerialisation != null)
                    {
                        model.Model.Documents.DocumentDematerialisation.CreateBiblosArchive = false;
                    }
                    repository.DSWEnvironment = savedRepository.DSWEnvironment;
                    repository.Name = savedRepository.Name;
                    repository.Alias = savedRepository.Alias;
                    repository.Container = savedRepository.Container;
                    repository.ModuleXML = model.SerializeToXml();
                }
                if (savedRepository == null && model.Model.Container != null && !string.IsNullOrEmpty(model.Model.Container.IdContainer) && short.TryParse(model.Model.Container.IdContainer, out idContainer))
                {
                    repository.Container = new Entity.Commons.Container() { EntityShortId = idContainer };
                }

                Save(repository);
            }
            else
            {
                repository.Container = savedRepository == null ? null : repository.Container;
                if (repository.Version > 0)
                {
                    model.Model.Container.IdContainer = savedRepository.Container.EntityShortId.ToString();
                    model.Model.Title = savedRepository.Name;
                    model.Model.Alias = savedRepository.Alias;
                    model.Model.Container.CreateContainer = false;
                    if (model.Model.Documents != null && model.Model.Documents.Document != null)
                    {
                        model.Model.Documents.Document.CreateBiblosArchive = false;
                    }
                    if (model.Model.Documents != null && model.Model.Documents.DocumentDematerialisation != null)
                    {
                        model.Model.Documents.DocumentDematerialisation.CreateBiblosArchive = false;
                    }
                    repository.DSWEnvironment = savedRepository.DSWEnvironment;
                    repository.Name = savedRepository.Name;
                    repository.Alias = savedRepository.Alias;
                    repository.Container = savedRepository.Container;
                    repository.ModuleXML = model.SerializeToXml();
                }
                if (savedRepository == null && model.Model.Container != null && !string.IsNullOrEmpty(model.Model.Container.IdContainer) && short.TryParse(model.Model.Container.IdContainer, out idContainer))
                {
                    repository.Container = new Entity.Commons.Container() { EntityShortId = idContainer };
                }
                repository.ModuleXML = model.SerializeToXml();
                repository.Name = model.Model.Title;
                repository.Alias = model.Model.Alias;

                Update(repository);
            }

            UDSRepository lastConfirmRepository = null;
            WebAPIImpersonatorFacade.ImpersonateFinder(CurrentRepositoryFinder, (impersonationType, finder) =>
            {
                finder.ResetDecoration();
                finder.Environment = repository.DSWEnvironment;
                finder.EnablePaging = false;
                finder.ActionType = UDSRepositoryFinderActionType.FindCurrentRepository;
                lastConfirmRepository = finder.DoSearch().SingleOrDefault()?.Entity;
            });
            if (lastConfirmRepository == null)
            {
                lastConfirmRepository = repository;
            }
            WebAPIDto<UDSFieldList> resultFieldsDto = null;
            WebAPIImpersonatorFacade.ImpersonateFinder(CurrentFieldListFinder, (impersonationType, finder) =>
            {
                finder.ResetDecoration();
                FileLogger.Debug(LogName, $"Checking UDS root TreeList of {lastConfirmRepository.Name}");
                finder.IdUDSRepository = lastConfirmRepository.UniqueId;
                finder.FieldUDSRootName = lastConfirmRepository.Name;
                finder.EnablePaging = false;
                resultFieldsDto = finder.DoSearch().SingleOrDefault();
            });

            UDSFieldList udsRootField = resultFieldsDto?.Entity;
            if (udsRootField == null)
            {
                udsRootField = new UDSFieldList()
                {
                    Environment = lastConfirmRepository.DSWEnvironment,
                    FieldName = lastConfirmRepository.Name,
                    ParentInsertId = null,
                    Repository = lastConfirmRepository,
                    Status = Entity.UDS.UDSFieldListStatus.Active,
                    Name = lastConfirmRepository.Name,
                    UniqueId = lastConfirmRepository.UniqueId
                };
                FileLogger.Debug(LogName, $"creating UDS root TreeList of {lastConfirmRepository.Name}");
                CurrentFieldListFacade.Save(udsRootField);
            }

            foreach (TreeListField item in model.Model.Metadata.SelectMany(f => f.Items.OfType<TreeListField>()))
            {
                WebAPIImpersonatorFacade.ImpersonateFinder(CurrentFieldListFinder, (impersonationType, finder) =>
                {
                    finder.ResetDecoration();
                    FileLogger.Debug(LogName, $"Checking FieldTree {item.ColumnName} of {lastConfirmRepository.Name}");
                    finder.IdUDSRepository = lastConfirmRepository.UniqueId;
                    finder.RootName = item.Label;
                    finder.EnablePaging = false;
                    resultFieldsDto = finder.DoSearch().SingleOrDefault();
                });
                if (resultFieldsDto?.Entity != null)
                {
                    resultFieldsDto.Entity.FieldName = item.ColumnName;
                    resultFieldsDto.Entity.Repository = lastConfirmRepository;
                    resultFieldsDto.Entity.Environment = lastConfirmRepository.DSWEnvironment;
                    resultFieldsDto.Entity.Status = Entity.UDS.UDSFieldListStatus.Active;

                    FileLogger.Debug(LogName, $"updating TreeList {item.ColumnName} of {lastConfirmRepository.Name}");
                    CurrentFieldListFacade.Update(resultFieldsDto.Entity);
                }
            }
            return repository.UniqueId;
        }

        public UDSFieldList AddUDSFieldListCtrlNode(Guid idUDSRepository, string name)
        {
            UDSRepository udsRepository = null;

            WebAPIImpersonatorFacade.ImpersonateFinder(CurrentRepositoryFinder, (impersonationType, finder) =>
            {
                finder.UniqueId = idUDSRepository;
                finder.EnablePaging = false;
                finder.ActionType = UDSRepositoryFinderActionType.FindElement;
                udsRepository = finder.DoSearch().FirstOrDefault().Entity;
                finder.ResetDecoration();
            });
            UDSFieldList udsFieldList = new UDSFieldList
            {
                UniqueId = Guid.NewGuid(),
                ParentInsertId = udsRepository.UniqueId,
                Repository = new UDSRepository()
                {
                    UniqueId = udsRepository.UniqueId
                },
                FieldName = name,
                Name = name,
                Environment = udsRepository.DSWEnvironment
            };
            FileLogger.Debug(LogName, $"creating TreeList '{udsFieldList.Name}' of {udsRepository.Name}");
            CurrentFieldListFacade.Save(udsFieldList);
            return udsFieldList;
        }

        public UDSFieldList UpdateUDSFieldListRootNode(Guid idUDSFieldList, string name)
        {
            UDSFieldList udsFieldList = null;
            WebAPIImpersonatorFacade.ImpersonateFinder(CurrentFieldListFinder, (impersonateType, finder) => {
                finder.ResetDecoration();
                finder.UniqueId = idUDSFieldList;
                finder.EnablePaging = false;
                udsFieldList = finder.DoSearch().FirstOrDefault()?.Entity;
                finder.ResetDecoration();
            });
            if (udsFieldList != null)
            {
                udsFieldList.Name = name;
                FileLogger.Debug(LogName, $"updating TreeList '{udsFieldList.Name}' of {udsFieldList.Repository.Name}");
                CurrentFieldListFacade.Update(udsFieldList);
            }
            return udsFieldList;
        }
        #endregion
    }
}
