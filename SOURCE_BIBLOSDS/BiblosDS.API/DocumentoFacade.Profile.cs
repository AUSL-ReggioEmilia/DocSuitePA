using BiblosDS.API.DocumentServiceReference;
using BiblosDS.API.Model;
using BiblosDS.Library.Common.Exceptions;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.UtilityService;
using System;
using System.Linq;
using System.ServiceModel;

namespace BiblosDS.API
{
    public partial class DocumentoFacade
    {
        /// <summary>
        /// Metodo che torna gli archivi associati ad un cliente con i relativi metadati
        /// </summary>
        /// <param name="request"><see cref="GetArchiviRequest">GetProfiliRequest</see></param>
        /// <example>
        /// GetProfiliRequest request = new GetProfiliRequest {  Token = token.TokenInfo.Token, IdCliente = token.TokenInfo.IdCliente };
        /// GetProfiliResponse actual = DocumentoFacade.GetProfili(request);
        /// </example>
        /// <remarks>
        /// Il Token è recuperato della chiamata al metodo Login
        /// </remarks>
        /// <returns>
        /// Risposta <see cref="GetArchiviResponse">GetProfiliResponse</see> con i profili configurati per il cliente
        /// </returns>
        public static GetArchiviResponse GetProfili(GetArchiviRequest request)
        {
            GetArchiviResponse response = new GetArchiviResponse();
            try
            {
                logger.DebugFormat("GetProfili request:{0}", request.ToString());
                response.TokenInfo = Helpers.ValidaToken(request);
                if (response.TokenInfo == null)
                {
                    response.CodiceEsito = CodiceErrore.TokenNonValidoOScaduto;
                }
                else
                {
                    using (var db = new BiblosDS2010APIEntities())
                    {
                        var archiviAbillitati = db.CustomerKeys.Where(x => x.IdCustomer == response.TokenInfo.IdCliente);
                        using (var client = new DocumentsClient())
                        {
                            foreach (var item in archiviAbillitati)
                            {
                                try
                                {
                                    var metadata = client.GetMetadataStructure(item.IdArchive);
                                    DocumentArchive archive = null;
                                    string nomeArchivio = string.Empty;
                                    if (metadata.Count() > 0)
                                    {
                                        archive = metadata.First().Archive;
                                        nomeArchivio = archive == null ? "ND" : archive.Name;
                                    }
                                    response.Archivi.Add(new Archivio { Descrizione = item.Description, TipoDocumento = item.DocumentClass, Nome = nomeArchivio, Metadati = metadata.Select(x => new MetadatoItem { Name = x.Name, Tipo = x.AttributeType, Posizione = x.ConservationPosition.GetValueOrDefault(), Obbligatorio = x.IsRequired }).ToList() });
                                    response.Eseguito = true;
                                    logger.DebugFormat("GetProfili response:{0}", response.ToString());
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
                                    response.CodiceEsito = CodiceErrore.ErroreChiamataAlServizioRemoto;
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
                response.CodiceEsito = CodiceErrore.ErroreGenerico;
                response.MessaggioErrore = ex.ToString();
            }
            response.CheckResponse();
            return response;
        }

        /// <summary>
        /// Metodo di autenticazione a biblos tramite Username e Password
        /// </summary>
        /// <param name="request"><see cref="LoginRequest">LoginRequest</see></param>
        /// <returns><see cref="LoginResponse">LoginResponse</see></returns>
        public static LoginResponse Login(LoginRequest request)
        {
            LoginResponse response = new LoginResponse();
            try
            {
                logger.Debug("Login request");
                if (string.IsNullOrEmpty(request.IdCliente))
                {
                    response.CodiceEsito = CodiceErrore.IdClienteErrato;
                }
                else if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
                {
                    response.CodiceEsito = CodiceErrore.LoginNonValido;
                }
                else
                {
                    using (var db = new BiblosDS2010APIEntities())
                    {
                        string passwordEncrypted = PasswordService.GenerateHash(request.Password);
                        var userLogin = db.CustomerLogins.Where(x => x.IdCustomer == request.IdCliente && x.UserName == request.UserName && x.Password == passwordEncrypted).FirstOrDefault();
                        if (userLogin != null)
                        {
                            response.TokenInfo = new TokenInfo { Token = Guid.NewGuid(), DataScadenza = DateTime.Now.AddMinutes(20) };
                            db.LoginTokens.AddObject(new LoginToken { IdLoginToken = response.TokenInfo.Token, IdCustomerLogin = userLogin.IdCustomerLogin, IdCustomer = userLogin.IdCustomer, DateCreated = DateTime.Now, DateExpire = response.TokenInfo.DataScadenza });
                            db.SaveChanges();
                            response.Eseguito = true;
                        }
                        else
                        {
                            response.CodiceEsito = CodiceErrore.LoginNonValido;
                        }
                    }
                }
                logger.DebugFormat("Login response:{0}", response.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                response.CodiceEsito = CodiceErrore.ErroreGenerico;
                response.MessaggioErrore = ex.ToString();
            }
            return response;
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <param name="token">Id del token di autenticazione</param>
        public static void Logout(Guid token)
        {
            try
            {
                logger.DebugFormat("Login Logout:{0}", token);
                using (var db = new BiblosDS2010APIEntities())
                {
                    var userLogin = db.LoginTokens.Where(x => x.IdLoginToken == token).FirstOrDefault();
                    if (userLogin != null)
                    {
                        db.DeleteObject(userLogin);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                //LogOut sempre valida non da mai eccezione
                //throw;
            }
        }
    }
}
