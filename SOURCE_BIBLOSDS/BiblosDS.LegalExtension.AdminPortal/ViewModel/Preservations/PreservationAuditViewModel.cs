using System;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel.Preservations
{
    public class PreservationAuditViewModel
    {
        public PreservationAuditViewModel()
        {
            FromDate = new DateTime(DateTime.Today.Year, 1, 1);
            ToDate = DateTime.Today;
        }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}