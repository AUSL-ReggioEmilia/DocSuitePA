
interface RoleModel {
    UniqueId: string;
    EntityShortId: number;
    Name: string;
    IdRoleTenant: number;
    TenantId: string;
    IdRole: number;
    FullIncrementalPath?: string;
    IsActive?: number;
    IdRoleFather?: number;
    ServiceCode: string;
    ActiveFrom?: string;
    Children?: RoleModel[];
}

export = RoleModel;