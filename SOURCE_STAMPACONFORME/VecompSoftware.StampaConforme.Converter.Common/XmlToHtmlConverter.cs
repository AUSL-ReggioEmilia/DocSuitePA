using log4net;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using VecompSoftware.Commons.BiblosDS.Objects.Converters;
using VecompSoftware.Commons.BiblosDS.Objects.Enums;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.XML.Converters.Factory;

namespace VecompSoftware.StampaConforme.Converter.Common
{
    public class XmlToHtmlConverter : IConverter
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(XmlToHtmlConverter));
        private readonly string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
        private readonly XmlFactory _xmlFactory;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public XmlToHtmlConverter()
        {
            _xmlFactory = new XmlFactory();
        }
        #endregion

        #region [ Methods ]
        public byte[] Convert(byte[] fileSource, string fileExtension, string extReq, AttachConversionMode mode)
        {
            return Convert(fileSource, _xmlFactory.GetDefaultXsl(), fileExtension, extReq);
        }

        public byte[] Convert(byte[] fileSource, string xsl, string fileExtension, string extReq, bool xslAutomaticDetection = true)
        {
            try
            {
                _logger.DebugFormat("Convert - {0} to .html", fileExtension);
                return ConvertToHtml(fileSource, xsl, fileExtension, xslAutomaticDetection);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }

        private byte[] ConvertToHtml(byte[] sourceXmlContent, string xsl, string fileExtension, bool xslAutomaticDetection)
        {
            if (!IsDocumentProcessable(fileExtension))
            {
                return null;
            }

            string documentXml = GetDocumentXmlStringFromBytes(sourceXmlContent);
            XMLConverterModel converterModel = new XMLConverterModel()
            {
                ModelKind = XMLModelKind.Invalid,
                Xsl = xsl
            };

            if (xslAutomaticDetection)
            {
                converterModel = GetConverterModel(documentXml);
                if (converterModel.ModelKind == XMLModelKind.Invalid)
                {
                    _logger.InfoFormat("ConvertToHtml - document {0} is invalid. Set base style.", fileExtension);
                    converterModel.Xsl = _xmlFactory.GetDefaultXsl();
                }
            }            

            if (string.IsNullOrEmpty(converterModel.Xsl))
            {
                _logger.InfoFormat("ConvertToHtml - XSLT not found for document {0}.", fileExtension);
                return null;
            }

            _logger.InfoFormat("ConvertToHtml - XSLT found, XML type: {0}", converterModel.ModelKind.ToString());
            string htmlResult = CreateHtmlFromXsltTemplate(documentXml, converterModel.Xsl);
            return Encoding.UTF8.GetBytes(htmlResult);
        }

        public bool IsDocumentProcessable(string fileExtension)
        {
            return fileExtension.ToLower().Equals(FileHelper.XML) || fileExtension.ToLower().Equals(FileHelper.P7M);
        }

        private string GetDocumentXmlStringFromBytes(byte[] sourceXmlContent)
        {
            string xml = Encoding.UTF8.GetString(sourceXmlContent);
            if (xml.StartsWith(_byteOrderMarkUtf8, StringComparison.Ordinal))
            {
                xml = xml.Remove(0, _byteOrderMarkUtf8.Length);
            }
            return xml;
        }

        private XMLConverterModel GetConverterModel(string documentXml)
        {
            XMLConverterModel converterModel = _xmlFactory.BuildXmlModel(documentXml);
            return converterModel;
        }

        private string CreateHtmlFromXsltTemplate(string documentXml, string documentXslt)
        {
            XslCompiledTransform transform = new XslCompiledTransform();
            StringWriter results = new StringWriter();
            using (XmlReader reader = XmlReader.Create(new StringReader(documentXslt)))
            {
                transform.Load(reader);
            }

            using (XmlReader reader = XmlReader.Create(new StringReader(documentXml)))
            {
                transform.Transform(reader, null, results);
            }
            return results.ToString();
        }

        public string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        #endregion
    }
}
