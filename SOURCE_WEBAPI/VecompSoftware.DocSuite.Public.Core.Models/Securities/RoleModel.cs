using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Securities
{
    /// <summary>
    /// Modello per la gestione dell'identità dei ruoli dell'utente
    /// </summary>
    public sealed class RoleModel : AuthorizationModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]
        /// <summary>
        /// Modello per la gestione dell'identità dei ruoli dell'utente.
        /// </summary>
        /// <param name="roleUniqueId">Chiave che identifica il settore della DocSuite</param>
        /// <param name="externalTagIdentifier">Riferimento all'identificativo di sicurezza nel contesto dell'applicazione esterna alla DocSuite (o chiave di trascodifica)</param>
        /// <param name="authorizationType">Tiplogia di sicurezza <seealso cref="AuthorizationType"/></param>
        public RoleModel(AuthorizationType authorizationType, Guid? roleUniqueId = null, string externalTagIdentifier = "")
            : base(authorizationType, roleUniqueId.HasValue ? roleUniqueId.Value : Guid.NewGuid())
        {
            Name = externalTagIdentifier;
            RoleUniqueId = roleUniqueId;
            ExternalTagIdentifier = externalTagIdentifier;
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Chiave che identifica il settore della DocSuite
        /// </summary>
        public Guid? RoleUniqueId { get; private set; }

        /// <summary>
        /// Riferimento all'identificativo di sicurezza nel contesto dell'applicazione esterna alla DocSuite (o chiave di trascodifica)
        /// </summary>
        public string ExternalTagIdentifier { get; private set; }

        #endregion
    }
}
