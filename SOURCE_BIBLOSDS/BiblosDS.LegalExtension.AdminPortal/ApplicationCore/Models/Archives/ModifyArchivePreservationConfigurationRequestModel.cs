using System;
using System.Collections.Generic;

namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Archives
{
    public class ModifyArchivePreservationConfigurationRequestModel
    {
        public ModifyArchivePreservationConfigurationRequestModel()
        {
            OrderedPrimaryKeyAttributes = new Dictionary<Guid, short>();
        }
        public Guid IdArchive { get; set; }
        public string PathPreservation { get; set; }
        public Guid MainDateAttribute { get; set; }
        public IDictionary<Guid, short> OrderedPrimaryKeyAttributes { get; set; }
    }
}