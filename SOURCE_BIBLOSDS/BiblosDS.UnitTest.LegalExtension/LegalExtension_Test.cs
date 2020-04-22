using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace BiblosDS.UnitTest.LegalExtension
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class LegalExtension_Test
  {
    public LegalExtension_Test()
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
    public void TestMethod1()
    {
      BiblosDS.UnitTest.LegalExtension.LegalExtension.ServiceLegalExtensionClient DsT
        = new BiblosDS.UnitTest.LegalExtension.LegalExtension.ServiceLegalExtensionClient();

      DsT.VerifyConservation(new Guid("00facb7a-c1c8-4a99-a682-b718e64d291d"));
    }

    [TestMethod]
    public void GetColseFile()
    {
        Preservation.PreservationClient client = new Preservation.PreservationClient();
        var res = client.GetPreservationCloseFile(new Guid("56AABD76-FAC9-46D2-96E2-FE0D9CE2E0A8"), Preservation.ContentFormat.ConformBinary);
        File.WriteAllBytes(@"C:\Lavori\Docs\BiblosDS\closefile.pdf", res.Blob);
    }
  }
}
