using System;
using System.Collections.Generic;

namespace VecompSoftware.Common
{
    public class SignInfo
    {
        #region [ Enums ]

        public enum SignTypes
        {
            Unknown = 0,
            CAdES = 1,
            PAdES = 2,
            TimeStamp = 3
        }

        #endregion

        #region [ Properties ]

        public string SignType { get; set; }
        public DateTime? SignDate { get; set; }
        public string Reason { get; set; }
        public bool? IsVerified { get; set; }
        public SigningCertificate Certificate { get; set; }
        public bool HasCertificate
        {
            get { return Certificate != null; }
        }
        public IEnumerable<SignInfo> Children { get; set; }
        public bool HasChildren
        {
            get { return !Children.IsNullOrEmpty(); }
        }

        #endregion

        [Serializable]
        public class SigningCertificate
        {

            #region [ Properties ]

            public string SerialNumber { get; set; }
            public DateTime? NotBefore { get; set; }
            public DateTime? NotAfter { get; set; }
            public IssuerDN Issuer { get; set; }
            public SubjectDN Subject { get; set; }

            public bool HasIssuer
            {
                get { return Issuer != null; }
            }
            public bool HasSubject
            {
                get { return Subject != null; }
            }

            #endregion

        }

        [Serializable]
        public class IssuerDN
        {

            #region [ Properties ]

            public string SerialNumber { get; set; }
            public string Organization { get; set; }
            public string OrganizationUnit { get; set; }

            #endregion

        }

        [Serializable]
        public class SubjectDN
        {

            #region [ Properties ]

            public string SerialNumber { get; set; }
            public string GivenName { get; set; }
            public string Surname { get; set; }
            public string CommonName { get; set; }
            public string Organization { get; set; }

            #endregion

        }

    }
}
