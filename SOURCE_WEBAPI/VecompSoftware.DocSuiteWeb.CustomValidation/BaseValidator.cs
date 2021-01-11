using Microsoft.Practices.EnterpriseLibrary.Validation;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Validation.Objects;

namespace VecompSoftware.DocSuiteWeb.CustomValidation
{
    public abstract class BaseValidator<E, T> : Validator<T>
        where T : IObjectValidator<E>
    {

        #region [ Fields ]
        private object _currentTarget;
        private string _key;
        private ValidationResults _validationResults;
        private T _objectToValidate;
        private readonly string _messageTemplate;
        private readonly string _messageException;
        private readonly string _tag;
        #endregion

        #region [ Properties ]
        protected IDataUnitOfWork CurrentUnitOfWork { get; private set; }
        #endregion

        #region [ Constructor ]

        public BaseValidator(string messageResult, string tag)
            : base(messageResult, tag)
        {
            _messageTemplate = messageResult;
            _messageException = $"Object validator '{GetType().Name}' is null";
            _tag = tag;
        }
        protected override string DefaultMessageTemplate => _messageTemplate;

        #endregion

        #region [ Methods ]

        protected override void DoValidate(T objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (objectToValidate == null)
            {
                throw new DSWException(_messageException, null, DSWExceptionCode.VA_CustomValidatorObject);
            }
            _objectToValidate = objectToValidate;
            _currentTarget = currentTarget;
            _key = key;
            _validationResults = validationResults;
            CurrentUnitOfWork = objectToValidate.UnitOfWork;
            ValidateObject(objectToValidate);
        }

        protected void GenerateInvalidateResult()
        {
            Tag = _tag;
            LogValidationResult(_validationResults, DefaultMessageTemplate != _messageTemplate ? DefaultMessageTemplate : GetMessage(_objectToValidate, _key), _currentTarget, _key);
            return;
        }
        protected abstract void ValidateObject(T objectToValidate);
        #endregion
    }
}
