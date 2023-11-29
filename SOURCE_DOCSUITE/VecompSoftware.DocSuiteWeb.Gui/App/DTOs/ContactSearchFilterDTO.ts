
interface ContactSearchFilterDTO {
    Filter: string;
    ApplyAuthorizations?: boolean;
    ExcludeRoleContacts?: boolean;
    ParentId?: number;
    ParentToExclude?: number;
    IdRole: number;
}

export = ContactSearchFilterDTO;