namespace VecompSoftware.DocSuite.Public.Core.Models.Customs.AUSL_RE.BandiDiGara
{
    public class ArchiveFinderModel
    {
        public int Skip { get; set; }
        public int Top { get; set; }
        public string ParentMenuName { get; set; }
        public string ChildMenuName { get; set; }
        public string Subject { get; set; }
        public string OrderColumn { get; set; }
        public bool OrderByDesc { get; set; }
        public int Year { get; set; }
    }
}
