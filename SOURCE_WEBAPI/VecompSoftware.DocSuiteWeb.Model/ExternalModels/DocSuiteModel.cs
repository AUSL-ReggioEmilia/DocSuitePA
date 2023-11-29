using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.ExternalModels
{
    public class DocSuiteModel
    {
        #region [ Fields ]
        public const string PROTOCOL_TITLE_FORMAT = "Protocollo {0:0000}/{1:0000000}";
        public const string COLLABORATION_TITLE_FORMAT = "Collaborazione n° {0}";
        public const string PEC_TITLE_FORMAT = "PEC {0}";
        #endregion
        #region [ Constructor ]

        public DocSuiteModel()
        {
            CustomProperties = new Dictionary<string, string>();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Rappresentazione testuale del modello
        /// es: Protocollo 004589 del 2017
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Anno di creazione dell'entità passata
        /// es: 2017
        /// NB: non tutti gli eventi contengono questo valore, ad esempio la PEC non ha anno
        /// </summary>
        public short? Year { get; set; }
        /// <summary>
        /// Numero di creazione dell'entità passata (dove prevista)
        /// es: 004589
        /// NB: non tutti gli eventi contengono questo valore, ad esempio la PEC non ha un numero
        /// </summary>
        public int? Number { get; set; }
        /// <summary>
        /// Identificativo univoco dell'entità passasta (dove prevista)
        /// es: C892E9E6-0C67-43AE-90C4-0C36BE71E947
        /// </summary>
        public Guid UniqueId { get; set; }
        /// <summary>
        /// Identificativo univoco dell'entità passasta in forma numerica (dove prevista)
        /// es: 004589
        /// </summary>
        public int? EntityId { get; set; }
        /// <summary>
        /// Lista che conterrà proprietà specifiche/custom dell’integrazione che il fornitore potrà richiedere.
        /// Ogni richiesta dovrà essere valuta da Dgroove e se possibile verrà resa disponibile.
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

        #endregion

    }
}
