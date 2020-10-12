using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.Models
{

    public class ProcessSummary
    {
        private List<ZipEntryProcessSummary> _zipEntrySummaries;
        public IReadOnlyList<ZipEntryProcessSummary> ZipEntrySummaries => _zipEntrySummaries;

        /// <summary>
        /// The number of zip entries in metadata
        /// </summary>
        public int InitialCount { get; }

        public ProcessSummary(int initialCount)
        {
            _zipEntrySummaries = new List<ZipEntryProcessSummary>();
            InitialCount = initialCount;
        }

        public void Add(ZipEntryProcessSummary processSummary)
        {
            _zipEntrySummaries.Add(processSummary);
        }
    }
}
