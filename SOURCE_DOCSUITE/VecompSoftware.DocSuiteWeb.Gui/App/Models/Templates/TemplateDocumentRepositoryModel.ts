import TemplateDocumentRepositoryStatus = require('App/Models/Templates/TemplateDocumentRepositoryStatus');
import LocationModel = require('App/Models/Commons/LocationModel');

interface TemplateDocumentRepository {
    UniqueId: string;
    Status: TemplateDocumentRepositoryStatus;    
    Name: string;
    QualityTag: string;
    Version: number;
    Object: string;
    IdArchiveChain: string;
    RegistrationUser: string;
    RegistrationDate: Date;
    LastChangedUser: string;
    LastChangedDate: Date;
}

export = TemplateDocumentRepository;