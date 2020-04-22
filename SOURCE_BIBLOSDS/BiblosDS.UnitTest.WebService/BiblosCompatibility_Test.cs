using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiblosDS.Library.Common.Objects;
using System.ComponentModel;
using BiblosDS.Library.Common.Services;
using BiblosDS.UnitTest.WebService.WebReferenceCompatibility;
using System.Configuration;
using System.IO;
using BiblosDS.Library.Common.Utility;

namespace BiblosDS.UnitTest.WebService
{
    /// <summary>
    /// Summary description for BiblosCompatibility_Test
    /// </summary>
    [TestClass]
    public class BiblosCompatibility_Test
    {
        Random rnd;
        public BiblosCompatibility_Test()
        {
            rnd = new Random();
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

//        [DataSource("BiblosDSDataSource_Document"), TestMethod()]
        public void BiblosCompatibility_GetChainInfo_Test()
        {
            WebReferenceCompatibility.BiblosWS2010 ws = new BiblosDS.UnitTest.WebService.WebReferenceCompatibility.BiblosWS2010();
            string xml = ws.GetChainInfo("BIBLAUSL", "BIBLAUSL", 756);
            TestContext.WriteLine(xml);
        }

        [TestMethod()]
        public void BiblosCompatibility_GetChainInfo_NoValidIdBiblos_Test()
        {
            WebReferenceCompatibility.BiblosWS2010 ws = new BiblosDS.UnitTest.WebService.WebReferenceCompatibility.BiblosWS2010();
            string xml = ws.GetChainInfo("Test", "Test", int.MaxValue -1);
            TestContext.WriteLine(xml);
        }

        [DataSource("BiblosDSDataSource_Document"), TestMethod()]
        public void BiblosCompatibility_GetDocumentHeaded_Test()
        {
            try
            {
                if (TestContext.DataRow["IdParentBiblos"] != System.DBNull.Value)
                    return;
                BindingList<Document> documentChain = DocumentService.GetChainDocuments((Guid)TestContext.DataRow["IdDocument"]);
                WebReferenceCompatibility.BiblosWS2010 ws = new BiblosDS.UnitTest.WebService.WebReferenceCompatibility.BiblosWS2010();
                stDoc doc = ws.GetDocumentHeaded("Test", "Test", (int)TestContext.DataRow["IdBiblos"], documentChain.Count() - 1, string.Empty);
                Assert.IsNotNull(doc.Blob);
            }
            catch (Exception)
            {
                
                throw;
            }            
        }

        [DataSource("BiblosDSDataSource_Document"), TestMethod()]
        public void BiblosCompatibility_GetDocumentHeadedEx_Test()
        {
            if (TestContext.DataRow["IdParentBiblos"] != System.DBNull.Value)
                return;
            BindingList<Document> documentChain = DocumentService.GetChainDocuments((Guid)TestContext.DataRow["IdDocument"]);
            WebReferenceCompatibility.BiblosWS2010 ws = new BiblosDS.UnitTest.WebService.WebReferenceCompatibility.BiblosWS2010();
            stDoc doc = ws.GetDocumentHeadedEx("Test", "Test", (int)TestContext.DataRow["IdBiblos"], documentChain.Count() - 1, ".doc", string.Empty);
            Assert.IsNotNull(doc.Blob);
        }

        [DataSource("BiblosDSDataSource_Document"), TestMethod()]
        public void BiblosCompatibility_GetDocument_Test()
        {
            if (TestContext.DataRow["IdParentBiblos"] != System.DBNull.Value)
                return;
            BindingList<Document> documentChain = DocumentService.GetChainDocuments((Guid)TestContext.DataRow["IdDocument"]);
            WebReferenceCompatibility.BiblosWS2010 ws = new BiblosDS.UnitTest.WebService.WebReferenceCompatibility.BiblosWS2010();
            stDoc doc = ws.GetDocument("Test", "Test", (int)TestContext.DataRow["IdBiblos"], documentChain.Count() - 1);
            Assert.IsNotNull(doc.Blob);
        }

        [DataSource("BiblosDSDataSource_Document"), TestMethod()]
        public void BiblosCompatibility_GetDocumentDirect_Test()
        {
            if (TestContext.DataRow["IdParentBiblos"] != System.DBNull.Value)
                return;
            BindingList<Document> documentChain = DocumentService.GetChainDocuments((Guid)TestContext.DataRow["IdDocument"]);
            WebReferenceCompatibility.BiblosWS2010 ws = new BiblosDS.UnitTest.WebService.WebReferenceCompatibility.BiblosWS2010();
            stDoc doc = ws.GetDocumentDirect("Test", "Test", (int)TestContext.DataRow["IdBiblos"], documentChain.Count() - 1);
            Assert.IsNotNull(doc.Blob);
        }

        [DataSource("BiblosDSDataSource_Document"), TestMethod()]
        public void BiblosCompatibility_AddDocument_Test()
        {
            if (TestContext.DataRow["IdParentBiblos"] == System.DBNull.Value)
                return;
            stDoc doc = new stDoc();
            byte[] blob = new byte[100];
            for (int i = 0; i < 100; i++)
            {
                blob[i] = Convert.ToByte(i % 2);
            }
            doc.Blob = UtilityService.GetStringFromBob(blob);
            BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(
                            new Guid(ConfigurationManager.AppSettings["IdArchiveDefault"].ToString()));
            StringBuilder docDI = new StringBuilder();
            docDI.Append("<?xml version=\"1.0\" ?>");
            docDI.AppendFormat(@"<Object Enum=""{0}""
DataCreazione=""{1:yyyy/MM/dd}""
IdConservazione=""{2}""
Size=""{3}""
Background=""""
Visible=""{4}""
Type=""{5}"" >{6}",
               1,
               DateTime.Now,
               false,
               blob.Length,
               true,
               Path.GetExtension("Test.pdf"),
               Environment.NewLine);
            ////Add the DocumentAttributeValue  
            //Clicle attributes
            string value = string.Empty;
            foreach (var item in attributes)
            {
                switch (item.AttributeType.ToLower())
                {
                    case "system.datetime":
                        value = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        break;                    
                    default:
                        value = rnd.Next().ToString();
                        break;
                }
                docDI.AppendFormat(@"<Attribute Name=""{0}"">{1}</Attribute>{2}",
                    item.Name,
                    value,
                    Environment.NewLine);
            }
            docDI.AppendFormat("</Object>{0}", Environment.NewLine);
            //
            doc.XmlValues = docDI.ToString();
            doc.FileExtension = "Test.pdf";
            WebReferenceCompatibility.BiblosWS2010 ws = new BiblosDS.UnitTest.WebService.WebReferenceCompatibility.BiblosWS2010();
            int result = ws.AddDocument(attributes.First().Archive.Name, attributes.First().Archive.Name, (int)TestContext.DataRow["IdBiblos"], doc);
            Assert.IsTrue(result > 0);            
        }

        [TestMethod()]
        public void BiblosCompatibility_AddDocumentChain_Test()
        {         
            stDoc doc = new stDoc();
            byte[] blob = new byte[100];
            for (int i = 0; i < 100; i++)
            {
                blob[i] = Convert.ToByte(i % 2);
            }
            doc.Blob = UtilityService.GetStringFromBob(blob);
            BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(
                            new Guid(ConfigurationManager.AppSettings["IdArchiveDefault"].ToString()));
            StringBuilder docDI = new StringBuilder();
            docDI.Append("<?xml version=\"1.0\" ?>");                      
            docDI.AppendFormat(@"<Object Enum=""{0}""
DataCreazione=""{1:yyyy/MM/dd}""
IdConservazione=""{2}""
Size=""{3}""
Background=""""
Visible=""{4}""
Type=""{5}"" >{6}",
               1,
               DateTime.Now,
               false,
               blob.Length,
               true,
               Path.GetExtension("Test.pdf"),
               Environment.NewLine);
            ////Add the DocumentAttributeValue  
            //Clicle attributes
            string value = string.Empty;
            foreach (var item in attributes)
            {
                switch (item.AttributeType.ToLower())
                {
                    case "system.datetime":
                        value = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case "system.string":
                        value = "2009";
                        break;
                    default:
                        value = "2009";
                        break;
                }
                docDI.AppendFormat(@"<Attribute Name=""{0}"">{1}</Attribute>{2}",
                    item.Name,
                    value,
                    Environment.NewLine);
            }
            docDI.AppendFormat("</Object>{0}", Environment.NewLine);            
            //
            doc.XmlValues = docDI.ToString();
            WebReferenceCompatibility.BiblosWS2010 ws = new BiblosDS.UnitTest.WebService.WebReferenceCompatibility.BiblosWS2010();
            int result = ws.AddDocument("Test", "Test", 0, doc);
            Assert.IsTrue(result > 0);
        }

        [TestMethod()]
        public void BiblosCompatibility_CheckMetadata_Test()
        {
            DocumentArchive archive;
            if ((archive = ArchiveService.GetArchiveByName("Test")) == null)
                archive = ArchiveService.GetArchive(new Guid(ConfigurationManager.AppSettings["IdArchiveDefault"].ToString()));
            
            BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(
                            archive.IdArchive);
            StringBuilder docDI = new StringBuilder();
            docDI.Append("<?xml version=\"1.0\" ?>");
            docDI.AppendFormat(@"<Object Enum=""{0}""
DataCreazione=""{1:yyyy/MM/dd}""
IdConservazione=""{2}""
Size=""{3}""
Background=""""
Visible=""{4}""
Type=""{5}"" >{6}",
               1,
               DateTime.Now,
               false,
               0,
               true,
               Path.GetExtension("Test.pdf"),
               Environment.NewLine);
            ////Add the DocumentAttributeValue  
            //Clicle attributes
            string value = string.Empty;
            foreach (var item in attributes)
            {
                switch (item.AttributeType.ToLower())
                {
                    case "system.datetime":
                        value = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case "system.string":
                        value = "2009";
                        break;
                    default:
                        value = "2009";
                        break;
                }
                docDI.AppendFormat(@"<Attribute Name=""{0}"">{1}</Attribute>{2}",
                    item.Name,
                    value,
                    Environment.NewLine);
            }
            docDI.AppendFormat("</Object>{0}", Environment.NewLine);
            //            
            WebReferenceCompatibility.BiblosWS2010 ws = new BiblosDS.UnitTest.WebService.WebReferenceCompatibility.BiblosWS2010();
            string result = ws.CheckMetaData("Test", "Test", docDI.ToString());
            Assert.IsTrue(result.Contains("OK"));
        }
    }
}
