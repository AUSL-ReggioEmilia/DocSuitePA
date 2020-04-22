using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using log4net;
using System;
using System.Web;
using System.Web.Mvc;

namespace BiblosDS.LegalExtension.AdminPortal.Helpers
{
    public class ActionResultHelper
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        #endregion

        #region [ Methods ]
        public static ActionResult TryCatchWithLogger(Func<ActionResult> func, ILogger logger)
        {
            try
            {
                return func();
            }
            catch (AggregateException ae)
            {
                foreach (Exception ie in ae.Flatten().InnerExceptions)
                {
                    logger.Error(ie.Message, ie);
                }
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw;
            }
        }
        #endregion
    }
}