using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiblosDS.LegalExtension.AdminPortal.Models
{
    public class PreservationItem
    {
        public Guid IdPreservation { get; set; }
        public string Label { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string Username { get; set; }

        public string DisplayCreate { get; set; }
        public string DisplaySign { get; set; }
        public string DisplayClose { get; set; }
        public string DisplayPurge { get; set; }
    }
}