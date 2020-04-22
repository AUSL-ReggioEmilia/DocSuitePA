using BiblosDS.Library.Common.Objects;
using System;
using System.Collections.Generic;

namespace BiblosDS.LegalExtension.AdminPortal.Models
{
    [Serializable]
    public class PreservationVerifyIndexModel
    {
        public IList<string> selectedArchives { get; set; }
        public List<DocumentArchive> archives { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }

        public PreservationVerifyIndexModel()
        {
            selectedArchives = new List<string>();
            archives = new List<DocumentArchive>();

            //da inizio annno
            fromDate = new DateTime(DateTime.Today.Year, 1, 1);
            toDate = DateTime.Today;
        }
    }

    public class PreservationVerifyListModel
    {
        public string verifyFolder { get; set; }
    }


    public class PreservationVerifyExecuteModel
    {
        public PreservationVerifyExecuteModel()
        {
            executionId = Guid.NewGuid();
        }
        public Guid executionId { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public PreservationVerifyJob[] jobs { get; set; }
    }


    public class PreservationVerifyJob
    {
        public string idArchive { get; set; }
        public string idPreservation { get; set; }
        public string archiveName { get; set; }

        public string preservationLabel { get; set; }
        public string verifyTitle { get; set; }
        public string result { get; set; }
        public string errors { get; set; }

        public PreservationVerifyJob()
        {
            idArchive = "";
            idPreservation = "";
            archiveName = "";
            preservationLabel = "";
            verifyTitle = "";
            result = "";
            errors = "";
        }
    }
}