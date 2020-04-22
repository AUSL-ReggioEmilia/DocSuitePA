using System;
using System.Security.Cryptography.X509Certificates;

namespace VecompSoftware.DocSuite.SPID.Model.SAML
{
    public class SamlRequestOption
    {
        #region [ Constructor ]
        public SamlRequestOption()
        {
            Id = Guid.NewGuid();
            Version = "2.0";
            SPIDLevel = SamlAuthLevel.SpidL1;
            NotBefore = new TimeSpan(0, -2, 0);
            NotOnOrAfter = new TimeSpan(0, 10, 0);
        }
        #endregion

        #region [ Properties ]
        public Guid Id { get; }

        /// <summary>
        /// URI riconducibile al dominio del Service Provider stesso.
        /// </summary>
        public string SPDomain { get; set; }

        /// <summary>
        /// Versione della specifica SAML adottata.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Indirizzo (URI reference) dell’Identity provider a cui è inviata la richiesta.
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Livello di autenticazione SPID.
        /// </summary>
        public SamlAuthLevel SPIDLevel { get; set; }

        /// <summary>
        /// Riportante indice posizionale facente riferimento ad uno degli elementi <AttributeConsumerService> 
        /// presenti nei metadata del Service Provider.
        /// </summary>
        public ushort AssertionConsumerServiceIndex { get; set; }        

        /// <summary>
        /// Riportante indice posizionale in riferimento alla struttura <AttributeConsumingService> 
        /// presente nei metadata del Service Provider.
        /// </summary>
        public ushort? AttributeConsumingServiceIndex { get; set; }

        public TimeSpan NotBefore { get; }

        public TimeSpan NotOnOrAfter { get; }

        /// <summary>
        /// Certificato utilizzato per firmare la richiesta
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        public string SubjectNameId { get; set; }

        public string AuthnStatementSessionIndex { get; set; }

        public string IdpEntityId { get; set; }
        #endregion
    }
}
