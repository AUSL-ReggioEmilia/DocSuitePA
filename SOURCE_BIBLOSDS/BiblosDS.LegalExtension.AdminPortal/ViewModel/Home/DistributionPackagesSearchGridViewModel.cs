using BiblosDS.LegalExtension.AdminPortal.Models;
using System;
using System.Collections.Generic;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel.Home
{
    public class DistributionPackagesSearchGridViewModel
    {
        public DistributionPackagesSearchGridViewModel()
        {
            ArchiveAttributes = new List<AttributeModel>();
        }

        public Guid IdArchive { get; set; }
        public ICollection<AttributeModel> ArchiveAttributes { get; set; }
    }
}