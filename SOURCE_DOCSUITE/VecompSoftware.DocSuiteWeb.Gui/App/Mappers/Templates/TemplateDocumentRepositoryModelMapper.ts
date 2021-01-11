import TemplateDocumentRepositoryModel = require('App/Models/Templates/TemplateDocumentRepositoryModel');
import TemplateDocumentRepositoryStatus = require('App/Models/Templates/TemplateDocumentRepositoryStatus');
import IMapper = require('App/Mappers/IMapper');

class TemplateDocumentRepositoryModelMapper implements IMapper<TemplateDocumentRepositoryModel>{
    constructor() {
    }

    public Map(source: any): TemplateDocumentRepositoryModel {
        let toMap: TemplateDocumentRepositoryModel = <TemplateDocumentRepositoryModel>{};
        toMap.UniqueId = source.UniqueId;
        toMap.Status = this.mapStatus(source.Status);        
        toMap.Name = source.Name;
        toMap.QualityTag = source.QualityTag;
        toMap.Version = source.Version;
        toMap.Object = source.Object;
        toMap.IdArchiveChain = source.IdArchiveChain;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.LastChangedUser = source.LastChangedUser;
        toMap.LastChangedDate = source.LastChangedDate;

        return toMap;
    }

    private mapStatus(status: any): TemplateDocumentRepositoryStatus {
        if (typeof (status) == "string") {
            return TemplateDocumentRepositoryStatus[status.toString()];
        }
        return <TemplateDocumentRepositoryStatus>status;
    }
}

export = TemplateDocumentRepositoryModelMapper;