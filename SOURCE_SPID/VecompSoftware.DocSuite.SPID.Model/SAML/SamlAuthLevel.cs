namespace VecompSoftware.DocSuite.SPID.Model.SAML
{
    public enum SamlAuthLevel
    {
        /// <summary>
        /// L’identità SPID di primo livello permette l’autenticazione
        /// tramite ID e password stabilita dall’utente
        /// </summary>
        SpidL1 = 1,
        /// <summary>
        /// L’identità SPID di secondo livello permette l’autenticazione
        /// tramite password + generazione di una One Time Password
        /// inviata all’utente
        /// </summary>
        SpidL2 = 2,
        /// <summary>
        /// L’identità SPID di terzo livello permette l’autenticazione
        /// tramite password + smart card
        /// </summary>
        SpidL3 = 3
    }
}
