using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.ObjectBuilder2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Model.Validations;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects
{
    [LogCategory(LogCategoryDefinition.VALIDATION)]
    public class ObjectValidator<T, E> : IObjectValidator<T>
        where E : IObjectValidator<T>
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ISecurity _currentSecurity;
        private readonly IDecryptedParameterEnvService _parameterEnvService;
        private readonly ILogger _logger;
        private readonly IValidatorMapper<T, E> _mapper;
        private static IEnumerable<LogCategory> _logCategories = null;

        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ObjectValidator<,>));
                }
                return _logCategories;
            }
        }

        public IDataUnitOfWork UnitOfWork => _unitOfWork;

        public ISecurity CurrentSecurity => _currentSecurity;
        public IDecryptedParameterEnvService ParameterEnvService => _parameterEnvService;

        public ObjectValidator<T, E> Self => this;

        #endregion

        #region [ Constructor ]

        public ObjectValidator(ILogger logger, IValidatorMapper<T, E> mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvService)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _currentSecurity = currentSecurity;
            _parameterEnvService = parameterEnvService;
        }

        #endregion

        #region [ Methods ]

        public IObjectValidator<T> MappingFrom(T source)
        {
            try
            {
                IObjectValidator<T> ee = this;
                return _mapper.Map(source, (E)ee);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw new DSWException(string.Concat("Object Validator - unexpected exception was thrown while invoking mapping: ", ex.Message), ex, DSWExceptionCode.VA_Mapper);
            }
        }

        public ICollection<ValidationMessageModel> Validate()
        {
            return new Collection<ValidationMessageModel>();
        }

        public ICollection<ValidationMessageModel> Validate(T source)
        {
            return Validate(source, string.Empty);
        }

        public ICollection<ValidationMessageModel> Validate(T source, string ruleset)
        {
            try
            {
                IObjectValidator<T> sourceMapped = MappingFrom(source);
                string rule = string.IsNullOrEmpty(ruleset) ? string.Empty : ruleset;
                Validator validator = ValidationFactory.CreateValidator<E>(rule);
                ValidationResults results = validator.Validate(sourceMapped);
                ICollection<ValidationMessageModel> validationResults = new Collection<ValidationMessageModel>();
                results.ForEach(x => validationResults.Add(new ValidationMessageModel() { Message = x.Message, Key = x.Tag, MessageCode = 0 }));
                return validationResults;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw new DSWException(string.Concat("Object Validator - unexpected exception was thrown while invoking validate: ", ex.Message), ex, DSWExceptionCode.VA_Anomaly);
            }
        }
        #endregion

    }
}
