using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BiblosDS.Library.Common.Objects;
using System.ComponentModel;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel
{
    public class ArchiveCompanyViewModel
    {
        public ArchiveCompany ArchiveCompany { get; set; }
        public ICollection<Company> Companies { get; set; }
        public Guid IdCompany { get; set; }
        public long TotalCount { get; set; }
        public int SelectedCompanyIndex { get; set; }
    }
}