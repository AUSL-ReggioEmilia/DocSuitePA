using System;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel.AwardBatches
{
    public class AwardBatchDetailsViewModel
    {
        public string Name { get; set; }
        public Guid IdAwardBatch { get; set; }
        public Guid IdArchive { get; set; }
        public bool IsOpen { get; set; }
    }
}