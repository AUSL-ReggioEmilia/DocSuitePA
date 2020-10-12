using System.Collections.Generic;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel.Archives
{
    public class ArchivePreservationAttributesConfigurationViewModel : ArchiveWizardStepBaseViewModel
    {
        public ArchivePreservationAttributesConfigurationViewModel()
        {
            ArchiveAttributes = new Dictionary<string, string>();
            PreservationAttributes = new Dictionary<string, string>();
        }
        public IDictionary<string, string> ArchiveAttributes { get; set; }
        public IDictionary<string, string> PreservationAttributes { get; set; }        
    }
}