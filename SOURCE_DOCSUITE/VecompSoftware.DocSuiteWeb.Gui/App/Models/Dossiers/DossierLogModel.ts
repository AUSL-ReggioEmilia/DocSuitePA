import DossierLogType = require("App/Models/Dossiers/DossierLogType");

interface DossierLogModel {
    UniqueId: string;
    SystemComputer: string;
    LogType: DossierLogType;
    LogDescription: string;
    Note: string;
    RegistrationDate: Date;
    RegistrationUser: string;
}

export = DossierLogModel;