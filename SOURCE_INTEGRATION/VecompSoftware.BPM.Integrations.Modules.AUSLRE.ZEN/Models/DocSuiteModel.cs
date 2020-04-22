using System;
using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.ZEN.Models
{
    public class DocSuiteModel
    {
        public const string PROTOCOL_TITLE_FORMAT = "Protocollo {0:0000}/{1:0000000}";
        public const string COLLABORATION_TITLE_FORMAT = "Collaborazione n° {0}";
        public const string PEC_TITLE_FORMAT = "PEC {0}";

        /// <summary>
        /// Rappresentazione testuale del modello
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Anno di creazione dell'entità passata
        /// </summary>
        public short? Year { get; set; }
        /// <summary>
        /// Numero di creazione dell'entità passata
        /// </summary>
        public int? Number { get; set; }
        /// <summary>
        /// Identificativo univoco dell'entità passasta
        /// </summary>
        public Guid UniqueId { get; set; }
        /// <summary>
        /// Identificativo univoco dell'entità passasta in forma numerica (dove prevista)
        /// </summary>
        public int? EntityId { get; set; }
        /// <summary>
        /// Lista che conterrà proprietà specifiche dell’integrazione col fornitore
        /// </summary>
        public IDictionary<string, string> CustomProperties { get; set; }
        /// <summary>
        /// Tipologia che determina a quale entità DocSuite si riferisce l'istanza del modello
        /// </summary>
        public DocSuiteType ModelType { get; set; }
        /// <summary>
        /// Stato dell'entità DocSuite
        /// </summary>
        public DocSuiteStatus ModelStatus { get; set; }
    }
}
