using System.Collections.Generic;

namespace VecompSoftware.StampaConforme.Models.Office.Word
{
    public class SaveDocumentToPdfRequest
    {
        public SaveDocumentToPdfRequest()
        {
            RedirectFilters = new List<string>();
        }

        public string SourceFilePath { get; set; }
        public string DestinationFilePath { get; set; }
        public ICollection<string> RedirectFilters { get; set; }
        public bool ForcePortrait { get; set; }
    }
}
