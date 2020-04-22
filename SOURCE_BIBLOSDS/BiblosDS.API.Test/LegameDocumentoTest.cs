using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BiblosDS.API.Test
{
    [TestClass]
    public class LegameDocumentoTest
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

        #region MAKE IT EXPLODE!!! ... but gracefully.

        [TestMethod]
        public void Esplodi_CreaLegameDocumenti_IdTokenTarocco()
        {
            //Qui si imposta un token tarocco.
            CreaLegameDocumentiRequest request = new CreaLegameDocumentiRequest { Token = Guid.Empty };
            var response = DocumentoFacade.CreaLegameDocumenti(request);

            TestContext.WriteLine("Si parte!");
            try
            {
                if (response != null && response.CodiceEsito != CodiceErrore.NessunErrore)
                {
                    TestContext.WriteLine(string.Format("ERRORE ED ESITO: {0} ; {1}", response.MessaggioErrore, response.MessaggioEsito));
                }
            }
            catch (Exception exx)
            {
                TestContext.WriteLine("ECCEZIONE: " + exx.Message);
            }
            Assert.IsTrue(response.Eseguito);
        }

        [TestMethod]
        public void Esplodi_CreaLegameDocumenti_IdClienteVuoto()
        {
            //Qui si imposta un id vuoto.
            CreaLegameDocumentiRequest request = new CreaLegameDocumentiRequest { Token = token.TokenInfo.Token, IdCliente = string.Empty };
            var response = DocumentoFacade.CreaLegameDocumenti(request);

            TestContext.WriteLine("Si parte!");
            try
            {
                if (response != null && response.CodiceEsito != CodiceErrore.NessunErrore)
                {
                    TestContext.WriteLine(string.Format("ERRORE ED ESITO: {0} ; {1}", response.MessaggioErrore, response.MessaggioEsito));
                }
            }
            catch (Exception exx)
            {
                TestContext.WriteLine("ECCEZIONE: " + exx.Message);
            }
            Assert.IsTrue(response.Eseguito);
        }

        [TestMethod]
        public void Esplodi_CreaLegameDocumenti_IdClienteTarocco()
        {
            //Qui si imposta un id tarocco.
            CreaLegameDocumentiRequest request = new CreaLegameDocumentiRequest { Token = token.TokenInfo.Token, IdCliente = "jumboree" };
            var response = DocumentoFacade.CreaLegameDocumenti(request);

            TestContext.WriteLine("Si parte!");
            try
            {
                if (response != null && response.CodiceEsito != CodiceErrore.NessunErrore)
                {
                    TestContext.WriteLine(string.Format("ERRORE ED ESITO: {0} ; {1}", response.MessaggioErrore, response.MessaggioEsito));
                }
            }
            catch (Exception exx)
            {
                TestContext.WriteLine("ECCEZIONE: " + exx.Message);
            }
            Assert.IsTrue(response.Eseguito);
        }

        [TestMethod]
        public void Esplodi_CreaLegameDocumenti_NoIdDocument()
        {
            //Qui si imposta un id tarocco.
            CreaLegameDocumentiRequest request = new CreaLegameDocumentiRequest { Token = token.TokenInfo.Token, IdClient = "whop", IdCliente = Config.ID_CLIENTE };
            var response = DocumentoFacade.CreaLegameDocumenti(request);

            TestContext.WriteLine("Si parte!");
            try
            {
                if (response != null && response.CodiceEsito != CodiceErrore.NessunErrore)
                {
                    TestContext.WriteLine(string.Format("ERRORE ED ESITO: {0} ; {1}", response.MessaggioErrore, response.MessaggioEsito));
                }
            }
            catch (Exception exx)
            {
                TestContext.WriteLine("ECCEZIONE: " + exx.Message);
            }
            Assert.IsTrue(response.Eseguito);
        }

        [TestMethod]
        public void Esplodi_CreaLegameDocumenti_IdDocumentoTarocco()
        {
            //Qui si imposta un IdDocument tarocco.
            CreaLegameDocumentiRequest request = new CreaLegameDocumentiRequest { Token = token.TokenInfo.Token, IdDocumento = Guid.Empty };
            var response = DocumentoFacade.CreaLegameDocumenti(request);

            TestContext.WriteLine("Si parte!");
            try
            {
                if (response != null && response.CodiceEsito != CodiceErrore.NessunErrore)
                {
                    TestContext.WriteLine(string.Format("ERRORE ED ESITO: {0} ; {1}", response.MessaggioErrore, response.MessaggioEsito));
                }
            }
            catch (Exception exx)
            {
                TestContext.WriteLine("ECCEZIONE: " + exx.Message);
            }
            Assert.IsTrue(response.Eseguito);
        }

        #endregion

        /// <summary>
        /// Verrà creato un collegamento fra un documento ed altri dieci diversi documenti.
        /// </summary>
        [TestMethod]
        public void LegameDocumenti_Crea_Test()
        {           
            CreaLegameDocumentiResponse response = null;
            CreaLegameDocumentiRequest request;
            
            TestContext.WriteLine("Si parte!");
                try
                {
                    request = new CreaLegameDocumentiRequest { Token = token.TokenInfo.Token, IdCliente = Config.ID_CLIENTE, IdClient = Config.USERNAME, IdDocumento = Config.IdDocument, IdDocumentoLink = Config.IdDocumentLegame };

                    response = DocumentoFacade.CreaLegameDocumenti(request);

                    if (response != null && response.CodiceEsito != CodiceErrore.NessunErrore)
                    {                        
                        TestContext.WriteLine(string.Format("ERRORE ED ESITO: {0} ; {1}", response.MessaggioErrore, response.MessaggioEsito));
                    }
                }
                catch (Exception exx)
                {
                    TestContext.WriteLine(string.Format("ECCEZIONE: ID = \"{0}\" , ERRORE = \"{1}\"", Config.IdDocumentLegame, exx.Message));
                }
            
            Assert.IsTrue(response.Eseguito);
        }

        /// <summary>
        /// Verrà creato un collegamento fra un documento ed altri dieci diversi documenti.
        /// </summary>
        [TestMethod]
        public void LegameDocumentiChiave_Crea_Test()
        {
            CreaLegameDocumentiChiaveResponse response = null;
            CreaLegameDocumentiChiaveRequest request;

            TestContext.WriteLine("Si parte!");
            try
            {
                request = new CreaLegameDocumentiChiaveRequest { Token = token.TokenInfo.Token, IdCliente = Config.ID_CLIENTE, IdClient = Config.USERNAME, TipoDocumento = "xxx",  Chiave = Config.CHIAVE, ChiaveDocumentoLink = Config.ChiaveDocucuntoLink, TipoDocumentoLink = Config.TipoDocumentoLink };

                response = DocumentoFacade.CreaLegameDocumentiChiave(request);

                if (response != null && response.CodiceEsito != CodiceErrore.NessunErrore)
                {
                    TestContext.WriteLine(string.Format("ERRORE ED ESITO: {0} ; {1}", response.MessaggioErrore, response.MessaggioEsito));
                }
            }
            catch (Exception exx)
            {
                TestContext.WriteLine(string.Format("ECCEZIONE: ID = \"{0}\" , ERRORE = \"{1}\"", Config.IdDocumentLegame, exx.Message));
            }

            Assert.IsTrue(response.Eseguito);
        }

        [TestMethod]
        public void LegameDocumenti_Info_Test()
        {
            InfoLegameDocumentoResponse response = null;
            InfoLegameDocumentoRequest request;

            TestContext.WriteLine("Si parte!");
            try
            {
                request = new InfoLegameDocumentoRequest { Token = token.TokenInfo.Token, IdCliente = Config.ID_CLIENTE, IdClient = Config.USERNAME, Chiave = Config.CHIAVE, IdDocumento = Config.IdDocument };

                response = DocumentoFacade.InfoLegameDocumento(request);

                if (response != null && response.CodiceEsito != CodiceErrore.NessunErrore)
                {
                    TestContext.WriteLine(string.Format("ERRORE ED ESITO: {0} ; {1}", response.MessaggioErrore, response.MessaggioEsito));
                }
            }
            catch (Exception exx)
            {
                TestContext.WriteLine(string.Format("ECCEZIONE: ID = \"{0}\" , ERRORE = \"{1}\"", Config.IdDocumentLegame, exx.Message));
            }

            Assert.IsTrue(response.Eseguito);
        }

        [TestMethod]
        public void LegameDocumenti_Cancella_Test()
        {
            CancellaLegameDocumentiResponse response = null;
            CancellaLegameDocumentiRequest request;

            TestContext.WriteLine("Si parte!");
            try
            {
                var responseLegami = DocumentoFacade.InfoLegameDocumento(new InfoLegameDocumentoRequest { Token = token.TokenInfo.Token, IdCliente = Config.ID_CLIENTE, IdClient = Config.USERNAME, Chiave = Config.CHIAVE, IdDocumento = Config.IdDocument });

                if (responseLegami.Documenti == null || responseLegami.Documenti.Count() <= 0)
                    return;
                request = new CancellaLegameDocumentiRequest { Token = token.TokenInfo.Token, IdCliente = Config.ID_CLIENTE, IdClient = Config.USERNAME, Chiave = Config.CHIAVE, IdDocumento = Config.IdDocument, IdDocumentoLink = responseLegami.Documenti.First().IdDocumento };

                response = DocumentoFacade.CancellaLegameDocumenti(request);

                if (response != null && response.CodiceEsito != CodiceErrore.NessunErrore)
                {
                    TestContext.WriteLine(string.Format("ERRORE ED ESITO: {0} ; {1}", response.MessaggioErrore, response.MessaggioEsito));
                }
            }
            catch (Exception exx)
            {
                TestContext.WriteLine(string.Format("ECCEZIONE: ID = \"{0}\" , ERRORE = \"{1}\"", Config.IdDocumentLegame, exx.Message));
            }

            Assert.IsTrue(response.Eseguito);
        }
    }
}
