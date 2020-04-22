namespace VecompSoftware.DocSuiteWeb.Model.Integrations.ZenDesk
{
    public class BaseZenDeskModel
    {
        public int? Page { get; set; }
        public string PreviousPage { get; set; }
        public string NextPage { get; set; }
        public int? PerPage { get; set; }
        public int? PageCount { get; set; }
        public int? Count { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
    }
}
