namespace VecompSoftware.DocSuiteWeb.Repository
{
    public static class CommonDefinition
    {
        #region [ Dossiers ]
        #region [ Params ]
        public const string SQL_Param_DossierFolder_IdParent = "@IdParent";
        public const string SQL_Param_DossierFolder_IdDossier = "@IdDossier";
        public const string SQL_Param_DossierFolder_AuthorizationType = "@AuthorizationType";
        public const string SQL_Param_DossierFolder_AuthorizationTypeDescription = "@AuthorizationTypeDescription";
        public const string SQL_Param_DossierFolder_RegistrationUser = "@RegistrationUser";
        public const string SQL_Param_DossierFolder_System = "@System";
        #endregion

        #region [ Store Name ]
        public const string SQL_SP_DossierFolder_PropagateAtuthorizationToDescendants = "[dbo].DossierFolder_SP_PropagateAuthorizationToDescendants";
        #endregion
        #endregion
    }
}
