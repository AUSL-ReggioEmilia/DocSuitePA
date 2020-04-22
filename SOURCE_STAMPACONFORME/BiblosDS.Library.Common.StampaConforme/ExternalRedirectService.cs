using System;
using VecompSoftware.Commons.BiblosDS.Objects.Enums;

namespace BiblosDS.Library.Common.StampaConforme
{
    public class ExternalRedirectService
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(PrintRedirected));
        int ret = 1;
        string lastWs = "";

        public byte[] Convert(byte[] blob, string fileExtension, string extReq, ConverterType converterType, AttachConversionMode mode = AttachConversionMode.Default)
        {
            ConverterServiceSvc.StampaConformeConverterClient svc = null;
            try
            {                            
                svc = new ConverterServiceSvc.StampaConformeConverterClient("StampaConformeConverter", PrintRedirectConfigurations.GetWsUrl(converterType, lastWs));
                //svc.Url = PrintRedirectConfigurations.GetWsUrl(lastWs);            
                logger.InfoFormat("Call External WS: {0}", svc.Endpoint.ListenUri.ToString());
                byte[] res = null;
                if (mode == AttachConversionMode.Default)
                    res = svc.Convert(blob, fileExtension);                
                else
                    res = svc.ConvertWithParameters(blob, fileExtension, (ConverterServiceSvc.AttachConversionMode)mode);                
                PrintRedirectConfigurations.SetWsUrlPriority(converterType, svc.Endpoint.ListenUri.ToString());
                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if (svc != null)
                    PrintRedirectConfigurations.SetWsUrlFault(converterType, svc.Endpoint.ListenUri.ToString());
                if (ret >= PrintRedirectConfigurations.GetIstance().ServicesCount)
                    throw;
                lastWs = svc.Endpoint.ListenUri.ToString();
                ret += 1;
                return Convert(blob, fileExtension, extReq, converterType);
            }            
        }

        public bool IsAlive(string fileExtension, ConverterType converterType)
        {
            ConverterServiceSvc.StampaConformeConverterClient svc = null;
            try
            {
                svc = new ConverterServiceSvc.StampaConformeConverterClient("StampaConformeConverter", PrintRedirectConfigurations.GetWsUrl(converterType, lastWs));                
                logger.InfoFormat("Call External WS: {0}", svc.Endpoint.ListenUri.ToString());
                return svc.IsAlive();                
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }


        public string GetVersion()
        {
            throw new NotImplementedException();
        }
    }
}
