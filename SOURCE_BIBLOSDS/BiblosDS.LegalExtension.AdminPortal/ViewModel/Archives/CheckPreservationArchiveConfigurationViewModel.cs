using System.Collections.Generic;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel.Archives
{
    public class CheckPreservationArchiveConfigurationViewModel : ArchiveWizardStepBaseViewModel
    {
        public CheckPreservationArchiveConfigurationViewModel()
        {
            DateAttributes = new Dictionary<string, string>();
            PrimaryKeyAttributes = new Dictionary<string, string>();
        }

        public bool HasPreservations { get; set; }
        public ICollection<string> ValidationErrors { get; set; }
        public IDictionary<string, string> ArchiveAttributes { get; set; }
        public IDictionary<string, string> DateAttributes { get; set; }
        public IDictionary<string, string> PrimaryKeyAttributes { get; set; }
        public bool IsValidated { get; set; }
    }
}