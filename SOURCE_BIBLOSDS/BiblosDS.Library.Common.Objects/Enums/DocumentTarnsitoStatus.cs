using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Enums
{
    public enum DocumentTarnsitoStatus
    {
        /// <summary>
        /// Storage process first time 
        /// the file
        /// </summary>
        DaProcessare = -1,
        /// <summary>
        /// Storage assembly check-out the file
        /// </summary>
        StorageProcessing = 1,
        /// <summary>
        /// The file is deleted from transito
        /// and from the db
        /// </summary>
        EndProcessing = 0,
        /// <summary>
        /// The file must be re-processed
        /// </summary>
        FaultProcessing = 2,
        /// <summary>
        /// No storage configured
        /// </summary>
        StorageNotFount = 3,
        /// <summary>
        /// No storage Area configured
        /// </summary>
        StorageAreaNotFount = 4,
    }
}
