namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class RoleUserModel
    {
        #region [ Constructors ]

        public RoleUserModel()
        {
        }

        #endregion

        #region [ Properties ]

        public string Type { get; set; }
        public string Description { get; set; }
        public string Account { get; set; }
        public bool? Enabled { get; set; }
        public string Email { get; set; }
        public bool? IsMainRole { get; set; }
        public DSWEnvironmentType DSWEnvironment { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public short IdRole { get; set; }

        #endregion
    }
}
