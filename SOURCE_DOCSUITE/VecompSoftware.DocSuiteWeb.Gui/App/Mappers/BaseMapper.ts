import IMapper = require('App/Mappers/IMapper');

abstract class BaseMapper<T> implements IMapper<T> {
    constructor() {
    }

    public abstract Map(source: any): T;
    public MapCollection = (source: any) => {
        let mappedEntities: Array<T> = new Array<T>();
        if (source) {
            if ('$values' in source) {
                source = source.$values;
            }
            source.forEach((item) => { if (item) { mappedEntities.push(this.Map(item)) } });
        }
        return mappedEntities;
    };
}

export = BaseMapper;