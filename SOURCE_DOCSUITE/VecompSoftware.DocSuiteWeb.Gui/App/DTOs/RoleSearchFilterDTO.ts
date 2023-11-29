interface RoleSearchFilterDTO {
    Name: string,
    UniqueId? : string,
    ParentId?: number,
    ServiceCode: string,
    IdTenantAOO: string,
    Environment?: number,
    LoadOnlyRoot?: boolean,
    LoadOnlyMy?: boolean,
    LoadAlsoParent?: boolean,
    RoleTypology: string;
    IdCategory?: number;
    IdDossierFolder?: string;
}

export = RoleSearchFilterDTO;