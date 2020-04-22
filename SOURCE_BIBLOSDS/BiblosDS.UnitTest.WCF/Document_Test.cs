using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiblosDS.UnitTest.WCF.ServiceReferenceDocument;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects;
using System.Reflection;
using BiblosDS.Library.Common.Services;
using System.IO;

namespace BiblosDS.UnitTest.WCF
{
    /// <summary>
    /// Summary description for Document_Test
    /// </summary>
    [TestClass]
    public class Document_Test
    {
        public Document_Test()
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

        BindingList<DocumentAttribute> attributes;
        [DataSource("BiblosDSDataSource_Archive"), TestInitialize()]
        public void Initialize_GetAttributes()
        {
            //attributes = AttributeService.GetAttributesFromArchive((Guid)TestContext.DataRow["IdArchive"]);
        }

        [DataSource("BiblosDSDataSource_Archive"), TestMethod()]
        public void DocumentServive_GetMetadataStructure_Test()
        {
            BindingList<DocumentAttribute> result;
            using (ServiceReferenceDocument.DocumentsClient client = new BiblosDS.UnitTest.WCF.ServiceReferenceDocument.DocumentsClient())
            {

                result = client.GetMetadataStructure((Guid)TestContext.DataRow["IdArchive"]);
            }
            Assert.IsNotNull(result);
            foreach (var item in result)
            {
                FieldInfo[] fi = item.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var vars in fi)
                {
                    TestContext.WriteLine("{0}={1}", vars.Name, vars.GetValue(item));
                }
                TestContext.WriteLine("---------------------------------------------");
            }
        }

        [DataSource("BiblosDSDataSource_Document"), TestMethod()]
        public void DocumentServive_GetDocumentContent_Test()
        {
            if (TestContext.DataRow["IdParentBiblos"] == System.DBNull.Value)
                return;
            Guid IdDocument = (Guid)TestContext.DataRow["IdDocument"];
            DocumentContent result;
            using (ServiceReferenceDocument.DocumentsClient client = new BiblosDS.UnitTest.WCF.ServiceReferenceDocument.DocumentsClient())
            {
                result = client.GetDocumentContentById(IdDocument);
            }
            Assert.IsNotNull(result);
        }

        [DataSource("BiblosDSDataSource_Document"), TestMethod()]
        public void DocumentServive_GetDocumentInfo_Test()
        {
            if (TestContext.DataRow["IdParentBiblos"] == System.DBNull.Value)
                return;
            Guid IdDocument = (Guid)TestContext.DataRow["IdDocument"];
            BindingList<Document> result;
            using (ServiceReferenceDocument.DocumentsClient client = new BiblosDS.UnitTest.WCF.ServiceReferenceDocument.DocumentsClient())
            {
                result = client.GetChainInfoById(IdDocument);
            }
            Assert.IsNotNull(result);
            foreach (var item in result)
            {
                FieldInfo[] fi = item.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var vars in fi)
                {
                    TestContext.WriteLine("{0}={1}", vars.Name, vars.GetValue(item));
                }
                TestContext.WriteLine("---------------------------------------------");
            }
        }

        [DataSource("BiblosDSDataSource_Document"), TestMethod()]
        public void DocumentServive_CheckMetaData_Test()
        {
            if (TestContext.DataRow["IdParentBiblos"] == System.DBNull.Value)
                return;
            Guid IdDocument = (Guid)TestContext.DataRow["IdDocument"];
            Document docuemnt = DocumentService.GetDocument(IdDocument);
            docuemnt.AttributeValues = AttributeService.GetAttributeValues(IdDocument);
            bool result;
            using (ServiceReferenceDocument.DocumentsClient client = new BiblosDS.UnitTest.WCF.ServiceReferenceDocument.DocumentsClient())
            {
                result = client.CheckMetaData(docuemnt);
            }
            Assert.IsTrue(result);
        }

        [DataSource("BiblosDSDataSource_Document"), TestMethod()]
        public void DocumentServive_GetDocumentLinked_Test()
        {
            if (TestContext.DataRow["IdParentBiblos"] == System.DBNull.Value || TestContext.DataRow["IdDocumentLink"] == System.DBNull.Value)
                return;
            Guid Id = (Guid)TestContext.DataRow["IdDocument"];
            DocumentContent result;
            using (ServiceReferenceDocument.DocumentsClient client = new BiblosDS.UnitTest.WCF.ServiceReferenceDocument.DocumentsClient())
            {
                result = client.GetDocumentContentById(Id);
            }
            Assert.IsNotNull(result);
        }

        [DataSource("BiblosDSDataSource_Document"), TestMethod()]
        public void DocumentServive_GetChainInfo_Test()
        {
            if (TestContext.DataRow["IdParentBiblos"] != System.DBNull.Value)
                return;
            Guid ParentId = (Guid)TestContext.DataRow["IdDocument"];

            BindingList<Document> result;
            using (ServiceReferenceDocument.DocumentsClient client = new BiblosDS.UnitTest.WCF.ServiceReferenceDocument.DocumentsClient())
            {

                result = client.GetChainInfoById(ParentId);
            }
            Assert.IsNotNull(result);
            foreach (var item in result)
            {
                FieldInfo[] fi = item.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var vars in fi)
                {
                    TestContext.WriteLine("{0}={1}", vars.Name, vars.GetValue(item));
                }
                TestContext.WriteLine("---------------------------------------------");
            }
        }

        [DataSource("BiblosDSDataSource_Document"), TestMethod()]
        public void DocumentServive_GetChainInfoDetails_Version_Test()
        {
            TestContext.WriteLine(TestContext.DataRow["IdParentBiblos"].ToString());
            if (string.IsNullOrEmpty(TestContext.DataRow["IdParentBiblos"].ToString()))
            {
                TestContext.WriteLine("------------Parent-----------------------");
                Guid ParentId = (Guid)TestContext.DataRow["IdDocument"];
                BindingList<Document> result;
                TestContext.WriteLine("------------Call-----------------------");
                using (ServiceReferenceDocument.DocumentsClient client = new BiblosDS.UnitTest.WCF.ServiceReferenceDocument.DocumentsClient())
                {
                    client.Open();
                    TestContext.WriteLine("------------Wait-----------------------" + client.State);
                    result = client.GetChainInfoDetails(ParentId, true, (decimal)1.01, true);
                    TestContext.WriteLine("------------Ok-----------------------");
                }
                Assert.IsNotNull(result);
                foreach (var item in result)
                {
                    FieldInfo[] fi = item.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var vars in fi)
                    {
                        TestContext.WriteLine("{0}={1}", vars.Name, vars.GetValue(item));
                    }
                    TestContext.WriteLine("---------------------------------------------");
                }
            }
        }

        [DataSource("BiblosDSDataSource_Document"), TestMethod()]
        public void DocumentServive_GetChainInfoDetails_NoVersion_Test()
        {
            if (TestContext.DataRow["IdParentBiblos"] != System.DBNull.Value)
                return;
            Guid ParentId = (Guid)TestContext.DataRow["IdDocument"];

            BindingList<Document> result;
            using (ServiceReferenceDocument.DocumentsClient client = new BiblosDS.UnitTest.WCF.ServiceReferenceDocument.DocumentsClient())
            {

                result = client.GetChainInfoDetails(ParentId, null, null, null);
            }
            Assert.IsNotNull(result);
            foreach (var item in result)
            {
                FieldInfo[] fi = item.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var vars in fi)
                {
                    TestContext.WriteLine("{0}={1}", vars.Name, vars.GetValue(item));
                }
                TestContext.WriteLine("---------------------------------------------");
            }
        }

        [DataSource("BiblosDSDataSource_Document"), TestMethod()]
        public void DocumentServive_DocumentAttributeCheckIn_Test()
        {
            if (TestContext.DataRow["IdParentBiblos"] == System.DBNull.Value || TestContext.DataRow["IdDocumentLink"] != System.DBNull.Value)
                return;
            Document editDocument = DocumentService.GetDocument(new Guid(TestContext.DataRow["IdDocument"].ToString()));
            editDocument.AttributeValues = AttributeService.GetAttributeValues(editDocument.IdDocument);
            if (editDocument.AttributeValues.Count() <= 0)
                return;
            foreach (var item in editDocument.AttributeValues)
            {
                switch (item.Attribute.AttributeType.ToLower())
                {
                    case "system.datetime":
                        break;
                    case "system.string":
                        item.Value = "2009_edit";
                        break;
                    default:
                        item.Value = "2009";
                        break;
                }
            }
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
            using (ServiceReferenceDocument.DocumentsClient client = new BiblosDS.UnitTest.WCF.ServiceReferenceDocument.DocumentsClient())
            {
                if (!editDocument.IsCheckOut)
                    client.DocumentCheckOut(editDocument.IdDocument, true, System.Security.Principal.WindowsIdentity.GetCurrent().Name);
                client.DocumentAttributeCheckIn(editDocument, System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            }
        }

        [DataSource("BiblosDSDataSource_Archive"), TestMethod()]
        public void DocumentServive_InsertDocumentChain_Test()
        {
            Document insertedDocument = new Document();
            Document newDocument = new Document();
            newDocument.Archive = new DocumentArchive();
            newDocument.Archive.IdArchive = (Guid)TestContext.DataRow["IdArchive"];
            newDocument.Archive.Name = (string)TestContext.DataRow["Name"];

            newDocument.AttributeValues = new System.ComponentModel.BindingList<DocumentAttributeValue>();
            ////Add the DocumentAttributeValue
            DocumentAttributeValue value = null;
            foreach (var item in attributes)
            {
                value = new DocumentAttributeValue();
                value.Attribute = item;
                switch (item.AttributeType.ToLower())
                {
                    case "system.datetime":
                        value.Value = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case "system.string":
                        value.Value = "2009";
                        break;
                    default:
                        value.Value = "2009";
                        break;
                }
                newDocument.AttributeValues.Add(value);
            }

            using (ServiceReferenceDocument.DocumentsClient client = new BiblosDS.UnitTest.WCF.ServiceReferenceDocument.DocumentsClient())
            {
                insertedDocument = client.InsertDocumentChain(newDocument);
            }
            Assert.IsNotNull(insertedDocument);
            Assert.IsTrue(insertedDocument.IdDocument != Guid.Empty);
        }

        [DataSource("BiblosDSDataSource_Document"), TestMethod()]
        public void DocumentServive_AddDocumentToChain_Test()
        {
            if (TestContext.DataRow["IdParentBiblos"] != System.DBNull.Value)
                return;
            Guid ParentId = (Guid)TestContext.DataRow["IdDocument"];
            Document newDocument = new Document();
            newDocument.Archive = new DocumentArchive();
            newDocument.Archive.IdArchive = (Guid)TestContext.DataRow["IdArchive"];
            //
            newDocument.Name = "Test.pdf";
            newDocument.Content = new DocumentContent();
            newDocument.Content.Blob = new byte[100];
            for (int i = 0; i < 100; i++)
            {
                newDocument.Content.Blob[i] = Convert.ToByte(i % 2);
            }
            //
            newDocument.AttributeValues = new System.ComponentModel.BindingList<DocumentAttributeValue>();
            ////Add the DocumentAttributeValue
            DocumentAttributeValue value = null;
            foreach (var item in attributes)
            {
                value = new DocumentAttributeValue();
                value.Attribute = item;
                switch (item.AttributeType.ToLower())
                {
                    case "system.datetime":
                        value.Value = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case "system.string":
                        value.Value = "2009";
                        break;
                    default:
                        value.Value = "2009";
                        break;
                }
                newDocument.AttributeValues.Add(value);
            }

            using (ServiceReferenceDocument.DocumentsClient client = new BiblosDS.UnitTest.WCF.ServiceReferenceDocument.DocumentsClient())
            {

                client.AddDocumentToChain(newDocument, ParentId, Library.Common.Enums.DocumentContentFormat.Binary);
            }
        }

        [TestMethod]
        public void DocumentService_AddDocumentToChainWithThumbnail()
        {
            var toAdd = new Document
            {
                Archive = ArchiveService.GetArchive(new Guid("AC92DD96-7354-4061-ACD6-F4D484C81C85")),
                Storage = StorageService.GetStorage(new Guid("ACE62B97-F1EE-4E07-A9C2-BC96490B9972")),
                StorageArea = StorageService.GetStorageArea(new Guid("89B95D35-3A6A-41CD-A15C-FB11937402BD")),
                Status = new Status(Library.Common.Enums.DocumentStatus.InStorage),
                IsVisible = true,
                ChainOrder = 1,
                Version = 0.1m,
                IsCheckOut = false,
                Name = "TEST",
                Content = new DocumentContent(File.ReadAllBytes(@"C:\TEST.doc")),
                AttributeValues = new BindingList<DocumentAttributeValue>(),
            };

            toAdd.AttributeValues.Add(new DocumentAttributeValue { Value = "1", Attribute = new DocumentAttribute { IdAttribute = new Guid("F12F5D05-CC3D-49F8-AFAE-666617829470"), Name = "Tipologia" } });
            toAdd.AttributeValues.Add(new DocumentAttributeValue { Value = (long)1, Attribute = new DocumentAttribute { IdAttribute = new Guid("0A8E76BD-535C-4780-ABE6-9C94C9EB4454"), Name = "ProgressivoDocumento" } });
            toAdd.AttributeValues.Add(new DocumentAttributeValue { Value = "1", Attribute = new DocumentAttribute { IdAttribute = new Guid("31AF6E56-8227-40E9-A83D-AB70554CE8DB"), Name = "NomeArchivio" } });
            //toAdd.AttributeValues.Add(new DocumentAttributeValue { Value = "1", Attribute = new DocumentAttribute { IdAttribute = new Guid("05C957E2-980D-4EFF-BC01-CD1D6921EC67"), Name = "" } });
            toAdd.AttributeValues.Add(new DocumentAttributeValue { Value = (long)1, Attribute = new DocumentAttribute { IdAttribute = new Guid("395574A0-A0EB-440D-B865-D090D69A8546"), Name = "idBiblos" } });
            toAdd.AttributeValues.Add(new DocumentAttributeValue { Value = DateTime.Now, Attribute = new DocumentAttribute { IdAttribute = new Guid("31AF6E56-8227-40E9-A83D-AB70554CE8DB"), Name = "DataInserimentoDocumento" } });

            //using (var svc = new DocumentsClient("ServiceDocument"))
            //{
            //    var added = svc.AddDocumentToChain(toAdd, null);
            //}
            using (var svc = new ServiceReferenceStorage.ServiceDocumentStorageClient("ServiceDocumentStorage"))
            {
                svc.AddDocument(DocumentService.GetDocument(new Guid("BC25A1D7-C0D7-4094-98F8-007BBD28EDA8")));
            }
        }
    }
}
