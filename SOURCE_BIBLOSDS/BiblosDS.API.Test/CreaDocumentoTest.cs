using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace BiblosDS.API.Test
{
    [TestClass]
    public class CreaDocumentoTest
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
        public void CreaDocumentoSoloMetadati()
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

            request.Metadati.Add(new MetadatoItem { Name = "Anno", Value = DateTime.Now.Year.ToString() });
            request.Metadati.Add(new MetadatoItem { Name = "Serie", Value = "IVA" });
            request.Metadati.Add(new MetadatoItem { Name = "Numero", Value = int.Parse(string.Format("{0:HHmmss}", DateTime.Now)) });
            request.Metadati.Add(new MetadatoItem { Name = "DataDocumento", Value = DateTime.Now });

            Assert.IsTrue(DocumentoFacade.CreaDocumento(request).CodiceEsito == CodiceErrore.NessunErrore);
        }

        [TestMethod]
        public void CreaDocumento()
        {
            var request = new CreaDocumentoRequest
                              {
                                  Chiave = string.Format("IVA00{0:HHmmss}", DateTime.Now),//"IVA00166739",//
                                  IdClient = "desktop", 
                                  IdRichiesta = "20081128000001",
                                  IdCliente = Config.ID_CLIENTE, 
                                  TipoDocumento = Config.TIPO_DOCUMENTO,
                                  File = new FileItem { Nome = "GodwillMayor.txt", Blob = File.ReadAllBytes(@"C:\Lavori\Docs\BiblosDS\closefile.pdf") },
                                  Token = token.TokenInfo.Token
                              };

            request.Metadati.Add(new MetadatoItem { Name = "Anno", Value = DateTime.Now.Year.ToString() });
            request.Metadati.Add(new MetadatoItem { Name = "Serie", Value = "IVA" });
            request.Metadati.Add(new MetadatoItem { Name = "Numero", Value = int.Parse(string.Format("{0:HHmmss}", DateTime.Now)) });                        
            request.Metadati.Add(new MetadatoItem { Name = "DataDocumento", Value = DateTime.Now });
            var response = DocumentoFacade.CreaDocumento(request);
            Assert.IsTrue(response.CodiceEsito == CodiceErrore.NessunErrore);
        }
    }
}