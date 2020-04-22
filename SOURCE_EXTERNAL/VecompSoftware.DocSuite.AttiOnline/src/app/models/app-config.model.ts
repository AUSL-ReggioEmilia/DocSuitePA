
export interface AppConfig {
    apiOdataAddress: string;
    executiveDocumentHandlerUrl: string;
    publishedDocumentHandlerUrl: string;
    alboPretorioDocumentHandlerUrl: string;
    gridItemsNumber: number;
    toastLife: number;
    alboPretorioProposerColumnVisibility: boolean;
    alboPretorioServiceColumnVisibility: boolean;
    executiveConsultationServiceColumnVisibility: boolean;
    executiveConsultationProposerColumnVisibility: boolean;
    publishedConsultationServiceColumnVisibility: boolean;
    publishedConsultationProposerColumnVisibility: boolean;
    attiGroupableGridEnabled: boolean;
    attiPublicationDateSorting: string;
    deliberePublicationDateSorting: string;
    activeRoutes: ActiveRoutes;
    delibere: string;
    determine: string;
    headerUrl: string;
    footerUrl: string;
    ASMNAlboUrl: string;
}

export interface ActiveRoutes {
    executiveConsultation: boolean;
    publishedConsultation: boolean;
    alboPretorio: boolean;
}