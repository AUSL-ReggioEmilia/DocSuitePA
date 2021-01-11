using System;

namespace VecompSoftware.DocSuite.SPID.Model.SAML
{
    public class SamlResponse
    {
        /// <summary>
        /// Id della risposta ricevuta dall'IDP.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Versione SAML della risposta
        /// </summary>
        /// <remarks>
        /// Deve valere sempre "2.0", coerentemente con la versione della specifica SAML adottata.
        /// </remarks>
        public string Version { get; set; }

        /// <summary>
        /// Il valore deve fare riferimento all'ID della richiesta a cui l'IDP risponde.
        /// </summary>
        public string SPRequestId { get; set; }

        /// <summary>
        /// Definisce l'entityID dell'entità emittente, cioè l'IDP stesso.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Dati dell'utente recuperato dall'IDP.
        /// </summary>
        public SamlUser User { get; set; }

        /// <summary>
        /// Stato della richiesta.
        /// </summary>
        public SamlResponseStatus Status { get; set; }

        /// <summary>
        /// Indica l'istante di emissione della risposta.
        /// </summary>
        public DateTime ResponseDate { get; set; }

        /// <summary>
        /// Messaggio opzionale allegato allo stato della richiesta.
        /// </summary>
        public string StatusMessage { get; set; }

        public bool IsValid
        {
            get
            {
                return Status == SamlResponseStatus.Success;
            }
        }
    }
}
