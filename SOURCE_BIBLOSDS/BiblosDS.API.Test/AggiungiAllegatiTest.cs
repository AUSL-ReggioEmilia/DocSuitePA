using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace BiblosDS.API.Test
{
    [TestClass]
    public class AggiungiAllegatiTest
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
        public void AggiungiAllegati()
        {
            var request = new AggiungiAllegatoRequest
            {
                Token = token.TokenInfo.Token,
                IdCliente = Config.ID_CLIENTE,
                IdClient = Config.USERNAME,
                IdDocumento = Config.IdDocument,
                Allegato = new FileItem(),
                TipoDocumento = Config.TIPO_DOCUMENTO
            };

            request.Allegato.Blob = File.ReadAllBytes(Config.FILE_TEST.FullName);
            request.Allegato.Nome = Config.FILE_TEST.Name;

            var response = DocumentoFacade.AggiungiAllegato(request);

            if (response != null && response.CodiceEsito != CodiceErrore.NessunErrore)
            {
                TestContext.WriteLine(string.Format("ERRORE ED ESITO: {0} ; {1}", response.MessaggioErrore, response.MessaggioEsito));
            }

            Assert.IsTrue(response.Eseguito);
        }

        [TestMethod]
        public void AggiungiAllegatiChiave()
        {
            var request = new AggiungiAllegatoChiaveRequest
            {
                Token = token.TokenInfo.Token,
                IdCliente = Config.ID_CLIENTE,
                IdClient = Config.USERNAME,
                Allegato = new FileItem(),
                Chiave = Config.CHIAVE,
                TipoDocumento = Config.TIPO_DOCUMENTO
            };

            request.Allegato.Blob = File.ReadAllBytes(Config.FILE_TEST.FullName);
            request.Allegato.Nome = Config.FILE_TEST.Name;

            var response = DocumentoFacade.AggiungiAllegatoChiave(request);

            if (response != null && response.CodiceEsito != CodiceErrore.NessunErrore)
            {
                TestContext.WriteLine(string.Format("ERRORE ED ESITO: {0} ; {1}", response.MessaggioErrore, response.MessaggioEsito));
            }

            Assert.IsTrue(response.Eseguito);
        }
    }
}
