using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiblosDS.UnitTest.WebService.eSign;
using System.IO;
using BiblosDS.WCF.DigitalSign;

namespace BiblosDS.UnitTest.WebService
{
    [TestClass]
    public class eSignService_Test
    {
        private static readonly string testFilesPath = @"C:\Lavori\Docs\BiblosDS\New folder (2)";
        private static readonly string testFileName = @"89535richiesta%20abilitazione%20doc.P7M";
        private static readonly string fullTestFilePathName = Path.Combine(testFilesPath, testFileName);

        private TestContext testContextInstance;

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

        public eSignService_Test() { }

        [TestMethod]
        public void EstraiP7M()
        {
            using (eSignEngine esign = new eSignEngine())
            {
                string c64out, metadata;
                string c64in = Convert.ToBase64String(File.ReadAllBytes(fullTestFilePathName));
                bool retval = esign.Extract(true, c64in, out c64out, out metadata);
                Assert.IsTrue(retval, "Impossibile estrarre dal blob.");
            }
        }

        [TestMethod]
        public void OttieniFirme()
        {
            using (eSignEngine esign = new eSignEngine())
            {
                string c64in = Convert.ToBase64String(File.ReadAllBytes(fullTestFilePathName));
                eSign.SimplyCert cert;
                string retval = esign.GetExpiryDates(fullTestFilePathName, c64in, out cert);
                Assert.IsTrue(!string.IsNullOrEmpty(retval), "Impossibile estrarre le firme dal blob.");
            }
        }

        [TestMethod]
        public void TestMassivoP7M()
        {
            int testOk = 0;
            var filesp7m = Directory.GetFiles(testFilesPath, "*.p7m", SearchOption.TopDirectoryOnly);

            using (eSignEngine esign = new eSignEngine())
            {
                string metadata, sBlob64, dummy;
                byte[] blob;
                eSign.SimplyCert cert;
                foreach (string fileName in filesp7m)
                {
                    try
                    {
                        blob = File.ReadAllBytes(fileName);
                        sBlob64 = Convert.ToBase64String(blob);

                        if (!esign.GetFileInfo("p7m", sBlob64, out metadata))
                            continue;

                        TestContext.WriteLine("Informazioni file: {0}", metadata);

                        if (!esign.Extract(true, sBlob64, out dummy, out metadata)
                            || (string.IsNullOrEmpty(dummy) && string.IsNullOrEmpty(metadata)))
                            continue;

                        TestContext.WriteLine("Metadati estratti: {0}", metadata);

                        dummy = esign.P7mSoftSign("Massimiliano", Convert.ToBase64String(Encoding.Default.GetBytes(fileName)), "01/01/2000", string.Empty, sBlob64);

                        if (string.IsNullOrEmpty(dummy))
                            continue;

                        TestContext.WriteLine("SoftSign: {0}", dummy);

                        //if (string.IsNullOrEmpty(esign.P7xTimeStampDocument(0, Convert.ToBase64String(Encoding.Default.GetBytes(fileName)), sBlob64)))
                        //    continue;

                        dummy = esign.GetExpiryDates(fileName, sBlob64, out cert);
                        if (string.IsNullOrEmpty(dummy))
                            continue;
                        //Occhio: viene estrapolata un sacco di roba!
                        //TestContext.WriteLine("Firme: {0}", dummy);
                        TestContext.WriteLine("File {0} processato correttamente", fileName);

                        testOk++;
                    }
                    catch { }
                }
            }

            Assert.IsTrue(testOk == filesp7m.Length, string.Format("Test riusciti: {0}/{1}.", testOk, filesp7m.Length));
        }

        [TestMethod]
        public void UsaLibreriaCompEd()
        {
            using (CompEdLib lib = new CompEdLib())
            {
                string metadata;
                byte[] content = lib.GetContent(true, File.ReadAllBytes(fullTestFilePathName), out metadata);
                Assert.IsNotNull(content, "Nessun contenuto recuperato.");
            }
        }
    }
}
