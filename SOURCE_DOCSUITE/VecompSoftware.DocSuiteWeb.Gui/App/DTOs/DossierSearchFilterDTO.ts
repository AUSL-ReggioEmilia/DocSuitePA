import MetadataFinderViewModel = require("App/ViewModels/Metadata/MetadataFinderViewModel");

class DossierSearchFilterDTO {
    Year?: number;
    Number?: number;
    Subject: string;
    IdContainer?: number;
    StartDateFrom: string;
    StartDateTo: string;
    EndDateFrom: string;
    EndDateTo: string;
    Note: string;
    IdMetadataRepository: string;
    MetadataValue: string;
    Skip: number;
    Top: number;
    MetadataValues: MetadataFinderViewModel[];
    IdCategory: number;
    DossierType: string;
    Status: string;
}

export = DossierSearchFilterDTO;