using System.Text;

namespace VecompSoftware.JeepService.Pec.IterationTrackerFiles
{
    /// <summary>
    /// Classe usata per descrivere i tentativi di archiviazzione che vengono fatti nel ArchiveFile nel Receiver
    /// </summary>
    public class IterationDescriptor
    {
        public IterationDescriptor()
        {
            Status = StatusAttempt.Started;
            Message = new StringBuilder();
        }

        /// <summary>
        /// N.B l'attuale implementazione di ArchiveFiles scrivera in questa proprieta il numero di tentativi in ordine inverso (es. 5 => 4 => 3 ecc)
        /// </summary>
        public int IterationIndex { get; set; } 

        public StringBuilder Message { get; set; }

        public StatusAttempt Status { get; set; }
    }
}
