using System.ComponentModel;

enum TranslateQuestions
{
    [Description("Lavora o collabora con una struttura sanitaria?")]
    lavoraConSanitaria,
    [Description("Consulta prevalentemente Amministrazione trasparente dell’AUSL di Reggio Emilia per:")]
    consultaAmministrazionePer,
    [Description("Come è venuto a conoscenza di questa sezione del sito internet dell’AUSL di Reggio Emilia?")]
    conoscenzaDiQuestaDelSito,
    [Description("Con quale frequenza consulta questa sezione?")]
    frequenzaConsulta,
    [Description("Per quale motivo ha consultato questa sezione?")]
    motivoHaConsultato,
    [Description("La consultazione di questa sezione ha favorito l’accesso alle informazioni che cercava?")]
    favoritoAccessoAlleInformazioni,
    [Description("Chiara nel linguaggio")]
    chiaraNelLinguaggio,
    [Description("Chiara nei contenuti")]
    chiaraNeiContenuti,
    [Description("Semplice nella ricerca delle informazioni")]
    sempliceNellaRicerca,
    [Description("Facile per reperire moduli e documenti")]
    facilePerReperire,
    [Description("Completa nelle informazioni")]
    completaNelleInformazioni,
    [Description("Veloce nello scaricare allegati")]
    veloceNelloScaricare,
    [Description("Chiara nella grafica")]
    chiaraNellaGrafica,
    [Description("Come giudica nel complesso la qualità di questa sezione?")]
    qualitàDiQuestaSezione,
    [Description("Suggerimenti per migliorare la sezione Amministrazione trasparente del sito dell’AUSL di Reggio Emilia")]
    suggerimentiPerMigliorare,
    [Description("Genere")]
    genere,
    [Description("Fascia di età")]
    fasciaDiEtà,
    [Description("Attuale occupazione")]
    attualeOccupazione,
    [Description("Il suo titolo di studio")]
    titoloDiStudio,
    [Description("Dove risiede")]
    doveRisiede
}