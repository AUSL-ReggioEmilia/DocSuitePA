namespace VecompSoftware.DocSuiteWeb.Model.Reports
{
    public class ReportModel
    {
        #region [ Properties ]

        public string Id { get; set; }
        public string CategoryId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public string LockedBy { get; set; }
        public string Extension { get; set; }
        public bool IsDraft { get; set; }
        public bool IsFavorite { get; set; }
        public string LastRevisionId { get; set; }
        public string CreatedByName { get; set; }
        public string LockedByName { get; set; }
        public string LastModifiedDate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanView { get; set; }

        #endregion
    }
}
