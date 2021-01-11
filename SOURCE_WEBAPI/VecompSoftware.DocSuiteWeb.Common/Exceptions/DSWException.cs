using System;

namespace VecompSoftware.DocSuiteWeb.Common.Exceptions
{
    public class DSWException : Exception
    {
        #region [ Fields ]
        private readonly DSWExceptionCode _exceptionCode = DSWExceptionCode.Invalid;
        #endregion

        #region [ Constructor ]

        public DSWException(string message, Exception innerException, DSWExceptionCode exceptionCode)
            : base(message, innerException)
        {
            _exceptionCode = exceptionCode;
        }

        public DSWException(Type type, string message, Exception innerException, DSWExceptionCode exceptionCode)
           : this(string.Concat(type.AssemblyQualifiedName, message), innerException, exceptionCode) { }

        #endregion

        #region [ Properties ]

        public DSWExceptionCode ExceptionCode => _exceptionCode;
        #endregion
    }
}
