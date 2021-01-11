using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel.Archives
{
    public class ArchiveConfigurableViewModel
    {
        public Guid IdArchive { get; set; }
        public string ArchiveName { get; set; }
        public bool IsPreservationEnabled { get; set; }
    }
}