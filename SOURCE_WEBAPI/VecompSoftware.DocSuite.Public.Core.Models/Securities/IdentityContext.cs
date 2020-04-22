using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VecompSoftware.DocSuite.Public.Core.Models.Securities
{
    /// <summary>
    /// Classe che contiene il contesto di sicurezza (es utente o ruoli dell'utente) del comando o dell'evento. 
    /// </summary>
    public sealed class IdentityContext
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]
        /// <summary>
        /// Classe che contiene il contesto di sicurezza (es utente o ruoli dell'utente) del comando o dell'evento. 
        /// 
        /// </summary>
        /// <param name="identityModel"> Modello per la gestione dell'identità dell'utente.</param>
        public IdentityContext(IdentityModel identityModel)
        {
            Identity = identityModel;
            Roles = new HashSet<RoleModel>();
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Collezione che contiene i ruoli dell'utente <see cref="RoleModel"/>
        /// </summary>
        public ICollection<RoleModel> Roles { get; private set; }

        /// <summary>
        ///  Modello per la gestione dell'identità dell'utente.
        /// </summary>
        [Required]
        public IdentityModel Identity { get; private set; }
        #endregion
    }
}
