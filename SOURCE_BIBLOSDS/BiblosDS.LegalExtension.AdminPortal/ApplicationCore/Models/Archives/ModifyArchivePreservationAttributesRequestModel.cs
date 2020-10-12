using System;
using System.Collections.Generic;

namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Archives
{
    public class ModifyArchivePreservationAttributesRequestModel
    {
        public ModifyArchivePreservationAttributesRequestModel()
        {
            OrderedPreservationAttributes = new Dictionary<Guid, short>();
        }
        public Guid IdArchive { get; set; }
        public IDictionary<Guid, short> OrderedPreservationAttributes { get; set; }
    }
}