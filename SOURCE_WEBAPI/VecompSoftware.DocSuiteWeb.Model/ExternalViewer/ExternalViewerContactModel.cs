namespace VecompSoftware.DocSuiteWeb.Model.ExternalViewer
{
    public class ExternalViewerContactModel
    {
        #region [Constructor]

        public ExternalViewerContactModel()
        {

        }

        #endregion

        #region [Properties]

        /// <summary>
        /// Nome del contatto
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indirizzo email del contatto
        /// </summary>
        public string Email { get; set; }


        /// <summary>
        /// SamAccount del contatto
        /// </summary>
        public string Account { get; set; }

        #endregion
    }
}
