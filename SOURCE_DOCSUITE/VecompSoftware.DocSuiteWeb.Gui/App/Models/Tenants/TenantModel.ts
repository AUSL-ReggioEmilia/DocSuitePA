import TenantTypologyTypeEnum = require("TenantTypologyTypeEnum");

interface TenantModel {
    UniqueId: string;
    TenantName: string;
    CompanyName: string;
    StartDate: Date;
    EndDate: Date;
    Note: string;
    TenantTypology: TenantTypologyTypeEnum;
}

export = TenantModel;