import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel');
import FascicleDocumentModel = require('App/Models/Fascicles/FascicleDocumentModel');
import FascicleLinkModel = require('App/Models/Fascicles/FascicleLinkModel');
import BaseMapper = require('App/Mappers/BaseMapper');
import CategoryModelMapper = require('App/Mappers/Commons/CategoryModelMapper');
import ContactModelMapper = require('App/Mappers/Commons/ContactModelMapper');
import FascicleRoleModelMapper = require('App/Mappers/Fascicles/FascicleRoleModelMapper');
import FascicleDocumentModelMapper = require('App/Mappers/Fascicles/FascicleDocumentModelMapper');
import FascicleLinkModelMapper = require('App/Mappers/Fascicles/FascicleLinkModelMapper');
import FascicleDocumentUnitModelMapper = require('App/Mappers/Fascicles/FascicleDocumentUnitModelMapper');
import RequireJSHelper = require('App/Helpers/RequireJSHelper');
import ContainerModelMapper = require('App/Mappers/Commons/ContainerModelMapper');
import DossierFolderModelMapper = require('../Dossiers/DossierFolderModelMapper');

class FascicleModelMapper extends BaseMapper<FascicleModel>{

    categoryMapper: typeof CategoryModelMapper;

    constructor() {
        super();
    }

    public Map(source: any): FascicleModel {

        let toMap: FascicleModel = <FascicleModel>{};

        if (!source) {
            return null;
        }

        const _fascicleDocumentUnitModelMapper: FascicleDocumentUnitModelMapper = RequireJSHelper.getModule<FascicleDocumentUnitModelMapper>(FascicleDocumentUnitModelMapper, 'App/Mappers/Fascicles/FascicleDocumentUnitModelMapper');
        const _fascicleDocumentModelMapper: FascicleDocumentModelMapper = RequireJSHelper.getModule<FascicleDocumentModelMapper>(FascicleDocumentModelMapper, 'App/Mappers/Fascicles/FascicleDocumentModelMapper');


        toMap.Category = source.Category ? new CategoryModelMapper().Map(source.Category) : null;

        toMap.Container = source.Container ? new ContainerModelMapper().Map(source.Container) : null;

        toMap.Conservation = source.Conservation;

        toMap.Contacts = source.Contacts ? new ContactModelMapper().MapCollection(source.Contacts) : null;

        toMap.EndDate = source.EndDate;

        toMap.FascicleDocuments = source.FascicleDocuments ? _fascicleDocumentModelMapper.MapCollection(source.FascicleDocuments) : null;

        toMap.FascicleDocumentUnits = source.FascicleDocumentUnits ? _fascicleDocumentUnitModelMapper.MapCollection(source.FascicleDocumentUnits) : null;

        toMap.FascicleLinks = source.FascicleLinks ? new FascicleLinkModelMapper().MapCollection(source.FascicleLinks) : null;

        toMap.FascicleObject = source.FascicleObject;

        toMap.FascicleRoles = source.FascicleRoles ? new FascicleRoleModelMapper().MapCollection(source.FascicleRoles) : null;

        toMap.FascicleType = source.FascicleType;

        toMap.LinkedFascicles = source.LinkedFascicles ? new FascicleLinkModelMapper().MapCollection(source.LinkedFascicles) : null;

        toMap.DossierFolders = source.DossierFolders ? new DossierFolderModelMapper().MapCollection(source.DossierFolders) : null;
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

        return toMap;
    }
}

export =FascicleModelMapper;