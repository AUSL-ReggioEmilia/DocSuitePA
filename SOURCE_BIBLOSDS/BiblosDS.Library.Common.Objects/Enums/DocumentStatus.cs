using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Enums
{
    public enum DocumentStatus
    {
        /// <summary>
        /// Document saved to Transito Path
        /// </summary>
        Undefined = 1,
        /// <summary>
        /// Document saved to Transito Path
        /// </summary>
        InTransito = 2,
        /// <summary>
        /// Docuemnt saved to the Storage
        /// </summary>
        InStorage = 3,
        /// <summary>
        /// Document put into the cache
        /// </summary>
        InCache = 4,
        /// <summary>
        /// Only the metadata profile is in DB
        /// </summary>
        ProfileOnly = 5,
        /// <summary>
        /// Document removed from storage
        /// </summary>
        RemovedFromStorage = 6,
        /// <summary>
        /// Document removed from storage and saved to preservation
        /// </summary>
        MovedToPreservation = 7
    }
}


