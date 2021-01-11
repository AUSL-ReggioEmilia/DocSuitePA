using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Modello di richiesta per la cancellazione di un allegato a partire dalla chiave.
    /// </summary>
    [DataContract]
    public class CancellaAllegatoDocumentoChiaveRequest : RequestBase
    {
    }
    /// <summary>
    /// Modello di risposta per le operazioni di cancellazione degli allegati di un documento a partire dalla chiave.
    /// </summary>
    [DataContract]
    public class CancellaAllegatoDocumentoChiaveResponse : ResponseBase
    {
    }
}
