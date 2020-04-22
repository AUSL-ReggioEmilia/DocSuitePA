using BiblosDS.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BiblosDS.Library.Common;
using BiblosDS.Library.Common.Objects.UtilityService;

namespace BiblosDS.API.Test
{       
    /// <summary>
    ///This is a test class for DocumentoFacadeTest and is intended
    ///to contain all DocumentoFacadeTest Unit Tests
    ///</summary>
     [TestClass()]
    public class ProfiliTest
    {
        #region PreRequisite
        internal static LoginResponse token = null;
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


        [ClassInitialize()]
        public static void Login(TestContext testContext)
        {
            token = DocumentoFacade.Login(new LoginRequest { UserName = Config.USERNAME, Password = Config.PASSWORD, IdCliente = Config.ID_CLIENTE });
        }

        [ClassCleanup()]
        public static void LogOut()
        {
            if (token != null)
            {
                DocumentoFacade.Logout(token.TokenInfo.Token);
            }
        }
        #endregion
        /// <summary>
        ///A test for GetProfili
        ///</summary>
        [TestMethod()]
        public void GetProfiliTest()
        {            
            GetArchiviRequest request = new GetArchiviRequest {  Token = token.TokenInfo.Token };
            GetArchiviResponse actual = DocumentoFacade.GetProfili(request);
            Assert.IsTrue(actual != null);
            Assert.IsTrue(actual.Archivi.Count > 0);
            
        }
    }
}
