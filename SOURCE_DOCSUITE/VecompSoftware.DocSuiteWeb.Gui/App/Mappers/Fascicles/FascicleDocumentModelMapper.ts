import FascicleDocumentModel = require('App/Models/Fascicles/FascicleDocumentModel');
import BaseMapper = require('App/Mappers/BaseMapper');
import FascicleModelMapper = require('App/Mappers/Fascicles/FascicleModelMapper');
import RequireJSHelper = require('App/Helpers/RequireJSHelper');

class FascicleDocumentModelMapper extends BaseMapper<FascicleDocumentModel>{
     constructor() {
        super();
    }

    public Map(source: any): FascicleDocumentModel {
        let toMap: FascicleDocumentModel = <FascicleDocumentModel>{};

        if (!source) {
            return null;
        }

        let _fascicleModelMapper: FascicleModelMapper;
        _fascicleModelMapper = RequireJSHelper.getModule<FascicleModelMapper>(FascicleModelMapper, 'App/Mappers/Fascicles/FascicleModelMapper');

        toMap.ChainType = source.ChainType;
        toMap.Fascicle = source.Fascicle ? _fascicleModelMapper.Map(source.Fascicle) : null;
        toMap.IdArchiveChain = source.IdArchiveChain;
        toMap.UniqueId = source.UniqueId;

        return toMap;
    }
}

export =FascicleDocumentModelMapper;