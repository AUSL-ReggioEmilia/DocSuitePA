using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Stato di un documento
    /// </summary>
    [DataContract]
    public class StatoDocumento
    {
        [DataMember]
        public List<MetadatoItem> ProprietaDocumento { get; set; }

        [DataMember]
        public StatoConservazione CodiceStato { get; set; }

        public string DescrizioneStato
        {
            get
            {
                switch (CodiceStato)
                {
                    case StatoConservazione.STATODOC_ARCHIVIATO:
                        return "Il documento è archiviato";                        
                    case StatoConservazione.STATODOC_NONARCHIVIATO:
                        return "Il documento non è archiviato";
                    case StatoConservazione.STATODOC_INCORSO:
                        return "Il documento è in archiviazione";
                    case StatoConservazione.STATODOC_SCONOSCIUTO:                        
                    case StatoConservazione.STATODOC_NONVERIFICATO:                        
                    default:
                        return "ND";
                }
            }
        }
    }
}