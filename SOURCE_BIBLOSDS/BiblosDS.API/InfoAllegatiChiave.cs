using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Richiesta attach documento a partire dalla chiave documento.
    /// </summary>
    [DataContract]
    public class InfoAllegatiChiaveRequest : RequestBase
    {
        public FileItem Allegato { get; set; }
    }

    /// <summary>
    /// Risposta alle richieste per ottenere gli attach di un documento partendo dalla chiave del medesimo.
    /// </summary>
    [DataContract]
    public class InfoAllegatiChiaveResponse : InfoAllegatiResponse
    {
    }
}
