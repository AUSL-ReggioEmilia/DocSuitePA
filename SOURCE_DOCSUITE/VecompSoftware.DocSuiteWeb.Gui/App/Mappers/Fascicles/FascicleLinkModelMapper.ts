import FascicleLinkModel = require('App/Models/Fascicles/FascicleLinkModel');
import FascicleModelMapper = require('App/Mappers/Fascicles/FascicleModelMapper');
import BaseMapper = require('App/Mappers/BaseMapper');
import RequireJSHelper = require('App/Helpers/RequireJSHelper');

class FascicleLinkModelMapper extends BaseMapper<FascicleLinkModel>{
    constructor() {
        super();
    }    

    public Map(source: any): FascicleLinkModel {
        let toMap: FascicleLinkModel = <FascicleLinkModel>{};

        if (!source) {
            return null;
        }

        let _fascicleModelMapper: FascicleModelMapper;        
        _fascicleModelMapper = RequireJSHelper.getModule<FascicleModelMapper>(FascicleModelMapper, 'App/Mappers/Fascicles/FascicleModelMapper');

        toMap.Fascicle = source.Fascicle ? _fascicleModelMapper.Map(source.Fascicle) : null;
        toMap.FascicleLinked = source.FascicleLinked ? _fascicleModelMapper.Map(source.FascicleLinked) : null;
        toMap.FascicleLinkType = source.FascicleLinkType;
        toMap.UniqueId = source.UniqueId;

        return toMap;
    }
}

export = FascicleLinkModelMapper;