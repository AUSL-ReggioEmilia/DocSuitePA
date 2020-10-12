namespace VecompSoftware.BPM.Integrations.Modules.VSW.Management.Tenants.Models
{
    internal static class CommonDefinitionModel
    {
        #region [ Stored Procedure Names ]
        internal const string SQL_SP_Tenant_Insert = "[dbo].Tenant_Insert";
        #endregion

        #region [ Parameter Names ]
        internal const string SQL_Param_Tenant_TenantId = "@TenantId";
        internal const string SQL_Param_Tenant_TenantType = "@TenantType";
        internal const string SQL_Param_Tenant_Name = "@Name";
        internal const string SQL_Param_Tenant_CompanyName = "@CompanyName";
        internal const string SQL_Param_Tenant_CategorySuffix = "@CategorySuffix";
        internal const string SQL_Param_Tenant_Note = "@Note";
        internal const string SQL_Param_Tenant_RegistrationUser = "@RegistrationUser";
        internal const string SQL_Param_Tenant_RegistrationDate = "@RegistrationDate";
        internal const string SQL_Param_Tenant_LastChangedUser = "@LastChangedUser";
        internal const string SQL_Param_Tenant_LastChangedDate = "@LastChangedDate";
        internal const string SQL_Param_Tenant_ParentInsertId = "@ParentInsertId";
        #endregion
    }
}
