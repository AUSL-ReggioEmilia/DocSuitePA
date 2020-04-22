using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Modello generico dell'unità documentaria
    /// </summary>
    public sealed class DocumentUnitModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello generico dell'unità documentaria
        /// </summary>
        /// <param name="uniqueId">Chiave univoca dell'unità documentaria</param>
        /// <param name="year">Anno dell'unità documentaria</param>
        /// <param name="number">Numero dell'unità documentaria</param>
        /// <param name="title">Rappresentazione standard in DocSuite dell'unità documentaria. Può essere utile per pulsanti di visualizzazione</param>
        /// <param name="subject">Oggetto dell'unità documentaria</param>
        /// <param name="category">Classificatore o Titolario dell'unità documentaria</param>
        /// <param name="direction">Specifica se l'unità documentaria è in ingresso o in uscita <see cref="DocumentUnitDirection"/></param>
        /// <param name="documentUnitType">Tipologia dell'unità documentaria <see cref="DocumentUnitType"/></param>
        /// <param name="documentUnitName">Nome distingubile dell'unità documentaria </param>
        public DocumentUnitModel(Guid uniqueId, short year, long number, string title, string subject,
            CategoryModel category, DocumentUnitDirection direction, DocumentUnitType documentUnitType,
            string documentUnitName)
        {
            UniqueId = uniqueId;
            Year = year;
            Number = number;
            Title = title;
            Subject = subject;
            Category = category;
            Direction = direction;
            DocumentUnitName = documentUnitName;
            DocumentUnitType = documentUnitType;
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Chiave univoca dell'unità documentaria
        /// </summary>
        public Guid UniqueId { get; private set; }

        /// <summary>
        /// Anno dell'unità documentaria
        /// </summary>
        public short Year { get; private set; }

        /// <summary>
        /// Numero dell'unità documentaria
        /// </summary>
        public long Number { get; private set; }

        /// <summary>
        /// Nome distingubile dell'unità documentaria
        /// </summary>
        public string DocumentUnitName { get; private set; }

        /// <summary>
        /// Rappresentazione standard in DocSuite dell'unità documentaria. Può essere utile per pulsanti di visualizzazione
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Oggetto dell'unità documentaria
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// Classificatore o Titolario dell'unità documentaria
        /// </summary>
        public CategoryModel Category { get; private set; }

        /// <summary>
        /// Specifica se l'unità documentaria è in ingresso o in uscita <see cref="DocumentUnitDirection"/>
        /// </summary>
        public DocumentUnitDirection Direction { get; private set; }

        /// <summary>
        /// Tipologia dell'unità documentaria <see cref="DocumentUnitType"/>
        /// </summary>
        public DocumentUnitType DocumentUnitType { get; private set; }

        #endregion
    }

}
