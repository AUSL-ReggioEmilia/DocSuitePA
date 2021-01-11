using System;
using System.Globalization;
using System.Text;
using System.Xml;
using System.IO;
using VecompSoftware.JeepService.SignEngine.SignEngineService;

namespace VecompSoftware.JeepService.SignEngine
{
    public class SignEngineFacade
    {
        public string Url {get; set;}
        public string User { get; set; }
        public string Password { get; set; }
        public string CertificateName { get; set; }
        public int InfocamereFormat { get; set; }

        private eSignEngine _service;

        public eSignEngine Service
        {
            get
            {
                if (_service == null)
                {
                    _service = new eSignEngine {Url = Url};
                    if (string.IsNullOrEmpty(Url))
                        throw new ArgumentNullException("Url del servizio mancante.", new Exception());
                }
                return _service;
            }
            set { _service = value; }
        }

        private int? _usedTimeStamp;
        public int UsedTimeStamps
        {
            get
            {
                if (_usedTimeStamp == null)
                {
                    var document = new XmlDocument();
                    var content = Service.GetTimeStampCount("INFOCAMERE", User, Password);
                    var temp = content.Remove(content.IndexOf("raw", 0, StringComparison.Ordinal) + 4, content.IndexOf("raw", content.IndexOf("raw", 0, StringComparison.Ordinal) + 3, StringComparison.Ordinal) - content.IndexOf("raw", 0, StringComparison.Ordinal));
                    document.LoadXml(temp.Replace("<raw>", ""));

                    var xmlAttributeCollection = document.GetElementsByTagName("status")[0].Attributes;
                    if (xmlAttributeCollection != null)
                        _usedTimeStamp = Convert.ToInt16(xmlAttributeCollection["used"].Value);
                }
                if (_usedTimeStamp != null) return _usedTimeStamp.Value;
                return 0;
            }
        }

        private int? _availableTimeStamps;
        public int AvailableTimeStamps
        {
            get
            {
                if (_availableTimeStamps == null)
                {
                    var document = new XmlDocument();
                    string content = Service.GetTimeStampCount("INFOCAMERE", User, Password);
                    string temp = content.Remove(content.IndexOf("raw", 0, StringComparison.Ordinal) + 4, content.IndexOf("raw", content.IndexOf("raw", 0, StringComparison.Ordinal) + 3, StringComparison.Ordinal) - content.IndexOf("raw", 0, StringComparison.Ordinal));
                    document.LoadXml(temp.Replace("<raw>", ""));

                    if (document.DocumentElement != null)
                    {
                        var xmlAttributeCollection = document.DocumentElement.GetElementsByTagName("status")[0].Attributes;
                        if (xmlAttributeCollection != null)
                            _availableTimeStamps = Convert.ToInt16(xmlAttributeCollection["available"].Value);
                    }
                }
                if (_availableTimeStamps != null) return _availableTimeStamps.Value;
                return 0;
            }
        }

        public bool HasAvailableTimeStamps(int warningThreshold)
        {
            if (warningThreshold > 0)
            {
                if (AvailableTimeStamps <= warningThreshold)
                {
                    return false;
                }
            }
            return true;
        }
        
        public SignEngineFacade(string url, string user, string password, string certificateName, int infocamereFormat)
        {
            Url = url;
            User = user;
            Password = password;
            CertificateName = certificateName;
            InfocamereFormat = infocamereFormat;
        }

        public string SignDocument(string source)
        {
            return SignDocument(source, string.Empty);
        }
        
        public string SignDocument(string source, string description)
        {
            if (!File.Exists(source))
                throw new FileNotFoundException("[SignDocument] File di input non trovato.");

            if (string.IsNullOrEmpty(CertificateName))
                throw new ArgumentNullException("Nome del certificato mancante.", new Exception());

            var fileName = Path.GetFileName(source);
            if (fileName != null)
            {
                var encoded = Encoding.ASCII.GetBytes(fileName);
                fileName = Convert.ToBase64String(encoded);
            }
            var blobInput = File.ReadAllBytes(source);
            var input = Convert.ToBase64String(blobInput);
            var output = Service.P7mSoftSign(CertificateName, fileName, DateTime.Now.ToUniversalTime().ToString(CultureInfo.InvariantCulture), description, input);
            var blobOutput = Convert.FromBase64String(output);
            var retval = source + ".p7m";
            File.WriteAllBytes(retval, blobOutput);
            if (!File.Exists(retval))
                throw new FileNotFoundException("[SignDocument] File di output non trovato.");
            return retval;
        }

        public string TimeStampDocument(string source)
        {
            if (!File.Exists(source))
                throw new FileNotFoundException("[TimeStampDocument] File di input non trovato.");
            var fileName = Path.GetFileName(source);
            if (fileName != null)
            {
                byte[] encoded = Encoding.ASCII.GetBytes(fileName);
                fileName = Convert.ToBase64String(encoded);
            }
            var blobInput = File.ReadAllBytes(source);
            var input = Convert.ToBase64String(blobInput);
            var output = Service.P7xTimeStampDocument(InfocamereFormat, fileName, input);
            var blobOutput = Convert.FromBase64String(output);
            var retval = source + ".p7x";
            File.WriteAllBytes(retval, blobOutput);
            if (!File.Exists(retval))
                throw new FileNotFoundException("[TimeStampDocument] File di output non trovato.");

            RefreshTimeStampCounters();

            return retval;
        }

        public void RefreshTimeStampCounters()
        {
            _availableTimeStamps = null;
            _usedTimeStamp = null;
        }

        public void GetExpiryDates(string fileName, string encObj, out SimplyCert firstExpCertificate)
        {
            Service.GetExpiryDates(fileName, encObj, out firstExpCertificate);
        }

    }
}
