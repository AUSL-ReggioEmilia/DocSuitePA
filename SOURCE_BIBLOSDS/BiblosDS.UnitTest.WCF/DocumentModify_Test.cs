using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Services;

namespace BiblosDS.UnitTest.WCF
{
    /// <summary>
    /// Summary description for DocumentModify_Test
    /// </summary>
    [TestClass]
    public class DocumentModify_Test
    {
        public DocumentModify_Test()
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

        Document CheckOutDocument;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            if (TestContext.DataRow["IdParentBiblos"] == System.DBNull.Value)
                return;
            
            CheckOutDocument = DocumentService.GetDocument((Guid)TestContext.DataRow["IdDocument"]);
            CheckOutDocument.AttributeValues = AttributeService.GetAttributeValues(CheckOutDocument.IdDocument);
            DocumentService.CheckOut(CheckOutDocument.IdDocument, "UnitTest");
            CheckOutDocument.Content = new DocumentContent();
            CheckOutDocument.Content.Blob = new byte[100];
            for (int i = 0; i < 100; i++)
            {
                CheckOutDocument.Content.Blob[i] = Convert.ToByte(i % 2);
            }
        }
    
        [DataSource("BiblosDSDataSource_Document"), TestMethod]
        public void DocumentServive_CheckIn_Test()
        {
            if (TestContext.DataRow["IdParentBiblos"] == System.DBNull.Value)
                return;
            Guid NewIdModify = Guid.Empty;
            using (ServiceReferenceDocument.DocumentsClient client = new BiblosDS.UnitTest.WCF.ServiceReferenceDocument.DocumentsClient())
            {
                NewIdModify = client.DocumentCheckIn(CheckOutDocument, "UnitTest");
            }
            Assert.IsTrue(NewIdModify != Guid.Empty);
            TestContext.WriteLine(NewIdModify.ToString());
        }
    }
}
