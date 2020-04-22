using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BiblosDS.API.Test
{
    [TestClass]
    public class ImmagineDocumentoTest
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
        public void ImmagineDocumentoConChiaveNulla()
        {
            var request = new ImmagineDocumentoChiaveRequest
                              {
                                  IdClient = "desktop",
                                  IdRichiesta = "20081128000001",
                                  IdCliente = Config.ID_CLIENTE,
                                  TipoDocumento = Config.TIPO_DOCUMENTO,
                                  Token = token.TokenInfo.Token
                              };
            var response = DocumentoFacade.ImmagineDocumentoChiave(request);
            TestContext.WriteLine("response.CodiceEsito:{0}, response.Eseguito:{1}, response.MessaggioEsito:{2}, response.MessaggioErrore:{3}", response.CodiceEsito, response.Eseguito, response.MessaggioEsito, response.MessaggioErrore);
            Assert.IsTrue(response.CodiceEsito == CodiceErrore.ChiaveDocumentoNonDefinita);
        }

        [TestMethod]
        public void ImmagineDocumentoConChiaveAltroCliente()
        {
            var request = new ImmagineDocumentoChiaveRequest
            {
                Chiave = Config.CHIAVE_ALTRO_CLIENTE,
                IdClient = "desktop",
                IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,
                Token = token.TokenInfo.Token
            };
            var response = DocumentoFacade.ImmagineDocumentoChiave(request);
            TestContext.WriteLine("response.CodiceEsito:{0}, response.Eseguito:{1}, response.MessaggioEsito:{2}, response.MessaggioErrore:{3}", response.CodiceEsito, response.Eseguito, response.MessaggioEsito, response.MessaggioErrore);
            Assert.IsTrue(response.CodiceEsito == CodiceErrore.ChiaveDocumentoNonDefinita);
        }

        [TestMethod]
        public void ImmagineDocumentoChiaveEsitoOk()
        {
            //var documento = CreaDocumento();
            //Sostituire i valori della request con quelli presi dalla CreaDocumento

            var request = new ImmagineDocumentoChiaveRequest
            {                
                IdClient = "desktop",
                IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,
                Chiave = Config.CHIAVE,
                Token = token.TokenInfo.Token
            };

            var response = DocumentoFacade.ImmagineDocumentoChiave(request);
            TestContext.WriteLine("response.CodiceEsito:{0}, response.Eseguito:{1}, response.MessaggioEsito:{2}, response.MessaggioErrore:{3}", response.CodiceEsito, response.Eseguito, response.MessaggioEsito, response.MessaggioErrore);
            Assert.IsTrue(response.CodiceEsito == CodiceErrore.NessunErrore);
        }

        [TestMethod]
        public void ImmagineDocumentoEsitoDocumentoNonTrovato()
        {
            //var documento = CreaDocumento();
            //Sostituire i valori della request con quelli presi dalla CreaDocumento

            var request = new ImmagineDocumentoRequest
            {
                IdDocumento = Guid.NewGuid(),
                IdClient = "desktop",
                IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,
                Token = token.TokenInfo.Token
            };

            var response = DocumentoFacade.ImmagineDocumento(request);
            TestContext.WriteLine("response.CodiceEsito:{0}, response.Eseguito:{1}, response.MessaggioEsito:{2}, response.MessaggioErrore:{3}", response.CodiceEsito, response.Eseguito, response.MessaggioEsito, response.MessaggioErrore);
            Assert.IsTrue(response.CodiceEsito == CodiceErrore.NessunErrore);
        }

        [TestMethod]
        public void ImmagineDocumentoEsitoOk()
        {
            //var documento = CreaDocumento();
            //Sostituire i valori della request con quelli presi dalla CreaDocumento

            var request = new ImmagineDocumentoRequest
            {
                IdDocumento = Config.IdDocument,
                IdClient = "desktop",
                IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,               
                Token = token.TokenInfo.Token
            };

            var response = DocumentoFacade.ImmagineDocumento(request);
            TestContext.WriteLine("response.CodiceEsito:{0}, response.Eseguito:{1}, response.MessaggioEsito:{2}, response.MessaggioErrore:{3}", response.CodiceEsito, response.Eseguito, response.MessaggioEsito, response.MessaggioErrore);
            Assert.IsTrue(response.CodiceEsito == CodiceErrore.NessunErrore);
        }
    }
}

