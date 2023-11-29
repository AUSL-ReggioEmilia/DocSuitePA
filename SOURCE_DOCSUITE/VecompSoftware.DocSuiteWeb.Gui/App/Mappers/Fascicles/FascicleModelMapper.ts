import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import BaseMapper = require('App/Mappers/BaseMapper');
import CategoryModelMapper = require('App/Mappers/Commons/CategoryModelMapper');
import ContactModelMapper = require('App/Mappers/Commons/ContactModelMapper');
import FascicleRoleModelMapper = require('App/Mappers/Fascicles/FascicleRoleModelMapper');
import FascicleDocumentModelMapper = require('App/Mappers/Fascicles/FascicleDocumentModelMapper');
import FascicleLinkModelMapper = require('App/Mappers/Fascicles/FascicleLinkModelMapper');
import FascicleDocumentUnitModelMapper = require('App/Mappers/Fascicles/FascicleDocumentUnitModelMapper');
import RequireJSHelper = require('App/Helpers/RequireJSHelper');
import ContainerModelMapper = require('App/Mappers/Commons/ContainerModelMapper');
import DossierFolderModelMapper = require('App/Mappers/Dossiers/DossierFolderModelMapper');
import TenantAOOModelMapper = require('App/Mappers/Tenants/TenantAOOModelMapper');
import MetadataRepositoryModelMapper = require('App/Mappers/Commons/MetadataRepositoryModelMapper');

class FascicleModelMapper extends BaseMapper<FascicleModel>{

    categoryMapper: typeof CategoryModelMapper;

    constructor() {
        super();
    }

    public Map(source: any): FascicleModel {

        const toMap: FascicleModel = {} as FascicleModel;

        if (!source) {
            return null;
        }

        const _fascicleDocumentUnitModelMapper: FascicleDocumentUnitModelMapper = RequireJSHelper.getModule<FascicleDocumentUnitModelMapper>(FascicleDocumentUnitModelMapper, 'App/Mappers/Fascicles/FascicleDocumentUnitModelMapper');
        const _fascicleDocumentModelMapper: FascicleDocumentModelMapper = RequireJSHelper.getModule<FascicleDocumentModelMapper>(FascicleDocumentModelMapper, 'App/Mappers/Fascicles/FascicleDocumentModelMapper');
        const _dossierFolderModelMapper: DossierFolderModelMapper = RequireJSHelper.getModule<DossierFolderModelMapper>(DossierFolderModelMapper, 'App/Mappers/Dossiers/DossierFolderModelMapper');
        const _tenantAOOModelMapper: TenantAOOModelMapper = RequireJSHelper.getModule<TenantAOOModelMapper>(TenantAOOModelMapper, 'App/Mappers/Tenants/TenantAOOModelMapper');
        const _metadataRepositoryModelMapper: MetadataRepositoryModelMapper = RequireJSHelper.getModule<MetadataRepositoryModelMapper>(MetadataRepositoryModelMapper, 'App/Mappers/Commons/MetadataRepositoryModelMapper');

        toMap.TenantAOO = source.TenantAOO ? _tenantAOOModelMapper.Map(source.TenantAOO) : null;
        toMap.Category = source.Category ? new CategoryModelMapper().Map(source.Category) : null;
        toMap.Container = source.Container ? new ContainerModelMapper().Map(source.Container) : null;
        toMap.Contacts = source.Contacts ? new ContactModelMapper().MapCollection(source.Contacts) : null;
        toMap.FascicleDocuments = source.FascicleDocuments ? _fascicleDocumentModelMapper.MapCollection(source.FascicleDocuments) : null;
        toMap.FascicleDocumentUnits = source.FascicleDocumentUnits ? _fascicleDocumentUnitModelMapper.MapCollection(source.FascicleDocumentUnits) : null;
        toMap.FascicleLinks = source.FascicleLinks ? new FascicleLinkModelMapper().MapCollection(source.FascicleLinks) : null;
        toMap.FascicleRoles = source.FascicleRoles ? new FascicleRoleModelMapper().MapCollection(source.FascicleRoles) : null;
        toMap.LinkedFascicles = source.LinkedFascicles ? new FascicleLinkModelMapper().MapCollection(source.LinkedFascicles) : null;
        toMap.DossierFolders = source.DossierFolders ? _dossierFolderModelMapper.MapCollection(source.DossierFolders) : [];
        toMap.MetadataRepository = source.MetadataRepository ? _metadataRepositoryModelMapper.Map(source.MetadataRepository) : null;

        toMap.FascicleObject = source.FascicleObject;
        toMap.FascicleType = source.FascicleType;
        toMap.Manager = source.Manager;
        toMap.Name = source.Name;
        toMap.Note = source.Note;
        toMap.Number = source.Number;
        toMap.Rack = source.Rack;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.LastChangedDate = source.LastChangedDate;
        toMap.LastChangedUser = source.LastChangedUser;
        toMap.StartDate = source.StartDate;
        toMap.Title = source.Title;
        toMap.UniqueId = source.UniqueId;
        toMap.VisibilityType = source.VisibilityType;
        toMap.Year = source.Year;
        toMap.MetadataValues = source.MetadataValues;
        toMap.MetadataDesigner = source.MetadataDesigner;
        toMap.CustomActions = source.CustomActions;
        toMap.ProcessLabel = source.ProcessLabel;
        toMap.DossierFolderLabel = source.DossierFolderLabel;
        toMap.EndDate = source.EndDate;
        toMap.Conservation = source.Conservation;

        return toMap;
    }
}

export =FascicleModelMapper;