using System;
using System.Collections.Generic;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel.Preservations
{
    public class PreservationCheckViewModel
    {
        public PreservationCheckViewModel()
        {
            VerifyFiles = new List<PreservationCheckVerifyFileViewModel>();
        }

        public Guid IdArchive { get; set; }
        public Guid IdPreservation { get; set; }
        public string ArchiveName { get; set; }
        public string ConservationDescription { get; set; }
        public string Path { get; set; }
        public string Manager { get; set; }
        public string CloseDateLabel { get; set; }
        public string LastVerifiedDateLabel { get; set; }
        public bool PathExist { get; set; }
        public bool IsClosed { get; set; }
        public ICollection<PreservationCheckVerifyFileViewModel> VerifyFiles { get; set; }
    }
}