using System;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel.AwardBatches
{
    public class AwardBatchViewModel
    {
        public Guid IdArchive { get; set; }
        public Guid? IdPreservation { get; set; }
        public string ArchiveName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public AwardBatchViewModel()
        {
            FromDate = new DateTime(DateTime.Today.Year, 1, 1);
            ToDate = DateTime.Today;
        }
    }
}