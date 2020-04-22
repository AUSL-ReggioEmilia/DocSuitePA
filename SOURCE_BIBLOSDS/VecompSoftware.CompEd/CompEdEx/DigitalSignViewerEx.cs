using CCypher;
using VecompSoftware.Common;
using System;
using VecompSoftware.MagicNumber;

namespace VecompSoftware.CompEd
{
    public static class DigitalSignViewerEx
    {

        public static string GetContentTypeBuf(this DigitalSignViewer source, byte[] buf)
        {
            enumContentTypeEx docType;
            string pDocExt = null;
            source.GetContentTypeBuf(buf, out docType, out pDocExt);
            if (pDocExt.IsNullOrWhiteSpace())
                throw new InvalidOperationException("Non è stato possibile identificare il contenuto.");
            return pDocExt;
        }

        /// <summary>
        /// Recupera la corretta estensione del file dall'array di byte.
        /// Se non si riesce a identificare l'estensione corretta, prima rimuovo le estensioni ".P7M" e in seguito vado a verificare il nome del file.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="buf">buffer di dati</param>
        /// <param name="filename">nome del file</param>
        /// <returns></returns>
        public static string GetContentTypeBuf(this DigitalSignViewer source, byte[] buf, string filename)
        {
            try
            {
               return GetContentTypeBuf(source, buf);
            }
            catch (Exception) 
            {
                try
                {
                    return ExtensionFileByMagicNumbers.GetExtension(buf).ToString();                   
                }
                catch (Exception)
                {
                    return ExtensionFile.none.ToString();
                }
            }           
        }

        public static string GetContentTypeBuf(this DigitalSignViewer source, IContent contentInfo)
        {
            return source.GetContentTypeBuf(contentInfo.Content);
        }

    }
}
