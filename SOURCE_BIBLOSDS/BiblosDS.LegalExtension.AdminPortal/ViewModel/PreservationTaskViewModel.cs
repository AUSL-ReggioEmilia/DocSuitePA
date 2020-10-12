using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BiblosDS.Library.Common.Objects;
using System.ComponentModel.DataAnnotations;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel
{
    public class PreservationTaskViewModel
    {
        public List<PreservationTask> Tasks { get; set; }

        public long TotalCount { get; set; }

        public string ArchiveName { get; set; }

        public Guid IdArchive { get; set; }

        public Company Company { get; set; }

        public DocumentArchive Archive { get; set; }

        public List<PreservationSchedule> PeriodSchedulers { get; set; }

        public Guid IdSchedule { get; set; }
     
        public int SelectedScheduleIndex { get; set; }

        public DateTime NextPreservationTaskStartDocumentDate { get; set; }

        public bool HasArchiveConfigurationFile { get; set; }

        public PreservationTask VerifyTask { get; set; }

        public bool HasVerifyTaskDefined { get; set; }

        public bool HasCompanyDefined { get; set; }
    }
}