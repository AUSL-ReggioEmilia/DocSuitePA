using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using BiblosDS.UnitTest.PublicWCF.ServiceReferenceDocument;
using System;

namespace BiblosDS.UnitTest.PublicWCF
{
    /// <summary>
    /// Summary description for DocumentsTest
    /// </summary>
    [TestClass]
    public class DocumentsTest
    {
        public DocumentsTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void SetVisibleChain()
        {
            DocumentsClient client = new DocumentsClient();
            client.SetVisibleChain(new System.Guid("fc6217c6-6eae-4dcc-91eb-0010c7e489fa"), false);
        }

        [TestMethod]
        public void GetDocumentSigned()
        {
            DocumentsClient client = new DocumentsClient();
            var res = client.GetDocumentSigned(new System.Guid("6d969e87-2f35-4517-9ef3-851cfcdaf300"));
        }

        [TestMethod]
        public void GetDocumentId()
        {
            DocumentsClient client = new DocumentsClient();
            var document = client.GetDocumentId("LegalGianni", 4199);
            Assert.IsTrue(document != System.Guid.Empty);
        }

        [TestMethod]
        public void AddDocument()
        {
            BindingList<AttributeValue> attributeValues = new BindingList<AttributeValue>();
            AttributeValue attributeValue;
            DocumentsClient client = new DocumentsClient();
            try
            {

                BindingList<Archive> archives = client.GetArchives();
                BindingList<BiblosDS.UnitTest.PublicWCF.ServiceReferenceDocument.Attribute> attributes = client.GetAttributesDefinition(archives[0].Name);
                foreach (var item in attributes)
                {

                    attributeValue = new AttributeValue();
                    attributeValue.Attribute = item;
                    switch (attributeValue.Attribute.AttributeType)
                    {
                        case "System.String":
                            attributeValue.Value = "Valore testuale";
                            break;
                        case "System.Int64":
                            attributeValue.Value = 1;
                            break;
                        case "System.Double":
                            attributeValue.Value = 1.1;
                            break;
                        case "System.DateTime":
                            attributeValue.Value = System.DateTime.Now;
                            break;
                        default:
                            break;
                    }
                    attributeValues.Add(attributeValue);
                }
                Document document = new Document();
                document.Name = "test.pdf";
                document.Archive = archives[0];
                document.AttributeValues = attributeValues;
                document.Content = new Content { Blob = new byte[3] { 1, 1, 1 } };
                client.AddDocumentToChain(document, null, ContentFormat.Bynary);

            }
            catch (System.Exception ex)
            {
                TestContext.WriteLine(ex.ToString());
                client.Close();
            }
        }

        [TestMethod]
        public void GetArchiveStatistics()
        {
            using (var client = new DocumentsClient())
            {
                var archs = client.GetArchives().OrderBy(x => x.IdArchive);

                foreach (var a in archs)
                {
                    var retval = client.GetArchiveStatistics(a.IdArchive);
                    Assert.IsFalse(retval == null);
                    TestContext.WriteLine("ARCHIVE " + a.Name + "{0}-----------------{0}IdArchiveDocumentsCount:\t{1}{0}DocumentsVolume:\t{2}{0}PreservationsCount:\t{3}{0}ForwardedDevicesCount:\t{4}{0}", Environment.NewLine, retval.DocumentsCount, retval.DocumentsVolume, retval.PreservationsCount, retval.ForwardedDevicesCount);
                }
            }
        }

        [TestMethod]
        public void GetDocumentsInfoByIdPaged()
        {
            var documenti = new List<Document>();

            using (var client = new DocumentsClient())
            {
                var lista = new BindingList<Guid>(new[]
                {
                    new Guid("D4D7A0DC-5CF2-4813-A1D7-0006B53C7799"),
                    new Guid("30FC674A-8DC6-4357-A849-0006D9316EBD"),
                    new Guid("0AFE0A2C-BCDF-4377-908B-00070F6A3353"),
                    new Guid("6BD3DF20-EBB2-4F10-BAF2-000A72A06185"),
                    new Guid("F5745CCD-C29E-4EA4-97FC-000DDD8AECF7"),
                    new Guid("FC6217C6-6EAE-4DCC-91EB-0010C7E489FA"),
                    new Guid("5A193C58-4A5B-4B18-8548-0012DCD22907"),
                    new Guid("7191DF96-E86E-4F67-965B-0013A6A3FB1A"),
                    new Guid("B534A991-2EA1-43B8-8BD3-0013EEF4E1F3"),
                    new Guid("42DB77F9-8D5C-4CDE-B0CD-00143272FDF0"),
                    new Guid("A9022BE0-48CB-4783-811A-00171752CE22"),
                    new Guid("8D7B5350-F5B7-47EE-9760-0017876CA47D"),
                    new Guid("51F11087-0EA2-4ED7-BB67-0019217DEC53"),
                    new Guid("ABE70CEF-BDC2-4BEE-8535-001A031362CA"),
                    new Guid("324063A6-E776-4CBF-9FD5-001F92CA2890"),
                    new Guid("DC2254C7-98F9-4B2D-9682-00238624B290"),
                    new Guid("DB5AC79E-D7E7-4FA2-8F14-002386699846"),
                    new Guid("3F16B142-A20E-4AF9-B94B-0025E51CBEFC"),
                    new Guid("551E1C1D-7552-4E9D-AB32-0029C3B9FFAB"),
                    new Guid("FE5139BF-40E8-414E-8721-002DA122D8D1"),
                    new Guid("A8348A3B-6C33-43F1-9BE3-002EA2946585"),
                    new Guid("6472F72A-7C1A-402D-90E3-0030D89BC23F"),
                    new Guid("71089045-50A7-469E-A1A5-0032AD45D1BA"),
                    new Guid("203F85F0-59AD-4E0B-9BBA-0032BDBEEF95"),
                    new Guid("1C39BA04-5A55-481B-AD34-0034FFD92592"),
                    new Guid("331A3FE0-8BB6-4249-A6C5-0036AD774D1C"),
                    new Guid("BB5C3979-E52A-4F45-9271-0036DFB42088"),
                    new Guid("5C01B667-E98D-4572-ABB8-0039005FCA3F"),
                    new Guid("A6A71401-1551-48F2-BCCB-004021A756A6"),
                    new Guid("70F1A89F-AB33-44D3-AA04-00423A19296A"),
                });

                //var ret = client.GetDocumentsInfoByIdPaged(lista, 0, -1);

                //TestContext.WriteLine("{0} -> {1}", ret.TotalRecords, ret.Documents.Count);

                //ret = client.GetDocumentsInfoByIdPaged(lista, 0, 1);

                //TestContext.WriteLine("{0} -> {1}", ret.TotalRecords, ret.Documents.Count);

                //ret = client.GetDocumentsInfoByIdPaged(lista, 1, 1); //Dovrebbe prendere solo il secondo record.

                //TestContext.WriteLine("{0} -> {1} , {2}", ret.Documents.Count, lista[1], ret.Documents.Single().IdDocument);

                int skip = 0, iterazione = 1;
                const int TAKE = 5;

                var ret = client.GetDocumentsInfoByIdPaged(lista, skip, TAKE);

                if (ret.HasErros)
                    throw new Exception(ret.Error.Message);

                documenti.AddRange(ret.Documents);

                TestContext.WriteLine("{0}°: {1} -> {2}, skip = {3} take = {4}", iterazione++, documenti.Count, ret.TotalRecords, skip, TAKE);

                for (skip += TAKE; skip < ret.TotalRecords; skip += TAKE)
                {
                    ret = client.GetDocumentsInfoByIdPaged(lista, skip, TAKE);

                    if (ret.HasErros)
                        throw new Exception(ret.Error.Message);

                    documenti.AddRange(ret.Documents);

                    TestContext.WriteLine("{0}°: {1} -> {2}, skip = {3} take = {4}", iterazione++, documenti.Count, ret.TotalRecords, skip, TAKE);
                }
            }

            foreach(var doc in documenti)
            {
                TestContext.WriteLine("{0}", doc.IdDocument);
            }
        }
    }
}
