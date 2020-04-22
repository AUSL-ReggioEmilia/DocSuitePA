using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using BiblosDS.Library.Common.Objects;

namespace BiblosDS.UnitTest.WCF
{
    [TestClass]
    public class ArchiveCertificateTest
    {
        [TestMethod]
        public void AddCriptedDocument()
        {
            Document document = new Document();
            document.Archive = new DocumentArchive(new Guid("FA27546C-D672-41A6-AC2F-AF9067CA9E9B"));
            document.Content = new DocumentContent();
            document.AttributeValues = new System.ComponentModel.BindingList<DocumentAttributeValue> { new DocumentAttributeValue { Value = DateTime.Now, Attribute = new DocumentAttribute() { Name = "DataDocumento", IdAttribute = new Guid("A76560A5-2A61-4BD6-9C76-FD19FDD88DEA") } } };
            document.Content.Blob = File.ReadAllBytes(@"C:\Lavori\Docs\BiblosDS\closefile.pdf");
            document.Name = "AN.Tutto In Ordine  - commerce_tuttoinordine_it_2.1.pdf";
            ServiceReferenceDocument.DocumentsClient client = new ServiceReferenceDocument.DocumentsClient("ServiceDocument");
            client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 2, 0); // 2 minuti 
            client.AddDocumentToChainPDFCrypted(document, null, Library.Common.Enums.DocumentContentFormat.Binary);
        }

        [TestMethod]
        public void GetCertificate()
        {
            ServiceReferenceAdmin.AdministrationClient client = new ServiceReferenceAdmin.AdministrationClient();
            var cert = client.GetArchiveCertificate(new Guid("FA27546C-D672-41A6-AC2F-AF9067CA9E9B"));

            File.WriteAllBytes(@"c:\lavori\" + "_"+cert.FileName, cert.CertificateBlob);

        }

        [TestMethod]
        public void CreateCertificate()
        {
            ServiceReferenceTIOCA.CertRequest thisRequest = new ServiceReferenceTIOCA.CertRequest();
            thisRequest.UserName = "PincoPalla3"; // diplayname 
            thisRequest.Email = "PincoPalla3@vecompsoftware.it"; // live ID
            thisRequest.Pin = "ABCDE"; // Password del certificato 

            ServiceReferenceTIOCA.ServiceTIOCAClient thisClient = new ServiceReferenceTIOCA.ServiceTIOCAClient();
            thisClient.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 2, 0); // 2 minuti 
            ServiceReferenceTIOCA.CertResponse thisResponse = thisClient.RequestCertificate(thisRequest);
            thisClient.Close();

            File.WriteAllBytes(@"c:\lavori\" + thisResponse.FileName, thisResponse.Cert);
            ServiceReferenceAdmin.AdministrationClient client = new ServiceReferenceAdmin.AdministrationClient();
            client.AddArchiveCertificate(new Guid("FA27546C-D672-41A6-AC2F-AF9067CA9E9B"), thisRequest.UserName, thisRequest.Pin, thisResponse.FileName, thisResponse.Cert);           

        }
    }
}
