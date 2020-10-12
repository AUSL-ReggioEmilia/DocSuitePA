import TenantAOOModel = require("../../Models/Tenants/TenantAOOModel");
import BaseMapper = require("../BaseMapper");

class TenantAOOModelMapper extends BaseMapper<TenantAOOModel>{
    constructor() {
        super();
    }
    public Map(source: any): TenantAOOModel {
        let toMap: TenantAOOModel = <TenantAOOModel>{};

        if (!source) {
            return null;
        }
        toMap.UniqueId = source.UniqueId;
        toMap.Name = source.Name;
        toMap.Note = source.Note;
        toMap.CategorySuffix = source.CategorySuffix;
        toMap.Tenants = source.Tenants;
        toMap.TenantTypology = source.TenantTypology;
        return toMap;
    }
}

export = TenantAOOModelMapper;