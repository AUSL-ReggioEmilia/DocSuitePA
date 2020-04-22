using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    [DataContract]
    public class VerificaStatoConservazioneDocumentoRequest: RequestIdBase
    {
    }

    [DataContract]
    public class VerificaStatoConservazioneDocumentoResponse : VerificaStatoConservazioneResponse
    {
       
    }
}
