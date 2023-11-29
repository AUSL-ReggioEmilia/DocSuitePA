import CategoryFascicleRightModel = require("App/Models/Commons/CategoryFascicleRightModel");

interface RoleModel {
    UniqueId: string;
    EntityShortId: number;
    Name: string;
    IdTenantAOO: string;
    IdRole: number;
    FullIncrementalPath?: string;
    IsActive?: boolean;
    IdRoleFather?: number;
    ServiceCode: string;
    Children?: RoleModel[];
    CategoryFascicleRights: CategoryFascicleRightModel[];
    RoleTypology: string;
    IsRealResult: boolean;
}

export = RoleModel;