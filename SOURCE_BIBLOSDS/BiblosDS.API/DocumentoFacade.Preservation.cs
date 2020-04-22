using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.API.Model;
using BiblosDS.API.DocumentServiceReference;
using BiblosDS.API.Exceptions;
using BiblosDS.Library.Common.Objects;
using System.ServiceModel;
using BiblosDS.Library.Common.Exceptions;
using BiblosDS.Library.Common.Objects.UtilityService;

namespace BiblosDS.API
{
    public partial class DocumentoFacade
    {        
        /// <summary>
        /// Esegue una verifica per controllare se il documento richiesto è stato conservato o meno, dato l'identificativo del documento (campo "IdDocumento" dell'oggetto Request).
        /// </summary>
        /// <remarks>La ricerca viene effettuata per ID, non per CHIAVE.</remarks>
        /// <param name="request">
        /// Proprietà obbligatorie per una corretta richiesta:
        ///  - IdDocumento impostato al GUID identificativo del documento archiviato in Biblos
        ///  - Token di accesso valido e non scaduto. Si intendono i seguenti campi valorizzati dell'oggetto Request: IdClient, IdCliente, TipoDocumento, Token.
        /// </param>
        /// <returns>
        /// Ritorna il flag "Conservato" dell'oggetto Response impostato a <code>TRUE</code> se il documento è conservato, <code>FALSE</code> altrimenti. 
        /// Inoltre vengono ritornati i metadati attualmente gestiti del documento, nel campo "StatoDocumento.ProprietaDocumento" dell'oggetto Response, in formato lista di oggetti 
        /// di tipo "MetadatoItem".
        /// </returns>
        /// <example>
        /// try {
        ///  var request = new VerificaStatoConservazioneDocumentoRequest
        ///     {
        ///         IdDocumento = new Guid("DBBB7CEB-5CCC-425D-AE48-7F4140F00B5B"),
        ///         IdClient = "TeraSoftware",
        ///         IdCliente = "TeraSoftware",
        ///         TipoDocumento = "DDT_ATH_TEST2",
        ///         Token = DocumentoFacade.Login(new LoginRequest { UserName = "Tera", Password = "Software", IdCliente = "TeraSoftware" }).TokenInfo.Token,
        ///     };
        ///     
        ///  var response = DocumentoFacade.VerificaStatoConservazioneDocumento(request);
        ///  
        ///  if(response.CodiceEsito != CodiceErrore.NessunErrore) {
        ///     throw new Exception(response.CodiceEsito.ToString());
        ///  }
        ///  
        ///  var conservato = response.Conservato;
        ///  var metadati = (response.StatoDocumento != null) ? response.StatoDocumento.ProprietaDocumento : null;
        ///  
        /// } catch(Exception ex) {
        ///     //TODO: gestione eccezioni (es.: loggare il messaggio d'errore).
        ///     throw; //Rilancia l'eccezione.
        /// }
        /// </example>
        public static VerificaStatoConservazioneDocumentoResponse VerificaStatoConservazioneDocumento(VerificaStatoConservazioneDocumentoRequest request)
        {
            var response = new VerificaStatoConservazioneDocumentoResponse { StatoDocumento = new StatoDocumento() };
            try
            {                
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
                                using (var client = new DocumentsClient())
                                {
                                    try
                                    {                                                                             
                                        var documentInfo = client.GetDocumentInfoById(request.IdDocumento);
                                        CheckPreservationResponse(response, documentInfo);                                      
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
        /// Esegue una verifica per controllare se il documento richiesto è stato conservato o meno, data la chiave del documento (campo "Chiave" dell'oggetto Request).
        /// </summary>
        /// <remarks>La ricerca viene effettuata per CHIAVE, non per ID.</remarks>
        /// <param name="request">
        /// Proprietà obbligatorie per una corretta richiesta:
        ///  - Chiave (il codice del documento) usata per recuperare l'identificativo del documento archiviato in Biblos (campo "Chiave" dell'oggetto Request).
        ///  - Token di accesso valido e non scaduto. Si intendono i seguenti campi valorizzati dell'oggetto Request: IdClient, IdCliente, TipoDocumento, Token.
        /// </param>
        /// <returns>
        /// Ritorna il flag "Conservato" dell'oggetto Response impostato a <code>TRUE</code> se il documento è conservato, <code>FALSE</code> altrimenti. 
        /// Inoltre vengono ritornati i metadati attualmente gestiti del documento, nel campo "StatoDocumento.ProprietaDocumento" dell'oggetto Response, in formato lista di oggetti 
        /// di tipo "MetadatoItem".
        /// </returns>
        /// <example>
        /// try {
        ///  var request = new VerificaStatoConservazioneDocumentoRequest
        ///     {
        ///         IdClient = "TeraSoftware",
        ///         IdRichiesta = "20081128000001",
        ///         IdCliente = "TeraSoftware",
        ///         TipoDocumento = "DDT_ATH_TEST2",
        ///         Chiave = "IVA00155759",
        ///         Token = DocumentoFacade.Login(new LoginRequest { UserName = "Tera", Password = "Software", IdCliente = "TeraSoftware" }).TokenInfo.Token,
        ///     };
        ///     
        ///  var response = DocumentoFacade.VerificaStatoConservazioneDocumentoChiave(request);
        ///  
        ///  if(response.CodiceEsito != CodiceErrore.NessunErrore) {
        ///     throw new Exception(response.CodiceEsito.ToString());
        ///  }
        ///  
        ///  var conservato = response.Conservato;
        ///  var metadati = (response.StatoDocumento != null) ? response.StatoDocumento.ProprietaDocumento : null;
        ///  
        /// } catch(Exception ex) {
        ///     //TODO: gestione eccezioni (es.: loggare il messaggio d'errore).
        ///     throw; //Rilancia l'eccezione.
        /// }
        /// </example>
        public static VerificaStatoConservazioneDocumentoChiaveResponse VerificaStatoConservazioneDocumentoChiave(VerificaStatoConservazioneDocumentoChiaveRequest request)
        {
            VerificaStatoConservazioneDocumentoChiaveResponse response = new VerificaStatoConservazioneDocumentoChiaveResponse { StatoDocumento = new StatoDocumento() };
            try
            {
                logger.DebugFormat("VerificaStatoConservazioneDocumentoChiave request:{0}", request.ToString());

                response.TokenInfo = Helpers.ValidaToken(request);

                if (response.TokenInfo == null)
                    response.CodiceEsito = CodiceErrore.TokenNonValidoOScaduto;
                else
                {
                    //var checkRequestResult = request.CheckRequest(response);
                    var checkRequestResult = request.CheckRichiestaChiave(response, request.Chiave);

                    if (checkRequestResult != CodiceErrore.NessunErrore)
                    {
                        response.CodiceEsito = checkRequestResult;
                        response.Eseguito = false;
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
                            using (var client = new DocumentsClient())
                            {
                                try
                                {
                                    //TODO Controllare come mai non torna l'ultima versione                                        
                                    var documentInfo = client.GetDocumentInfoById(idDocumento.Value);                                   
                                    CheckPreservationResponse(response, documentInfo);
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
                response.CheckResponse();            

                logger.DebugFormat("VerificaStatoConservazioneDocumentoChiave response:{0}", response.ToString());
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

        private static void CheckPreservationResponse(VerificaStatoConservazioneDocumentoResponse response, Document documentInfo)
        {
            response.Eseguito = true;

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
            response.Conservato = documentInfo.IdPreservation.HasValue;
            if (documentInfo.Preservation != null)
            {
                if (documentInfo.Preservation.CloseDate.HasValue)
                    response.StatoDocumento.CodiceStato = StatoConservazione.STATODOC_ARCHIVIATO;
                else
                    response.StatoDocumento.CodiceStato = StatoConservazione.STATODOC_INCORSO;
            }
            else
                response.StatoDocumento.CodiceStato = response.Conservato ? StatoConservazione.STATODOC_ARCHIVIATO : StatoConservazione.STATODOC_NONARCHIVIATO;
        }
    }
}
