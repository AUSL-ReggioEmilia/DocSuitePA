using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BiblosDS.API.Test
{
    [TestClass]
    public class CancellaAllegatoTest
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

        [TestMethod]
        public void CancellaAllegato()
        {
            var request = new CancellaAllegatoDocumentoRequest
            {
                IdClient = "Terasoftware",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,
                Token = token.TokenInfo.Token,
                IdDocumento = Config.IdDocument,
                IdAllegato = new Guid("ef9e397e-6638-4f81-bc22-8f7f1fe5029c"),
            };

            var response = DocumentoFacade.CancellaAllegatoDocumento(request);
            TestContext.WriteLine("{0} - {1}", response.CodiceEsito.ToString(), response.MessaggioErrore);
            Assert.IsTrue(response.CodiceEsito == CodiceErrore.NessunErrore);
        }
    }
}
