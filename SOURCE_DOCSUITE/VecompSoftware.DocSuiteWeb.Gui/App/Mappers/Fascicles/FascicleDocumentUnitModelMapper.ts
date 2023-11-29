import FascicleDocumentUnitModel = require('App/Models/Fascicles/FascicleDocumentUnitModel');
import BaseMapper = require('App/Mappers/BaseMapper');
import FascicleModelMapper = require('App/Mappers/Fascicles/FascicleModelMapper');
import DocumentUnitModelMapper = require('App/Mappers/DocumentUnits/DocumentUnitModelMapper');
import RequireJSHelper = require('App/Helpers/RequireJSHelper');

class FascicleDocumentUnitModelMapper extends BaseMapper<FascicleDocumentUnitModel>{
     constructor() {
        super();
    }

    public Map(source: any): FascicleDocumentUnitModel {
        let toMap: FascicleDocumentUnitModel = <FascicleDocumentUnitModel>{};   

        if (!source) {
            return null;
        }

        let _fascicleModelMapper: FascicleModelMapper;
        _fascicleModelMapper = RequireJSHelper.getModule<FascicleModelMapper>(FascicleModelMapper, 'App/Mappers/Fascicles/FascicleModelMapper');

        toMap.Fascicle = source.Fascicle ? _fascicleModelMapper.Map(source.Fascicle) : null;       
        toMap.UniqueId = source.UniqueId;
        toMap.DocumentUnit = source.DocumentUnit ? new DocumentUnitModelMapper().Map(source.DocumentUnit) : null;
        toMap.ReferenceType = source.ReferenceType;
        toMap.SequenceNumber = source.SequenceNumber;
        return toMap;
    }
}

export = FascicleDocumentUnitModelMapper;