using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Validations;

namespace VecompSoftware.BPM.Integrations.Services.WebAPI.Exceptions
{
    public class ValidationException : Exception
    {
        #region [ Fields ]
        private readonly IList<ValidationMessageModel> _validationMessages;
        #endregion

        #region [ Constructor ]

        public ValidationException(string message, Exception innerException, IList<ValidationMessageModel> validationMessages)
            : base(message, innerException)
        {
            _validationMessages = validationMessages;
        }

        public ValidationException(Type type, string message, Exception innerException, IList<ValidationMessageModel> validationMessages)
           : this(string.Concat(type.AssemblyQualifiedName, message), innerException, validationMessages) { }

        #endregion

        #region [ Properties ]

        public IList<ValidationMessageModel> ValidationErrors => _validationMessages;
        #endregion
    }
}
