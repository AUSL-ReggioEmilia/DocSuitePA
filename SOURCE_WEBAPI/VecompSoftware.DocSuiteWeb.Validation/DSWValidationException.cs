using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Model.Validations;

namespace VecompSoftware.DocSuiteWeb.Validation
{
    public class DSWValidationException : DSWException
    {
        #region [ Fields ]
        private readonly IReadOnlyCollection<ValidationMessageModel> _validationMessages = null;
        #endregion

        #region [ Constructor ]

        public DSWValidationException(string message, IReadOnlyCollection<ValidationMessageModel> validationMessages,
            DSWException innerException, DSWExceptionCode exceptionCode)
            : base(message, innerException, exceptionCode)
        {
            _validationMessages = validationMessages ?? new ReadOnlyCollection<ValidationMessageModel>(new List<ValidationMessageModel>());
        }

        public DSWValidationException(Type type, string message, IReadOnlyCollection<ValidationMessageModel> validationMessages, DSWException innerException, DSWExceptionCode exceptionCode)
           : this(string.Concat(type.AssemblyQualifiedName, message), validationMessages, innerException, exceptionCode)
        {
        }

        #endregion

        #region [ Properties ]

        public IReadOnlyCollection<ValidationMessageModel> ValidationMessages => _validationMessages;
        #endregion

    }
}
