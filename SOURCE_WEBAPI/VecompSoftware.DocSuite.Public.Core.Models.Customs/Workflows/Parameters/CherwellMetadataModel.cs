namespace VecompSoftware.DocSuite.Public.Core.Models.Customs.Workflows.Parameters
{
    public sealed class CherwellMetadataModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello del metadato dinamico per l'integrazione Cherwell
        /// </summary>
        public CherwellMetadataModel()
        {
        }

        #endregion

        #region [ Properties ]

        public bool dirty { get; set; }
        public string displayName { get; set; }
        public string fieldId { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        #endregion
    }
}