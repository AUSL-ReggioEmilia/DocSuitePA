using System;

namespace VecompSoftware.DocSuiteWeb.Common.Exceptions
{
    public class DSWSecurityException : DSWException
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public DSWSecurityException(string message, DSWException innerException, DSWExceptionCode exceptionCode)
           : base(message, innerException, exceptionCode)
        {
        }

        public DSWSecurityException(Type type, string message, DSWException innerException, DSWExceptionCode exceptionCode)
           : this(string.Concat(type.AssemblyQualifiedName, message), innerException, exceptionCode)
        {
        }

        #endregion

        #region [ Properties ]

        #endregion
    }
}
