namespace VecompSoftware.DocSuiteWeb.API
{
    public interface IDocumentDTO : IAPIArgument
    {
        #region [ Properties ]

        string BiblosGuid { get; set; }

        string BiblosArchive { get; set; }

        int? BiblosId { get; set; }

        string Name { get; set; }

        string FullName { get; set; }

        byte[] Content { get; set; }

        #endregion
    }
}
