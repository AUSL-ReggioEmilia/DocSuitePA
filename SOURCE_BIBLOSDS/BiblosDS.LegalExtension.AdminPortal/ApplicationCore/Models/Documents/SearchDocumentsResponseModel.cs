using BiblosDS.Library.Common.Objects;
using System.Collections.Generic;

namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Documents
{
    public class SearchDocumentsResponseModel
    {
        public SearchDocumentsResponseModel()
        {
            Documents = new List<Document>();
        }

        public int Counter { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}