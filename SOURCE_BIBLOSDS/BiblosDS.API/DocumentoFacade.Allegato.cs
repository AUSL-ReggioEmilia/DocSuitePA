using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using BiblosDS.Library.Common.Exceptions;
using BiblosDS.Library.Common.Objects;
using BiblosDS.API.DocumentServiceReference;

namespace BiblosDS.API
{
    public partial class DocumentoFacade
    {
        /// <summary>
        /// Aggiunta di un allegato ad un documento.
        /// </summary>
        /// <param name="request">Modello di richiesta per l'aggiunta allegato.</param>
        /// <returns>Esito aggiunta <see cref="AggiungiAllegatoResponse"/>.</returns>
        public static AggiungiAllegatoResponse AggiungiAllegato(AggiungiAllegatoRequest request)
        {
            var response = new AggiungiAllegatoResponse { Eseguito = false, CodiceEsito = CodiceErrore.NessunErrore };

            try
            {
                logger.DebugFormat("AggiungiAllegati request:{0}", request.ToString());
                //Validazione token usato per la login.
                response.TokenInfo = Helpers.ValidaToken(request);
                //Viene controllato se il token è scaduto o non trovato.
                if (response.TokenInfo == null)
                    response.CodiceEsito = CodiceErrore.TokenNonValidoOScaduto;
                else
                {
                    //Verifico che la request sia corretta
                    var checkRequestResult = request.CheckRequest(response);
                    if (checkRequestResult != CodiceErrore.NessunErrore)
                    {
                        response.CodiceEsito = checkRequestResult;
                    }
                    else
                    {
                        AggiungiAllegato(request.IdDocumento, request.Allegato, response);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                response.CodiceEsito = CodiceErrore.ErroreGenerico;
                response.MessaggioErrore = ex.ToString();
            }
            //Controlla se è tutto ok.
            response.CheckResponse();
            //Torna al chiamante.
            logger.DebugFormat("AggiungiAllegati response:{0}", response.ToString());

            return response;
        }
       
        /// <summary>
        /// Aggiunta di un allegato ad un documento tramite chiave documento.
        /// </summary>
        /// <param name="request">Parametri necessari alla creazione allegato.</param>
        /// <returns>Esito trattamento <see cref="AggiungiAllegatoChiaveResponse"/>.</returns>
        public static AggiungiAllegatoChiaveResponse AggiungiAllegatoChiave(AggiungiAllegatoChiaveRequest request)
        {
            var response = new AggiungiAllegatoChiaveResponse { Eseguito = false, CodiceEsito = CodiceErrore.NessunErrore };

            try
            {
                logger.DebugFormat("AggiungiAllegatiChiave request:{0}", request.ToString());
                //Validazione token usato per la login.
                response.TokenInfo = Helpers.ValidaToken(request);
                //Viene controllato se il token è scaduto o non trovato.
                if (response.TokenInfo == null)
                    response.CodiceEsito = CodiceErrore.TokenNonValidoOScaduto;
                else
                {
                    //Verifico che la request sia corretta
                    var checkRequestResult = request.CheckRichiestaChiave(response, request.Chiave);
                    if (checkRequestResult != CodiceErrore.NessunErrore)
                    {
                        response.CodiceEsito = checkRequestResult;
                    }
                    else
                    {
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
                            AggiungiAllegato(idDocumento.Value, request.Allegato, response);
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
            //Controlla se è tutto ok.
            response.CheckResponse();
            //Torna al chiamante.
            logger.DebugFormat("AggiungiAllegatiChiave response:{0}", response.ToString());
            return response;
        }
        
        /// <summary>
        /// Lista di allegati collegati con il documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static InfoAllegatiResponse InfoAllegatiDocumento(InfoAllegatiRequest request)
        {
            //TODO Arriva IdDocument
            //Chiamai BiblosDS2010 passando l'idDocumet e gli allegati
            var response = new InfoAllegatiResponse { Eseguito = false, CodiceEsito = CodiceErrore.NessunErrore };

            try
            {
                logger.DebugFormat("InfoAllegatiDocumento request:{0}", request.ToString());
                //Validazione token usato per la login.
                response.TokenInfo = Helpers.ValidaToken(request);
                //Viene controllato se il token è scaduto o non trovato.
                if (response.TokenInfo == null)
                    response.CodiceEsito = CodiceErrore.TokenNonValidoOScaduto;
                else
                {
                    //Verifico che la request sia corretta
                    var checkRequestResult = request.CheckRequest(response);
                    if (checkRequestResult != CodiceErrore.NessunErrore)
                    {
                        response.CodiceEsito = checkRequestResult;
                    }
                    else
                    {
                        InfoAllegatiDocumento(request.IdDocumento, response);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                response.CodiceEsito = CodiceErrore.ErroreGenerico;
                response.MessaggioErrore = ex.ToString();
            }
            //Controlla se è tutto ok.
            response.CheckResponse();
            //Torna al chiamante.
            logger.DebugFormat("InfoAllegatiDocumento response:{0}", response.ToString());

            return response;
        }
       
        /// <summary>
        /// Lista di allegati collegati con il documento ottenuto tramite chiave.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static InfoAllegatiChiaveResponse InfoAllegatiDocumentoChiave(InfoAllegatiChiaveRequest request)
        {
            var response = new InfoAllegatiChiaveResponse { Eseguito = false, CodiceEsito = CodiceErrore.NessunErrore };

            try
            {
                logger.DebugFormat("InfoAllegatiDocumento request:{0}", request.ToString());
                //Validazione token usato per la login.
                response.TokenInfo = Helpers.ValidaToken(request);
                //Viene controllato se il token è scaduto o non trovato.
                if (response.TokenInfo == null)
                    response.CodiceEsito = CodiceErrore.TokenNonValidoOScaduto;
                else
                {
                    //Verifico che la request sia corretta
                    var checkRequestResult = request.CheckRequest(response);
                    if (checkRequestResult != CodiceErrore.NessunErrore)
                    {
                        response.CodiceEsito = checkRequestResult;
                    }
                    else
                    {
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
                            InfoAllegatiDocumento(idDocumento.Value, response);
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
            //Controlla se è tutto ok.
            response.CheckResponse();
            //Torna al chiamante.
            logger.DebugFormat("InfoAllegatiDocumento response:{0}", response.ToString());

            return response;
        }
      
        /// <summary>
        /// Contenuto (byte array) dell'allegato a partire dall'idattch
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static ImmagineAllegatoDocumentoResponse ImmagineAllegatoDocumento(ImmagineAllegatoDocumentoRequest request)
        {
            var response = new ImmagineAllegatoDocumentoResponse();
            try
            {
                logger.DebugFormat("ImmagineAllegatoDocumento request:{0}", request.ToString());
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
                        if (request.IdAllegato == Guid.Empty)
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
                                    var res = client.GetDocumentAttachContent(request.IdAllegato);
                                    response.File = new FileItem { Blob = res.Blob, Nome = res.Description };
                                    response.Eseguito = true;
                                    response.Allegato = new AllegatoItem { IdAllegato = request.IdAllegato, Nome = res.Description, Stato = Operazione.Default };
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
            logger.DebugFormat("ImmagineAllegatoDocumento response:{0}", response.ToString());
            response.CheckResponse();
            return response;
        }

        /// <summary>
        /// Metodo per cancellare un allegato del documento dato il suo ID.
        /// </summary>
        /// <remarks>
        /// E' possibile cancellare un allegato UNICAMENTE usando il suo apposito identificativo (membro "IdAllegato" della request): non
        /// lo si può infatti eliminare sfruttando la chiave del documento cui appartiene.
        /// </remarks>
        /// <param name="request">
        /// I parametri obbligatori perchè la richiesta risulti corretta sono:
        ///     - IdClient
        ///     - IdCliente
        ///     - TipoDocumento
        ///     - Token
        ///     - IdAllegato
        /// </param>
        /// <returns></returns>
        /// <example>
        /// var request = new CancellaAllegatoDocumentoRequest
        ///    {
        ///        IdClient = "Terasoftware",
        ///        IdCliente = "TeraSoftware",
        ///        TipoDocumento = "DDT_ATH_TEST2",
        ///        Token = Token = DocumentoFacade.Login(new LoginRequest { UserName = "Tera", Password = "Software", IdCliente = "TeraSoftware" }).TokenInfo.Token,
        ///        IdDocumento = new Guid("e3afd62f-0a23-4257-9e09-3c19ea8c497c")
        ///        IdAllegato = new Guid("057136A3-7B5E-4658-A144-774F86021D38"),
        ///    };
        /// 
        /// try {
        ///     var response = CancellaAllegatoDocumento(request);
        ///     if(!response.Eseguito || response.CodiceEsito != CodiceErrore.NessunErrore)
        ///         throw new Exception("Cancellazione allegato non eseguita. Identificativo allegato: " + request.IdAllegato);
        /// } catch(Exception exx) {
        ///     //Qualcosa è andato storto. 
        ///     //TODO: Eventuale gestione eccezioni (per esempio loggare il messaggio d'errore).
        ///     throw; //Rilancia l'eccezione.
        /// }
        /// </example>
        public static CancellaAllegatoDocumentoResponse CancellaAllegatoDocumento(CancellaAllegatoDocumentoRequest request)
        {
            var response = new CancellaAllegatoDocumentoResponse();
            try
            {
                logger.DebugFormat("CancellaAllegatoDocumento request:{0}", request.ToString());
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
                        if (request.IdAllegato == Guid.Empty)
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
                                    var allegati = client.GetDocumentAttachs(request.IdDocumento);
                                    if (allegati.Any(x => x.IdDocumentAttach == request.IdAllegato))
                                    {
                                        client.SetVisibleDocumentAttach(request.IdAllegato, false);
                                        response.Eseguito = true;
                                    }
                                    else
                                    {
                                        response.CodiceEsito = CodiceErrore.IdAllegatoNonValido;
                                        response.Eseguito = false;
                                    }
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
            logger.DebugFormat("CancellaAllegatoDocumento response:{0}", response.ToString());
            response.CheckResponse();
            return response;
        } 

        /// <summary>
        /// Contenuto (array di bytes) dell'allegato.
        /// </summary>
        /// <param name="request">Informazioni inerenti all'allegato.</param>
        /// <returns>Esito operazione e contenuto dell'allegato.</returns>
        protected static void AggiungiAllegato(Guid idDocumento, FileItem allegato, AggiungiAllegatoResponse response)
        {
            response.Eseguito = false;
            response.CodiceEsito = CodiceErrore.NessunErrore;

            if (allegato == null) //L'allegato è nullo?
            {
                response.CodiceEsito = CodiceErrore.AllegatoNonPresente;
                response.Eseguito = false;
            }
            else
            {
                if (allegato.Blob == null || allegato.Blob.Length < 1 || string.IsNullOrWhiteSpace(allegato.Nome)) //L'allegato non è valido: il file è lungo 0 bytes oppure il nome è nullo.
                {
                    response.CodiceEsito = CodiceErrore.AllegatoNonValido;
                }

                if (response.CodiceEsito == CodiceErrore.NessunErrore && idDocumento == Guid.Empty)
                {
                    response.CodiceEsito = CodiceErrore.IdDocumentoNonValido;
                }

                //TODO: eventuali altre validazioni qui.
            }
            //Procede solo se non ci sono stati errori prima di arrivare qui.
            if (response.CodiceEsito == CodiceErrore.NessunErrore)
            {
                try
                {
                    using (var client = new DocumentServiceReference.DocumentsClient())
                    {
                        var result = client.AddDocumentAttach(idDocumento, new DocumentAttach { Content = new DocumentContent(allegato.Blob, allegato.Gruppo), Name = allegato.Nome, IdDocument = idDocumento });
                        if (result != null)
                        {
                            client.ConfirmDocumentAttach(result.IdDocumentAttach);
                            response.Eseguito = true;
                        }
                    }
                }
                catch (FaultException<BiblosDsException> faultEx)
                {
                    logger.Error(faultEx);
                    ParseBiblosDSFaultException(response, faultEx);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        protected static void InfoAllegatiDocumento(Guid idDocumento, InfoAllegatiResponse response)
        {
            response.Eseguito = false;
            response.CodiceEsito = CodiceErrore.NessunErrore;

            if (idDocumento == Guid.Empty)
            {
                response.CodiceEsito = CodiceErrore.IdDocumentoNonValido;
            }
            else
            {
                try
                {
                    using (var client = new DocumentServiceReference.DocumentsClient())
                    {
                        var allegati = client.GetDocumentAttachs(idDocumento);
                        response.Allegati = allegati.Select(x => new AllegatoItem { IdAllegato = x.IdDocumentAttach, Nome = x.Name }).ToList();
                        response.Eseguito = true;
                    }
                }
                catch (FaultException<BiblosDsException> faultEx)
                {
                    logger.Error(faultEx);
                    ParseBiblosDSFaultException(response, faultEx);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
