var DigitalSign = {};

//#region Enum

DigitalSign.enumVerInfoItem = {
    EnumVerInfoVersion: 0,              // specifica l’intera stringa
    EnumVerInfoVersionMajorNumber: 1,   // specifica il primo numero della versione
    EnumVerInfoVersionMinorNumber: 2,   // specifica il secondo numero della versione
    EnumVerInfoVersionBuildType: 3,     // specifica il terzo numero della versione
    EnumVerInfoVersionBuildNumber: 4,   // specifica il quarto numero della versione
    EnumVerInfoEdition: 5              // specifica l’edizione di DigitalSign.
};

DigitalSign.enumP7xCreator = {
    p7xCreatorDefault: 0,       // nelle versioni 3.0 equivale a p7xCreatorDigitalSign, dalla 3.1 equivale a p7xCreatorTSD
    p7xCreatorDigitalSign: 1,   // formato MIME nativo DigitalSign, .p7x.
    p7xCreatorDike: 2,          // formato MIME nativo Dike ™, .m7m.
    p7xCreatorTSD: 3            // formato RFC 5544, .tsd
};

DigitalSign.enumKeyType = {
    KTC_UNUSED: 0,          // Chiave non inizializzata.
    KTC_SIGN: 1,            // Chiave di sottoscrizione.
    KTC_SIGN_AUTO: 2,       // Chiave di sottoscrizione mediante procedura automatica.
    KTC_SIGN_CERT: 4,       // Chiave di certificazione.
    KTC_SIGN_SELF_CERT: 8,  // Riservato.
    KTC_ENCRYPTION: 16      // Chiave di cifratura documenti.  
};

DigitalSign.enumHashType = {
    HTC_SHA1: 1,    // Seleziona algoritmo SHA1.
    HTC_RIPEMD: 2,  // Seleziona algoritmo RIPEMD160.
    HTC_MD2: 4,     // Seleziona algoritmo MD-2.
    HTC_MD5: 8,     // Seleziona algoritmo MD-5.
    HTC_SHA256: 16  // Seleziona algoritmo SHA256. Obbligatorio
};

DigitalSign.enumASN1Type = {
    asn1_boolean: 1,            // Boolean.
    asn1_integer: 2,            // Integer.
    asn1_neg_integer: 0x102,    // Negative integer.
    asn1_bit_string: 3,         // Array of zero or more bits.
    asn1_octet_string: 4,       // Array of zero or more bytes.
    asn1_null: 5,               // Single “null” value.
    asn1_real: 9,               // Real number.
    asn1_utf8string: 12,        // Alphabetical string in UTF8 format.
    asn1_numericstring: 18,     // Alphabetical string of digits.
    asn1_printablestring: 19,   // Printable string*.
    asn1_t61string: 20,         // TeletexString*.
    asn1_ia5string: 22,         // IA5String*.
    asn1_utctime: 23            // Coordinated
};

DigitalSign.enumContentType = {
    ContentTypeHex: 1,          // Esadecimale.
    ContentTypeTxt: 2,          // Testo semplice.
    ContentTypeRtf: 3,          // Rich text.
    ContentTypeOle: 10,         // ActiveDocument.
    ContentTypeImage: 20,       // Immagine raster.
    ContentTypeHtml: 21,        // HTML.
    ContentTypePdf: 22,         // PDF.
    ContentTypeExternal: 23,    // Visualizzato con viewer esterno.
    ContentTypeUndisplayed: 24, // Impossibile da decriptare.
    ContentTypePkcs7: 25,       // PKCS # 7 interno.
    ContentTypeXml: 26,         // XML.
    ContentTypeTS: 27,          // Marca Temporale.
    ContentTypeCertStore: 28,   // CertStore.
    ContentTypeAuto: 0xff       // Identificazione automatica.
};

DigitalSign.enumToolBarButtons = {
    Btn_dsv_save_content: 32852,    // Salva Contenuto.
    Btn_dsv_save_p7k: 32856,        // Salva PKCS#7.
    Btn_dsv_print: 32855,           // Stampa.
    Btn_dsv_add_sign: 32853,        // Aggiungi firma.
    Btn_dsv_add_weak_sign: 57682,   // Aggiungi Attestazione.
    Btn_dsv_add_recipient: 32854,   // Aggiungi destinatario.
    Btn_get_time_stamp2: 33082,     // Aggiungi marca temporale.
    Btn_dsv_undo: 32848,            // Annulla tutto.
    Btn_dsv_wipe: 33148,            // Wipe file.
    Btn_dsv_add_pdfsign: 33152,     // Aggiungi firma PAdES
    Btn_dsv_atadate: 33150,         // Verifica alla data
    Btn_change_view_type: 32860     // Cambia modalità visualizzazione.
};

//#endregion