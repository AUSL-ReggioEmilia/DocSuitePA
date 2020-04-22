using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.SPID.Clients
{
    public class PDFGeneratorClient
    {
        #region [ Fields ]
        private readonly string _url;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public PDFGeneratorClient(string url)
        {
            _url = url;
        }
        #endregion

        #region [ Methods ]
        public async Task<byte[]> GeneratePDFAsync(DocumentGeneratorModel generatorModel, Guid idTemplate)
        {
            using (PDFGeneratorService.PDFGeneratorClient svc = CreateServiceClient())
            {
                if (idTemplate.Equals(Guid.Empty))
                {
                    throw new ArgumentException("Parameter idTemplate is empty", nameof(idTemplate));
                }

                string xsd = await svc.GetXSDSchemaAsync(idTemplate);
                if (string.IsNullOrEmpty(xsd))
                {
                    throw new Exception(string.Concat("Template '", idTemplate, "' not found"));
                }

                string generatedXml = FillXmlFromXsd(xsd, generatorModel);
                if (string.IsNullOrEmpty(generatedXml))
                {
                    throw new Exception(string.Concat("Error on generate XML from template '", idTemplate, "'"));
                }

                return await svc.GenerateDocumentPDFAAsync(generatedXml, idTemplate);
            }
        }

        private string FillXmlFromXsd(string xsd, DocumentGeneratorModel generatorModel)
        {
            PDFGeneratorXMLBuilder builder = new PDFGeneratorXMLBuilder(xsd, generatorModel.DocumentGeneratorParameters);
            XmlDocument buildedXml = new XmlDocument();
            buildedXml.LoadXml(builder.GetGeneratedXML());
            return buildedXml.DocumentElement.InnerXml;
        }

        private PDFGeneratorService.PDFGeneratorClient CreateServiceClient()
        {
            BasicHttpBinding binding = new BasicHttpBinding { Name = "PDFGeneratorServicePort", MaxReceivedMessageSize = 2147483647, MaxBufferSize = 2147483647 };
            EndpointAddress endpoint = new EndpointAddress(_url);
            return new PDFGeneratorService.PDFGeneratorClient(binding, endpoint);
        }
        #endregion
    }
}
