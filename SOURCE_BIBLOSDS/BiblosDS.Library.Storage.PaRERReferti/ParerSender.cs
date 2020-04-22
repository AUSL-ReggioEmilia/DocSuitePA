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
    public static class Parer
    {
        public static string Send(string xmlParerDocument, Document thisDocument, ParerContext thisContext, string Ambiente) 
        {
            string urlWebService ;

            if (Ambiente == "PARER")
                urlWebService = Settings.Default.HTTPVersamentoService_PROD;
            else
                urlWebService = Settings.Default.HTTPVersamentoService_PRE; 
            
            string userAgent = "Browser";
                    
            // Genera i dati della chiamata POST
            Dictionary<string, object> parametriPerPost = new Dictionary<string, object>();
            parametriPerPost.Add("VERSIONE", "1.3");
            parametriPerPost.Add("LOGINNAME ", thisContext.IdCliente);
            parametriPerPost.Add("PASSWORD ", thisContext.UserPwd);
            parametriPerPost.Add("XMLSIP", xmlParerDocument);

            // documento principale 
            string IDDocumento = thisDocument.Documento.IDDocumentKey(0); 
            parametriPerPost.Add(IDDocumento, new ParerSender.FileParameter(thisDocument.Documento.BytesDocumento, IDDocumento, "binary/octetstream"));

            // annessi
/* 
            if (thisDocument._Annessi.Count > 0)
            {
                for (int iAnnesso = 0; iAnnesso < thisDocument._Annessi.Count; iAnnesso++)
                {
                    string IDAnnesso = thisDocument._Annessi[iAnnesso].IDDocumentKey(iAnnesso + 1);
                    parametriPerPost.Add(IDAnnesso, new ParerSender.FileParameter(thisDocument._Annessi[iAnnesso].BytesDocumento, IDAnnesso, "binary/octetstream")); 
                }
            }

            // allegati 
            if (thisDocument._Allegati.Count > 0)
            {
                for (int iAllegato = 0; iAllegato < thisDocument._Allegati.Count; iAllegato++)
                {
                    string IDAllegato = thisDocument._Allegati[iAllegato].IDDocumentKey(iAllegato + 1); 
                    parametriPerPost.Add(IDAllegato, new ParerSender.FileParameter(thisDocument._Allegati[iAllegato].BytesDocumento, IDAllegato, "binary/octetstream")); 
                }
            }
*/
                   
            // Prepara la request e ottieni la response
            string fullResponse;

#if DEBUG
            fullResponse = "<xml><message>DEBUG</message></xml>" ; 
#else 
            try
            {
                HttpWebResponse webResponse = ParerSender.MultipartFormDataPost(urlWebService, userAgent, parametriPerPost);

                // Elabora la response
                StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                fullResponse = responseReader.ReadToEnd();
                webResponse.Close();
            }
            catch (WebException e)
            {
                fullResponse = "<xml><message>" + e.Message + "</message></xml>"; 
            }
#endif 
            return fullResponse ; 
        }
    }


    /// <summary>
    /// classe per invio al Parer su restFull
    /// </summary>
    static class ParerSender
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// Struttura che ospita le informazioni relative ad un file da spedire
        /// </summary>
        public class FileParameter
        {
            public byte[] File { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public FileParameter(byte[] file) : this(file, null) { }
            public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
            public FileParameter(byte[] file, string filename, string contenttype)
            {
                File = file;
                FileName = filename;
                ContentType = contenttype;
            }
        }

        public static HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent, Dictionary<string, object> postParameters)
        {
            string formDataBoundary = "-----------------------------28947758029299";
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;
            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);
            return PostForm(postUrl, userAgent, contentType, formData);
        }

        private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (Settings.Default.UseHTTPProxy == true)
            {
                IWebProxy proxy = WebRequest.GetSystemWebProxy();
                proxy.Credentials = CredentialCache.DefaultCredentials;

                request.Proxy = proxy;
            }

            if (request == null)
            {
                throw new NullReferenceException("la request deve essere di tipo HTTP.");
            }
            // Imposta le proprietà della request
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length; // Dobbiamo contare quanti bite stiamo inviando
            using (Stream requestStream = request.GetRequestStream())
            {
                // Scrittura sullo stream della request
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// restituisce la request multipart a partire dai parametry e dal documento
        /// </summary>
        /// <param name="postParameters"></param>
        /// <param name="boundary"></param>
        /// <returns></returns>
        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            // stream temporaneo in cui scrivere la request multipart
            Stream formDataStream = new System.IO.MemoryStream();
            foreach (var param in postParameters)
            {
                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;
                    // prepara l'header di un form-field di tipo file
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data;name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                    boundary,
                    param.Key,
                    fileToUpload.FileName ?? param.Key,
                    fileToUpload.ContentType ?? "application/octetstream");
                    formDataStream.Write(encoding.GetBytes(header), 0, header.Length);
                    // Scrivi i dati sullo stream, senza passare per una stringa
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                    // aggiunge un "a capo" per permettere di caricare più file
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, 2);
                }
                else
                {
                    // prepara l'header di un form-field normale, contenente una stringa
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data;name=\"{1}\"\r\n\r\n{2}\r\n",
                    boundary,
                    param.Key,
                    param.Value);
                    byte[] thisdata = encoding.GetBytes(postData) ; 
                    formDataStream.Write(thisdata, 0, thisdata.Length);
                }
            }

            // Aggiunge l'ultima parte della request
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, footer.Length);
            // Scarica lo stream preparato in un array di byte
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();
            return formData;
        }
    }
}
