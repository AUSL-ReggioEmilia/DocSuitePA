namespace VecompSoftware.DocSuiteWeb.Finder
{
    internal static class CommonDefinition
    {
        #region [Schema Name]
        internal const string Schema_DBO = "dbo";
        internal const string Schema_CQRS = "cqrs";
        internal const string Schema_WebApiPrivate = "webapiprivate";
        internal const string Schema_WebApiPublic = "webapipublic";
        #endregion

        #region [ Common ]
        internal const string SQL_Param_UserName = "@UserName";
        internal const string SQL_Param_Domain = "@Domain";
        #endregion

        #region[ Fascicles]
        #region [ Function Names ]
        internal const string SQL_FX_Fascicle_AvailableFasciclesFromDocumentUnit = "[webapiprivate].Fascicles_FX_AvailableFasciclesFromDocumentUnit";
        internal const string SQL_FX_Fascicle_PeriodicFasciclesFromDocumentUnit = "[webapiprivate].Fascicles_FX_PeriodicFasciclesFromDocumentUnit";
        internal const string SQL_FX_Fascicle_GetFascicleContactModels = "[webapipublic].Fascicles_FX_FascicleContactModels";
        internal const string SQL_FX_Fascicle_IsCurrentUserManagerOnActivityFascicle = "[webapiprivate].Fascicle_FX_IsCurrentUserManagerOnActivityFascicle";
        internal const string SQL_FX_Fascicle_HasDocumentVisibilityRight = "[webapiprivate].[Fascicle_FX_HasDocumentVisibilityRight]";
        internal const string SQL_FX_Fascicle_AvailableFascicles = "[webapiprivate].[Fascicles_FX_AvailableFascicles]";
        internal const string SQL_FX_Fascicle_IsProcedureSecretary = "[webapiprivate].[Fascicles_FX_IsProcedureSecretary]";
        internal const string SQL_FX_Fascicle_HasInsertRight = "[webapiprivate].[Fascicles_FX_HasInsertRight]";
        internal const string SQL_FX_Fascicle_HasManageableRight = "[webapiprivate].[Fascicles_FX_HasManageableRight]";
        internal const string SQL_FX_Fascicle_HasViewableRight = "[webapiprivate].[Fascicles_FX_HasViewableRight]";
        internal const string SQL_FX_Fascicle_AuthorizedFascicles = "[webapiprivate].[Fascicles_FX_AuthorizedFascicles]";
        internal const string SQL_FX_Fascicle_CountAuthorizedFascicles = "[webapiprivate].Fascicles_FX_CountAuthorizedFascicles";
        internal const string SQL_FX_Fascicle_IsManager = "[webapiprivate].Fascicles_FX_IsManager";
        internal const string SQL_FX_Fascicle_HasProcedureDistributionInsertRight = "[webapiprivate].Fascicles_FX_HasProcedureDistributionInsertRight";
        internal const string SQL_FX_FascicleFolder_AllChildrenByParent = "[webapiprivate].FascicleFolder_FX_AllChildrenByParent";
        internal const string SQL_FX_FascicleFolder_GetParent = "[webapiprivate].FascicleFolder_FX_GetParent";
        internal const string SQL_FX_FascicleFolder_NameAlreadyExists = "[webapiprivate].FascicleFolder_FX_NameAlreadyExists";
        internal const string SQL_FX_FascicleFolder_HasParent = "[webapiprivate].FascicleFolder_FX_HasParent";
        internal const string SQL_FX_FascicleFolder_CountChildren = "[webapiprivate].FascicleFolder_FX_CountChildren";
        internal const string SQL_FX_Fascicle_CountAuthorizedFasciclesFromDocumentUnit = "[webapiprivate].Fascicles_FX_CountAuthorizedFasciclesFromDocumentUnit";
        internal const string SQL_FX_Fascicle_AuthorizedFasciclesFromDocumentUnit = "[webapiprivate].Fascicles_FX_AuthorizedFasciclesFromDocumentUnit";
        internal const string SQL_FX_FascicleFolder_FX_FascicleFoldersWithSameName = "[webapiprivate].FascicleFolder_FX_FascicleFoldersWithSameName";
        internal const string SQL_FX_FascicleFolder_GetChildren = "[webapiprivate].[FascicleFolder_FX_GetChildren]";
        internal const string SQL_FX_Fascicle_FindAuthorizedCategoryFascicles = "[webapiprivate].[Fascicles_FX_FindAuthorizedCategoryFascicles]";
        internal const string SQL_FX_Fascicle_CountAuthorizedCategoryFascicles = "[webapiprivate].[Fascicles_FX_CountAuthorizedCategoryFascicles]";
        internal const string SQL_FX_FascicleDocumentUnit_FX_GetFascicleDocumentUnits = "[webapiprivate].[FascicleDocumentUnit_FX_GetFascicleDocumentUnits]";
        internal const string SQL_FX_Fascicle_FX_FasciclesFromDocumentUnit = "[webapiprivate].[Fascicles_FX_FasciclesFromDocumentUnit]";
        #endregion
        #region [ Params ]
        internal const string SQL_Param_Fascicle_UserName = "@UserName";
        internal const string SQL_Param_Fascicle_Domain = "@Domain";
        internal const string SQL_Param_Fascicle_Skip = "@Skip";
        internal const string SQL_Param_Fascicle_Top = "@Top";
        internal const string SQL_Param_Fascicle_Year = "@Year";
        internal const string SQL_Param_Fascicle_IdFascicle = "@IdFascicle";
        internal const string SQL_Param_Fascicle_Environment = "@Environment";
        internal const string SQL_Param_Fascicle_UniqueIdDocumentUnit = "@UniqueId";
        internal const string SQL_Param_Fascicle_Description = "@Description";
        internal const string SQL_Param_Fascicle_HasProcess = "@HasProcess";
        internal const string SQL_Param_Fascicle_IdCategory = "@CategoryId";
        internal const string SQL_Param_Fascicle_FascicleType = "@FascicleType";
        internal const string SQL_Param_Fascicle_StartDateFrom = "@StartDateFrom";
        internal const string SQL_Param_Fascicle_StartDateTo = "@StartDateTo";
        internal const string SQL_Param_Fascicle_EndDateFrom = "@EndDateFrom";
        internal const string SQL_Param_Fascicle_EndDateTo = "@EndDateTo";
        internal const string SQL_Param_Fascicle_FascicleStatus = "@FascicleStatus";
        internal const string SQL_Param_Fascicle_Manager = "@Manager";
        internal const string SQL_Param_Fascicle_Name = "@Name";
        internal const string SQL_Param_Fascicle_ViewConfidential = "@ViewConfidential";
        internal const string SQL_Param_Fascicle_ViewAccessible = "@ViewAccessible";
        internal const string SQL_Param_Fascicle_Subject = "@Subject";
        internal const string SQL_Param_Fascicle_Note = "@Note";
        internal const string SQL_Param_Fascicle_Rack = "@Rack";
        internal const string SQL_Param_Fascicle_IdMetadataRepository = "@IdMetadataRepository";
        internal const string SQL_Param_Fascicle_MetadataValue = "@MetadataValue";
        internal const string SQL_Param_Fascicle_Classifications = "@Classifications";
        internal const string SQL_Param_Fascicle_IncludeChildClassifications = "@IncludeChildClassifications";
        internal const string SQL_Param_Fascicle_Roles = "@Roles";
        internal const string SQL_Param_Fascicle_MasterRole = "@MasterRole";
        internal const string SQL_Param_Fascicle_ApplySecurity = "@ApplySecurity";
        internal const string SQL_Param_Fascicle_Container = "@Container";
        internal const string SQL_Param_Fascicle_MetadataValues = "@MetadataValues";
        internal const string SQL_Param_FascicleFolder_IdFascicleFolder = "@IdFascicleFolder";
        internal const string SQL_Param_Fascicle_ViewOnlyClosable = "@ViewOnlyClosable";
        internal const string SQL_Param_Fascicle_ThresholdDate = "@ThresholdDate";
        internal const string SQL_Param_Fascicle_Title = "@Title";
        internal const string SQL_Param_Fascicle_IsManager = "@IsManager";
        internal const string SQL_Param_Fascicle_IsSecretary = "@IsSecretary";
        internal const string SQL_Param_Fascicle_IdProcess = "@IdProcess";
        internal const string SQL_Param_Fascicle_IdDossierFolder = "@IdDossierFolder";
        internal const string SQL_Param_Fascicle_ProcessLabel = "@ProcessLabel";
        internal const string SQL_Param_Fascicle_DossierFolderLabel = "@DossierFolderLabel";
        internal const string SQL_Param_FascicleFolder_ReferenceFascicleId = "@ReferenceFascicleId";
        internal const string SQL_Param_FascicleFolder_DestinationFascicleId = "@DestinationFascicleId";
        internal const string SQL_Param_FascicleFolder_FascicleFolderLevel = "@FascicleFolderLevel";
        #endregion
        #endregion

        #region[ Document Unit]
        #region [ Function Names ]
        internal const string SQL_FX_DocumentUnit_FascicleDocumentUnitsPublic = "[webapipublic].DocumentUnit_FX_FascicleDocumentUnits";
        internal const string SQL_FX_DocumentUnit_AllowedDocumentUnits = "[webapiprivate].DocumentUnit_FX_AllowedDocumentUnits";
        internal const string SQL_FX_DocumentUnit_HasVisibilityRight = "[webapiprivate].DocumentUnit_FX_HasVisibilityRight";
        internal const string SQL_FX_DocumentUnit_FascicleDocumentUnitsPrivate = "[webapiprivate].DocumentUnit_FX_FascicleDocumentUnits";
        internal const string SQL_FX_DocumentUnits_FascicolableDocumentUnitsSecurityUser = "[webapiprivate].DocumentUnit_FX_FascicolableDocumentUnitsSecurityUser";
        internal const string SQL_FX_DocumentUnits_FascicolableDocumentUnits = "[webapiprivate].DocumentUnit_FX_FascicolableDocumentUnits";
        internal const string SQL_FX_DocumentUnits_AuthorizedDocumentUnitsByFascicle = "[webapiprivate].DocumentUnit_FX_AuthorizedDocumentUnitsByFascicle";
        internal const string SQL_FX_DocumentUnits_CountAuthorizedDocumentUnitsByFascicle = "[webapiprivate].DocumentUnit_FX_CountAuthorizedDocumentUnitsByFascicle";
        internal const string SQL_FX_DocumentUnits_CanBeFascicolable = "[webapiprivate].DocumentUnit_FX_CanBeFascicolable";
        internal const string SQL_FX_DocumentUnit_DocumentUnitsByChain = "[webapiprivate].DocumentUnit_FX_DocumentUnitsByChain";
        #endregion
        #region [ Params ]
        internal const string SQL_Param_DocumentUnit_UserName = "@UserName";
        internal const string SQL_Param_DocumentUnit_Domain = "@Domain";
        internal const string SQL_Param_DocumentUnit_DateTo = "@DateTo";
        internal const string SQL_Param_DocumentUnit_DateFrom = "@DateFrom";
        internal const string SQL_Param_DocumentUnit_FascicleId = "@FascicleId";
        internal const string SQL_Param_DocumentUnit_IdDocumentUnit = "@IdDocumentUnit";
        internal const string SQL_Param_DocumentUnit_FilterNameTitle = "@FilterNameTitle";
        internal const string SQL_Param_DocumentUnit_IncludeThreshold = "@IncludeThreshold";
        internal const string SQL_Param_DocumentUnit_ThresholdFrom = "@ThresholdFrom";
        internal const string SQL_Param_DocumentUnit_Year = "@Year";
        internal const string SQL_Param_DocumentUnit_Number = "@Number";
        internal const string SQL_Param_DocumentUnit_DocumentUnitName = "@DocumentUnitName";
        internal const string SQL_Param_DocumentUnit_CategoryId = "@CategoryId";
        internal const string SQL_Param_DocumentUnit_ContainerId = "@ContainerId";
        internal const string SQL_Param_DocumentUnit_Subject = "@Subject";
        internal const string SQL_Param_DocumentUnit_Skip = "@Skip";
        internal const string SQL_Param_DocumentUnit_Top = "@Top";
        internal const string SQL_Param_DocumentUnit_IncludeChildClassification = "@IncludeChildClassification";
        internal const string SQL_Param_DocumentUnit_IdFascicleFolder = "@IdFascicleFolder";
        internal const string SQL_Param_DocumentUnit_ExcludeLinked = "@ExcludeLinked";
        internal const string SQL_Param_DocumentUnit_FascicleIdCategory = "@FascicleIdCategory";
        internal const string SQL_Param_DocumentUnit_FascicleEnvironment = "@FascicleEnvironment";
        internal const string SQL_Param_DocumentUnit_FascicleType = "@FascicleType";
        internal const string SQL_Param_DocumentUnit_Environment = "@Environment";
        internal const string SQL_Param_DocumentUnit_IdTenantAOO = "@IdTenantAOO";
        internal const string SQL_Param_DocumentUnit_Chains = "@Chains";
        #endregion
        #endregion

        #region [ Massimari Scarto ]
        #region [ Function Names ]
        internal const string SQL_FX_MassimariScarto_AllChildrenByParentd = "[webapiprivate].MassimarioScarto_FX_AllChildrenByParent";
        internal const string SQL_FX_MassimarioScarto_RootChildren = "[webapiprivate].MassimarioScarto_FX_RootChildren";
        internal const string SQL_FX_MassimarioScarto_FilteredMassimari = "[webapiprivate].MassimarioScarto_FX_FilteredMassimari";
        #endregion

        #region [ Params ]
        internal const string SQL_Param_MassimariScarto_ParentId = "@parentId";
        internal const string SQL_Param_MassimariScarto_IncludeLogicalDelete = "@includeLogicalDelete";
        internal const string SQL_Param_MassimariScarto_Name = "@name";
        internal const string SQL_Param_MassimariScarto_FullCode = "@fullcode";
        #endregion

        #endregion

        #region[ Commons ]
        #region[Function Names]
        internal const string SQL_FX_Category_HierarcyCode = "[webapipublic].Category_FX_HierarcyCode";
        internal const string SQL_FX_Category_HierarcyDescription = "[webapipublic].Category_FX_HierarcyDescription";
        internal const string SQL_FX_Category_CategorySubFascicles = "[webapiprivate].Category_FX_CategorySubFascicles";
        internal const string SQL_FX_Category_GeAvailablePeriodicCategoryFascicles = "[webapiprivate].Category_FX_GeAvailablePeriodicCategoryFascicles";
        internal const string SQL_FX_Category_FindCategories = "[webapiprivate].Category_FX_FindCategories";
        internal const string SQL_FX_Category_FindCategory = "[webapiprivate].Category_FX_FindCategory";
        internal const string SQL_FX_Container_FascicleInsertAuthorized = "[webapiprivate].Container_FX_FascicleInsertAuthorized";
        internal const string SQL_FX_Contact_FindContacts = "[webapiprivate].Contact_FX_FindContacts";
        internal const string SQL_FX_Contact_GetContactParents = "[webapiprivate].Contact_FX_GetContactParents";
        internal const string SQL_FX_Contact_GetAuthorizedRoleContacts = "[webapiprivate].Contact_FX_GetAuthorizedRoleContacts";
        internal const string SQL_FX_Role_FindRoles = "[webapiprivate].Role_FX_FindRoles";
        internal const string SQL_FX_Role_CountFindRoles = "[webapiprivate].Role_FX_CountFindRoles";
        internal const string SQL_FX_Category_FindFascicolableCategory = "[webapiprivate].Category_FX_FindFascicolableCategory";
        internal const string SQL_FX_UserDomain_UserRights = "[webapiprivate].UserDomain_FX_UserRights";
        internal const string SQL_FX_RoleUser_AllSecretariesFromDossier = "[webapiprivate].[RoleUser_FX_AllSecretariesFromDossier]";
        #endregion

        #region[Params]
        internal const string SQL_Param_Category_IdCategory = "@IdCategory";
        internal const string SQL_Param_Category_UserName = "@UserName";
        internal const string SQL_Param_Category_Domain = "@Domain";
        internal const string SQL_Param_Category_Name = "@Name";
        internal const string SQL_Param_Category_FascicleType = "@FascicleType";
        internal const string SQL_Param_Category_HasFascicleInsertRights = "@HasFascicleInsertRights";
        internal const string SQL_Param_Category_Manager = "@Manager";
        internal const string SQL_Param_Category_Secretary = "@Secretary";
        internal const string SQL_Param_Category_Role = "@Role";
        internal const string SQL_Param_Category_LoadRoot = "@LoadRoot";
        internal const string SQL_Param_Category_ParentId = "@ParentId";
        internal const string SQL_Param_Category_FullCode = "@FullCode";
        internal const string SQL_Param_Category_FascicleFilterEnabled = "@FascicleFilterEnabled";
        internal const string SQL_Param_Category_ParentAllDescendants = "@ParentAllDescendants";
        internal const string SQL_Param_Category_Container = "@Container";
        internal const string SQL_Param_Category_IdTenantAOO = "@IdTenantAOO";
        internal const string SQL_Param_Container_IdCategory = "@IdCategory";
        internal const string SQL_Param_Container_UserName = "@UserName";
        internal const string SQL_Param_Container_Domain = "@Domain";
        internal const string SQL_Param_Container_FascicleType = "@FascicleType";
        internal const string SQL_Param_Contact_UserName = "@UserName";
        internal const string SQL_Param_Contact_Domain = "@Domain";
        internal const string SQL_Param_Contact_Filter = "@Filter";
        internal const string SQL_Param_Contact_ApplyAuthorizations = "@ApplyAuthorizations";
        internal const string SQL_Param_Contact_ExcludeRoleContacts = "@ExcludeRoleContacts";
        internal const string SQL_Param_Contact_ParentId = "@ParentId";
        internal const string SQL_Param_Contact_ParentToExclude = "@ParentToExclude";
        internal const string SQL_Param_Contact_AVCPParentId = "@AVCPParentId";
        internal const string SQL_Param_Contact_FascicleParentId = "@FascicleParentId";
        internal const string SQL_Param_Contact_IdContact = "@IdContact";
        internal const string SQL_Param_Contact_AddressBookAdministratorGroups = "@AddressBookAdministratorGroups";
        internal const string SQL_Param_Contact_IdRole = "@IdRole";
        internal const string SQL_Param_Role_UserName = "@UserName";
        internal const string SQL_Param_Role_Domain = "@Domain";
        internal const string SQL_Param_Role_Name = "@Name";
        internal const string SQL_Param_Role_UniqueId = "@UniqueId";
        internal const string SQL_Param_Role_ParentId = "@ParentId";
        internal const string SQL_Param_Role_ServiceCode = "@ServiceCode";
        internal const string SQL_Param_Role_IdTenantAOO = "@IdTenantAOO";
        internal const string SQL_Param_Role_Environment = "@Environment";
        internal const string SQL_Param_Role_LoadOnlyRoot = "@LoadOnlyRoot";
        internal const string SQL_Param_Role_LoadOnlyMy = "@LoadOnlyMy";
        internal const string SQL_Param_Role_LoadAlsoParent = "@LoadAlsoParent";
        internal const string SQL_Param_Role_RoleTypology = "@RoleTypology";
        internal const string SQL_Param_Role_IdCategory = "@IdCategory";
        internal const string SQL_Param_Role_IdDossierFolder = "@IdDossierFolder";
        internal const string SQL_Param_UserDomain_UserName = "@UserName";
        internal const string SQL_Param_UserDomain_Domain = "@Domain";
        internal const string SQL_Param_UserDomain_RoleGroupPECRightEnabled = "@RoleGroupPECRightEnabled";
        internal const string SQL_Param_UserRole_IdDossier = "@IdDossier";
        #endregion


        #endregion

        #region[Protocols]
        #region[Function Name]
        internal const string SQL_FX_Protocol_AuthorizedProtocols = "[webapiprivate].Protocol_FX_AuthorizedProtocols";
        internal const string SQL_FX_Protocol_ProtocolContactModels = "[webapipublic].ProtocolContact_FX_ProtocolContacts";
        internal const string SQL_FX_Protocol_ProtocolRoleModels = "[webapipublic].ProtocolRole_FX_ProtocolRoles";
        #endregion
        #region[Params]
        internal const string SQL_Param_Protocol_UserName = "@UserName";
        internal const string SQL_Param_Protocol_Domain = "@Domain";
        internal const string SQL_Param_Protocol_DateFrom = "@DateFrom";
        internal const string SQL_Param_Protocol_DateTo = "@DateTo";
        internal const string SQL_Param_Protocol_Year = "@ProtocolYear";
        internal const string SQL_Param_Protocol_Number = "@ProtocolNumber";
        #endregion
        #endregion

        #region[Resolutions]
        #region[Function Name]
        internal const string SQL_FX_Resolution_AlboPretorioResolutions = "[webapipublic].Resolution_FX_AlboPretorioResolutions";
        #endregion
        #region[Params]
        internal const string SQL_Param_Resolution_OnlinePublicationInterval = "@OnlinePublicationInterval";
        internal const string SQL_Param_Resolution_IdType = "@IdType";
        #endregion
        #endregion

        #region [ Templates ]
        #region [ Function Name ]
        internal const string SQL_FX_Template_CountInvalidTemplateUsers = "[webapiprivate].TemplateCollaboration_FX_CountInvalidTemplateUsers";
        internal const string SQL_FX_Template_CountInvalidTemplateRoles = "[webapiprivate].TemplateCollaboration_FX_CountInvalidTemplateRoles";
        internal const string SQL_FX_TemplateCollaboration_AuthorizedTemplates = "[webapiprivate].TemplateCollaboration_FX_AuthorizedTemplates";
        internal const string SQL_FX_TemplateCollaboration_FX_GetAllParentsOfTemplate = "[webapiprivate].TemplateCollaboration_FX_GetAllParentsOfTemplate";
        internal const string SQL_FX_TemplateCollaboration_InvalidateTemplatesByUserAccounts = "[webapiprivate].TemplateCollaboration_FX_InvalidTemplatesByUserAccount";
        internal const string SQL_FX_TemplateCollaboration_GetChildren = "[webapiprivate].TemplateCollaboration_FX_GetChildren";
        #endregion

        #region [ Params ]
        internal const string SQL_Param_Template_IdTemplateCollaboration = "@IdTemplateCollaboration";
        internal const string SQL_Param_Template_UserName = "@UserName";
        internal const string SQL_Param_Template_Domain = "@Domain";
        internal const string SQL_Param_Template_Account = "@Account";
        internal const string SQL_Param_Template_idRole = "@idRole";
        internal const string SQL_Param_Template_IdParent = "@IdParent";
        internal const string SQL_Param_Template_Status = "@Status";
        #endregion
        #endregion

        #region[Collaborations]
        #region[Function Name]
        internal const string SQL_FX_Collaboration_CollaborationsSigning = "[webapiprivate].Collaboration_FX_CollaborationsSigning";
        internal const string SQL_FX_Collaboration_CollaborationsDelegationSigning = "[webapiprivate].Collaboration_FX_CollaborationsDelegationSigning";
        internal const string SQL_FX_Collaboration_ProposedCollaborations = "[webapiprivate].Collaboration_FX_ProposedCollaborations";
        internal const string SQL_FX_Collaboration_AllUserCollaborations = "[webapiprivate].Collaboration_FX_AllUserCollaborations";
        internal const string SQL_FX_Collaboration_ActiveUserCollaborations = "[webapiprivate].Collaboration_FX_ActiveUserCollaborations";
        internal const string SQL_FX_Collaboration_RegisteredCollaborations = "[webapiprivate].Collaboration_FX_RegisteredCollaborations";
        internal const string SQL_FX_Collaboration_AlreadySignedCollaborations = "[webapiprivate].Collaboration_FX_AlreadySignedCollaborations";
        internal const string SQL_FX_Collaboration_ProtocolAdmissionCollaborations = "[webapiprivate].Collaboration_FX_ProtocolAdmissionCollaborations";
        internal const string SQL_FX_Collaboration_CollaborationsManaging = "[webapiprivate].Collaboration_FX_CollaborationsManaging";
        internal const string SQL_FX_Collaboration_CheckedOutCollaborations = "[webapiprivate].Collaboration_FX_CheckedOutCollaborations";

        internal const string SQL_FX_Collaboration_CountCollaborationsSigning = "[webapiprivate].Collaboration_FX_CountCollaborationsSigning";
        internal const string SQL_FX_Collaboration_CountCollaborationsDelegationSigning = "[webapiprivate].Collaboration_FX_CountCollaborationsDelegationSigning";
        internal const string SQL_FX_Collaboration_CountProposedCollaborations = "[webapiprivate].Collaboration_FX_CountProposedCollaborations";
        internal const string SQL_FX_Collaboration_CountAllUserCollaborations = "[webapiprivate].Collaboration_FX_CountAllUserCollaborations";
        internal const string SQL_FX_Collaboration_CountActiveUserCollaborations = "[webapiprivate].Collaboration_FX_CountActiveUserCollaborations";
        internal const string SQL_FX_Collaboration_CountRegisteredCollaborations = "[webapiprivate].Collaboration_FX_CountRegisteredCollaborations";
        internal const string SQL_FX_Collaboration_CountAlreadySignedCollaborations = "[webapiprivate].Collaboration_FX_CountAlreadySignedCollaborations";
        internal const string SQL_FX_Collaboration_CountProtocolAdmissionCollaborations = "[webapiprivate].Collaboration_FX_CountProtocolAdmissionCollaborations";
        internal const string SQL_FX_Collaboration_CountCollaborationsManaging = "[webapiprivate].Collaboration_FX_CountCollaborationsManaging";
        internal const string SQL_FX_Collaboration_CountCheckedOutCollaborations = "[webapiprivate].Collaboration_FX_CountCheckedOutCollaborations";
        #endregion
        #region[Params]
        internal const string SQL_Param_Collaboration_IsRequired = "@IsRequired";
        internal const string SQL_Param_Collaboration_DateFrom = "@DateFrom";
        internal const string SQL_Param_Collaboration_DateTo = "@DateTo";
        internal const string SQL_Param_Collaboration_UserName = "@UserName";
        internal const string SQL_Param_Collaboration_Signers = "@Signers";
        internal const string SQL_Param_Collaboration_EntityId = "@EntityId";
        internal const string SQL_Param_Collaboration_DocumentType = "@DocumentType";
        internal const string SQL_Param_Collaboration_IdTenant = "@IdTenant";
        internal const string SQL_Param_Collaboration_MemorandumDate = "@MemorandumDate";
        internal const string SQL_Param_Collaboration_Object = "@Object";
        internal const string SQL_Param_Collaboration_Note = "@Note";
        internal const string SQL_Param_Collaboration_RegistrationName = "@RegistrationName";
        internal const string SQL_Param_Collaboration_Skip = "@Skip";
        internal const string SQL_Param_Collaboration_Top = "@Top";
        #endregion
        #endregion

        #region[Dossiers]
        #region[Function Names]
        internal const string SQL_FX_Dossier_AuthorizedDossiers = "[webapiprivate].Dossiers_FX_AuthorizedDossiers";
        internal const string SQL_FX_Dossier_CountAuthorizedDossiers = "[webapiprivate].Dossiers_FX_CountAuthorizedDossiers";
        internal const string SQL_FX_Dossier_HasVisibilityRightDossier = "[webapiprivate].Dossiers_FX_HasVisibilityRightDossier";
        internal const string SQL_FX_Dossier_HasViewableRight = "[webapiprivate].Dossiers_FX_HasViewableRight";
        internal const string SQL_FX_Dossier_HasManageableRight = "[webapiprivate].Dossiers_FX_HasManageableRight";
        internal const string SQL_FX_Dossier_GetDossierContacts = "[webapiprivate].Dossiers_FX_GetDossierContacts";
        internal const string SQL_FX_Dossier_HasInsertRight = "[webapiprivate].Dossiers_FX_HasInsertRight";
        internal const string SQL_FX_Dossier_HasModifyRight = "[webapiprivate].Dossiers_FX_HasModifyRight";
        internal const string SQL_FX_Dossier_HasRootNode = "[webapiprivate].Dossiers_FX_HasRootNode";
        internal const string SQL_FX_DossierFolder_RootChildren = "[webapiprivate].DossierFolder_FX_RootChildren";
        internal const string SQL_FX_DossierFolder_AllChildrenByParent = "[webapiprivate].DossierFolder_FX_AllChildrenByParent";
        internal const string SQL_FX_DossierFolder_CountChildren = "[webapiprivate].DossierFolder_FX_CountChildren";
        internal const string SQL_FX_DossierFolder_NameAlreadyExists = "[webapiprivate].DossierFolder_FX_NameAlreadyExists";
        internal const string SQL_FX_DossierFolder_FascicleAlreadyExists = "[webapiprivate].DossierFolder_FX_FascicleAlreadyExists";
        internal const string SQL_FX_DossierFolder_FindProcessFolders = "[webapiprivate].[DossierFolders_FX_FindProcessFolders]";
        internal const string SQL_FX_DossierFolder_GetAllParentsOfFascicle = "[webapiprivate].[DossierFolder_FX_GetAllParentsOfFascicle]";
        internal const string SQL_FX_DossierFolder_GetParent = "[webapiprivate].[DossierFolder_FX_GetParent]";
        internal const string SQL_FX_DossierFolder_GetChildren = "[webapiprivate].[DossierFolder_FX_GetChildren]";
        internal const string SQL_FX_DossierFolder_FX_GetAllDossierFolderLabelName = "[webapiprivate].[DossierFolders_FX_GetAllDossierFolderLabelName]";
        #endregion

        #region [ Params ]
        internal const string SQL_Param_Dossier_IdDossier = "@IdDossier";
        internal const string SQL_Param_Dossier_UserName = "@UserName";
        internal const string SQL_Param_Dossier_Domain = "@Domain";
        internal const string SQL_Param_Dossier_Year = "@Year";
        internal const string SQL_Param_Dossier_Skip = "@Skip";
        internal const string SQL_Param_Dossier_Top = "@Top";
        internal const string SQL_Param_Dossier_Number = "@Number";
        internal const string SQL_Param_Dossier_Subject = "@Subject";
        internal const string SQL_Param_Dossier_Container = "@ContainerId";
        internal const string SQL_Param_Dossier_StartDateFrom = "@StartDateFrom";
        internal const string SQL_Param_Dossier_StartDateTo = "@StartDateTo";
        internal const string SQL_Param_Dossier_EndDateFrom = "@EndDateFrom";
        internal const string SQL_Param_Dossier_EndDateTo = "@EndDateTo";
        internal const string SQL_Param_Dossier_Note = "@Note";
        internal const string SQL_Param_Dossier_IdMetadataRepository = "@IdMetadataRepository";
        internal const string SQL_Param_Dossier_MetadataValue = "@MetadataValue";
        internal const string SQL_Param_Dossier_MetadataValues = "@MetadataValues";
        internal const string SQL_Param_Dossier_DescendingOrder = "@DescendingOrder";
        internal const string SQL_Param_DossierFolder_IdDossierFolder = "@IdDossierFolder";
        internal const string SQL_Param_DossierFolder_IdDossierParentFolder = "@IdParent";
        internal const string SQL_Param_DossierFolder_Status = "@Status";
        internal const string SQL_Param_DossierFolder_FolderName = "@FolderName";
        internal const string SQL_Param_DossierFolder_IdFascicle = "@IdFascicle";
        internal const string SQL_Param_FascicleFolder_FolderName = "@FolderName";
        internal const string SQL_Param_FascicleFolder_IdFascicle = "@IdFascicle";
        internal const string SQL_Param_Dossier_MetadataRepository = "@MetadataRepositoryId";
        internal const string SQL_Param_DossierFolder_Name = "@Name";
        internal const string SQL_Param_DossierFolder_ProcessId = "@ProcessId";
        internal const string SQL_Param_DossierFolder_LoadOnlyActive = "@LoadOnlyActive";
        internal const string SQL_Param_DossierFolder_LoadOnlyMy = "@LoadOnlyMy";
        internal const string SQL_Param_Dossier_DossierType = "@DossierType";
        internal const string SQL_Param_DossierFolder_Skip = "@Skip";
        internal const string SQL_Param_DossierFolder_Top = "@Top";
        internal const string SQL_Param_DossierFolder_LoadOnlyFolders = "@LoadOnlyFolders";

        #endregion
        #endregion

        #region [WorkflowRepositories]
        #region [Function Name]
        internal const string SQL_FX_WorkflowRepository_AuthorizedActiveWorkflowRepositories = "[webapiprivate].WorkflowRepository_FX_AuthorizedActiveWorkflowRepositories";
        internal const string SQL_FX_WorkflowRepository_HasAuthorizedWorkflowRepositories = "[webapiprivate].WorkflowRepository_FX_HasAuthorizedWorkflowRepositories";
        #endregion
        #region [Params ]
        internal const string SQL_Param_WorkflowRepository_UserName = "@UserName";
        internal const string SQL_Param_WorkflowRepository_Domain = "@Domain";
        internal const string SQL_Param_WorkflowRepository_Environment = "@Environment";
        internal const string SQL_Param_WorkflowRepository_AnyEnvironment = "@AnyEnvironment";
        internal const string SQL_Param_WorkflowRepository_DocumentRequired = "@DocumentRequired";
        internal const string SQL_Param_WorkflowRepository_DocumentUnitRequired = "@DocumentUnitRequired";
        internal const string SQL_Param_WorkflowRepository_ShowOnlyNoInstanceWorkflows = "@ShowOnlyNoInstanceWorkflows";
        internal const string SQL_Param_WorkflowRepository_ShowOnlyHasIsFascicleClosedRequired = "@ShowOnlyHasIsFascicleClosedRequired";

        #endregion
        #endregion

        #region [UDS]
        #region [Function Name]
        internal const string SQL_FX_UDSRepository_ViewableRepositoriesByTypology = "[webapiprivate].[UDSRepository_FX_ViewableRepositoriesByTypology]";
        internal const string SQL_FX_UDSRepository_InsertableRepositoriesByTypology = "[webapiprivate].[UDSRepository_FX_InsertableRepositoriesByTypology]";
        internal const string SQL_FX_UDSRepository_AvailableCQRSPublishedUDSRepositories = "[webapiprivate].[UDSRepository_FX_AvailableCQRSPublishedUDSRepositories]";
        internal const string SQL_FX_UDSFieldList_GetChildrenByParent = "[webapiprivate].[UDSFieldList_FX_GetChildrenByParent]";
        internal const string SQL_FX_UDSFieldList_GetParent = "[webapiprivate].[UDSFieldList_FX_GetParent]";
        internal const string SQL_FX_UDSFieldList_GetAllParents = "[webapiprivate].[UDSFieldList_FX_GetAllParents]";
        #endregion
        #region [Params ]
        internal const string SQL_Param_UDSRepository_UserName = "@UserName";
        internal const string SQL_Param_UDSRepository_Domain = "@Domain";
        internal const string SQL_Param_UDSRepository_IdUDSTypology = "@IdUDSTypology";
        internal const string SQL_Param_UDSRepository_PECAnnexedEnabled = "@PECAnnexedEnabled";
        internal const string SQL_Param_UDSRepository_Name = "@Name";
        internal const string SQL_Param_UDSRepository_Alias = "@Alias";
        internal const string SQL_Param_UDSRepository_Container = "@ContainerId";
        internal const string SQL_Param_UDSFieldList_IdUDSFieldList = "@IdUDSFieldList";
        #endregion
        #endregion

        #region [ Conservation ]
        #region [ Function Names ]
        internal const string SQL_FX_Conservation_AvailableProtocolLogs = "[webapiprivate].Conservation_FX_AvailableProtocolLogsToConservate";
        internal const string SQL_FX_Conservation_AvailableDocumentSeriesItemLogs = "[webapiprivate].Conservation_FX_AvailableDocumentSeriesItemLogsToConservate";
        internal const string SQL_FX_Conservation_AvailableDossierLogs = "[webapiprivate].Conservation_FX_AvailableDossierLogsToConservate";
        internal const string SQL_FX_Conservation_AvailableFascicleLogs = "[webapiprivate].Conservation_FX_AvailableFascicleLogsToConservate";
        internal const string SQL_FX_Conservation_AvailablePECMailLogs = "[webapiprivate].Conservation_FX_AvailablePECMailLogsToConservate";
        internal const string SQL_FX_Conservation_AvailableTableLogs = "[webapiprivate].Conservation_FX_AvailableTableLogsToConservate";
        internal const string SQL_FX_Conservation_AvailableUDSLogs = "[webapiprivate].Conservation_FX_AvailableUDSLogsToConservate";

        internal const string SQL_FX_Conservation_CountAvailableProtocolLogs = "[webapiprivate].Conservation_FX_CountProtocolLogsToConservate";
        internal const string SQL_FX_Conservation_CountAvailableDocumentSeriesItemLogs = "[webapiprivate].Conservation_FX_CountDocumentSeriesItemLogsToConservate";
        internal const string SQL_FX_Conservation_CountAvailableDossierLogs = "[webapiprivate].Conservation_FX_CountDossierLogsToConservate";
        internal const string SQL_FX_Conservation_CountAvailableFascicleLogs = "[webapiprivate].Conservation_FX_CountFascicleLogsToConservate";
        internal const string SQL_FX_Conservation_CountAvailablePECMailLogs = "[webapiprivate].Conservation_FX_CountPECMailLogsToConservate";
        internal const string SQL_FX_Conservation_CountAvailableTableLogs = "[webapiprivate].Conservation_FX_CountTableLogsToConservate";
        internal const string SQL_FX_Conservation_CountAvailableUDSLogs = "[webapiprivate].Conservation_FX_CountUDSLogsToConservate";
        #endregion

        #region [ Params ]
        internal const string SQL_Param_Conservation_Skip = "@Skip";
        internal const string SQL_Param_Conservation_Top = "@Top";
        #endregion
        #endregion

        #region [ Monitors ]
        #region [ Function Names ]
        internal const string SQL_FX_TransparentAdministrationMonitors_MonitoringSeriesSection = "[webapiprivate].TransparentAdministrationMonitors_FX_MonitoringSeriesSection";
        internal const string SQL_FX_TransparentAdministrationMonitors_MonitoringQualitySummary = "[webapiprivate].TransparentAdministrationMonitors_FX_MonitoringQualitySummary";
        internal const string SQL_FX_TransparentAdministrationMonitors_MonitoringQualitySummaryDetails = "[webapiprivate].TransparentAdministrationMonitors_FX_MonitoringQualityDetails";
        internal const string SQL_FX_TransparentAdministrationMonitors_MonitoringSeriesRole = "[webapiprivate].TransparentAdministrationMonitors_FX_MonitoringSeriesRole";
        #endregion
        #region [ Params ]
        internal const string SQL_Param_TransparentAdministrationMonitors_DateFrom = "@DateFrom";
        internal const string SQL_Param_TransparentAdministrationMonitors_DateTo = "@DateTo";
        internal const string SQL_Param_TransparentAdministrationMonitors_IdDocumentSeries = "@IdDocumentSeries";
        internal const string SQL_Param_TransparentAdministrationMonitors_IdRole = "@IdRole";
        #endregion
        #endregion

        #region [ Tenants ]
        #region [ Function Names ]
        internal const string SQL_FX_Tenant_UserTenants = "[webapiprivate].[Tenants_FX_UserTenants]";
        #endregion
        #region [ Params ]
        internal const string SQL_Param_Tenant_UserName = "@Username";
        internal const string SQL_Param_Tenant_Domain = "@Domain";

        #endregion

        #endregion

        #region [ Processes ]

        #region [ Function Names ]

        internal const string SQL_FX_Process_FindProcesses = "[webapiprivate].[Processes_FX_FindProcesses]";
        internal const string SQL_FX_Process_FindCategoryProcesses = "[webapiprivate].[Processes_FX_FindCategoryProcesses]";
        internal const string SQL_FX_Process_CountCategoryProcesses = "[webapiprivate].[Processes_FX_CountCategoryProcesses]";

        #endregion

        #region [ Params ]

        internal const string SQL_Param_Process_UserName = "@Username";
        internal const string SQL_Param_Process_Domain = "@Domain";
        internal const string SQL_Param_Process_Name = "@Name";
        internal const string SQL_Param_Process_CategoryId = "@CategoryId";
        internal const string SQL_Param_Process_DossierId = "@DossierId";
        internal const string SQL_Param_Process_LoadOnlyMy = "@LoadOnlyMy";
        internal const string SQL_Param_Process_IsProcessActive = "@IsProcessActive";
        internal const string SQL_Param_Process_Skip = "@Skip";
        internal const string SQL_Param_Process_Top = "@Top";

        #endregion

        #endregion
    }
}
