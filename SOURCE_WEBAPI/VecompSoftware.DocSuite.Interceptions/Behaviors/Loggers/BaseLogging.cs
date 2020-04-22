using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.DocSuite.Interceptors.Behaviors.Loggers
{
    public abstract class BaseLogging : IInterceptionBehavior
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly bool _interceptorEnabled = false;

        #endregion

        #region [ Properties ]
        public bool WillExecute => _interceptorEnabled;

        #endregion

        #region [ Constructor ]
        public BaseLogging(ILogger logger, bool interceptorEnabled)
        {
            _logger = logger;
            _interceptorEnabled = interceptorEnabled;
        }

        #endregion

        #region [ Methods ]

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            if (_logger.IsLoggingEnabled)
            {
                ICollection<LogCategory> logCategories = LogCategoryHelper.GetCategoriesAttribute(input.MethodBase.DeclaringType);
                string messageLog = string.Format("Invoking method {0} with this params {1}", input.MethodBase,
                    input.Arguments.ToString());
                _logger.WriteDebug(new LogMessage(messageLog), logCategories);
            }

            var result = getNext()(input, getNext);
            return result;
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        #endregion
    }
}
