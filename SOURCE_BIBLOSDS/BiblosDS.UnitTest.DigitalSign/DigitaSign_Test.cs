using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiblosDS.WCF.DigitalSign;
using BiblosDS.Library.Common.CompEd;

namespace BiblosDS.UnitTest.DigitalSign
{
  /// <summary>
  /// Summary description for DigitaSign_Test
  /// </summary>
  [TestClass]
  public class DigitaSign_Test
  {
    public DigitaSign_Test()
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
    public void GetAllExpireDates()
    {
      string FileName = "1.txt.p7m",FilePath="C:/";

      SimplyCert sCert;

      BiblosDS.UnitTest.DigitalSign.ServiceDigitalSign.ServiceDigitalSignClient DsT = new BiblosDS.UnitTest.DigitalSign.ServiceDigitalSign.ServiceDigitalSignClient();
      Byte[] FileBlob=System.IO.File.ReadAllBytes(FilePath+FileName);
      //DsT.GetAllExpireDates(out sCert,FileName, FileBlob);
    }

    [TestMethod]
    public void CalculateHash()
    {
      BiblosDS.UnitTest.DigitalSign.ServiceDigitalSign.ServiceDigitalSignClient DsT = new BiblosDS.UnitTest.DigitalSign.ServiceDigitalSign.ServiceDigitalSignClient();
      string FileName = "1.txt.p7m", FilePath = "C:/";
      Byte[] FileBlob = System.IO.File.ReadAllBytes(FilePath + FileName);
      //DsT.CalculateBlobHash(FileBlob);
    }

    [TestMethod]
    public void GetContent()
    {
      BiblosDS.UnitTest.DigitalSign.ServiceDigitalSign.ServiceDigitalSignClient DsT = new BiblosDS.UnitTest.DigitalSign.ServiceDigitalSign.ServiceDigitalSignClient();
      string FileName = "1.txt.p7m", FilePath = "C:/", MetaData;
      Byte[] FileBlob = System.IO.File.ReadAllBytes(FilePath + FileName);
      //DsT.GetContent(out MetaData, FileName, FileBlob);
    }

    [TestMethod]
    public void TimeStampDocument()
    {
      // brucia una marca ad ogni test
      //BiblosDS.UnitTest.DigitalSign.ServiceDigitalSign.ServiceDigitalSignClient DsT = new BiblosDS.UnitTest.DigitalSign.ServiceDigitalSign.ServiceDigitalSignClient();
      //string FileName = "1.txt.p7m", FilePath = "C:/", MetaData;
      //Byte[] FileBlob = System.IO.File.ReadAllBytes(FilePath + FileName);
      //Byte[] oBlob= DsT.TimeStampDocument(FileName,FileBlob,true);
      //System.IO.File.WriteAllBytes(FilePath + FileName + ".p7x", oBlob);
    }

    [TestMethod]
    public void GetTimeStampAvailableFromClass()
    {
        string v = CompEdLib.GetTimeStampCount("INFOCAMERE", "alessandro.giachi@vecompsoftware.it", "alessandro11");
        Assert.IsTrue(v.Length > 0);
    }

    [TestMethod]
    public void GetTimeStampAvailable()
    {
      BiblosDS.UnitTest.DigitalSign.ServiceDigitalSign.ServiceDigitalSignClient DsT = new BiblosDS.UnitTest.DigitalSign.ServiceDigitalSign.ServiceDigitalSignClient();
      string res = DsT.GetTimeStampAvailable("INFOCAMERE", "alessandro.giachi@vecompsoftware.it", "alessandro11");
    }

    /// <summary>
    /// fa una firma client per testare il ArrRawSignature con la congiunzione server side
    /// </summary>
    [TestMethod]
    public void AddRawSignature()
    {
      BiblosDS.UnitTest.DigitalSign.ServiceDigitalSign.ServiceDigitalSignClient DsT = new BiblosDS.UnitTest.DigitalSign.ServiceDigitalSign.ServiceDigitalSignClient();

      string FileName = "1.txt", FilePath = "C:/", OutName="out.txt.p7m";
      Byte[] FileBlob = System.IO.File.ReadAllBytes(FilePath + FileName);

      // calc hash
      byte[] hash = CompEdObj.CalculateBlobHash(FileBlob, Library.Common.Objects.Enums.ComputeHashType.Default);

      CompEdObj p7mO = new CompEdObj();
      p7mO.Logon();
      string err = p7mO.GetLastError();
      int CertId, KeyId;
      p7mO.GetSmartCardCertificate(out CertId, out KeyId);
      err = p7mO.GetLastError();
      byte[] cert = p7mO.ExportCertificateBuf(CertId);
      err = p7mO.GetLastError();

      int digHndl = p7mO.DigestInit(hash);
      err = p7mO.GetLastError();
      object digestData;
      p7mO.DigestGetSignedValue(digHndl, CertId, out digestData);
      err = p7mO.GetLastError();
      p7mO.Logoff();

      //byte[] file = DsT.AddRawSignature(FileName, cert, new ServiceDigitalSign.Content() { Blob = (byte[])digestData }, new ServiceDigitalSign.Content() { Blob = FileBlob });
      //System.IO.File.WriteAllBytes(FilePath+OutName,file);
    }

    [TestMethod]
    public void CompEdServiceTest()
    {
        CompEdService comp = new CompEdService();
        comp.GetDocumentCertificates(@"C:\Lavori\Docs\BiblosDs\TestStampaConforme\Filtri.doc.P7M");
    }
  }
}
