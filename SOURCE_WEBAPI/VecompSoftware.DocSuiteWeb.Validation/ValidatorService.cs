using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Validations;

namespace VecompSoftware.DocSuiteWeb.Validation
{
    [LogCategory(LogCategoryDefinition.VALIDATION)]
    public class ValidatorService : IValidatorService
    {
        #region [ Fields ]

        private readonly IUnityContainer _container;
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories = null;

        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ValidatorService));
                }
                return _logCategories;
            }
        }

        #endregion

        public ValidatorService(IUnityContainer container, ILogger logger)
        {
            _container = container;
            _logger = logger;
        }

        public bool Validate<T>(T obj) where T : class
        {
            return Validate(obj, string.Empty);
        }

        public bool Validate<T>(T obj, string ruleset) where T : class
        {
            ICollection<ValidationMessageModel> results = null;
            try
            {
                IValidator<T> validator = _container.Resolve<IValidator<T>>();
                results = validator.Validate(obj, ruleset);
            }
            catch (DSWException) { throw; }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw new DSWException(string.Concat("Validator Service - unexpected exception was thrown while invoking validate: ", ex.Message), ex, DSWExceptionCode.VA_Anomaly);
            }
            if (results != null && results.Count > 0)
            {
                foreach (ValidationMessageModel item in results)
                {
                    _logger.WriteWarning(new LogMessage(item.ToString()), LogCategories);
                }
                throw new DSWValidationException(string.Concat("Il validatore ha prodotto una serie di eccezioni sull'entità inserita ", typeof(T)),
                    new ReadOnlyCollection<ValidationMessageModel>(results.ToList()), null, DSWExceptionCode.VA_RulesetValidation);
            }
            return true;
        }
    }
}
