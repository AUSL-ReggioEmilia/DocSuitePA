using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace BiblosDS.API.Test
{
    [TestClass]
    public class AggiornaDocumentoByChiaveTest
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
        public void AggiornaDocumentoByChiaveNoDocumentoEsitoOk()
        {
            //var documento = CreaDocumento();
            //Sostituire i valori della request con quelli presi dalla CreaDocumento

            var request = new AggiornaDocumentoChiaveRequest
            {
                IdClient = "ClienteTest",
                IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,
                Chiave = Config.CHIAVE,
                Token = token.TokenInfo.Token                
            };

            request.Metadati.Add(new MetadatoItem { Name = "TipoDocumento", Value = "IVA" });
            request.Metadati.Add(new MetadatoItem { Name = "Tipologia", Value = "IVA" });
            request.Metadati.Add(new MetadatoItem { Name = "ProgressivoDocumento", Value = 1 });
            request.Metadati.Add(new MetadatoItem { Name = "NomeArchivio", Value = "Test" });
            request.Metadati.Add(new MetadatoItem { Name = "idBiblos", Value = 1 });
            request.Metadati.Add(new MetadatoItem { Name = "DataInserimentoDocumento", Value = DateTime.Now });

            Assert.IsTrue(DocumentoFacade.AggiornaDocumentoChiave(request).CodiceEsito == CodiceErrore.NessunErrore);
        }

        [TestMethod]
        public void AggiornaDocumentoByChiaveEsitoOk()
        {
            //var documento = CreaDocumento();
            //Sostituire i valori della request con quelli presi dalla CreaDocumento

            var request = new AggiornaDocumentoChiaveRequest
            {                
                IdClient = "ClienteTest",
                IdRichiesta = "20081128000001",
                IdCliente = Config.ID_CLIENTE,
                TipoDocumento = Config.TIPO_DOCUMENTO,
                Chiave = "IVA00102432",
                Token = token.TokenInfo.Token,
                File = new FileItem
                {
                    Nome = "ThisIsOneTest.txt",
                    Blob = File.ReadAllBytes(@"C:\Lavori\Impronta Archivio.xml")
                }
            };

            request.Metadati.Add(new MetadatoItem { Name = "CodiceContabile", Value = "C0005" });
            //request.Metadati.Add(new MetadatoItem { Name = "Anno", Value = DateTime.Now.Year.ToString() });
            //request.Metadati.Add(new MetadatoItem { Name = "Serie", Value = "IVA" });
            //request.Metadati.Add(new MetadatoItem { Name = "Numero", Value = int.Parse(string.Format("{0:HHmmss}", DateTime.Now)) });
            //request.Metadati.Add(new MetadatoItem { Name = "DataDocumento", Value = DateTime.Now });

            //request.Metadati.Add(new MetadatoItem { Name = "TipoDocumento", Value = "IVA" });
            //request.Metadati.Add(new MetadatoItem { Name = "Tipologia", Value = "IVA" });
            //request.Metadati.Add(new MetadatoItem { Name = "ProgressivoDocumento", Value = 1 });
            //request.Metadati.Add(new MetadatoItem { Name = "NomeArchivio", Value = "Test" });
            //request.Metadati.Add(new MetadatoItem { Name = "idBiblos", Value = 1 });
            //request.Metadati.Add(new MetadatoItem { Name = "DataInserimentoDocumento", Value = DateTime.Now });

            Assert.IsTrue(DocumentoFacade.AggiornaDocumentoChiave(request).CodiceEsito == CodiceErrore.NessunErrore);
        }
    }
}

