using System.Runtime.Serialization;
namespace BiblosDS.API
{
    /// <summary>
    /// Enumerato dei possibili esiti di un'operazione.
    /// </summary>
    [DataContract]
    public enum Operazione
    {
        [EnumMember]
        Default = 0,
        [EnumMember]
        Insert = 1,
        [EnumMember]
        Update = 2,
        [EnumMember]
        Delete = 3,
    }

    /// <summary>
    /// Enumerato stato della conservazione
    /// </summary>
    [DataContract]
    public enum StatoConservazione
    {
        [EnumMember]
        STATODOC_ARCHIVIATO = 1,
        [EnumMember]
        STATODOC_NONARCHIVIATO = 0,
        [EnumMember]
        STATODOC_INCORSO = 2,
        [EnumMember]
        STATODOC_SCONOSCIUTO = 3,
        [EnumMember]
        STATODOC_NONVERIFICATO = 4
    }

    /// <summary>
    /// Tipologie di errore/esito.
    /// </summary>
    [DataContract]
    public enum CodiceErrore
    {
        [EnumMember]
        NessunErrore = 0,
        /// <summary>
        /// La configurazione del servizio invocato non contiene la definizione della classe di documento
        /// </summary>
        [EnumMember]
        ArchivioNonDefinito = 1,
        /// <summary>
        /// Eccezione non prevista.
        /// </summary>
        [EnumMember]
        ErroreGenerico = 2,
        /// <summary>
        /// Il nome attributo passato nei metadati non risulta definito in BiblosDS
        /// </summary>
        [EnumMember]
        AttributoNonTrovato = 3,
        /// <summary>
        /// L'IdDocumento del documento fornito non risulta essere valido
        /// </summary>
        [EnumMember]
        IdDocumentoNonValido = 4,
        /// <summary>
        /// Errore durante l''accesso al sistema documentale
        /// </summary>
        [EnumMember]
        IdClientErrato = 5,
        /// <summary>
        /// Il cliente non risulta valido
        /// </summary>
        [EnumMember]
        IdClienteErrato = 6,
        [EnumMember]
        IdRichiestaErrata = 7,
        [EnumMember]
        ChiaveDocumentoNonDefinita = 8,
        /// <summary>
        /// Nessun documento è stato definito per questa chiave
        /// </summary>
        [EnumMember]
        DocumentoNonDefinito = 9,
        /// <summary>
        /// Il documento richiesto risulta essere archiviato
        /// </summary>
        [EnumMember]
        IdDocumentoNonTrovato = 10,
        /// <summary>
        /// Utente/password non corretti o istanza inesistente
        /// </summary>
        [EnumMember]
        LoginNonValido = 11,
        /// <summary>
        /// Token inesistente o scaduto
        /// </summary>
        [EnumMember]
        TokenNonValidoOScaduto = 12,
        [EnumMember]
        ErroreChiamataAlServizioRemoto = 13,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        ErroreGenericoBiblosDS = 14,
        /// <summary>
        /// NO: Utente senza diritti per l'archivio utente 
        /// MEGLIO: Il servizio invocato non si riferisce al cliente indicato
        /// </summary>
        [EnumMember]
        UtenteNonAbilitatoPerIlCliente = 15,
        /// <summary>
        /// Attributo non valido, non modificabile o tipo non conforme
        /// </summary>
        [EnumMember]
        AttributoNonValido = 16,
        /// <summary>
        /// Attributo richiesto non specificato
        /// </summary>
        [EnumMember]
        AttributeRequired = 17,
        /// <summary>
        /// La chiave inserita non è univoca
        /// </summary>
        [EnumMember]
        PrimaryKeyErrata = 18,
        /// <summary>
        /// L'allegato selezionato non puo' essere caricato
        /// </summary>
        [EnumMember]
        AllegatoNonValido = 19,
        /// <summary>
        /// Il file indicato è inesistente
        /// </summary>
        [EnumMember]
        FileInesistente = 20,
        /// <summary>
        /// Lo stato del documento non è compatibile con il trattamento richiesto
        /// </summary>
        [EnumMember]
        StatoDocumentoNonValido = 21,
        /// <summary>
        /// Attachment non trovato nel response
        /// </summary>
        [EnumMember]
        AllegatoNonPresente = 22,
        /// <summary>
        /// Tentativo di accesso ad un documento di un'altro cliente
        /// </summary>
        [EnumMember]
        DocumentoNonDelCliente = 23,
        /// <summary>
        /// Il documento è stato caricato con il solo profilo. Nessuna immagine documento presente
        /// </summary>
        [EnumMember]
        ProfileOnly = 24,
        /// <summary>
        /// Il legame fra i documenti risulta già definito
        /// </summary>
        [EnumMember]
        LegameDocumentiDefinito = 25,
        /// <summary>
        /// Classe documentale della richiesta per chiave non definita
        /// </summary>
        [EnumMember]
        ClasseDocumentaleNonDefinita = 26,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        ChiaveDocumentoDuplicata = 27,
        [EnumMember]
        IdAllegatoNonValido = 28,
    }
}