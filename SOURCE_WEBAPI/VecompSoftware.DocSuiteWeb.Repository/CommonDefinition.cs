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
        public const string SQL_SP_DossierFolder_UpdateHierarchy = "[dbo].[DossierFolder_Clone_Hierarchy]";

        #endregion
        #endregion

        #region [ UDS ]
        #region [ Params ]
        public const string SQL_Param_UDSFieldList_IdUDSField = "@IdUDSField";
        public const string SQL_Param_UDSFieldList_IdUDSFieldList = "@IdUDSFieldList";
        public const string SQL_Param_UDSFieldList_IdUDS = "@UDSId";
        public const string SQL_Param_UDS_SelectedMenuUniqueId = "@SelectedMenuUniqueId";
        public const string SQL_Param_UDS_Subject = "@Subject";
        public const string SQL_Param_UDS_Year = "@Year";
        public const string SQL_Param_UDS_IsIntranet = "@IsIntranet";
        public const string SQL_Param_UDS_OrderColumn = "@OrderColumn";
        public const string SQL_Param_UDS_OrderType = "@OrderType";
        public const string SQL_Param_UDS_IdArchiveChain = "@IdArchiveChain";
        public const string SQL_Param_UDS_Skip = "@Skip";
        public const string SQL_Param_UDS_Top = "@Top";
        #endregion

        #region [ Store Name ]
        public const string SQL_FX_UDSFieldList_GetChildrenByParent = "[webapipublic].[UDSFieldList_FX_GetChildrenByParent]";
        public const string SQL_SP_UDSFieldList_PropagateUDSField_Status = "[dbo].[UDSFieldList_SP_PropagateUDSField_Status]";
        public const string SQL_FX_Get_UDS_T_BandiDiGara_HasDocument = "[webapipublic].[UDS_T_BandiDiGara_HasDocument]";
        public const string SQL_FX_Get_UDS_T_BandiDiGara_MenuValue = "[webapipublic].[Get_UDS_T_BandiDiGara_MenuValue]";
        public const string SQL_FX_Get_UDS_T_BandiDiGara_GetArchiveInfo = "[webapipublic].[Get_UDS_T_BandiDiGara_GetArchiveInfo]";
        public const string SQL_FX_Get_UDS_T_BandiDiGara_FilterByDataScadenzaDESC = "[webapipublic].[Get_UDS_T_BandiDiGara_FilterByDataScadenzaDESC]";
        public const string SQL_FX_Get_UDS_T_BandiDiGara_FilterByDataScadenzaASC = "[webapipublic].[Get_UDS_T_BandiDiGara_FilterByDataScadenzaASC]";
        public const string SQL_FX_Get_UDS_T_BandiDiGara_FilterByDataPubblicazioneDESC = "[webapipublic].[Get_UDS_T_BandiDiGara_FilterByDataPubblicazioneDESC]";
        public const string SQL_FX_Get_UDS_T_BandiDiGara_FilterByDataPubblicazioneASC = "[webapipublic].[Get_UDS_T_BandiDiGara_FilterByDataPubblicazioneASC]";
        public const string SQL_FX_Get_UDS_T_BandiDiGara_CountActiveStatus = "[webapipublic].[UDS_T_BandiDiGara_CountActiveStatus]";

        public const string SQL_FX_Get_UDS_T_Committente_HasDocument = "[webapipublic].[UDS_T_Committente_HasDocument]";
        public const string SQL_FX_Get_UDS_T_Committente_MenuValue = "[webapipublic].[Get_UDS_T_Committente_MenuValue]";
        public const string SQL_FX_Get_UDS_T_Committente_GetArchiveInfo = "[webapipublic].[Get_UDS_T_Committente_GetArchiveInfo]";
        public const string SQL_FX_Get_UDS_T_Committente_FilterByDataScadenzaDESC = "[webapipublic].[Get_UDS_T_Committente_FilterByDataScadenzaDESC]";
        public const string SQL_FX_Get_UDS_T_Committente_FilterByDataScadenzaASC = "[webapipublic].[Get_UDS_T_Committente_FilterByDataScadenzaASC]";
        public const string SQL_FX_Get_UDS_T_Committente_FilterByDataPubblicazioneDESC = "[webapipublic].[Get_UDS_T_Committente_FilterByDataPubblicazioneDESC]";
        public const string SQL_FX_Get_UDS_T_Committente_FilterByDataPubblicazioneASC = "[webapipublic].[Get_UDS_T_Committente_FilterByDataPubblicazioneASC]";
        public const string SQL_FX_Get_UDS_T_Committente_CountActiveStatus = "[webapipublic].[UDS_T_Committente_CountActiveStatus]";

        #endregion
        #endregion
    }
}
