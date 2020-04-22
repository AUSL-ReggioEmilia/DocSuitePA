using Telerik.Web.UI;

namespace VecompSoftware.DocSuiteWeb.Report
{
    public class Column
    {
        #region "Properties"

        public string DisplayName { get; set; }
        public string OriginalName { get; set; }

        #endregion

        #region "Constructor"

        /// <summary>
        /// Costruttore ad 1 parametro
        /// </summary>
        public Column(string displayName)
        {
            DisplayName = displayName;
            OriginalName = displayName;
        }

        /// <summary>
        /// Costruttore completo
        /// </summary>
        public Column(string displayName, string originalName)
        {
            DisplayName = displayName;
            OriginalName = originalName;
        }

        /// <summary>
        /// Costruttore specifico a partire da una colonna di Griglia
        /// </summary>
        /// <param name="gridColumn"></param>
        public Column(GridColumn gridColumn)
        {
            DisplayName = gridColumn.HeaderText;
            OriginalName = gridColumn.UniqueName.Replace(".", "_");
        }

        #endregion
    }
}
