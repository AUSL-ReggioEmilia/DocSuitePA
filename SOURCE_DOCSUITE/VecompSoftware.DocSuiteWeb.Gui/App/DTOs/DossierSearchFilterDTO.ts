class DossierSearchFilterDTO {
    year?: number;
    number?: number;
    subject: string;
    idContainer?: number;
    startDateFrom: string;
    startDateTo: string;
    endDateFrom: string;
    endDateTo: string;
    note: string;
    idMetadataRepository: string;
    metadataValue: string;
}

export = DossierSearchFilterDTO;