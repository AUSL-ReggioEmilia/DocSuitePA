using System;
using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Services.BiblosDS
{
    public class ArchiveDocument
    {
        #region [Constructor]

        public ArchiveDocument()
        {
            Metadata = new Dictionary<string, object>();
        }

        #endregion

        #region [Properties]
        /// <summary>
        /// Chiave Raggruppatore del documento Legacy
        /// </summary>
        public int IdLegacyChain { get; set; }

        /// <summary>
        /// Chiave Raggruppatore del documento
        /// </summary>
        public Guid? IdChain { get; set; }

        /// <summary>
        /// Chiave del documento
        /// </summary>
        public Guid IdDocument { get; set; }

        /// <summary>
        /// Nome del documento
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nome archivio
        /// </summary>
        public string Archive { get; set; }

        /// <summary>
        /// Dimensione del documento
        /// </summary>
        public long? Size { get; set; }

        /// <summary>
        /// Versione del documento
        /// </summary>
        public decimal Version { get; set; }

        /// <summary>
        /// Stream del documento
        /// </summary>
        public byte[] ContentStream { get; set; }

        /// <summary>
        /// Attributi aggiuntivi alla catena
        /// </summary>
        public IDictionary<string, object> Metadata { get; set; }

        #endregion
    }
}
