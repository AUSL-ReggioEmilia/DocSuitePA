using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Preservations
{
    public enum PreservationVerifyStatus
    {
        Error,
        Ok
    }

    public class ExecutePreservationVerifyResponseModel
    {
        public ExecutePreservationVerifyResponseModel(Guid idPreservation)
        {
            IdPreservation = idPreservation;
            Status = PreservationVerifyStatus.Error;
            Errors = new List<string>();
            VerifyTitle = "negativo";
        }

        public Guid IdPreservation { get; set; }
        public string VerifyTitle { get; set; }
        public PreservationVerifyStatus Status { get; set; }
        public ICollection<string> Errors { get; set; }
    }
}