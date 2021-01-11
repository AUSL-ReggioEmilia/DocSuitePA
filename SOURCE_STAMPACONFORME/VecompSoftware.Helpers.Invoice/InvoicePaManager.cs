using System;
using System.Linq;


namespace VecompSoftware.Helpers.Invoice
{
    public class InvoicePaManager : XmlHeaderManager
    {
        private readonly string regularExpressionDefault = "(.*www.fatturapa.gov.it)(.*fatturapa)(.*v[0-9].[0-9])";

        /// <summary>
        /// Stringa contente la regular expression che identifica il tag contente il valore del Namespace         
        /// </summary>
        /// <value>"(.*www.fatturapa.gov.it)(.*fatturapa)(.*v[0-9].[0-9])" è quello correntemente specificato.</value>
        internal override string RegularExpressionToCheck
        {
            get
            {
                return regularExpressionDefault;
            }
        }

        private readonly string stringVersionDefault = "VERSION";

        /// <summary>
        /// Stringa contente il nome del tag contente il valore della versione. 
        /// "VERSION" è quello correntemente specificato.
        /// </summary>
        /// <value>"VERSION" è quello correntemente specificato.</value>
        internal override string StringVersion
        {
            get
            {
                return stringVersionDefault;
            }
        }

        private readonly string stringCultureInfoParsing = "en-US";

        /// <summary>
        /// Ritorna il culture info utilizzata per il parsing della stringa testuale della versione.
        /// </summary>
        /// <value>CultureInfo</value>
        internal override string CultureInfoParsing
        {
            get
            {
                return stringCultureInfoParsing;
            }
        }

        private readonly string document;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoicePaManager" /> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public InvoicePaManager(string document) : base(document)
        {
        }

    }
}
