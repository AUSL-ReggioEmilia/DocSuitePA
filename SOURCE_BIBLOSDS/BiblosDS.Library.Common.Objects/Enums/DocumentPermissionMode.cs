using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Enums
{
    /// <summary>
    /// Permessi di accesso al documento
    /// (In salvataggio si definiscono i gruppi o gli utenti
    /// che diritti hanno sul documento)
    /// Ad esempio se si definisce che il gruppo users ha diritto in 
    /// sola lettura
    /// nessun utente del gruppo users riuscirà a fare check-out del documento 
    /// per la modifica
    /// </summary>
    [DataContract(Name = "PermissionMode", Namespace = "http://BiblosDS/2009/10/PermissionMode")]
    public enum DocumentPermissionMode
    {
        /// <summary>
        /// RWU 
        /// Diritti il lettura, scrittura e modifica
        /// </summary>
        [EnumMember]
        FullControl = -1,
        /// <summary>
        /// Diritti in lettura
        /// </summary>
        [EnumMember]
        Read = 1,
        /// <summary>
        /// Diritti in scrittura
        /// </summary>        
        [EnumMember]
        Write = 2,
        /// <summary>
        /// Diritti in modifica
        /// Se si disabilita questa modalità non sarà
        /// possibile fare check-out del documento.
        /// </summary>
        [EnumMember]
        Modify = 3,
    }
}
