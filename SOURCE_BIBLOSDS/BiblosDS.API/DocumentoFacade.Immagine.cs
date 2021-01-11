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
        /// <summary>
        /// Richiede il contenuto di un documento (il file che lo rappresenta).
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static ImmagineDocumentoResponse ImmagineDocumento(ImmagineDocumentoRequest request)
        {
            var response = new ImmagineDocumentoResponse();
            try
            {
                logger.DebugFormat("GetImmagineDocumento request:{0}", request.ToString());
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
                            //TODO Verifica che l'id documento appartenga al cliente.
                            //se non appartiene errore:
                            //1027	Il documento identificato dal parametro IdDocumento non corrisponde alla chiave fornita
                            using (var client = new DocumentsClient())
                            {
                                try
                                {
                                    var res = client.GetDocumentContentById(request.IdDocumento);
                                    response.File = new FileItem { Blob = res.Blob, Nome = res.Description };
                                    response.Eseguito = true;
                                    response.Documento = new DocumentoItem { IdDocumento = request.IdDocumento, Nome = res.Description, Stato = Operazione.Default };                                    
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
            logger.DebugFormat("GetImmagineDocumento response:{0}", response.ToString());
            response.CheckResponse();
            return response;
        }

        /// <summary>
        /// Richiede il file che rappresenta il documento a partire dalla chiave che identifica quest'ultimo.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static ImmagineDocumentoChiaveResponse ImmagineDocumentoChiave(ImmagineDocumentoChiaveRequest request)
        {
            var response = new ImmagineDocumentoChiaveResponse();

            try
            {
                logger.DebugFormat("GetImmagineDocumentoChiavi request:{0}", request.ToString());
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
                                    var res = client.GetDocumentContentById(idDocumento.Value);
                                    response.File = new FileItem { Blob = res.Blob, Nome = res.Description };
                                    response.Eseguito = true;
                                    response.Documento = new DocumentoItem { IdDocumento = idDocumento.Value, Nome = res.Description, Stato = Operazione.Default };
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
            logger.DebugFormat("GetImmagineDocumentoChiavi response:{0}", response.ToString());
            response.CheckResponse();
            return response;
        }
    }
}
