using BiblosDS.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace BiblosDS.API.Test
{
    
    
    /// <summary>
    ///This is a test class for DocumentoFacadeTest and is intended
    ///to contain all DocumentoFacadeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AggiornaDocumentoTest
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
        ///A test for AggiornaDocumentoId
        ///</summary>
        [TestMethod()]
        public void AggiornaDocumentoNoFile()
        {
            var request = new AggiornaDocumentoRequest
            {
                IdClient = "desktop",
                IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,
                Chiave = Config.CHIAVE,
                IdDocumento = Config.IdDocument,
                Token = token.TokenInfo.Token,               
            };

            request.Metadati.Add(new MetadatoItem { Name = "TipoDocumento", Value = "IVA" });
            request.Metadati.Add(new MetadatoItem { Name = "Tipologia", Value = "IVA" });
            request.Metadati.Add(new MetadatoItem { Name = "ProgressivoDocumento", Value = 1 });
            request.Metadati.Add(new MetadatoItem { Name = "NomeArchivio", Value = "Test" });
            request.Metadati.Add(new MetadatoItem { Name = "idBiblos", Value = 1 });
            request.Metadati.Add(new MetadatoItem { Name = "DataInserimentoDocumento", Value = DateTime.Now });

            AggiornaDocumentoResponse actual;
            actual = DocumentoFacade.AggiornaDocumento(request);
            Assert.AreEqual(actual.CodiceEsito, CodiceErrore.NessunErrore);
        }
        /// <summary>
        ///A test for AggiornaDocumentoId
        ///</summary>
        [TestMethod()]
        public void AggiornaDocumento()
        {
            var request = new AggiornaDocumentoRequest
            {
                IdClient = "desktop",
                IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,                
                IdDocumento = Config.IdDocument,
                Token = token.TokenInfo.Token,
                File = new FileItem
                {
                    Nome = "pippo.txt",
                    Blob = File.ReadAllBytes(@"C:\Lavori\Docs\BiblosDS\original.pdf.PDF")
                }
            };

            request.Metadati.Add(new MetadatoItem { Name = "Anno", Value = DateTime.Now.Year.ToString() });
            request.Metadati.Add(new MetadatoItem { Name = "Serie", Value = "IVA" });            
            request.Metadati.Add(new MetadatoItem { Name = "DataDocumento", Value = DateTime.Now });
            /*
            request.Metadati.Add(new MetadatoItem { Name = "TipoDocumento", Value = "IVA" });
            request.Metadati.Add(new MetadatoItem { Name = "Tipologia", Value = "IVA" });
            request.Metadati.Add(new MetadatoItem { Name = "ProgressivoDocumento", Value = 1 });
            request.Metadati.Add(new MetadatoItem { Name = "NomeArchivio", Value = "Test" });
            request.Metadati.Add(new MetadatoItem { Name = "idBiblos", Value = 1 });
            request.Metadati.Add(new MetadatoItem { Name = "DataInserimentoDocumento", Value = DateTime.Now });
            **/
            AggiornaDocumentoResponse actual;
            actual = DocumentoFacade.AggiornaDocumento(request);
            Assert.AreEqual(actual.CodiceEsito, CodiceErrore.NessunErrore);            
        }
    }
}
