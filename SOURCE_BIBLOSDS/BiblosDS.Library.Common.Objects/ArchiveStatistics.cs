using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "ArchiveStatistics", Namespace = "http://BiblosDS/2009/10/ArchiveStatistics")]
    public class ArchiveStatistics : BiblosDSObject
    {
        /// <summary>
        /// Numero totale di documenti in archivio.
        /// </summary>
        [DataMember]
        public int DocumentsCount { get; set; }
        /// <summary>
        /// Dimensione complessiva in bytes di tutti i documenti contenuti nell'archivio.
        /// </summary>
        [DataMember]
        public long DocumentsVolume { get; set; }
        /// <summary>
        /// Numero di conservazioni, chiuse, che sono state effettuate per l'archivio.
        /// </summary>
        [DataMember]
        public int PreservationsCount { get; set; }
        /// <summary>
        /// Numero di supporti fisici inviati \ spediti.
        /// </summary>
        [DataMember]
        public int ForwardedDevicesCount { get; set; }
    }
}
