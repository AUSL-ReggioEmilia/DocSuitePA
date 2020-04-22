using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BiblosDS.API.Test
{
    [TestClass]
    public class InfoAllegatiDocumentoTest
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
        public void InfoAllegatiDocumento()
        {
            var request = new InfoAllegatiRequest { Token = token.TokenInfo.Token, IdCliente = Config.ID_CLIENTE, IdClient = Config.USERNAME, IdDocumento = Config.IdDocument, TipoDocumento = Config.TIPO_DOCUMENTO };
            var response = DocumentoFacade.InfoAllegatiDocumento(request);

            TestContext.WriteLine("Esito: {0} {1}.", response.MessaggioErrore, response.MessaggioEsito);

            Assert.IsTrue(response.Eseguito);
        }

        [TestMethod]
        public void ImmagineAllegatiDocumento()
        {
            var responseInfo = DocumentoFacade.InfoAllegatiDocumento(new InfoAllegatiRequest { 
                Token = token.TokenInfo.Token, 
                IdCliente = Config.ID_CLIENTE,
                IdClient = Config.USERNAME,
                IdDocumento = Config.IdDocument, TipoDocumento = Config.TIPO_DOCUMENTO });

            var request = new ImmagineAllegatoDocumentoRequest { Token = token.TokenInfo.Token, IdCliente = Config.ID_CLIENTE, IdClient = Config.USERNAME, IdAllegato = responseInfo.Allegati.Last().IdAllegato, TipoDocumento = Config.TIPO_DOCUMENTO };
            var response = DocumentoFacade.ImmagineAllegatoDocumento(request);

            TestContext.WriteLine("Esito: {0} {1}.", response.MessaggioErrore, response.MessaggioEsito);

            Assert.IsTrue(response.Eseguito);
        }
    }
}
