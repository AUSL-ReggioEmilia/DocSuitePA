import BaseMapper = require('App/Mappers/BaseMapper');
import DocumentUnitModel = require('App/Models/DocumentUnits/DocumentUnitModel');
import UDSRepositoryModel = require('App/Models/UDS/UDSRepositoryModel');
import CategoryModelMapper = require('App/Mappers/Commons/CategoryModelMapper');
import ContainerModelMapper = require('App/Mappers/Commons/ContainerModelMapper');
import FascicleReferenceType = require('App/Models/Fascicles/FascicleReferenceType');
import UDSRepositoryModelMapper = require('App/Mappers/UDS/UDSRepositoryModelMapper')
import DocumentUnitRoleMapper = require('./DocumentUnitRoleMapper');
import DocumentUnitChainMapper = require('./DocumentUnitChainMapper');


class DocumentUnitModelMapper extends BaseMapper<DocumentUnitModel> {
    constructor() {
        super();
    }

    public Map(source: any): DocumentUnitModel {
        let toMap: DocumentUnitModel = <DocumentUnitModel>{};

        if (!source) {
            return null;
        }

        toMap.Environment = source.Environment;
        toMap.UniqueId = source.UniqueId;
        toMap.EntityId = source.EntityId;
        toMap.DocumentUnitName = source.DocumentUnitName;
        toMap.Year = source.Year;
        toMap.Number = source.Number;
        toMap.Title = source.Title;
        toMap.ReferenceType = source.ReferenceType;
        toMap.SequenceNumber = source.SequenceNumber;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.Subject = source.Subject;
        toMap.Category = source.Category ? new CategoryModelMapper().Map(source.Category) : null;
        toMap.Container = source.Container ? new ContainerModelMapper().Map(source.Container) : null;
        toMap.UDSRepository = source.UDSRepository ? new UDSRepositoryModelMapper().Map(source.UDSRepository) : null;
        toMap.DocumentUnitRoles = source.DocumentUnitRoles ? new DocumentUnitRoleMapper().MapCollection(source.DocumentUnitRoles) : null;
        toMap.DocumentUnitChains = source.DocumentUnitChains ? new DocumentUnitChainMapper().MapCollection(source.DocumentUnitChains) : null;

        if (source.IdUDSRepository && !toMap.UDSRepository) {
            toMap.UDSRepository = new UDSRepositoryModel();
            toMap.UDSRepository.UniqueId = source.IdUDSRepository;
        }

        toMap.IdFascicle = source.IdFascicle;
        toMap.IsFascicolable = source.IsFascicolable;        

        return toMap;
    }
}

export = DocumentUnitModelMapper;