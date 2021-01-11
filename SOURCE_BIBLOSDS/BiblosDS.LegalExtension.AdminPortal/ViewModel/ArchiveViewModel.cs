using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BiblosDS.Library.Common.Objects;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel
{
    public class ArchiveViewModel
    {
        public List<DocumentArchive> Archives { get; set; }

        public long TotalCount { get; set; }
    }
}