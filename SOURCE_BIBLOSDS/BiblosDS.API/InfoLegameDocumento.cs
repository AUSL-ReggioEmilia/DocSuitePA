using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Modello di richiesta per trattamenti che coinvolgono i legami documento.
    /// </summary>
    [DataContract]
    public class InfoLegameDocumentoRequest : RequestIdBase
    {
        
    }
    /// <summary>
    /// Modello di risposta per trattamenti che coinvolgono i legami documento.
    /// </summary>
    [DataContract]
    public class InfoLegameDocumentoResponse : ResponseBase
    {
        [DataMember]
        public List<DocumentoItem> Documenti { get; set; }      
    }   
}
