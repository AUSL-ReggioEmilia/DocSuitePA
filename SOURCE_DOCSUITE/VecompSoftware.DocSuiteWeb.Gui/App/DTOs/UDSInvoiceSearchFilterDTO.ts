class UDSInvoiceSearchFilterDTO {
    cmdRepositoriName: string;
    startDateFromFilter: string;
    endDateFromFilter: string;
    numerofatturafilter: string;
    numerofatturafilterEq: boolean;
    importoFilter: string;
    importoFilterEq: boolean;
    pivacfFilter: string = "";
    pivacfFilterEq: boolean;
    denomiazioneFilter: string;
    denomiazioneFilterEq: boolean;
    annoivaFilter: string;
    dataIvaFromFilter: string;
    dataReceivedFromFilter: string;
    dataacceptFromFilter: string;

    dataIvaToFilter: string;
    dataReceivedToFilter: string;
    dataacceptToFilter: string;

    identificativoSdiFilter: string = "";
    identificativoSdiFilterEq: boolean;
    progressIDSDIFilter: string = "";
    progressIDSDIFilterEq: boolean;
    statofatturaFilter: string;

    pecMailBoxFilter: string;
    pecMailBoxFilterEq: boolean;
}

export =UDSInvoiceSearchFilterDTO;