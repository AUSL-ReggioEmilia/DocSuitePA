namespace VecompSoftware.DocSuiteWeb.DTO.Commons
{
    public class DomainUser
    {
        #region [ Constructor ]
        public DomainUser() { }
        #endregion

        #region [ Properties ]
        public string Account { get; set; }

        public string Name { get; set; }

        public string Domain { get; set; }

        public bool MainRole { get; set; }

        public string DisplayName { get { return string.Format("{0}\\{1}", Domain, Account); } }
        #endregion
    }
}
