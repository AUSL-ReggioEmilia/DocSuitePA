interface SetiContactModel {
    AnasId: number;
    Cognome: string;
    Nome: string;
    DataNascita: string;
    Sesso: string;
    CodiceFiscale: string;
    ResIndirizzo: string;
    ResComune: string;
    ResCap: string;
    ResSiglaProvincia: string;
    DomIndirizzo: string;
    DomComune: string;
    DomCap: string;
    DomSiglaProvincia: string;
    DataDecesso?: string;
    NasComune: string;
    NasCap: string;
    NasSiglaProvincia: string;
    NazioneCodice: string;
    Nazione: string;
    Telefono1: string;
    Telefono2: string;
    AuslAssistenza: number;
    AuslResidenza: string;
    TesseraSanitaria: string;
    TipoAssCodice: string;
    TipoAssistito: string;
}

export = SetiContactModel;