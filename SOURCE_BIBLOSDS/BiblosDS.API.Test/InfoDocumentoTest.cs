using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BiblosDS.API.Test
{
    [TestClass]
    public class InfoDocumentoTest
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
            token = DocumentoFacade.Login(new LoginRequest { UserName = "Gianni", Password = "Passw0rd", IdCliente = "TeraSoftware" });
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
        public void InfoDocumentoConChiaveNulla()
        {
            var request = new InfoDocumentoRequest
                              {
                                  IdClient = "desktop",
                                  IdRichiesta = "20081128000001",
                                  IdCliente = "ClienteTest",
                                  TipoDocumento = "DDT_ATH_TEST2",
                                  Token = token.TokenInfo.Token
                              };

            Assert.IsTrue(DocumentoFacade.InfoDocumento(request).CodiceEsito == CodiceErrore.ChiaveDocumentoNonDefinita);
        }

        [TestMethod]
        public void InfoDocumentoEsitoOk()
        {
            //var documento = CreaDocumento();
            //Sostituire i valori della request con quelli presi dalla CreaDocumento

            var request = new InfoDocumentoRequest
            {
                IdDocumento = Config.IdDocument,
                IdClient = "ClienteTest",
                IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,               
                Token = token.TokenInfo.Token
            };

            Assert.IsTrue(DocumentoFacade.InfoDocumento(request).CodiceEsito == CodiceErrore.NessunErrore);
        }
    }
}

