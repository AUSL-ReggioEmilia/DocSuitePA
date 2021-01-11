using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.EnterpriseLogging;
using VecompSoftware.StampaConforme.Interfaces.Office;
using VecompSoftware.StampaConforme.Models.Office.Word;

namespace VecompSoftware.StampaConforme.Services.Office.Test
{
    [TestClass]
    public class UnitTestWordToPdfService
    {
        #region [ Fields ]
        private ILogger _logger;
        #endregion

        #region [ Constructor ]
        public UnitTestWordToPdfService()
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
        public void TestMethod_ConvertDocxDocumentToPDF_No_Portrait()
        {
            using (IWordToPdfService service = new WordToPdfService(_logger))
            {
                SaveDocumentToPdfRequest model = new SaveDocumentToPdfRequest()
                {
                    DestinationFilePath = Path.Combine(Directory.GetCurrentDirectory(), "VecompSoftwareTest.docx.pdf"),
                    SourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"TestFiles\VecompSoftwareTest.docx"),
                    ForcePortrait = false,
                };

                Assert.IsTrue(service.SaveTo(model));
            }            
        }

        [TestMethod]
        public void TestMethod_ConvertDocxDocumentToPDF_Force_Portrait()
        {
            using (IWordToPdfService service = new WordToPdfService(_logger))
            {
                SaveDocumentToPdfRequest model = new SaveDocumentToPdfRequest()
                {
                    DestinationFilePath = Path.Combine(Directory.GetCurrentDirectory(), "VecompSoftwareTest.docx.pdf"),
                    SourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"TestFiles\VecompSoftwareTest.docx"),
                    ForcePortrait = true,
                };

                Assert.IsTrue(service.SaveTo(model));
            }
        }

        [TestMethod]
        public void TestMethod_ConvertDocxDocumentToPDF_No_Portrait_With_Filters()
        {
            using (IWordToPdfService service = new WordToPdfService(_logger))
            {
                SaveDocumentToPdfRequest model = new SaveDocumentToPdfRequest()
                {
                    DestinationFilePath = Path.Combine(Directory.GetCurrentDirectory(), "VecompSoftwareTest.docx.pdf"),
                    SourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"TestFiles\VecompSoftwareTest.docx"),
                    ForcePortrait = false,
                    RedirectFilters = {"1033"}
                };

                Assert.IsTrue(service.SaveTo(model));
            }
        }
        #endregion        
    }
}
