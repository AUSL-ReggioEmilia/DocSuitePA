using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Recupero dei profili a partire dalle credenziali utente
    /// </summary>
    /// <remarks>A partire da username e password si recupera l'id customer</remarks>
    /// <example>
    /// GetProfiliRequest request = new GetProfiliRequest {  Token = token.TokenInfo.Token, IdCliente = token.TokenInfo.IdCliente };
    /// </example>
     [DataContract]     
    public class GetArchiviRequest: RequestBase
    {         

    }

     /// <summary>
     /// Risposta alla richiesta di recupero profilo
     /// </summary>     
     /// <example>
     /// GetProfiliResponse actual = DocumentoFacade.GetProfili(request);
     /// </example>
     [DataContract]
     public class GetArchiviResponse: ResponseBase
     {         
         /// <summary>
         /// Archivi (Profili) associati ad un customet
         /// </summary>
         [DataMember]
         public List<Archivio> Archivi { get; set; }

         public GetArchiviResponse()
         {
             Archivi = new List<Archivio>();
         }
     }

    /// <summary>
     /// Archivio documentale
    /// </summary>
     [DataContract]
     public class Archivio
     {
         /// <summary>
         /// Nome del profilo
         /// </summary>      
         /// <remarks>
         /// Nome dell'archivio BiblosDS
         /// </remarks>
         [DataMember]
         public string Nome { get; set; }
         /// <summary>
         /// Descrizione del profilo
         /// </summary>
         /// <remarks>
         /// CustomerKey->Description
         /// </remarks>
         [DataMember]
         public string Descrizione { get; set; }
         /// <summary>
         /// Classe documentale del profilo
         /// </summary>
         /// <remarks>
         /// CustomerKey->DocumentClass
         /// </remarks>
         [DataMember]
         public string TipoDocumento { get; set; }
         /// <summary>
         /// Metadati legati al profilo
         /// </summary>
         [DataMember]
         public List<MetadatoItem> Metadati { get; set; }
     }
}
