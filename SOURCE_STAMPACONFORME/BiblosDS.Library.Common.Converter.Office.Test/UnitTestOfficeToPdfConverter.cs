using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VecompSoftware.Commons.BiblosDS.Objects.Enums;

namespace BiblosDS.Library.Common.Converter.Office.Test
{
    [TestClass]
    public class UnitTestOfficeToPdfConverter
    {
        [TestMethod]
        public void TestMethod_ConvertDocxToPdf_Extension_Without_Dot_Attach_Default()
        {
            OfficeToPdfConverter converter = new OfficeToPdfConverter();
            byte[] sourceContent = File.ReadAllBytes(Path.Combine(@"D:\Workspaces\Casi di studio\TesterApplications\OfficeTester\bin\Debug", "Documento di Sviluppo – Integrazione SPID e FedERa.docx"));
            byte[] convertedContent = converter.Convert(sourceContent, "docx", "pdf", AttachConversionMode.Default);
            File.WriteAllBytes(Path.Combine(@"D:\Workspaces\Casi di studio\TesterApplications\OfficeTester\bin\Debug", "test.pdf"), convertedContent);

            Assert.IsNotNull(convertedContent);
        }

        [TestMethod]
        public void TestMethod_ConvertDocToPdf_Attach_Default()
        {
            OfficeToPdfConverter converter = new OfficeToPdfConverter();
            byte[] sourceContent = File.ReadAllBytes(Path.Combine(@"D:\Workspaces\Casi di studio\TesterApplications\OfficeTester\bin\Debug", "test.doc"));
            byte[] convertedContent = converter.Convert(sourceContent, ".doc", "pdf", AttachConversionMode.Default);
            File.WriteAllBytes(Path.Combine(@"D:\Workspaces\Casi di studio\TesterApplications\OfficeTester\bin\Debug", "test_doc.pdf"), convertedContent);

            Assert.IsNotNull(convertedContent);
        }

        [TestMethod]
        public void TestMethod_ConvertEmlToPdf_Attach_Default()
        {
            OfficeToPdfConverter converter = new OfficeToPdfConverter();
            byte[] sourceContent = File.ReadAllBytes(Path.Combine(@"D:\Workspaces\Casi di studio\TesterApplications\OfficeTester\bin\Debug", "conimmagine.eml"));
            byte[] convertedContent = converter.Convert(sourceContent, ".eml", "pdf", AttachConversionMode.Default);
            File.WriteAllBytes(Path.Combine(@"D:\Workspaces\Casi di studio\TesterApplications\OfficeTester\bin\Debug", "test_eml.pdf"), convertedContent);

            Assert.IsNotNull(convertedContent);
        }

        [TestMethod]
        public void TestMethod_ConvertMsgToPdf_Attach_Default()
        {
            OfficeToPdfConverter converter = new OfficeToPdfConverter();
            byte[] sourceContent = File.ReadAllBytes(Path.Combine(@"D:\Workspaces\Casi di studio\TesterApplications\OfficeTester\bin\Debug", "test.msg"));
            byte[] convertedContent = converter.Convert(sourceContent, ".msg", "pdf", AttachConversionMode.Default);
            File.WriteAllBytes(Path.Combine(@"D:\Workspaces\Casi di studio\TesterApplications\OfficeTester\bin\Debug", "test_msg.pdf"), convertedContent);

            Assert.IsNotNull(convertedContent);
        }

        [TestMethod]
        public void TestMethod_ConvertXlsToPdf_Attach_Default()
        {
            OfficeToPdfConverter converter = new OfficeToPdfConverter();
            byte[] sourceContent = File.ReadAllBytes(Path.Combine(@"D:\Workspaces\Casi di studio\TesterApplications\OfficeTester\bin\Debug", "ComuneSGL.xls"));
            byte[] convertedContent = converter.Convert(sourceContent, ".xls", "pdf", AttachConversionMode.Default);
            File.WriteAllBytes(Path.Combine(@"D:\Workspaces\Casi di studio\TesterApplications\OfficeTester\bin\Debug", "test_xls.pdf"), convertedContent);

            Assert.IsNotNull(convertedContent);
        }

        [TestMethod]
        public void TestMethod_ConvertMultipleMsgToPdf_Parallel_Attach_Default()
        {
            OfficeToPdfConverter converter = new OfficeToPdfConverter();
            Parallel.ForEach(new string[] { "1", "2", "3", "4", "5" }, (s) =>
            {
                byte[] sourceContent = File.ReadAllBytes(Path.Combine(@"D:\Workspaces\Casi di studio\TesterApplications\OfficeTester\bin\Debug", "test.msg"));
                byte[] convertedContent = converter.Convert(sourceContent, ".msg", "pdf", AttachConversionMode.Default);
                File.WriteAllBytes(Path.Combine(@"D:\Workspaces\Casi di studio\TesterApplications\OfficeTester\bin\Debug", "test_msg"+s+".pdf"), convertedContent);
            });
            

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestMethod_ConvertMultipleDocxToPdf_Parallel_Attach_Default()
        {
            OfficeToPdfConverter converter = new OfficeToPdfConverter();
            Parallel.ForEach(new string[] { "1", "2", "3", "4", "5" }, (s) =>
            {
                byte[] sourceContent = File.ReadAllBytes(Path.Combine(@"D:\Workspaces\Casi di studio\TesterApplications\OfficeTester\bin\Debug", "Documento di Sviluppo – Integrazione SPID e FedERa.docx"));
                byte[] convertedContent = converter.Convert(sourceContent, ".docx", "pdf", AttachConversionMode.Default);
                File.WriteAllBytes(Path.Combine(@"D:\Workspaces\Casi di studio\TesterApplications\OfficeTester\bin\Debug", "test_docx" + s + ".pdf"), convertedContent);
            });


            Assert.IsTrue(true);
        }
    }
}
