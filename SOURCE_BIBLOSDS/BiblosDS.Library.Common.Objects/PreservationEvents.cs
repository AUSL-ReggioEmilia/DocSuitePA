using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationEvents", Namespace = "http://BiblosDS/2009/10/PreservationEvents")]
    public class PreservationEvents
    {
        /// <summary>
        /// Timestamp dell'evento
        /// </summary>
        [DataMember]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// utente che ha compiuto l'azione (può essere BiblosDS se processo auto generato lato server)
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// descrizione dell'evento
        /// </summary>
        public string Description { get; set; }
    }
}
