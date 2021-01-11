namespace VecompSoftware.DocSuiteWeb.API
{
    internal class APIProviderDTO : IAPIProviderDTO
    {

        #region [ Properties ]

        public bool? Enabled { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public bool? Main { get; set; }

        #endregion

    }
}
