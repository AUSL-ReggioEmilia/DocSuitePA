namespace VecompSoftware.BPM.Integrations.Modules.VSW.Management.Contact.Models
{
    internal static class CommonDefinitionModel
    {
        #region [ Stored Procedure Names ]
        internal const string SQL_SP_Contact_Insert = "[dbo].Contact_Insert";
        internal const string SQL_SP_Contact_Delete = "[dbo].Contact_Delete";
        internal const string SQL_SP_Contact_Update = "[dbo].Contact_Update";
        #endregion

        #region [ Parameter Names ]
        internal const string SQL_Param_Contact_ContactId = "@ContactId";
        internal const string SQL_Param_Contact_IdContactType = "@IdContactType";
        internal const string SQL_Param_Contact_IdTitle = "@IdTitle";
        internal const string SQL_Param_Contact_IdPlaceName = "@IdPlaceName";
        internal const string SQL_Param_Contact_Description = "@Description";
        internal const string SQL_Param_Contact_BirthDate = "@BirthDate";
        internal const string SQL_Param_Contact_Code = "@Code";
        internal const string SQL_Param_Contact_SearchCode = "@SearchCode";
        internal const string SQL_Param_Contact_FiscalCode = "@FiscalCode";
        internal const string SQL_Param_Contact_Address = "@Address";
        internal const string SQL_Param_Contact_CivicNumber = "@CivicNumber";
        internal const string SQL_Param_Contact_ZipCode = "@ZipCode";
        internal const string SQL_Param_Contact_City = "@City";
        internal const string SQL_Param_Contact_CityCode = "@CityCode";
        internal const string SQL_Param_Contact_TelephoneNumber = "@TelephoneNumber";
        internal const string SQL_Param_Contact_FaxNumber = "@FaxNumber";
        internal const string SQL_Param_Contact_EMailAddress = "@EMailAddress";
        internal const string SQL_Param_Contact_CertifiedMail = "@CertifiedMail";
        internal const string SQL_Param_Contact_Note = "@Note";
        internal const string SQL_Param_Contact_Language = "@Language";
        internal const string SQL_Param_Contact_Nationality = "@Nationality";
        internal const string SQL_Param_Contact_BirthPlace = "@BirthPlace";
        internal const string SQL_Param_Contact_SDIIdentification = "@SDIIdentification";
        internal const string SQL_Param_Contact_RegistrationUser = "@RegistrationUser";
        internal const string SQL_Param_Contact_RegistrationDate = "@RegistrationDate";
        internal const string SQL_Param_Contact_LastChangedUser = "@LastChangedUser";
        internal const string SQL_Param_Contact_LastChangedDate = "@LastChangedDate";
        internal const string SQL_Param_Contact_IsActive = "@IsActive";
        internal const string SQL_Param_Contact_ParentInsertId = "@ParentInsertId";
        #endregion
    }
}
