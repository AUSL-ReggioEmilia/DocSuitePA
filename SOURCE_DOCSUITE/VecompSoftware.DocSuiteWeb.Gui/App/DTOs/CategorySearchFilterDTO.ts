import FascicleType = require("App/Models/Fascicles/FascicleType");

interface CategorySearchFilterDTO {
    Name: string;
    FascicleType: string;
    HasFascicleInsertRights?: boolean;
    Manager: string;
    Secretary: string;
    IdRole?: number;
    IdContainer?: number;
    LoadRoot?: boolean;
    ParentId?: number;
    FullCode: string;
    FascicleFilterEnabled?: boolean;
    ParentAllDescendants?: boolean;
    IdTenantAOO: string;
}

export = CategorySearchFilterDTO;