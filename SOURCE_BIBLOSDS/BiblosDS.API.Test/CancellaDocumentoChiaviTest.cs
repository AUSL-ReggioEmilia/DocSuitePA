using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BiblosDS.API.Test
{
    [TestClass]
    public class CancellaDocumentoChiaviTest
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
        public void CancellaDocumentoChiaviConChiaveNulla()
        {
            var request = new CancellaDocumentoChiaveRequest
                              {
                                  IdClient = "desktop",
                                  IdRichiesta = "20081128000001",
                                  IdCliente = Config.ID_CLIENTE,
                                  TipoDocumento = Config.TIPO_DOCUMENTO,
                                  Token = token.TokenInfo.Token
                              };

            Assert.IsTrue(DocumentoFacade.CancellaDocumentoChiavi(request).CodiceEsito == CodiceErrore.ChiaveDocumentoNonDefinita);
        }

        [TestMethod]
        public void CancellaDocumentoChiaviEsitoOk()
        {
            //var documento = CreaDocumento();
            //Sostituire i valori della request con quelli presi dalla CreaDocumento

            var request = new CancellaDocumentoChiaveRequest
            {
                IdClient = "ClienteTest",
                IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,
                Chiave = Config.CHIAVE,
                Token = token.TokenInfo.Token
            };

            Assert.IsTrue(DocumentoFacade.CancellaDocumentoChiavi(request).CodiceEsito == CodiceErrore.NessunErrore);
        }

        private CreaDocumentoResponse CreaDocumento()
        {
            var request = new CreaDocumentoRequest
            {
                IdClient = "desktop",
                IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,
                Chiave = "IVA0000011",
                Token = token.TokenInfo.Token
            };
            //request.Sovrascrivi = true;
            //request.PathFileImmagine = @"C:\Lavori\Docs\BiblosDS\Scansione61058.pdf.PDF";

            request.Metadati.Add(new MetadatoItem { Name = "TipoDocumento", Value = "IVA" });
            request.Metadati.Add(new MetadatoItem { Name = "Tipologia", Value = "IVA" });
            request.Metadati.Add(new MetadatoItem { Name = "ProgressivoDocumento", Value = 1 });
            request.Metadati.Add(new MetadatoItem { Name = "NomeArchivio", Value = "Test" });
            request.Metadati.Add(new MetadatoItem { Name = "idBiblos", Value = 1 });
            request.Metadati.Add(new MetadatoItem { Name = "DataInserimentoDocumento", Value = DateTime.Now });

            return DocumentoFacade.CreaDocumento(request);
        }

    }
}

