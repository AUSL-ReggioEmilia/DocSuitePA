using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.JeepService.PosteWeb.TolService;
using VecompSoftware.Services.Logging;
using Recipient = VecompSoftware.JeepService.PosteWeb.TolService.Recipient;

namespace VecompSoftware.JeepService.PosteWeb.Service
{
    public class WsTelegramma
    {
        #region [ Fields ]

        private const int DestinatariLimit = 999;
        private readonly WCFTelegrammiService _tolService;
        private readonly PosteWebParameters _parameters;

        #endregion

        #region [ Constructors ]

        public WsTelegramma(string userName, string password, X509Certificate certificate, string url, double timeoutMilliseconds, PosteWebParameters parameters)
        {
            var objNetworkCredential = new NetworkCredential
            {
                UserName = userName,
                Password = password
            };

            _tolService = new WCFTelegrammiService
            {
                Credentials = objNetworkCredential,
                Url = url
            };
            _tolService.ClientCertificates.Add(certificate);
            _tolService.Timeout = (int)timeoutMilliseconds;
            _parameters = parameters;
        }

        #endregion

        #region [ Methods ]

        public string RecuperaIdRichiesta()
        {
            string idRequest = _tolService.GetIdRequest();
            if (string.IsNullOrEmpty(idRequest))
            {
                throw new PosteWebException("Impossibile creare richiesta telegramma.");
            }

            return idRequest;
        }

        public bool RecipientsValidation(string idRequest, Recipient[] recipients)
        {
            RecipientValidationResult[] validations = _tolService.RecipientsValidation(recipients, idRequest);

            // Controllo i risultati della validazione
            var errata = new List<string>();
            foreach (var validation in validations)
            {
                if (validation.Result.ResType == CEResultResType.I)
                {
                    continue;
                }

                foreach (var recipient in validation.Destinatari)
                {
                    var name = string.IsNullOrWhiteSpace(recipient.destinatario.RagioneSociale)
                        ? recipient.destinatario.Nome + " " + recipient.destinatario.Cognome
                        : recipient.destinatario.RagioneSociale;

                    errata.Add(string.Format("[{0}:{1}]", validation.Result.Description, name));
                }
            }

            if (errata.Count > 0)
            {
                throw new PosteWebException("Impossibile validare i destinatari: {0}.", string.Join(",", errata));
            }
            return true;
        }

        /// <summary> Invia la richiesta di spedizione del telegramma. </summary>
        /// <returns> Stringa contenente la descrizione dell'invio andato a buon fine. </returns>
        public string Send(string idRequest, ref Telegramma telegramma, string customer)
        {
            SubmitResult res = _tolService.Submit(ref telegramma, customer, idRequest);

            if (res == null || res.Result == null)
            {
                throw new PosteWebException("Impossibile inviare: Oggetto di ritorno vuoto.");
            }

            if (res.Result.ResType == CEResultResType.E)
            {
                throw new PosteWebException("Impossibile inviare: ({0}) - {1}", res.Result.ResType, res.Result.Description);
            }

            return res.Result.Description;
        }

        public PreconfirmResult Confirm(string idRequest)
        {
            PreconfirmResult res = _tolService.PreConfirm(new[] { idRequest }, true, true, true, true);

            if (res == null)
            {
                throw new PosteWebException("Errore preconferma: Oggetto di ritorno nullo.");
            }

            if (res.Result.ResType != CEResultResType.I)
            {
                throw new PosteWebException("Errore preconferma: ({0}) - {1}.", res.Result.ResType, res.Result.Description);
            }
            return res;
        }

        /// <summary> Crea il telegramma utile a fare la richiesta. </summary>
        /// <remarks> Questo oggetto, una volta ritornato dal WS, conterrà anche tutti i dati di ritorno. </remarks>
        public Telegramma CreaTelegramma(TOLRequest telegramma, int truncateAddress)
        {
            var telegram = new Telegramma
            {
                DataTelegramma = DateTime.Now,
                Firma = "",
                GUIDMessage = "",
                Mittente = CreaMittente(telegramma.Sender),
                Destinatari = CreaDestinatari(telegramma.Recipients, truncateAddress),
                Opzioni = new Opzioni { CTA = false, Note = "" },
                Nazionale = true,
                PartiTesto = new InfoTesto { NumeroParteCorrente = 1, Testo = telegramma.Testo },
                Valorizzazione = new Valorizzazione()
            };

            return telegram;
        }

        private TelegrammaDestinatario[] CreaDestinatari(IList<POLRequestRecipient> contacts, int truncateAddress)
        {
            if (contacts.Count >= DestinatariLimit)
            {
                throw new PosteWebException("Il telegramma ha [{0}] destinatari, massimo {1}.", contacts.Count, DestinatariLimit);
            }

            var destinatari = new List<TelegrammaDestinatario>(contacts.Count);

            int index = 0;
            foreach (var contact in contacts)
            {
                destinatari.Add(new TelegrammaDestinatario
                {
                    Destinatario = CreaDestinatario(contact, truncateAddress),
                    Frazionario = "",
                    IDTelegramma = "",
                    LineaPilota = "",
                    NumeroDestinatarioCorrente = index++,
                    TipoRec = TelegrammaDestinatarioTipoRec.Item
                });
            }

            return destinatari.ToArray();
        }

        public Recipient[] BuildRecipients(IList<POLRequestRecipient> recipients, int truncateAddress)
        {
            var toReturn = new List<Recipient>(recipients.Count);
            foreach (var recipient in recipients)
            {
                toReturn.Add(new Recipient
                {
                    ClientIDRecipient = recipient.Id.ToString(),
                    destinatario = CreaDestinatario(recipient, truncateAddress),
                    Provincia = recipient.Province
                });
            }
            return toReturn.ToArray();
        }

        private Destinatario CreaDestinatario(POLRequestRecipient contact, int truncateAddress)
        {
            var recipient = new Destinatario
            {
                Nome = "",
                Cognome = "",
                RagioneSociale = contact.Name.Truncate(41),
                Telefono = contact.PhoneNumber.Truncate(12),
                Indirizzo = String.Format("{0} {1} ", contact.Address, contact.CivicNumber).Truncate(truncateAddress),
                Citta = contact.City.Truncate(36),
                CAP = contact.ZipCode.Truncate(5),
                Stato = "Italia"
            };

            return recipient;
        }

        private Mittente CreaMittente(POLRequestContact contact)
        {
            var sender = new Mittente
            {
                Nome = "",
                Cognome = "",
                RagioneSociale = contact.Name.Truncate(41),
                Telefono = contact.PhoneNumber.Truncate(12),
                InvioAlMittente = _parameters.SendTOLToSender,
                Indirizzo = String.Format("{0} {1} ", contact.Address, contact.CivicNumber).Truncate(41),
                Citta = contact.City.Truncate(36),
                CAP = contact.ZipCode
            };

            return sender;
        }

        #endregion
    }
}

