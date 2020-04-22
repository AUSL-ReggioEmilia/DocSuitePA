using System;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel.Preservations
{
    public class PreservationAuditGridViewModel
    {
        public Guid? IdPreservation { get; set; }
        public string ActivityName { get; set; }
        public DateTime? ActivityDate { get; set; }
        public string ActivityUser { get; set; }
        public string Description { get; set; }
    }
}