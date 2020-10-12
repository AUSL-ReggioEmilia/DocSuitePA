using System;
using System.Collections.Generic;

namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Archives
{
    public class ValidateArchiveForPreservationResponseModel
    {
        public ValidateArchiveForPreservationResponseModel()
        {
            ValidationErrors = new List<string>();
        }

        public Guid IdArchive { get; set; }        
        public ICollection<string> ValidationErrors { get; set; }
        public bool HasPathPreservation { get; set; }
        public bool HasDateMainAttribute { get; set; }
        public bool HasPrimaryKeyAttribute { get; set; }
        public bool HasPreservations { get; set; }
        public bool IsValidated => ValidationErrors.Count == 0;
    }
}