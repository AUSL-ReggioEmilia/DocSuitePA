using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.EnterpriseLogging;
using VecompSoftware.StampaConforme.Interfaces.Office;
using VecompSoftware.StampaConforme.Models.Office.Excel;

namespace VecompSoftware.StampaConforme.Services.Office.Test
{
    [TestClass]
    public class UnitTestExcelToPdfService
    {
        #region [ Fields ]
        private ILogger _logger;
        #endregion

        #region [ Constructor ]
        public UnitTestExcelToPdfService()
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
        public void TestMethod_ConvertXlsxDocumentToPDF_No_Portrait()
        {
            using (IExcelToPdfService service = new ExcelToPdfService(_logger))
            {
                SaveWorkbookToPdfRequest model = new SaveWorkbookToPdfRequest()
                {
                    DestinationFilePath = Path.Combine(Directory.GetCurrentDirectory(), "VecompSoftwareTest.xlsx.pdf"),
                    SourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"TestFiles\VecompSoftwareTest.xlsx"),
                    ForcePortrait = false,
                };

                Assert.IsTrue(service.SaveTo(model));
            }
        }

        [TestMethod]
        public void TestMethod_ConvertXlsxDocumentToPDF_Force_Portrait()
        {
            using (IExcelToPdfService service = new ExcelToPdfService(_logger))
            {
                SaveWorkbookToPdfRequest model = new SaveWorkbookToPdfRequest()
                {
                    DestinationFilePath = Path.Combine(Directory.GetCurrentDirectory(), "VecompSoftwareTest.xlsx.pdf"),
                    SourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"TestFiles\VecompSoftwareTest.xlsx"),
                    ForcePortrait = true,
                };

                Assert.IsTrue(service.SaveTo(model));
            }
        }
        #endregion   
    }
}
