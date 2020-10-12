using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Securities
{
    /// <summary>
    /// Modello per la gestione dell'identità dell'utente.
    /// Un buon esempio di utilizzo è passare il samAccountName nei contesti di autenticazione NTLM.
    /// Nei contesti di autenticazione OAuth si dovrebbe specificare l'accountId, o l'indirizzo email.
    /// In generale è necessario identifica univocamente un utente, quindi si deve passare un valore che permette di 
    /// disambiguare le omonimie.
    /// </summary>
    public sealed class IdentityModel : AuthorizationModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]
        /// <summary>
        /// Modello per la gestione dell'identità dell'utente.
        /// Un buon esempio di utilizzo è passare il samAccountName nei contesti di autenticazione NTLM.
        /// Nei contesti di autenticazione OAuth si dovrebbe specificare l'accountId, o l'indirizzo email.
        /// In generale è necessario identifica univocamente un utente, quindi si deve passare un valore che permette di 
        /// disambiguare le omonimie.
        /// </summary>
        /// <param name="account">Riferimento all'identificativo di sicurezza. Es: samAccountName</param>
        /// <param name="authorizationType">Tiplogia di sicurezza <seealso cref="AuthorizationType"/></param>
        public IdentityModel(string account, AuthorizationType authorizationType)
            : base(authorizationType, Guid.NewGuid())
        {
            Name = account;
            Account = account;
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Riferimento all'identificativo di sicurezza. Es: samAccountName
        /// </summary>
        public string Account { get; private set; }

        /// <summary>
        /// Email dell'Identity
        /// </summary>
        public string Email { get; set; }

        #endregion
    }
}
