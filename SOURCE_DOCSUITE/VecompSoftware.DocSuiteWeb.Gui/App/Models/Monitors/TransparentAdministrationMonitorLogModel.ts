import DocumentUnitModel = require('App/Models/DocumentUnits/DocumentUnitModel');
import RoleModel = require('App/Models/Commons/RoleModel');

interface TransparentAdministrationMonitorLogModel {
    UniqueId: string;
    DocumentUnitName: string;
    Date: Date;
    Note: string;
    Rating: string;
    RegistrationUser: string;
    RegistrationDate: Date;
    LastChangedUser?: string;
    LastChangedDate?: Date;
    DocumentUnit: DocumentUnitModel;
    Role: RoleModel;
}

export = TransparentAdministrationMonitorLogModel;