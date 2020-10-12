
interface ContactSearchFilterDTO {
    Filter: string;
    ApplyAuthorizations?: boolean;
    ExcludeRoleContacts?: boolean;
    ParentId?: number;
    ParentToExclude?: number;
    IdTenant?: string;
}

export = ContactSearchFilterDTO;