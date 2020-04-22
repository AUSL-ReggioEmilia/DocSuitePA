namespace VecompSoftware.DocSuite.Public.Core.Models.Securities
{
    /// <summary>
    /// Enumeratore che identifica il corretto contesto di autenticazione usato nel sistema esterno alla DocSuite
    /// </summary>
    public enum AuthorizationType : short
    {
        /// <summary>
        /// Non definito
        /// </summary>
        Invalid = -1,
        /// <summary>
        /// Modello di autenticazione non integrato nella DocSuite
        /// </summary>
        External = 0,
        /// <summary>
        /// Autenticazione integrata di Windows
        /// </summary>
        NTLM = 1,
        /// <summary>
        /// Sicurezza integrata della DocSuite (SecurityUser)
        /// </summary>
        DocSuiteSecurity = 2,
        /// <summary>
        /// Sicurezza OAuth - Owin DocSuite WebAPI Token Bearer
        /// </summary>
        DocSuiteToken = DocSuiteSecurity * 2,
        /// <summary>
        /// Sicurezza OAuth (non DocSuite ex: Google/Live)
        /// </summary>
        OAuth = DocSuiteToken * 2,
        /// <summary>
        /// Autenticazione JSON Web Token 
        /// </summary>
        JWT = OAuth * 2,
        /// <summary>
        /// Autenticazione SPID
        /// </summary>
        SPID = JWT * 2,
    }
}