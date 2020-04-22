using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace BiblosDS.API.Test
{
    [TestClass]
    public class CancellaDocumentoTest
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
            DocumentoFacade.Login(new LoginRequest { UserName = Config.USERNAME, Password = Config.PASSWORD, IdCliente = Config.ID_CLIENTE });
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
        //[ExpectedException(typeof(Exceptions.ErroreChiamataAlServizioRemoto), "Nessun documento trovato con i parametri passati")]
        public void CancellaDocumentoIdConIdDocumentononEsistente()
        {
            var request = new CancellaDocumentoRequest
                              {                                  
                                  IdClient = "desktop",
                                  IdRichiesta = "20081128000001",
                                  IdCliente = Config.ID_CLIENTE,
                                  TipoDocumento = Config.TIPO_DOCUMENTO,
                                  IdDocumento = Guid.NewGuid(),
                                  Token = token.TokenInfo.Token
                              };

            Assert.IsTrue(DocumentoFacade.CancellaDocumento(request).CodiceEsito == CodiceErrore.IdDocumentoNonTrovato);
        }
        
        [TestMethod]
        public void CancellaDocumentoIdConIdDocumentoNullo()
        {
            var request = new CancellaDocumentoRequest
                              {                                  
                                  IdClient = "desktop",
                                  IdRichiesta = "20081128000001",
                                  IdCliente = Config.ID_CLIENTE,
                                  TipoDocumento = Config.TIPO_DOCUMENTO,
                                  IdDocumento = Guid.Empty,
                                  Token = token.TokenInfo.Token
                              };

            Assert.IsTrue(DocumentoFacade.CancellaDocumento(request).CodiceEsito == CodiceErrore.IdDocumentoNonValido);
        }

        [TestMethod]
        public void CancellaDocumentoIdEsitoOk()
        {
            var documento = CreaDocumento();

            var request = new CancellaDocumentoRequest
            {                
                IdClient = "desktop",
                IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,
                IdDocumento = documento.Documento.IdDocumento,
                Token = token.TokenInfo.Token
            };

            Assert.IsTrue(DocumentoFacade.CancellaDocumento(request).CodiceEsito == CodiceErrore.IdDocumentoNonValido);
        }

        private CreaDocumentoResponse CreaDocumento()
        {
            var request = new CreaDocumentoRequest
            {
                IdClient = "desktop",
                IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,
                Chiave = string.Format("IVA00{0:HHmmss}", DateTime.Now),
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

