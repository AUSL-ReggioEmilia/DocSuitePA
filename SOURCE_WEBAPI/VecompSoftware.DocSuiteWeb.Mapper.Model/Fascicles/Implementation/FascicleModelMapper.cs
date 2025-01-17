﻿using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Processes;
using VecompSoftwareFascicle = VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Fascicles
{
    public class FascicleModelMapper : BaseModelMapper<Fascicle, VecompSoftwareFascicle.FascicleModel>, IFascicleModelMapper
    {
        #region [ Fields ]
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion

        #region [ Constructor ]
        public FascicleModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        #endregion

        #region [ Methods ]
        public override VecompSoftwareFascicle.FascicleModel Map(Fascicle entity, VecompSoftwareFascicle.FascicleModel modelTransformed)
        {
            modelTransformed.UniqueId = entity.UniqueId;
            modelTransformed.Year = entity.Year;
            modelTransformed.Number = entity.Number;
            modelTransformed.Conservation = entity.Conservation;
            modelTransformed.StartDate = entity.StartDate;
            modelTransformed.EndDate = entity.EndDate;
            modelTransformed.Name = entity.Name;
            modelTransformed.Title = entity.Title;
            modelTransformed.FascicleObject = entity.FascicleObject;
            modelTransformed.RegistrationDate = entity.RegistrationDate;
            modelTransformed.RegistrationUser = entity.RegistrationUser;
            modelTransformed.Manager = entity.Manager;
            modelTransformed.Rack = entity.Rack;
            modelTransformed.Note = entity.Note;
            modelTransformed.FascicleType = (VecompSoftwareFascicle.FascicleType)entity.FascicleType;
            modelTransformed.VisibilityType = (VecompSoftwareFascicle.VisibilityType)entity.VisibilityType;
            modelTransformed.MetadataValues = entity.MetadataValues;
            modelTransformed.MetadataDesigner = entity.MetadataDesigner;
            modelTransformed.DSWEnvironment = entity.DSWEnvironment;
            modelTransformed.CustomActions = entity.CustomActions;
            modelTransformed.ProcessLabel = entity.ProcessLabel;
            modelTransformed.DossierFolderLabel = entity.DossierFolderLabel;
            modelTransformed.FascicleDocumentUnits = _mapperUnitOfWork.Repository<IDomainMapper<FascicleDocumentUnit, VecompSoftwareFascicle.FascicleDocumentUnitModel>>().MapCollection(entity.FascicleDocumentUnits);
            modelTransformed.FascicleDocuments = _mapperUnitOfWork.Repository<IDomainMapper<FascicleDocument, VecompSoftwareFascicle.FascicleDocumentModel>>().MapCollection(entity.FascicleDocuments);
            modelTransformed.Category = entity.Category == null ? null : _mapperUnitOfWork.Repository<IDomainMapper<Category, CategoryModel>>().Map(entity.Category, new CategoryModel());
            modelTransformed.Container = entity.Container == null ? null : _mapperUnitOfWork.Repository<IDomainMapper<Container, ContainerModel>>().Map(entity.Container, new ContainerModel());
            modelTransformed.Contacts = entity.Contacts == null ? null : _mapperUnitOfWork.Repository<IDomainMapper<Contact, ContactModel>>().MapCollection(entity.Contacts);
            modelTransformed.DossierFolders = entity.DossierFolders == null ? null : _mapperUnitOfWork.Repository<IDomainMapper<DossierFolder, DossierFolderModel>>().MapCollection(entity.DossierFolders);
            modelTransformed.FascicleTemplate = entity.FascicleTemplate == null ? null : _mapperUnitOfWork.Repository<IDomainMapper<ProcessFascicleTemplate, ProcessFascicleTemplateModel>>().Map(entity.FascicleTemplate, new ProcessFascicleTemplateModel());
            modelTransformed.LastChangedDate = entity.LastChangedDate;
            return modelTransformed;
        }
        #endregion
    }
}
