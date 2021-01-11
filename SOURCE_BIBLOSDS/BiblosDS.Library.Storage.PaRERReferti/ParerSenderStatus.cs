using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

using BiblosDS.Library.Storage.PaRERReferti.XSD;
using BiblosDS.Library.Storage.PaRERReferti.Entities;

using BiblosDS.Library.Storage.PaRERReferti.Properties;

namespace BiblosDS.Library.Storage.PaRERReferti.Util
{
    public static class ParerEsibizione
    {
        public static byte[] Send(string xmlParerDocument, string IdCliente, string Ambiente, string User)
        {
            string urlWebService;

            if (Ambiente == "PARER")
                urlWebService = Settings.Default.HTTPEsibizioneService_PROD;
            else
                urlWebService = Settings.Default.HTTPEsibizioneService_PRE;

            string userAgent = "Browser";

            // Genera i dati della chiamata POST
            Dictionary<string, object> parametriPerPost = new Dictionary<string, object>();
            parametriPerPost.Add("VERSIONE", "1.0");
            parametriPerPost.Add("LOGINNAME ", User);
            parametriPerPost.Add("PASSWORD ", "qaz12PO");
            parametriPerPost.Add("XMLSIP", xmlParerDocument);

            // Prepara la request e ottiene la response
            // La response o è un xml o è un byte[] con lo zip 
            byte[] fullResponse;

#if DEBUG
            char[] messagedebug = "<xml><message>DEBUG</message></xml>".ToCharArray();
            fullResponse = Convert.FromBase64CharArray(messagedebug, 0, messagedebug.Length);
#else 
            try
            {
                HttpWebResponse webResponse = ParerSender.MultipartFormDataPost(urlWebService, userAgent, parametriPerPost);

                // traccia della response
                try
                {
                    if (Properties.Settings.Default.TraceEnable == true)
                    {
                        StreamWriter writer = File.CreateText(@"C:\BiblosDS2010\BiblosDSParerService\Response\Resp_Status_Response.txt");

                        writer.Write(webResponse.StatusCode) ; 

                        writer.Close();
                    }
                }
                catch
                {
                    // none 
                }



                // Elabora la response
                BinaryReader responseReader = new BinaryReader(webResponse.GetResponseStream());

                if (webResponse.ContentLength == -1)
                    fullResponse = responseReader.ReadBytes(100000);
                else 
                    fullResponse = responseReader.ReadBytes((int)webResponse.ContentLength);
                
                webResponse.Close();
            }
            catch (Exception e)
            {
                try
                {
                    if (Properties.Settings.Default.TraceEnable == true)
                    {
                        StreamWriter writer = File.CreateText(@"C:\BiblosDS2010\BiblosDSParerService\Response\Resp_Status_Exception.txt");

                        writer.Write(e.Message + "\n" + e.StackTrace);
                        writer.Close();
                    }
                }
                catch
                {
                    // none 
                }
                
                fullResponse = null; 
            }
#endif
            return fullResponse;
        }
    }


}
