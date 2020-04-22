using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.BusinessRule.Relations
{
    /// <summary>
    /// Verifica la relazione tra 2 serie documentali
    /// </summary>
    public class DocumentSeriesItemRelations
    {
        #region [Fields]
        private DocumentSeriesItem _firstItem;
        private DocumentSeriesItem _secondItem;
        #endregion

        #region [Constructor]
        public DocumentSeriesItemRelations(DocumentSeriesItem FirstDocumentSeriesItem, DocumentSeriesItem SecondDocumentSeriesItem)
        {
            _firstItem = FirstDocumentSeriesItem;
            _secondItem = SecondDocumentSeriesItem;
        }  
        #endregion

        #region [Properties]
        #endregion

        #region [Methods]
        /// <summary>
        /// Funzione da implementare durante la formalizzazione delle logiche di business.
        /// </summary>
        /// <returns>Viene ritornato sempre il valore true</returns>
        public bool CanUpdateMetadata()
        {
            return true;
        }
        #endregion
    }
}
