using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BiblosDS.API.Test
{
    [TestClass]
    public class VerificaStatoConservazioneDocumentoTest
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
        public void VerificaStatoConservazioneDocumentoConChiaveNulla()
        {
            var request = new VerificaStatoConservazioneDocumentoChiaveRequest
            {
                IdClient = "desktop",
                //IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,
                Token = token.TokenInfo.Token
            };

            Assert.IsTrue(DocumentoFacade.VerificaStatoConservazioneDocumentoChiave(request).CodiceEsito == CodiceErrore.ChiaveDocumentoNonDefinita);
        }

        [TestMethod]
        public void VerificaStatoConservazioneDocumentoEsitoOk()
        {
            //var documento = CreaDocumento();
            //Sostituire i valori della request con quelli presi dalla CreaDocumento

            var request = new VerificaStatoConservazioneDocumentoRequest
            {
                IdDocumento = new Guid("DBBB7CEB-5CCC-425D-AE48-7F4140F00B5B"),
                IdClient = "ClienteTest",
                //IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,
                Token = token.TokenInfo.Token
            };

            Assert.IsTrue(DocumentoFacade.VerificaStatoConservazioneDocumento(request).CodiceEsito == CodiceErrore.NessunErrore);
        }

        [TestMethod]
        public void VerificaStatoConservazioneDocumentoChiave()
        {
            var request = new VerificaStatoConservazioneDocumentoChiaveRequest
            {
                IdClient = "ClienteTest",
                //IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,
                Chiave = Config.CHIAVE,
                Token = token.TokenInfo.Token
            };

            var esito = DocumentoFacade.VerificaStatoConservazioneDocumentoChiave(request).CodiceEsito;

            Assert.IsTrue(esito == CodiceErrore.NessunErrore);
        }
    }
}
