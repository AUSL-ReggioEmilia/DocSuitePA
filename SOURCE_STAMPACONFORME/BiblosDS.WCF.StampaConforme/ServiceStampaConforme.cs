using System;
using BiblosDS.WCF.Interface;
using BiblosDS.Library.Common.StampaConforme;
using VecompSoftware.Commons.BiblosDS.Objects;

namespace BiblosDS.WCF.StampaConforme
{
    public class ServiceStampaConforme : IServiceStampaConforme
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ServiceStampaConforme));
 
        public DocumentContent ConvertToFormat(DocumentContent content, string fileName, string extReq, out DocumentContent wmfSigns)
        {
            try
            {
                logger.DebugFormat("ConvertToFormat: {0} ({1})", fileName, extReq);
                PrintRedirected pr = new PrintRedirected();
                byte[] sign = null;
                bool isEncripted;
                var result = new DocumentContent(pr.ToRasterFormat(content.Blob, fileName, extReq, out sign, out isEncripted));
                wmfSigns = new DocumentContent(sign);
                return result;
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }          
        }

        public DocumentContent ConvertToFormatLabeled(DocumentContent content, string fileName, string extReq, string label)
        {
            try
            {
                logger.DebugFormat("ConvertToFormat: {0} ({1})", fileName, extReq);
                PrintRedirected pr = new PrintRedirected();
                return new DocumentContent(pr.ConvertToFormatLabeled(content.Blob, fileName, extReq, label));
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }          
        }      

        #region IServiceStampaConforme Members


        public bool IsAlive()
        {
            //Logging.WriteLogEvent(LoggingSource.BiblosDS_WCF_StampaConforme, "ServiceStampaConforme.IsAlive", "IsAlive called", LoggingOperationType.BiblosDS_GetAlive, LoggingLevel.BiblosDS_Trace); 

            return true;
        }

        #endregion
    }
}
