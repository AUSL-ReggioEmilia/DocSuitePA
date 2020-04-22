using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.API.DocumentServiceReference;
using System.ServiceModel;
using BiblosDS.Library.Common.Exceptions;

namespace BiblosDS.API
{
    public partial class DocumentoFacade
    {
        public static CreaLegameDocumentiResponse CreaLegameDocumenti(CreaLegameDocumentiRequest request)
        {
            var response = new CreaLegameDocumentiResponse();
            try
            {
                logger.DebugFormat("CreaLegameDocumenti request:{0}", request.ToString());
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
                        if (request.IdDocumento == Guid.Empty || request.IdDocumentoLink == Guid.Empty)
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
                                    client.AddDocumentLink(request.IdDocumento, request.IdDocumentoLink);
                                    response.Eseguito = true;
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
            logger.DebugFormat("CreaLegameDocumenti response:{0}", response.ToString());
            return response;
        }
        
        public static CreaLegameDocumentiChiaveResponse CreaLegameDocumentiChiave(CreaLegameDocumentiChiaveRequest request)
        {
            var response = new CreaLegameDocumentiChiaveResponse();
            try
            {
                logger.DebugFormat("CreaLegameDocumentiChiave request:{0}", request.ToString());
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
                        CodiceErrore error;
                        var idDocumento = Helpers.GetDocumentByChiave(request.TipoDocumento, request.IdCliente, request.Chiave, out error);
                        Guid? idDocumentoLink = null;
                        if (idDocumento.HasValue && error == CodiceErrore.NessunErrore)
                        {
                            idDocumentoLink = Helpers.GetDocumentByChiave(request.TipoDocumentoLink, request.IdCliente, request.ChiaveDocumentoLink, out error);
                        }
                        if (!idDocumento.HasValue || !idDocumentoLink.HasValue || error != CodiceErrore.NessunErrore)
                        {
                            response.CodiceEsito = error == CodiceErrore.NessunErrore ? CodiceErrore.DocumentoNonDefinito : error;
                            response.Eseguito = false;
                        }
                        else
                        {
                            using (var client = new DocumentsClient())
                            {
                                try
                                {
                                    client.AddDocumentLink(idDocumento.Value, idDocumentoLink.Value);
                                    response.Eseguito = true;
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
            logger.DebugFormat("CreaLegameDocumentiChiave response:{0}", response.ToString());
            return response;
        }

        public static CancellaLegameDocumentiChiaveResponse CancellaLegameDocumentiChiave(CancellaLegameDocumentiChiaveRequest request)
        {
            var response = new CancellaLegameDocumentiChiaveResponse();
            try
            {
                logger.DebugFormat("CancellaLegameDocumentiChiave request:{0}", request.ToString());
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
                        CodiceErrore error;
                        var idDocumento = Helpers.GetDocumentByChiave(request.TipoDocumento, request.IdCliente, request.Chiave, out error);
                        Guid? idDocumentoLink = null;
                        if (idDocumento.HasValue && error == CodiceErrore.NessunErrore)
                        {
                            idDocumentoLink = Helpers.GetDocumentByChiave(request.TipoDocumentoLink, request.IdCliente, request.ChiaveDocumentoLink, out error);
                        }
                        if (!idDocumento.HasValue || !idDocumentoLink.HasValue || error != CodiceErrore.NessunErrore)
                        {
                            response.CodiceEsito = error == CodiceErrore.NessunErrore ? CodiceErrore.DocumentoNonDefinito : error;
                            response.Eseguito = false;
                        }
                        else
                        {
                            using (var client = new DocumentsClient())
                            {
                                try
                                {
                                    client.DeleteDocumentLink(idDocumento.Value, idDocumentoLink.Value);
                                    response.Eseguito = true;
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
            logger.DebugFormat("CancellaLegameDocumentiChiave response:{0}", response.ToString());
            return response;
        }

        public static CancellaLegameDocumentiResponse CancellaLegameDocumenti(CancellaLegameDocumentiRequest request)
        {
            var response = new CancellaLegameDocumentiResponse();
            try
            {
                logger.DebugFormat("CancellaLegameDocumenti request:{0}", request.ToString());
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
                        if (request.IdDocumento == Guid.Empty || request.IdDocumentoLink == Guid.Empty)
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
                                    response.Eseguito = client.DeleteDocumentLink(request.IdDocumento, request.IdDocumentoLink);                                    
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
            logger.DebugFormat("CancellaLegameDocumenti response:{0}", response.ToString());
            return response;
        }

        public static InfoLegameDocumentoChiaveResponse InfoLegameDocumentoChiave(InfoLegameDocumentoChiaveRequest request)
        {
            InfoLegameDocumentoChiaveResponse response = new InfoLegameDocumentoChiaveResponse();
            //Recupero il Documento
            try
            {
                logger.DebugFormat("InfoLegameDocumentoChiave request:{0}", request.ToString());
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
                            var links = client.GetDocumentLinks(idDocumento.Value);
                            response.Documenti = links.Select(x => new DocumentoItem { IdDocumento = x.IdDocument, Nome = x.Name, Chiavi = GetMetadati(x), Stato = Operazione.Default }).ToList();                            
                            response.Eseguito = true;
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
            catch (Exception ex)
            {
                logger.Error(ex);
                response.Eseguito = false;
                response.CodiceEsito = CodiceErrore.ErroreGenerico;
                response.MessaggioErrore = ex.ToString();
            }

            response.CheckResponse();
            logger.DebugFormat("InfoLegameDocumentoChiave response:{0}", response.ToString());
            return response;
        }

        public static InfoLegameDocumentoResponse InfoLegameDocumento(InfoLegameDocumentoRequest request)
        {
            var response = new InfoLegameDocumentoResponse();
            try
            {
                logger.DebugFormat("InfoLegameDocumento request:{0}", request.ToString());
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
                                    var links = client.GetDocumentLinks(request.IdDocumento);
                                    response.Documenti = links.Select(x => new DocumentoItem { IdDocumento = x.IdDocument, Nome = x.Name, Chiavi = GetMetadati(x), Stato = Operazione.Default }).ToList();
                                    response.Eseguito = true;
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
            logger.DebugFormat("InfoLegameDocumento response:{0}", response.ToString());
            return response;
        }

        private static List<MetadatoItem> GetMetadati(Library.Common.Objects.Document document)
        {
            if (document.AttributeValues == null)
                return new List<MetadatoItem>();
            return document.AttributeValues.Select(x => new MetadatoItem { Name = x.Attribute == null ? "" : x.Attribute.Name, Tipo = x.Attribute == null ? "" : x.Attribute.AttributeType, Value = x.Value, Obbligatorio = x.Attribute == null ? false : x.Attribute.IsRequired }).ToList();
        }
    }
}
