namespace VecompSoftware.BPM.Integrations.Modules.VSW.Management.Audit.Models
{
    internal static class CommonDefinitionModel
    {
        #region [ Stored Procedure Names ]
        internal const string SQL_SP_Audit_Insert = "[dbo].Audit_Insert";
        #endregion

        #region [ Parameter Names ]
        internal const string SQL_Param_Audit_AuditId = "@AuditId";
        internal const string SQL_Param_Audit_EntityUniqueId = "@EntityUniqueId";
        internal const string SQL_Param_Audit_EntityName = "@EntityName";
        internal const string SQL_Param_Audit_LogDate = "@LogDate";
        internal const string SQL_Param_Audit_UserHost = "@UserHost";
        internal const string SQL_Param_Audit_Account = "@Account";
        internal const string SQL_Param_Audit_Type = "@Type";
        internal const string SQL_Param_Audit_Description = "@Description";
        internal const string SQL_Param_Audit_LastChangedUser = "@LastChangedUser";
        internal const string SQL_Param_Audit_LastChangedDate = "@LastChangedDate";
        #endregion
    }
}
