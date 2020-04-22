using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Desks
{

    public class DeskStoryBoard : DSWBaseEntity
    {
        #region [ Constructor ]

        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base. 
        /// L'entità mappa la funzione di "LAVAGNA".
        /// </summary>
        public DeskStoryBoard() : this(Guid.NewGuid()) { }

        public DeskStoryBoard(Guid uniqueId)
            : base(uniqueId)
        { }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Commento lasciato sulla lavagna
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Data e ora d'inserimento del commento
        /// </summary>
        public DateTime? DateBoard { get; set; }
        /// <summary>
        /// Nome completo dell'autore del commento
        /// </summary>
        public string Author { get; set; }

        #endregion

        #region [ Navigation Properties ]

        /// <summary>
        /// Relazione tra la "Lavagna" e "Tavoli"
        /// </summary>
        public virtual Desk Desk { get; set; }
        /// <summary>
        /// Tipologia di commento lasciato in lavagna
        /// </summary>
        public DeskStoryBoardType BoardType { get; set; }
        /// <summary>
        /// Relazione tra un commento in lavagna e l'utente che l'ha scritto.
        /// </summary>
        public virtual DeskRoleUser DeskRoleUser { get; set; }
        /// <summary>
        /// Relazione tra un commento in lavagna e una specifica versione di un documento.
        /// </summary>
        public virtual DeskDocumentVersion DeskDocumentVersion { get; set; }
        #endregion
    }
}
