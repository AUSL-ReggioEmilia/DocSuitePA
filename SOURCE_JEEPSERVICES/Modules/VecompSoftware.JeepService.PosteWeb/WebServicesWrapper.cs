using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.JeepService.PosteWeb.Service;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;
using LolValidaDestinatariResult = VecompSoftware.JeepService.PosteWeb.LolService.ValidaDestinatariResult;

namespace VecompSoftware.JeepService.PosteWeb
{
    public class WebServicesWrapper
    {
        #region [ Fields ]

        static readonly TimeSpan TimeoutSpan = new TimeSpan(0, 30, 0);
        static Location _location;

        /// <summary> Nome del Appender sul quale loggare. </summary>
        private readonly string _name;

        private readonly WsRaccomandata _raccomandataWebService;
        private readonly WsLettera _letteraWebService;
        private readonly WsTelegramma _telegrammaWebService;
        private readonly PosteWebParameters _parameters;

        #endregion

        #region [ Properties ]
        private static Location PosteWebLocation
        {
            get
            {
                if (_location == null)
                {
                    _location = FacadeFactory.Instance.LocationFacade.GetById(DocSuiteContext.Current.ProtocolEnv.PosteWebRequestLocation);
                }
                return _location;
            }
        }
        #endregion

        #region [ Constructor ]

        public WebServicesWrapper(POLAccount account, string name, PosteWebParameters parameters)
        {
            _name = name;

            string completePath;
            if (!Path.IsPathRooted(account.X509Certificate))
            {
                // TODO: mettere metodo per la ricerca della directory corretta nel modulebase
                string assemblyFolder = Path.GetDirectoryName(GetType().Assembly.Location);
                if (string.IsNullOrEmpty(assemblyFolder))
                {
                    string localPath = new Uri(GetType().Assembly.CodeBase).LocalPath;
                    assemblyFolder = Path.GetDirectoryName(localPath);
                }
                if (string.IsNullOrEmpty(assemblyFolder))
                {
                    throw new PosteWebException("Impossibile determinare il percorso corretto dell'assembly.");
                }
                completePath = Path.Combine(assemblyFolder, account.X509Certificate);
            }
            else
            {
                completePath = account.X509Certificate;
            }
            if (!File.Exists(completePath))
            {
                throw new FileNotFoundException(string.Format("Impossibile caricare il certificato dal percorso [{0}].", completePath), completePath);
            }
            var certificate = X509Certificate.CreateFromCertFile(completePath);
            _parameters = parameters;

            _raccomandataWebService = new WsRaccomandata(account.Username, account.Password, certificate, account.WsRaccomandataUrl, TimeoutSpan.TotalMilliseconds, _parameters);
            _letteraWebService = new WsLettera(account.Username, account.Password, certificate, account.WsLetteraUrl, TimeoutSpan.TotalMilliseconds, _parameters);
            _telegrammaWebService = new WsTelegramma(account.Username, account.Password, certificate, account.WsTelegrammaUrl, TimeoutSpan.TotalMilliseconds, _parameters);

        }

        #endregion

        #region [ Methods ]

        public void LetteraSendRequest(LOLRequest lettera)
        {
            try
            {
                // Recupero l'id della richiesta alle poste web
                lettera.IdRichiesta = _letteraWebService.GetIdRichiesta(lettera);
                lettera.StatusDescrition = "Recuperato IdRichiesta";
                FileLogger.Info(_name, string.Format("Ricevuta IdRichiesta: [{0}].", lettera.IdRichiesta));

                // Valido i destinatari
                if (lettera.Recipients.Count <= 0)
                {
                    throw new PosteWebException("Lettera [{0}] senza destinatari.", lettera.Id);
                }
                var recipients = _letteraWebService.CreaDestinatari(lettera.Recipients);
                if (_letteraWebService.RecipientsValidation(lettera.IdRichiesta, recipients))
                {
                    lettera.StatusDescrition = "Destinatari Validati";
                    FileLogger.Info(_name, "Tutti i destinatari sono stati validati.");
                }
                else
                {
                    throw new PosteWebException("Caso validazione lettera non previsto.");
                }

                // Invio della richiesta a poste web
                var result = _letteraWebService.Send(lettera.IdRichiesta, lettera.Account.Customer, _letteraWebService.CreaSubmit(lettera));
                lettera.GuidPoste = result.GuidUtente;
                foreach (LolValidaDestinatariResult validation in result.LOLResponse.ValidaDestinatariResults.DatiDestinatari)
                {
                    POLRequestRecipient rcp = lettera.Recipients.FirstOrDefault(x => x.Id.ToString().Eq(validation.Destinatari[0].IdDestinatario));
                    if (rcp != null)
                    {
                        rcp.IdRicevuta = validation.Destinatari[0].IdRicevuta;
                        rcp.Status = POLMessageContactEnum.Created;
                        rcp.StatusDescrition = "In attesa di Conferma";
                    }
                }

                lettera.Status = POLRequestStatusEnum.RequestSent;
                lettera.StatusDescrition = "Richiesta inviata correttamente";

                FileLogger.Info(_name, "Lettera Presa in Carico");
            }
            catch (PosteWebException exception)
            {
                lettera.SetError(exception.Message);
                SetTaskError(lettera);
                FileLogger.Error(_name, exception.Message);
            }
        }

        public void LetteraCheckRequest(LOLRequest lettera)
        {
            try
            {
                var enquiry = new LolService.ServizioSingleEnquirySubmit
                {
                    RestituisciDocumento = true,
                    Richiesta = WsLettera.InstantiateRequest(lettera)
                };
                var res = _letteraWebService.CheckRequest(enquiry);

                // Controllo stato
                if (ValorizzaStatoLavorazione.ErrorStatus.Contains(res.ServizioEnquiryResponse.StatoLavorazione.Id[0]))
                {
                    throw new PosteWebException("Richiesta non validata da Poste Online: {0}.", res.ServizioEnquiryResponse.StatoLavorazione.Descrizione);
                }
                if (ValorizzaStatoLavorazione.OkStatus.Contains(res.ServizioEnquiryResponse.StatoLavorazione.Id[0]))
                {
                    lettera.Status = POLRequestStatusEnum.NeedConfirm;
                    lettera.StatusDescrition = "Validata in attesa di conferma";
                }
                else
                {
                    // Lettera ancora transitoria
                    lettera.StatusDescrition = res.ServizioEnquiryResponse.StatoLavorazione.Descrizione;
                }
                FileLogger.Info(Tools.ModuleName, string.Format("Stato lavorazione lettera: {0} - {1}", res.ServizioEnquiryResponse.StatoLavorazione.Id, res.ServizioEnquiryResponse.StatoLavorazione.Descrizione));

                // Controllo del prezzo
                if (res.ServizioEnquiryResponse.OggettoValorizzazione.Length > 0 && res.ServizioEnquiryResponse.OggettoValorizzazione[0].Totale != null)
                {
                    if (res.ServizioEnquiryResponse.OggettoValorizzazione.Length > 1)
                    {
                        FileLogger.Warn(Tools.ModuleName, string.Format("IdRichiesta [{0}] Guid Poste [{1}] ha troppi OggettoValorizzazione, CONTROLLARE!", res.ServizioEnquiryResponse.Richiesta.IDRichiesta, res.ServizioEnquiryResponse.Richiesta.GuidUtente));
                    }

                    lettera.CostoTotale = res.ServizioEnquiryResponse.Totale.ImportoTotale;
                    FileLogger.Info(Tools.ModuleName, string.Format("Costo totale lettera: {0}.", lettera.CostoTotale));
                    // Imposto il prezzo per ogni raccomandata spedita
                    if (lettera.CostoTotale > 0)
                    {
                        double costoUnitario = lettera.CostoTotale / lettera.Recipients.Count;
                        foreach (POLRequestRecipient rec in lettera.Recipients)
                        {
                            rec.Costo = costoUnitario;
                        }
                    }
                }
                FileLogger.Info(Tools.ModuleName, "Recupero prezzo andato a buon fine.");

                // Controllo del documento
                lettera.IdArchiveChainPoste = SaveDocumentInBiblos(res.Documento.Immagine);
                lettera.DocumentPosteMD5 = res.Documento.MD5;
                lettera.DocumentPosteFileType = res.Documento.TipoDocumento;
#if DEBUG
                if (!string.IsNullOrEmpty(Convert.ToBase64String(res.Documento.Immagine)))
                {
                    using (var fs = new FileStream(@"C:\DocumentPosteStream_Lettera.pdf", FileMode.Create))
                    {
                        byte[] cb = res.Documento.Immagine;
                        fs.Write(cb, 0, cb.Length);
                        fs.Close();
                    }
                }
#endif
                FileLogger.Info(Tools.ModuleName, "Recupero del documento della lettera eseguito.");

            }
            catch (PosteWebException exception)
            {
                lettera.SetError(exception.Message);
                SetTaskError(lettera);
                FileLogger.Error(_name, exception.Message);
            }
        }

        public void LetteraConfirm(LOLRequest lettera)
        {
            try
            {
                LolService.Richiesta[] requests = { WsLettera.InstantiateRequest(lettera) };
                var result = _letteraWebService.Confirm(requests);

                lettera.IdOrdine = result.IdOrdine;

                if (result.DestinatariLettera != null)
                {
                    IFormatProvider culture = new CultureInfo("it-IT", true);

                    foreach (var dest in result.DestinatariLettera)
                    {
                        POLRequestRecipient rcp = lettera.Recipients.FirstOrDefault(x => x.IdRicevuta.Eq(dest.IdRicevuta));
                        if (rcp != null)
                        {
                            rcp.Status = POLMessageContactEnum.Confirmed;
                            rcp.StatusDescrition = "Lettera Spedita";

                            if (!string.IsNullOrEmpty(dest.EPM.TimeStamp))
                            {
                                rcp.DataSpedizione = DateTime.Parse(dest.EPM.TimeStamp, culture, DateTimeStyles.NoCurrentDateDefault);
                            }
                        }
                    }
                }

                lettera.Status = POLRequestStatusEnum.Executed;
                lettera.StatusDescrition = "Lettera Spedita";

                FileLogger.Info(Tools.ModuleName, "PreConferma & Conferma lettera eseguita.");
            }
            catch (PosteWebException exception)
            {
                lettera.SetError(exception.Message);
                SetTaskError(lettera);
                FileLogger.Error(_name, exception.Message);
            }
        }

        public void RaccomandataSendRequest(ROLRequest raccomandata)
        {
            try
            {
                // Recupero l'id della richiesta alle poste web
                raccomandata.IdRichiesta = _raccomandataWebService.GetIdRichiesta();
                raccomandata.StatusDescrition = "Recuperato IdRichiesta";
                FileLogger.Info(Tools.ModuleName, string.Format("Ricevuta IdRichiesta: [{0}].", raccomandata.IdRichiesta));

                // Valido i destinatari
                if (raccomandata.Recipients.Count <= 0)
                {
                    throw new PosteWebException("Raccomandata [{0}] senza destinatari.", raccomandata.Id);
                }
                var recipients = _raccomandataWebService.CreaDestinatari(raccomandata.Recipients);
                if (_raccomandataWebService.RecipientsValidation(raccomandata.IdRichiesta, recipients))
                {
                    raccomandata.StatusDescrition = "Destinatari Validati";
                    FileLogger.Info(Tools.ModuleName, "Tutti i destinatari sono stati validati");
                }
                else
                {
                    throw new PosteWebException("Caso validazione raccomandata non previsto.");
                }

                // Invio della richiesta a poste web
                var result = _raccomandataWebService.Send(raccomandata.IdRichiesta, raccomandata.Account.Customer, _raccomandataWebService.CreaSubmit(raccomandata));
                raccomandata.GuidPoste = result.GuidUtente;
                foreach (var validation in result.ROLResponse.ValidaDestinatariResults.DatiDestinatari)
                {
                    POLRequestRecipient rcp = raccomandata.Recipients.FirstOrDefault(x => x.Id.ToString().Eq(validation.Destinatari[0].IdDestinatario));
                    if (rcp != null)
                    {
                        rcp.IdRicevuta = validation.Destinatari[0].IdRicevuta;
                        rcp.Status = POLMessageContactEnum.Created;
                        rcp.StatusDescrition = "In attesa di Conferma";
                    }
                }

                raccomandata.Status = POLRequestStatusEnum.RequestSent;
                raccomandata.StatusDescrition = "Richiesta inviata correttamente";

                FileLogger.Info(Tools.ModuleName, "Raccomandata Presa in Carico");
            }
            catch (PosteWebException exception)
            {
                raccomandata.SetError(exception.Message);
                SetTaskError(raccomandata);
                FileLogger.Error(_name, exception.Message);
            }
        }

        public void RaccomandataCheckRequest(ROLRequest raccomandata)
        {
            try
            {
                var enquiry = new RolService.ServizioSingleEnquirySubmit
                {
                    RestituisciDocumento = true,
                    Richiesta = WsRaccomandata.InstantiateRequest(raccomandata)
                };
                var res = _raccomandataWebService.CheckRequest(enquiry);

                // Controllo stato
                if (ValorizzaStatoLavorazione.ErrorStatus.Contains(res.ServizioEnquiryResponse.StatoLavorazione.Id[0]))
                {
                    throw new PosteWebException("Richiesta non validata da Poste Online: {0}.", res.ServizioEnquiryResponse.StatoLavorazione.Descrizione);
                }
                if (ValorizzaStatoLavorazione.OkStatus.Contains(res.ServizioEnquiryResponse.StatoLavorazione.Id[0]))
                {
                    raccomandata.Status = POLRequestStatusEnum.NeedConfirm;
                    raccomandata.StatusDescrition = "Validata in attesa di conferma";
                }
                else
                {
                    // Raccomandata ancora transitoria
                    raccomandata.StatusDescrition = res.ServizioEnquiryResponse.StatoLavorazione.Descrizione;
                }
                FileLogger.Info(Tools.ModuleName, string.Format("Stato lavorazione raccomandata: {0} - {1}", res.ServizioEnquiryResponse.StatoLavorazione.Id, res.ServizioEnquiryResponse.StatoLavorazione.Descrizione));

                // Controllo del prezzo
                if (res.ServizioEnquiryResponse.OggettoValorizzazione.Length > 0 && res.ServizioEnquiryResponse.OggettoValorizzazione[0].Totale != null)
                {
                    if (res.ServizioEnquiryResponse.OggettoValorizzazione.Length > 1)
                    {
                        FileLogger.Warn(Tools.ModuleName, string.Format("IdRichiesta [{0}] Guid Poste [{1}] ha troppi OggettoValorizzazione, CONTROLLARE!", res.ServizioEnquiryResponse.Richiesta.IDRichiesta, res.ServizioEnquiryResponse.Richiesta.GuidUtente));
                    }

                    raccomandata.CostoTotale = res.ServizioEnquiryResponse.Totale.ImportoTotale;
                    FileLogger.Info(Tools.ModuleName, string.Format("Costo totale raccomandata: {0}.", raccomandata.CostoTotale));
                    // Imposto il prezzo per ogni raccomandata spedita
                    if (raccomandata.CostoTotale > 0)
                    {
                        double costoUnitario = raccomandata.CostoTotale / raccomandata.Recipients.Count;
                        foreach (POLRequestRecipient rec in raccomandata.Recipients)
                        {
                            rec.Costo = costoUnitario;
                        }
                    }
                }
                FileLogger.Info(Tools.ModuleName, "Recupero prezzo andato a buon fine.");

                // Controllo del documento
                raccomandata.IdArchiveChainPoste = SaveDocumentInBiblos(res.Documento.Immagine);
                raccomandata.DocumentPosteMD5 = res.Documento.MD5;
                raccomandata.DocumentPosteFileType = res.Documento.TipoDocumento;
#if DEBUG
                if (!string.IsNullOrEmpty(Convert.ToBase64String(res.Documento.Immagine)))
                {
                    using (var fs = new FileStream(@"C:\DocumentPosteStream_Raccomandata.pdf", FileMode.Create))
                    {
                        byte[] cb = res.Documento.Immagine;
                        fs.Write(cb, 0, cb.Length);
                        fs.Close();
                    }
                }
#endif
                FileLogger.Info(Tools.ModuleName, "Recupero del documento della raccomandata eseguita.");
            }
            catch (PosteWebException exception)
            {
                raccomandata.SetError(exception.Message);
                SetTaskError(raccomandata);
                FileLogger.Error(_name, exception.Message);
            }
        }

        public void RaccomandataConfirm(ROLRequest raccomandata)
        {
            try
            {
                RolService.Richiesta[] requests = { WsRaccomandata.InstantiateRequest(raccomandata) };
                var result = _raccomandataWebService.Confirm(requests);

                raccomandata.IdOrdine = result.IdOrdine;

                if (result.DestinatariRaccomandata != null)
                {
                    IFormatProvider culture = new CultureInfo("it-IT", true);

                    foreach (var dest in result.DestinatariRaccomandata)
                    {
                        POLRequestRecipient rcp = raccomandata.Recipients.FirstOrDefault(x => x.IdRicevuta.Eq(dest.IdRicevuta));
                        if (rcp != null)
                        {
                            rcp.Status = POLMessageContactEnum.Confirmed;
                            rcp.StatusDescrition = "In Elaborazione dalle Poste";
                            rcp.Numero = dest.NumeroRaccomandata;
                            if (!string.IsNullOrEmpty(dest.EPM.TimeStamp))
                            {
                                rcp.DataSpedizione = DateTime.Parse(dest.EPM.TimeStamp, culture, DateTimeStyles.NoCurrentDateDefault);
                            }
                        }
                    }
                }

                raccomandata.Status = POLRequestStatusEnum.Confirmed;
                raccomandata.StatusDescrition = "Richiesta Confermata";

                FileLogger.Info(Tools.ModuleName, "PreConferma & Conferma raccomandata eseguita.");
            }
            catch (PosteWebException exception)
            {
                raccomandata.SetError(exception.Message);
                SetTaskError(raccomandata);
                FileLogger.Error(_name, exception.Message);
            }
        }

        public void RaccomandateGetStatus(IList<ROLRequest> raccomandate)
        {
            try
            {
                var requests = WsRaccomandata.InstantiateRequests(raccomandate);
                var res = _raccomandataWebService.StatoInviiPerId(requests);

                if (res.ArrayDiRichieste == null)
                {
                    FileLogger.Warn(Tools.ModuleName, "Nessuno stato ricevuto.");
                }

                // Ciclo le raccomandate
                foreach (var raccomandata in raccomandate)
                {
                    var recipientStatus = !res.ArrayDiRichieste.IsNullOrEmpty() ? res.ArrayDiRichieste.FirstOrDefault(x => x.IdRichiesta == raccomandata.IdRichiesta) : null;
                    if (recipientStatus == null)
                    {
                        raccomandata.Status = POLRequestStatusEnum.Confirmed;
                        raccomandata.StatusDescrition = "Confermata in attesa";
                        continue;
                    }

                    // Ciclo i destinatari
                    for (int index = 0; index < raccomandata.Recipients.Count; index++)
                    {
                        var recipient = raccomandata.Recipients[index];
                        // Estraggo la richiesta corretta
                        var statoRichiesta = recipientStatus.StatoRichieste.First(x => x.NumeroServizio.Eq(recipient.Numero));
                        if (statoRichiesta == null)
                        {
                            recipient.Status = POLMessageContactEnum.WorkingInProgress;
                            recipient.StatusDescrition = "Confermata in attesa.";
                            continue;
                        }

                        if (statoRichiesta.StatoType.Eq(RichiestaStatiLavorazione.Consegnato))
                        {
                            recipient.Status = POLMessageContactEnum.Received;
                        }
                        else if (statoRichiesta.StatoType.Eq(RichiestaStatiLavorazione.InRestituzione))
                        {
                            recipient.Status = POLMessageContactEnum.Rejected;
                        }
                        else
                        {
                            recipient.Status = POLMessageContactEnum.WorkingInProgress;
                        }
                        recipient.StatusDescrition = statoRichiesta.StatoDescrizione;
                    }

                    // Controllo se la raccomandata è da considerarsi completa
                    if (raccomandata.IsComplete())
                    {
                        raccomandata.Status = POLRequestStatusEnum.Executed;
                        raccomandata.StatusDescrition = "Raccomandata spedita";
                    }
                }

                FileLogger.Info(Tools.ModuleName, "Stato raccomandate ricevuto correttamente.");
            }
            catch (PosteWebException exception)
            {
                FileLogger.Error(_name, exception.Message);
            }
        }

        public void TelegrammaSend(TOLRequest telegramma, int truncateAddress)
        {
            try
            {
                // Recupero l'id della richiesta alle poste web
                telegramma.IdRichiesta = _telegrammaWebService.RecuperaIdRichiesta();
                telegramma.StatusDescrition = "Recuperato IdRichiesta.";
                FileLogger.Info(Tools.ModuleName, string.Format("Ricevuta IdRichiesta: [{0}].", telegramma.IdRichiesta));

                // Valido i destinatari
                if (telegramma.Recipients.Count <= 0)
                {
                    throw new PosteWebException("Telegramma [{0}] senza destinatari.", telegramma.Id);
                }
                var recipients = _telegrammaWebService.BuildRecipients(telegramma.Recipients, truncateAddress);
                if (_telegrammaWebService.RecipientsValidation(telegramma.IdRichiesta, recipients))
                {
                    FileLogger.Info(Tools.ModuleName, "Tutti i destinatari sono stati validati");
                }
                else
                {
                    throw new PosteWebException("Caso validazione telegramma non previsto.");
                }

                // Invio della richiesta a poste web
                var serviceTelegram = _telegrammaWebService.CreaTelegramma(telegramma, truncateAddress);
                _telegrammaWebService.Send(telegramma.IdRichiesta, ref serviceTelegram, telegramma.Account.Customer);
                for (int index = 0; index < telegramma.Recipients.Count; index++)
                {
                    var destinatario = serviceTelegram.Destinatari.FirstOrDefault(x => x.NumeroDestinatarioCorrente == index);
                    if (destinatario != null)
                    {
                        POLRequestRecipient rcp = telegramma.Recipients[index];
                        rcp.Numero = destinatario.IDTelegramma;
                        rcp.Status = POLMessageContactEnum.Created;
                        rcp.StatusDescrition = "In attesa di Conferma";
                    }
                }

                IFormatProvider culture = new CultureInfo("en-US", true);
                telegramma.GuidPoste = serviceTelegram.GUIDMessage;
                telegramma.CostoTotale = Double.Parse(serviceTelegram.Valorizzazione.ImportoTotale, culture);
                telegramma.Status = POLRequestStatusEnum.RequestSent;
                telegramma.StatusDescrition = "Richiesta inviata correttamente";

                FileLogger.Info(Tools.ModuleName, string.Format("Telegramma preso in carico, costo totale: [{0}].", telegramma.CostoTotale));
            }
            catch (PosteWebException exception)
            {
                telegramma.SetError(exception.Message);
                SetTaskError(telegramma);
                FileLogger.Error(_name, exception.Message);
            }
        }

        public void TelegrammaConfirm(TOLRequest telegramma)
        {
            try
            {
                var res = _telegrammaWebService.Confirm(telegramma.IdRichiesta);

                telegramma.IdOrdine = res.OrderResponse.IdOrder;
                DateTime dataSpedizione = DateTime.Now;
                if (res.ConfirmOrderResponse != null && res.ConfirmOrderResponse.EPM != null)
                {
                    IFormatProvider culture = new CultureInfo("it-IT", true);
                    dataSpedizione = DateTime.Parse(res.ConfirmOrderResponse.EPM.TimeStamp, culture, DateTimeStyles.NoCurrentDateDefault);
                }

                if (res.ConfirmOrderResponse != null && res.ConfirmOrderResponse.TicketIds != null)
                {
                    foreach (var ticket in res.ConfirmOrderResponse.TicketIds)
                    {
                        int index;
                        if (!Int32.TryParse(ticket.ReceipientId, out index) || index >= telegramma.Recipients.Count)
                        {
                            continue;
                        }

                        POLRequestRecipient rcp = telegramma.Recipients[index];
                        rcp.DataSpedizione = dataSpedizione;
                        rcp.Status = POLMessageContactEnum.Confirmed;
                        rcp.StatusDescrition = "Telegramma Spedito";
                        rcp.Numero = ticket.ID;
                    }
                }

                telegramma.Status = POLRequestStatusEnum.Executed;
                telegramma.StatusDescrition = "Telegramma Spedito";

                FileLogger.Info(Tools.ModuleName, "Conferma telegramma eseguita.");
            }
            catch (PosteWebException exception)
            {
                telegramma.SetError(exception.Message);
                SetTaskError(telegramma);
                FileLogger.Error(_name, exception.Message);
            }
        }

        private Guid SaveDocumentInBiblos(byte[] document)
        {
            MemoryDocumentInfo documentToSave = new MemoryDocumentInfo(document, "DocumentoPosteStream.pdf");
            List<DocumentInfo> documentsToSave = new List<DocumentInfo>();
            documentsToSave.Add(documentToSave);
            BiblosChainInfo chain = new BiblosChainInfo(documentsToSave);

            return chain.ArchiveInBiblos(PosteWebLocation.DocumentServer, PosteWebLocation.ProtBiblosDSDB);

        }

        private void SetTaskError(POLRequest request)
        {
            TaskHeader taskHeader = FacadeFactory.Instance.TaskHeaderFacade.GetByPOL(request);
            if (taskHeader != null)
            {
                taskHeader.SendedStatus = TaskHeaderSendedStatus.Errors;
                FacadeFactory.Instance.TaskHeaderFacade.Update(ref taskHeader);
            }
        }

        public void CompleteRequestTask(POLRequest request)
        {
            TaskHeader taskHeader = FacadeFactory.Instance.TaskHeaderFacade.GetByPOL(request);
            if (taskHeader != null)
            {
                FacadeFactory.Instance.TaskHeaderFacade.CompleteTaskProcess(taskHeader);
            }
        }
        #endregion

    }
}
