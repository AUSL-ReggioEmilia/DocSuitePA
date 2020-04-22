using System;

namespace VecompSoftware.DocSuiteWeb.Model.WebAPI.Client
{
    public class BaseAddress : IBaseAddress
    {
        #region [ Constructor ]

        public BaseAddress() { }

        #endregion

        #region [ Properties ]

        public Uri Address { get; set; }

        public string AddressName { get; set; }

        public ICredential NetworkCredential { get; set; }

        /// <summary>
        /// Se il valore è null verrà utilizzato il valore di default 
        /// della proprietà Timeout della classe HttpClient che è di 100.000 millisecondi (100 secondi)
        /// </summary>
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/system.net.http.httpclient.timeout%28v=vs.110%29.aspx"/>
        public TimeSpan? Timeout { get; set; }

        #endregion
    }
}
