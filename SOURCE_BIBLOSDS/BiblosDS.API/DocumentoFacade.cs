using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using BiblosDS.API.DocumentServiceReference;
using BiblosDS.API.Exceptions;
using BiblosDS.Library.Common.Objects;
using System.ServiceModel;
using BiblosDS.Library.Common.Exceptions;
using BiblosDS.Library.Common.Enums;

namespace BiblosDS.API
{
    /// <summary>
    /// Facade di accesso alla funzionalità delle API
    /// </summary>
    public partial class DocumentoFacade
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DocumentoFacade));

        /// <summary>
        /// Creazione di un documento
        /// </summary>
        /// <param name="request"><see cref="CreaDocumentoRequest">request</see></param>
        /// <returns>
        /// <see cref="CreaDocumentoResponse">response</see>
        /// </returns>
        /// <example>
        ///  var request = new CreaDocumentoRequest
        ///  {
        ///  Chiave = "IVA0000001",
        ///  IdClient = "desktop", 
        ///  IdRichiesta = "20081128000001", 
        ///  IdCliente = "ClienteTest", 
        ///  TipoDocumento = "DDT_ATH_TEST2",
        ///  File = new FileItem { Nome = "", Blob = File.ReadAllBytes(@"C:\Lavori\Docs\BiblosDS\Scansione61058.pdf.PDF") }                                   
        ///  };
        ///  
        ///  request.Metadati.Add(new MetadatoItem {Name = "TipoDocumento", Value = "IVA"});
        ///  request.Metadati.Add(new MetadatoItem {Name = "Tipologia", Value = "IVA"});
        ///  request.Metadati.Add(new MetadatoItem {Name = "ProgressivoDocumento", Value = 1});
        ///  request.Metadati.Add(new MetadatoItem {Name = "NomeArchivio", Value = "Test"});
        ///  request.Metadati.Add(new MetadatoItem {Name = "idBiblos", Value = 1});
        ///  request.Metadati.Add(new MetadatoItem {Name = "DataInserimentoDocumento", Value = DateTime.Now});
        /// </example>
        public static CreaDocumentoResponse CreaDocumento(CreaDocumentoRequest request)
        {
            var response = new CreaDocumentoResponse();
            try
            {
                logger.DebugFormat("CreaDocumento request:{0}", request.ToString());
                response.TokenInfo = Helpers.ValidaToken(request);
                if (response.TokenInfo == null)
                    response.CodiceEsito = CodiceErrore.TokenNonValidoOScaduto;
                else
                {
                    var checkRequestResult = request.CheckRichiestaChiave(response, request.Chiave);
                    if (checkRequestResult != CodiceErrore.NessunErrore)
                    {
                        response.CodiceEsito = checkRequestResult;
                        response.Eseguito = false;
                    }
                    else
                    {
                        var documento = new Document();
                        //Recupero Archive
                        var idArchive = Helpers.GetArchive(request);
                        if (!idArchive.HasValue)
                        {
                            response.CodiceEsito = CodiceErrore.ArchivioNonDefinito;
                            response.Eseguito = false;
                        }
                        else
                        {
                            try
                            {
                                var metadata = Helpers.GetBiblosDSMetadataStructure(idArchive.Value);
                                if (request.File != null)
                                {
                                    documento.Content = new DocumentContent(request.File.Blob);
                                    documento.Name = Path.GetFileName(request.File.Nome);
                                }
                                if (request.Metadati != null)
                                    documento.AttributeValues = new BindingList<DocumentAttributeValue>(request.Metadati.Convert(metadata));
                                documento.Archive = new DocumentArchive(idArchive.Value);
                                using (var client = new DocumentsClient())
                                {
                                    try
                                    {
                                        Document result = client.AddDocumentToChain(documento, null, DocumentContentFormat.Binary);
                                        Helpers.SetDocumentKey(request, result.IdDocument, request.Chiave);
                                        client.ConfirmChain(result.IdDocument);
                                        response.Eseguito = true;
                                        //TODO Popolare Chiavi = 
                                        response.Documento = new DocumentoItem { IdDocumento = result.IdDocument, Stato = Operazione.Insert, Chiavi = result.AttributeValues.Where(x => x.Attribute.IsRequired).Select(x => new MetadatoItem { Name = x.Attribute.Name, Value = x.Value }).ToList() };
                                        logger.DebugFormat("CreaDocumento response:{0}", response.ToString());

                                    }
                                    catch (FaultException<BiblosDsException> faultEx)
                                    {
                                        logger.Error(faultEx);
                                        ParseBiblosDSFaultException(response, faultEx);
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error(ex);
                                        response.CodiceEsito = CodiceErrore.ErroreChiamataAlServizioRemoto;
                                        response.MessaggioErrore = ex.ToString();
                                    }
                                }
                            }
                            catch (AttributoNonTrovato ex)
                            {
                                logger.Error(ex);
                                response.CodiceEsito = CodiceErrore.AttributoNonTrovato;
                                response.MessaggioErrore = ex.ToString();
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                                response.CodiceEsito = CodiceErrore.ErroreGenerico;
                                response.MessaggioErrore = ex.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                response.CodiceEsito = CodiceErrore.ErroreGenerico;
                response.MessaggioErrore = ex.ToString();
            }
            response.CheckResponse();
            return response;
        }

        /// <summary>
        /// Cancellazione di un documento e di tutte le versioni collegate
        /// </summary>
        /// <param name="request">
        /// <see cref="CancellaDocumentoRequest">request</see>
        /// </param>
        /// <remarks>
        /// IdDocumento richiesto per il funzionamento.
        /// </remarks>
        /// <returns>
        /// <see cref="CancellaDocumentoResponse">response</see>
        /// </returns>
        /// <example>
        /// var documento = CreaDocumento();
        /// var request = new CancellaDocumentoRequest
        /// {       
        /// IdClient = "desktop",
        /// IdRichiesta = "20081128000001",
        /// IdCliente = "ClienteTest",
        /// TipoDocumento = "DDT_ATH_TEST2",
        /// IdDocumento = documento.Documento.IdDocumento,
        /// };
        /// var result = DocumentoFacade.CancellaDocumento(request);
        /// </example>                                               
        public static CancellaDocumentoResponse CancellaDocumento(CancellaDocumentoRequest request)
        {
            var response = new CancellaDocumentoResponse();
            try
            {
                logger.DebugFormat("CancellaDocumento request:{0}", request.ToString());
                response.TokenInfo = Helpers.ValidaToken(request);
                if (response.TokenInfo == null)
                    response.CodiceEsito = CodiceErrore.TokenNonValidoOScaduto;
                else
                {
                    //Verifico che la request sia corretta
                    var checkRequestResult = request.CheckRequest(response);
                    if (checkRequestResult != CodiceErrore.NessunErrore)
                    {
                        response.CodiceEsito = checkRequestResult;
                        response.Eseguito = false;
                    }
                    else
                    {
                        if (request.IdDocumento == Guid.Empty)
                        {
                            response.CodiceEsito = CodiceErrore.IdDocumentoNonValido;
                            response.Eseguito = false;
                        }
                        else
                        {
                            using (var client = new DocumentsClient())
                            {
                                try
                                {
                                    client.SetVisibleChain(request.IdDocumento, false);
                                    Helpers.DeleteDocumentKey(request.IdDocumento);
                                    response.Eseguito = true;
                                    response.Documento = new DocumentoItem { IdDocumento = request.IdDocumento, Stato = Operazione.Delete };
                                }
                                catch (FaultException<BiblosDsException> faultEx)
                                {
                                    logger.Error(faultEx);
                                    response.Eseguito = false;
                                    ParseBiblosDSFaultException(response, faultEx);
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex);
                                    response.Eseguito = false;
                                    response.CodiceEsito = CodiceErrore.ErroreChiamataAlServizioRemoto;
                                    response.MessaggioErrore = ex.StackTrace;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                response.Eseguito = false;
                response.CodiceEsito = CodiceErrore.ErroreGenerico;
                response.MessaggioErrore = ex.ToString();
            }
            response.CheckResponse();
            logger.DebugFormat("CancellaDocumento response:{0}", response.ToString());
            return response;
        }

        /// <summary>
        /// Cancellazione di un documento e di tutte le versioni collegate
        /// </summary>
        /// <param name="request">
        /// <see cref="CancellaDocumentoRequest">request</see>
        /// </param>
        /// <remarks>
        /// Chiave richiesta per il funzionamento.
        /// </remarks>
        /// <returns>
        /// <see cref="CancellaDocumentoResponse">response</see>
        /// </returns>
        /// <example>
        /// var documento = CreaDocumento();
        /// var request = new CancellaDocumentoRequest
        /// {
        /// Chiave = "IVA0000001",
        /// IdClient = "desktop",
        /// IdRichiesta = "20081128000001",
        /// IdCliente = "ClienteTest",
        /// TipoDocumento = "DDT_ATH_TEST2",        
        /// };
        /// var result = DocumentoFacade.CancellaDocumento(request);
        /// </example>                  
        public static CancellaDocumentoChiaveResponse CancellaDocumentoChiavi(CancellaDocumentoChiaveRequest request)
        {
            var response = new CancellaDocumentoChiaveResponse();

            try
            {
                logger.DebugFormat("CancellaDocumentoChiavi request:{0}", request.ToString());
                response.TokenInfo = Helpers.ValidaToken(request);
                if (response.TokenInfo == null)
                    response.CodiceEsito = CodiceErrore.TokenNonValidoOScaduto;
                else
                {
                    //Verifico che la request sia corretta
                    var checkRequestResult = request.CheckRequest(response);
                    if (checkRequestResult != CodiceErrore.NessunErrore)
                    {
                        response.CodiceEsito = checkRequestResult;
                        response.Eseguito = false;
                    }
                    else
                    {
                        //Recupero il Documento
                        CodiceErrore error;
                        var idDocumento = Helpers.GetDocumentByChiave(request, request.Chiave, out error);
                        if (!idDocumento.HasValue)
                        {
                            response.CodiceEsito = CodiceErrore.DocumentoNonDefinito;
                            response.Eseguito = false;
                        }
                        else if (error != CodiceErrore.NessunErrore)
                        {
                            response.CodiceEsito = error;
                            response.Eseguito = false;
                        }
                        else
                        {

                            using (var client = new DocumentsClient())
                            {
                                try
                                {
                                    client.SetVisibleChain(idDocumento.Value, false);
                                    Helpers.DeleteDocumentKey(idDocumento.Value);
                                    response.Eseguito = true;
                                    response.Documento = new DocumentoItem { IdDocumento = idDocumento.Value, Stato = Operazione.Delete };
                                }
                                catch (FaultException<BiblosDsException> faultEx)
                                {
                                    logger.Error(faultEx);
                                    response.Eseguito = false;
                                    ParseBiblosDSFaultException(response, faultEx);
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex);
                                    response.Eseguito = false;
                                    response.CodiceEsito = CodiceErrore.ErroreChiamataAlServizioRemoto;
                                    response.MessaggioErrore = ex.StackTrace;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                response.Eseguito = false;
                response.CodiceEsito = CodiceErrore.ErroreGenerico;
                response.MessaggioErrore = ex.ToString();
            }
            response.CheckResponse();
            logger.DebugFormat("CancellaDocumentoChiavi response:{0}", response.ToString());
            return response;
        }

        /// <summary>
        /// Aggiornamento di un documento
        /// </summary>
        /// <remarks>
        /// Il metodo aggiorna creando una nuova versione solo se ci sono delle modifiche apportate al file o ai metadati
        /// </remarks>
        /// <param name="request">
        /// <see cref="AggiornaDocumentoRequest">request</see>
        /// </param>
        /// <returns>
        /// <see cref="AggiornaDocumentoResponse">response</see>
        /// </returns>
        /// <example>
        /// var request = new AggiornaDocumentoRequest
        /// {
        ///     IdClient = "desktop",
        ///     IdRichiesta = "20081128000001",
        ///     IdCliente = "ClienteTest",
        ///     TipoDocumento = "DDT_ATH_TEST2",
        ///     Chiave = "IVA0000001",
        ///     IdDocumento = new Guid("dbd44243-d746-4e5d-b3c4-2caf4cf9ac32")
        /// };
        ///  
        ///  request.Metadati.Add(new MetadatoItem { Name = "TipoDocumento", Value = "IVA" });
        ///  request.Metadati.Add(new MetadatoItem { Name = "Tipologia", Value = "IVA" });
        ///  request.Metadati.Add(new MetadatoItem { Name = "ProgressivoDocumento", Value = 1 });
        ///  request.Metadati.Add(new MetadatoItem { Name = "NomeArchivio", Value = "Test" });
        ///  request.Metadati.Add(new MetadatoItem { Name = "idBiblos", Value = 1 });
        ///  request.Metadati.Add(new MetadatoItem { Name = "DataInserimentoDocumento", Value = DateTime.Now });
        ///  
        ///  AggiornaDocumentoResponse actual;
        ///  actual = DocumentoFacade.AggiornaDocumento(request);
        ///  Assert.AreEqual(actual.CodiceEsito, CodiceErrore.NessunErrore);   
        /// </example>         
        public static AggiornaDocumentoResponse AggiornaDocumento(AggiornaDocumentoRequest request)
        {
            var response = new AggiornaDocumentoResponse();
            try
            {
                logger.DebugFormat("AggiornaDocumento request:{0}", request.ToString());
                response.TokenInfo = Helpers.ValidaToken(request);
                if (response.TokenInfo == null)
                    response.CodiceEsito = CodiceErrore.TokenNonValidoOScaduto;
                else
                {
                    var checkRequestResult = request.CheckRequest(response);
                    if (checkRequestResult != CodiceErrore.NessunErrore)
                    {
                        response.CodiceEsito = checkRequestResult;
                        response.Eseguito = false;
                    }
                    else
                    {
                        //Recupero Archive
                        var idArchive = Helpers.GetArchive(request);
                        if (!idArchive.HasValue)
                        {
                            response.CodiceEsito = CodiceErrore.ArchivioNonDefinito;
                            response.Eseguito = false;
                        }
                        else
                        {
                            EseguiAggiornamento(request.Metadati, request.File, request.IdClient, request.IdDocumento, idArchive.Value, response);
                            logger.DebugFormat("AggiornaDocumento response:{0}", response.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                response.Eseguito = false;
                response.CodiceEsito = CodiceErrore.ErroreGenerico;
                response.MessaggioErrore = ex.ToString();
            }
            response.CheckResponse();
            return response;
        }

        /// <summary>
        /// Aggiornamento di un documento
        /// </summary>
        /// <remarks>
        /// Il metodo aggiorna creando una nuova versione solo se ci sono delle modifiche apportate al file o ai metadati
        /// </remarks>
        /// <param name="request">
        /// <see cref="AggiornaDocumentoRequest">request</see>
        /// </param>
        /// <returns>
        /// <see cref="AggiornaDocumentoResponse">response</see>
        /// </returns>
        /// <example>
        /// var request = new AggiornaDocumentoRequest
        /// {
        ///     IdClient = "desktop",
        ///     IdRichiesta = "20081128000001",
        ///     IdCliente = "ClienteTest",
        ///     TipoDocumento = "DDT_ATH_TEST2",
        ///     Chiave = "IVA0000001",        
        /// };
        ///  
        ///  request.Metadati.Add(new MetadatoItem { Name = "TipoDocumento", Value = "IVA" });
        ///  request.Metadati.Add(new MetadatoItem { Name = "Tipologia", Value = "IVA" });
        ///  request.Metadati.Add(new MetadatoItem { Name = "ProgressivoDocumento", Value = 1 });
        ///  request.Metadati.Add(new MetadatoItem { Name = "NomeArchivio", Value = "Test" });
        ///  request.Metadati.Add(new MetadatoItem { Name = "idBiblos", Value = 1 });
        ///  request.Metadati.Add(new MetadatoItem { Name = "DataInserimentoDocumento", Value = DateTime.Now });
        ///  
        ///  AggiornaDocumentoResponse actual;
        ///  actual = DocumentoFacade.AggiornaDocumento(request);
        ///  Assert.AreEqual(actual.CodiceEsito, CodiceErrore.NessunErrore);   
        /// </example>         
        public static AggiornaDocumentoChiaveResponse AggiornaDocumentoChiave(AggiornaDocumentoChiaveRequest request)
        {
            var response = new AggiornaDocumentoChiaveResponse();
            try
            {
                logger.DebugFormat("AggiornaDocumentoChiave request:{0}", request.ToString());
                response.TokenInfo = Helpers.ValidaToken(request);
                if (response.TokenInfo == null)
                    response.CodiceEsito = CodiceErrore.TokenNonValidoOScaduto;
                else
                {
                    var checkRequestResult = request.CheckRichiestaChiave(response, request.Chiave);
                    if (checkRequestResult != CodiceErrore.NessunErrore)
                    {
                        response.CodiceEsito = checkRequestResult;
                        response.Eseguito = false;
                    }
                    else
                    {
                        //Recupero il Documento
                        CodiceErrore error;
                        var idDocumento = Helpers.GetDocumentByChiave(request, request.Chiave, out error);
                        if (!idDocumento.HasValue)
                        {
                            response.CodiceEsito = CodiceErrore.DocumentoNonDefinito;
                            response.Eseguito = false;
                        }
                        else if (error != CodiceErrore.NessunErrore)
                        {
                            response.CodiceEsito = error;
                            response.Eseguito = false;
                        }
                        else
                        {
                            //Recupero Archive
                            var idArchive = Helpers.GetArchive(request);
                            if (!idArchive.HasValue)
                            {
                                response.CodiceEsito = CodiceErrore.ArchivioNonDefinito;
                                response.Eseguito = false;
                            }
                            else
                            {
                                EseguiAggiornamento(request.Metadati, request.File, request.IdClient, idDocumento.Value, idArchive.Value, response);
                                logger.DebugFormat("AggiornaDocumentoChiave response:{0}", response.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                response.Eseguito = false;
                response.CodiceEsito = CodiceErrore.ErroreGenerico;
                response.MessaggioErrore = ex.ToString();
            }
            response.CheckResponse();
            return response;
        }

        private static void EseguiAggiornamento(IEnumerable<MetadatoItem> metadati, FileItem file, string idClient,
                                                Guid idDocumento, Guid idArchive, AggiornaDocumentoResponse response)
        {
            try
            {
                var documento = new Document();

                var metadata = Helpers.GetBiblosDSMetadataStructure(idArchive);

                documento.AttributeValues = new BindingList<DocumentAttributeValue>(metadati.Convert(metadata));
                documento.Archive = new DocumentArchive(idArchive);
                documento.IdDocument = idDocumento;
                //TODO gestire gli allegati
                using (var client = new DocumentsClient())
                {
                    var isCheckOut = false;
                    try
                    {
                        var forceUndo = true;
                        var checkOutDocument = client.DocumentCheckOut(idDocumento, true, idClient);
                        isCheckOut = true;
                        DocumentContent content = null;
                        if (file != null)
                        {
                            documento.Content = new DocumentContent(file.Blob);
                            content = documento.Content;
                            logger.DebugFormat("Check Document Status: {0}", checkOutDocument.Status == null ? "Null" : checkOutDocument.Status.IdStatus.ToString());
                            if (checkOutDocument.DocumentLink != null || (checkOutDocument.Status != null && checkOutDocument.Status.IdStatus != (short)DocumentStatus.ProfileOnly))
                            {
                                content = client.GetDocumentContentById(documento.IdDocument);
                                //Verifica se i file sono modificati
                                if (content != null)
                                    if (Helpers.GetDocumentHash(content) != Helpers.GetDocumentHash(documento.Content))
                                    {
                                        content = documento.Content;
                                        forceUndo = false;
                                        documento.Name = Path.GetFileName(file.Nome);
                                    }
                                    else
                                        content = null;
                            }
                            else
                                documento.Name = Path.GetFileName(file.Nome);
                        }
                        foreach (var item in documento.AttributeValues)
                        {
                            if (checkOutDocument.AttributeValues.Any(x => x.Attribute.IdAttribute == item.IdAttribute))
                            {
                                if (item.Value != checkOutDocument.AttributeValues.First(x => x.Attribute.IdAttribute == item.IdAttribute).Value)
                                {
                                    checkOutDocument.AttributeValues.First(x => x.Attribute.IdAttribute == item.IdAttribute).Value = item.Value;
                                    forceUndo = false;
                                }
                            }
                            else
                            {
                                checkOutDocument.AttributeValues.Add(item);
                                forceUndo = false;
                            }
                        }
                        Guid newId = documento.IdDocument;
                        if (forceUndo)
                            client.DocumentUndoCheckOut(idDocumento, idClient);
                        else
                        {
                            checkOutDocument.Content = content;
                            checkOutDocument.Name = documento.Name;
                            newId = client.DocumentCheckIn(checkOutDocument, idClient);
                            Helpers.UpdateDocumentIdByIdDocument(idDocumento, newId);
                            response.Eseguito = true;
                        }
                        //TODO Popolare Chiavi = 
                        response.Documento = new DocumentoItem
                                                        {
                                                            IdDocumento = newId,
                                                            Stato = forceUndo ? Operazione.Default : Operazione.Update,
                                                            Chiavi = documento.AttributeValues.Where(x => x.Attribute.IsRequired).Select(x => new MetadatoItem { Name = x.Attribute.Name, Value = x.Value }).ToList(),
                                                            Nome = documento.Name
                                                        };
                    }
                    catch (FaultException<BiblosDsException> faultEx)
                    {
                        logger.Error(faultEx);
                        response.Eseguito = false;
                        ParseBiblosDSFaultException(response, faultEx);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        if (isCheckOut)
                        {
                            try
                            {
                                client.DocumentUndoCheckOut(idDocumento, idClient);
                            }
                            catch (Exception exUndo)
                            {
                                logger.Fatal(exUndo);
                            }
                        }
                        response.Eseguito = false;
                        response.CodiceEsito = CodiceErrore.ErroreGenerico;
                        response.MessaggioErrore = ex.ToString();
                    }
                }
            }
            catch (AttributoNonTrovato ex)
            {
                logger.Error(ex);
                response.Eseguito = false;
                response.CodiceEsito = CodiceErrore.AttributoNonTrovato;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                response.Eseguito = false;
                response.CodiceEsito = CodiceErrore.ErroreGenerico;
            }
        }

        /// <summary>
        /// Informazioni su di un documento
        /// </summary>
        /// <param name="request">
        /// <see cref="InfoDocumentoChiaveRequest">request</see>
        /// </param>
        /// <returns>
        /// <see cref="InfoDocumentoChiaveResponse">response</see>
        /// </returns>
        /// <remarks>
        /// Chiave richiesta per effattuare la richiesta
        /// </remarks>
        /// <example>
        /// var request = new InfoDocumentoRequest
        /// {        
        ///     IdClient = "ClienteTest",
        ///     dRichiesta = "20081128000001",
        ///     IdCliente = "ClienteTest",
        ///     TipoDocumento = "DDT_ATH_TEST2",
        ///     Chiave = "IVA0000001"
        /// };
        /// Assert.IsTrue(DocumentoFacade.InfoDocumento(request).CodiceEsito == CodiceErrore.NessunErrore);
        /// </example>
        public static InfoDocumentoChiaveResponse InfoDocumentoChiave(InfoDocumentoChiaveRequest request)
        {
            InfoDocumentoChiaveResponse response = new InfoDocumentoChiaveResponse();
            //Recupero il Documento
            try
            {
                logger.DebugFormat("InfoDocumentoChiave request:{0}", request.ToString());
                CodiceErrore error;
                var idDocumento = Helpers.GetDocumentByChiave(request, request.Chiave, out error);

                if (!idDocumento.HasValue)
                {
                    response.CodiceEsito = CodiceErrore.DocumentoNonDefinito;
                    response.Eseguito = false;
                }
                else if (error != CodiceErrore.NessunErrore)
                {
                    response.CodiceEsito = error;
                    response.Eseguito = false;
                }
                else
                {
                    request.IdDocumento = idDocumento.Value;
                    var infoDocResponse = InfoDocumento(request);

                    if (infoDocResponse.CodiceEsito == CodiceErrore.NessunErrore)
                    {
                        response.CodiceEsito = infoDocResponse.CodiceEsito;
                        response.Documento = infoDocResponse.Documento;
                        response.Eseguito = infoDocResponse.Eseguito;
                        response.IdDocumento = infoDocResponse.IdDocumento;
                        response.IdRichiesta = infoDocResponse.IdRichiesta;
                        response.MessaggioErrore = infoDocResponse.MessaggioErrore;
                        response.MessaggioEsito = infoDocResponse.MessaggioEsito;
                        response.StatoDocumento = infoDocResponse.StatoDocumento;
                        response.TokenInfo = infoDocResponse.TokenInfo;
                        response.Eseguito = true;
                    }
                    else
                    {
                        response.CodiceEsito = infoDocResponse.CodiceEsito;
                    }
                }

                response.CheckResponse();

                logger.DebugFormat("InfoDocumentoChiave response:{0}", response.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                response.Eseguito = false;
                response.CodiceEsito = CodiceErrore.ErroreGenerico;
                response.MessaggioErrore = ex.ToString();
            }

            return response;
        }

        /// <summary>
        /// Informazioni su di un documento
        /// </summary>
        /// <param name="request">
        /// <see cref="InfoDocumentoChiaveRequest">request</see>
        /// </param>
        /// <returns>
        /// <see cref="InfoDocumentoChiaveResponse">response</see>
        /// </returns>
        /// <remarks>
        /// IdDocumento richiesto per effattuare la richiesta
        /// </remarks>
        /// <example>
        /// var request = new InfoDocumentoRequest
        /// {
        ///     IdDocumento = new Guid("d4d7a0dc-5cf2-4813-a1d7-0006b53c7799"),
        ///     IdClient = "ClienteTest",
        ///     dRichiesta = "20081128000001",
        ///     IdCliente = "ClienteTest",
        ///     TipoDocumento = "DDT_ATH_TEST2"        
        /// };
        /// Assert.IsTrue(DocumentoFacade.InfoDocumento(request).CodiceEsito == CodiceErrore.NessunErrore);
        /// </example>
        public static InfoDocumentoResponse InfoDocumento(InfoDocumentoRequest request)
        {
            var response = new InfoDocumentoResponse { StatoDocumento = new StatoDocumento() };
            try
            {
                logger.DebugFormat("InfoDocumento request:{0}", request.ToString());
                response.TokenInfo = Helpers.ValidaToken(request);
                if (response.TokenInfo == null)
                    response.CodiceEsito = CodiceErrore.TokenNonValidoOScaduto;
                else
                {
                    var checkRequestResult = request.CheckRequest(response);
                    if (checkRequestResult != CodiceErrore.NessunErrore)
                    {
                        response.CodiceEsito = checkRequestResult;
                        response.Eseguito = false;
                    }
                    else
                    {
                        if (request.IdDocumento == Guid.Empty)
                        {
                            response.CodiceEsito = CodiceErrore.IdDocumentoNonValido;
                            response.Eseguito = false;
                        }
                        else
                        {

                            //Recupero Archive
                            var idArchive = Helpers.GetArchive(request);
                            if (!idArchive.HasValue)
                            {
                                response.CodiceEsito = CodiceErrore.ArchivioNonDefinito;
                                response.Eseguito = false;
                            }
                            else
                            {

                                using (var client = new DocumentsClient())
                                {
                                    try
                                    {
                                        //TODO Controllare come mai non torna l'ultima versione
                                        response.Documento = new DocumentoItem { IdDocumento = request.IdDocumento, Stato = Operazione.Default };
                                        var documentInfo = client.GetDocumentInfoById(request.IdDocumento);
                                        response.Eseguito = true;
                                        response.Documento.Nome = documentInfo.Name;
                                        var listaMetadatoItem = new List<MetadatoItem>();

                                        foreach (var attributeValue in documentInfo.AttributeValues)
                                        {
                                            var proprietaDocumento = new MetadatoItem
                                            {
                                                Name = attributeValue.Attribute.Name,
                                                Posizione = attributeValue.Attribute.ConservationPosition,
                                                Value = attributeValue.Value,
                                                Tipo = attributeValue.Attribute.AttributeType
                                            };

                                            listaMetadatoItem.Add(proprietaDocumento);
                                        }

                                        response.StatoDocumento.ProprietaDocumento = new List<MetadatoItem>();
                                        response.StatoDocumento.ProprietaDocumento.AddRange(listaMetadatoItem);
                                        logger.DebugFormat("InfoDocumento response:{0}", response.ToString());
                                    }
                                    catch (FaultException<BiblosDsException> faultEx)
                                    {
                                        logger.Error(faultEx);
                                        response.Eseguito = false;
                                        ParseBiblosDSFaultException(response, faultEx);
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error(ex);
                                        response.Eseguito = false;
                                        response.CodiceEsito = CodiceErrore.ErroreGenerico;
                                        response.MessaggioErrore = ex.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                response.Eseguito = false;
                response.CodiceEsito = CodiceErrore.ErroreGenerico;
                response.MessaggioErrore = ex.ToString();
            }
            response.CheckResponse();
            return response;
        }
    }
}
