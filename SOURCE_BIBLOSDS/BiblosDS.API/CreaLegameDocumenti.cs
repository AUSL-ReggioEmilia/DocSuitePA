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
    public class CreaLegameDocumentiRequest : RequestIdBase
    {        
        public new string TipoDocumento
        {
            get
            {
                return base.TipoDocumento;
            }
        }

        [DataMember]
        public Guid IdDocumentoLink { get; set; }
    }
    /// <summary>
    /// Modello di risposta per trattamenti che coinvolgono i legami documento.
    /// </summary>
    [DataContract]
    public class CreaLegameDocumentiResponse : ResponseBase
    {
      
    }   
}
