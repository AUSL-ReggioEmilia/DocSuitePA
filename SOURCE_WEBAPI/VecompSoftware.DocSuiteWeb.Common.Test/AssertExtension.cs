using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace VecompSoftware.DocSuiteWeb.Common.Test
{
    public static class AssertExtension
    {
        #region [ Fields ]

        private const string SUCCESS_MESSAGE = "No exception thrown";
        private const string WARNING_MESSAGE = "Wrong exception thrown {0}\n{1}";
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Assert Fail if the correct execption is not throwed
        /// </summary>
        /// <typeparam name="TEx">Exception to check</typeparam>
        /// <param name="method">Action to monitoring</param>
        /// <param name="message">Error message</param>
        public static void AssertInnerThrows<TEx>(Action method, string message = "") where TEx : Exception
        {
            var managedEx = false;
            try
            {
                method.Invoke();
            }
            catch (AggregateException ex)
            {
                ex.Handle(e => managedEx = e is TEx);

                if (false.Equals(managedEx))
                {
                    Assert.Fail(WARNING_MESSAGE, ex.Message, ex.StackTrace);
                }
            }
            catch (TEx) { return; }
            catch (Exception ex)
            {
                Assert.Fail(WARNING_MESSAGE, ex.Message, ex.StackTrace);
            }
            Assert.Fail(string.IsNullOrEmpty(message) ? SUCCESS_MESSAGE : message);
        }
        #endregion
    }
}
