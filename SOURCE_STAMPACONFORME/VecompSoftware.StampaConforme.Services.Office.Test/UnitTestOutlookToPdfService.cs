using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.EnterpriseLogging;
using VecompSoftware.StampaConforme.Interfaces.Office;
using VecompSoftware.StampaConforme.Models.Office.Outlook;

namespace VecompSoftware.StampaConforme.Services.Office.Test
{
    [TestClass]
    public class UnitTestOutlookToPdfService
    {
        #region [ Fields ]
        private ILogger _logger;
        #endregion

        #region [ Constructor ]
        public UnitTestOutlookToPdfService()
        {

        }
        #endregion

        #region [ Methods ]
        [TestInitialize]
        public void TestInitialize()
        {
            _logger = new GlobalLogger();
        }

        [TestMethod]
        public void TestMethod_ConvertEmlDocumentToPDF()
        {
            using (IOutlookToPdfService service = new OutlookToPdfService(_logger))
            {
                SaveMailToPdfRequest model = new SaveMailToPdfRequest()
                {
                    DestinationFilePath = Path.Combine(Directory.GetCurrentDirectory(), "VecompSoftwareTest.eml.pdf"),
                    SourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"TestFiles\VecompSoftwareTest.eml")
                };

                Assert.IsTrue(service.SaveTo(model));
            }
        }
        #endregion   
    }
}
