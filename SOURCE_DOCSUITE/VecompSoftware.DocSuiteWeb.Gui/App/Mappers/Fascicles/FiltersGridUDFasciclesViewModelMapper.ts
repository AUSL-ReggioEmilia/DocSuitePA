import FiltersGridUDFasciclesGridViewModel = require('App/ViewModels/Fascicles/FiltersGridUDFasciclesViewModel');
import BaseMapper = require('App/Mappers/BaseMapper');

class FiltersGridUDFasciclesViewModelMapper extends BaseMapper<FiltersGridUDFasciclesGridViewModel>{
    constructor() {
        super();
    }
    public Map(source: any): FiltersGridUDFasciclesGridViewModel {
        let toMap: FiltersGridUDFasciclesGridViewModel = <FiltersGridUDFasciclesGridViewModel>{};

        if (!source) {
            return null;
        }

        toMap.DocumentUnitName = source.DocumentUnitName;
        toMap.ReferenceType = source.ReferenceType;
        toMap.Title = source.Title;

        return toMap;
    }
}

export = FiltersGridUDFasciclesViewModelMapper;