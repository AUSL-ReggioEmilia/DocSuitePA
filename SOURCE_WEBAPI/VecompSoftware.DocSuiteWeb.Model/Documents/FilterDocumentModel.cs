using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Documents
{
    public class FilterDocumentModel
    {
        public string Filter { get; set; }

        public IList<string> ArchiveNames { get; set; }

        public int Skip { get; set; }

        public int Top { get; set; }

    }
}
