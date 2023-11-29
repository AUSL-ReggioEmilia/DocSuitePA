using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Documents
{
    public class Document
    {
        #region [Constructor]

        public Document()
        {
            AttributeValues = new List<AttributeValue>();
        }

        #endregion

        #region [Properties]

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
        /// Nome dell'archivio
        /// </summary>
        public string ArchiveName { get; set; }

        /// <summary>
        /// Dimensione del documento
        /// </summary>
        public long? Size { get; set; }

        /// <summary>
        /// Hash del documento
        /// </summary>
        public string DocumentHash { get; set; }

        /// <summary>
        /// Versione del documento
        /// </summary>
        public decimal Version { get; set; }

        /// <summary>
        /// Data di creazione del documento
        /// </summary>
        public DateTime? CreatedDate { get; set; }
        
        /// <summary>
        /// Attributi 
        /// </summary>
        public ICollection<AttributeValue> AttributeValues { get; set; }
        #endregion
    }
}
