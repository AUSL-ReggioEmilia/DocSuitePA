using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Richiesta di Login
    /// </summary>
    /// <example>
    /// new LoginRequest { UserName = "Gianni", Password = "Passw0rd", IdCliente = "VecompSoftware" }
    /// </example>
    [DataContract]
    public class LoginRequest
    {
        [DataMember(IsRequired = true)]
        public string IdCliente { get; set; }
        [DataMember(IsRequired = true)]
        public string UserName { get; set; }
        [DataMember(IsRequired=true)]
        public string Password { get; set; }        

        public override string ToString()
        {
            return string.Format("IdCliente:{0}, UserName:{1}", IdCliente, UserName);
        }
    }

    /// <summary>
    /// Risposta alla richiesta di login
    /// </summary>
    [DataContract]
    public class LoginResponse : ResponseBase
    {
        /// <summary>
        /// Token persistente per l'autenticazione
        /// </summary>
        [DataMember]
        public TokenInfo UserToken { get; set; }
    }
}
