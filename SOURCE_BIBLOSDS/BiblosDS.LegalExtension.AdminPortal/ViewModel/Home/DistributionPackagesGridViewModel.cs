using System;
using System.Collections.Generic;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel.Home
{
    public class DistributionPackagesGridViewModel
    {
        public DistributionPackagesGridViewModel()
        {
            Metadata = new DistributionPackagesGridMetadataViewModel[] { };
        }

        public Guid IdDocument { get; set; }
        public string DocumentName { get; set; }
        public DateTime? DocumentCreated { get; set; }
        public Guid? IdPreservation { get; set; }
        public bool IsConservated { get; set; }
        public DistributionPackagesGridMetadataViewModel[] Metadata { get; set; }
    }
}