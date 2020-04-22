using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects;

namespace BiblosDS.UnitTest.WCF
{
    /// <summary>
    /// Summary description for Storage_Test
    /// </summary>
    [TestClass]
    public class Storage_Test
    {
        public Storage_Test()
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
        [DataSource(
            "System.Data.SqlClient",
            "Data Source=localhost\\SQLEXPRESS;Initial Catalog=BiblosDS2009;Integrated Security=True",
            "Archive",
            DataAccessMethod.Sequential),
        TestInitialize()]
        public void Initialize_GetAttributes()
        {

            attributes = new BindingList<DocumentAttribute>();
            //
            BindingList<DocumentAttribute> result = new BindingList<DocumentAttribute>();
            using (ServiceReferenceDocument.DocumentsClient client = new BiblosDS.UnitTest.WCF.ServiceReferenceDocument.DocumentsClient())
            {

                result = client.GetMetadataStructure(((Guid)TestContext.DataRow["IdArchive"]));
            }
            //            
            foreach (var item in result)
            {
                attributes.Add(new DocumentAttribute(item.IdAttribute,
                    item.Name,
                    item.IsRequired,
                    item.KeyOrder, 
                    new DocumentAttributeMode(item.Mode.IdMode, item.Mode.Description),
                    item.IsMainDate,
                    item.IsEnumerator,
                    item.AttributeType,
                    item.ConservationPosition,
                    item.KeyFilter,
                    item.KeyFormat));
            }
        }

        [DataSource(
            "System.Data.SqlClient",
            "Data Source=localhost\\SQLEXPRESS;Initial Catalog=BiblosDS2009;Integrated Security=True",
            "Document",
            DataAccessMethod.Sequential),
        TestMethod()]      
        public void Storage_AddDocument_Test()
        {
            using (ServiceReferenceStorage.ServiceDocumentStorageClient client = new BiblosDS.UnitTest.WCF.ServiceReferenceStorage.ServiceDocumentStorageClient())
            {
                Document newDocument = new Document();
                newDocument.IdDocument = (Guid)TestContext.DataRow["IdDocument"];
                newDocument.IdBiblos = (int)TestContext.DataRow["IdBiblos"];
                if (TestContext.DataRow["IdParentBiblos"] != null)
                    newDocument.DocumentParent = new Document();
                newDocument.DocumentParent.IdDocument = (Guid)TestContext.DataRow["IdParentBiblos"];
                newDocument.Archive = new DocumentArchive();
                newDocument.Archive.IdArchive = (Guid)TestContext.DataRow["IdArchive"];
                //Add the attributes...
                newDocument.AttributeValues = new System.ComponentModel.BindingList<DocumentAttributeValue>();
                ////Add the DocumentAttributeValue
                DocumentAttributeValue value = null;
                foreach (var item in attributes)
                {
                    value = new DocumentAttributeValue();
                    value.Attribute = item;
                    switch (item.Name)
                    {
                        case "Anno":
                            value.Value = "2009";
                            break;
                        default:
                            value.Value = "2009";
                            break;
                    }
                    newDocument.AttributeValues.Add(value);
                }

                client.AddDocument(newDocument);
            }
        }
    }
}
