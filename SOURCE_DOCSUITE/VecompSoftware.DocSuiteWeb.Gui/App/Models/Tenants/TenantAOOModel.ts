import TenantViewModel = require("ViewModels/Tenants/TenantViewModel");
import TenantTypologyTypeEnum = require("TenantTypologyTypeEnum");

interface TenantAOOModel {
    UniqueId: string;
    Name: string;
    Note: string;
    CategorySuffix: string;
    Tenants: TenantViewModel[];
    TenantTypology: TenantTypologyTypeEnum;
}

export = TenantAOOModel;