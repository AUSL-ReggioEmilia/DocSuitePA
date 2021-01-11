interface DocumentUnitSearchFilterDTO {
    Skip?: number;
    Top?: number;
    IdFascicle: string;
    Year?: number;
    Number: string;
    DocumentUnitName: string;
    IdCategory?: number;
    IdContainer?: number;
    Subject: string;
    IncludeChildClassification?: boolean;
    IdTenantAOO: string;
    DateFrom?: Date;
    DateTo?: Date;
    IncludeThreshold?: boolean;
    Threshold: string;
}

export = DocumentUnitSearchFilterDTO;