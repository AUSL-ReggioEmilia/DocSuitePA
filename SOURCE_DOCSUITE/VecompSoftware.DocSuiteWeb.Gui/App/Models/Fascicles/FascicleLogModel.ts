import FascicleLogType = require("App/Models/Fascicles/FascicleLogType");

interface FascicleLogModel {
    UniqueId: string;
    SystemComputer: string;
    LogType: FascicleLogType;
    LogDescription: string;
    Note: string;
    RegistrationDate: Date;
    RegistrationUser: string;
}

export = FascicleLogModel ;