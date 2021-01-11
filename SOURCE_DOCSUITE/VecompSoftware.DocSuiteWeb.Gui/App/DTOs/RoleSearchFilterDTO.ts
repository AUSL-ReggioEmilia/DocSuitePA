interface RoleSearchFilterDTO {
    Name: string,
    UniqueId? : string,
    ParentId?: number,
    ServiceCode: string,
    TenantId: string,
    Environment?: number,
    LoadOnlyRoot?: boolean,
    LoadOnlyMy?: boolean,
    LoadAlsoParent?:boolean
}

export = RoleSearchFilterDTO;