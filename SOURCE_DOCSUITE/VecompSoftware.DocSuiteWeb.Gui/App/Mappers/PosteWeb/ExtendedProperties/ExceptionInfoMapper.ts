import BaseMapper = require("App/Mappers/BaseMapper");
import ExceptionInfo = require("App/Models/PosteWeb/ExtendedProperties/ExceptionInfo");

class ExceptionInfoMapper extends BaseMapper<ExceptionInfo>
{

    constructor() {
        super();
    }

    public Map(source: any): ExceptionInfo {
        let toMap: ExceptionInfo = <ExceptionInfo>{};

        if (!source) {
            return null;
        }

        toMap.ExceptionMessage = source.ExceptionMessage;
        toMap.ExceptionStackTrace = source.ExceptionStackTrace;

        return toMap;
    }
}

export = ExceptionInfoMapper