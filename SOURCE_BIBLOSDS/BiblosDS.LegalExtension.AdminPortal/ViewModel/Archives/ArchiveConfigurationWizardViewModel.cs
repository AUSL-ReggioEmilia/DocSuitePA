using System;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel.Archives
{
    public class ArchiveConfigurationWizardViewModel
    {
        public ArchiveConfigurationWizardViewModel()
        {
            FlowActiveIndex = 1;
        }
        public Guid IdArchive { get; set; }
        public int FlowActiveIndex { get; set; }
        public bool IsCompleteWithErrors { get; set; }
    }
}