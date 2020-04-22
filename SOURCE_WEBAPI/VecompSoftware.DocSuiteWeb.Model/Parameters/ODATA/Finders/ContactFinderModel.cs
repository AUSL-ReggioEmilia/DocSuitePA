namespace VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders
{
    public class ContactFinderModel
    {
        #region [ Properties ]
        public string Filter { get; set; }
        public bool? ApplyAuthorizations { get; set; }
        public bool? ExcludeRoleContacts { get; set; }
        public int? ParentId { get; set; }
        public int? ParentToExclude { get; set; }
        #endregion
    }
}
