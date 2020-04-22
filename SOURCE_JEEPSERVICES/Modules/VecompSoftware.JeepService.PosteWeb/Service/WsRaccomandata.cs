using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.JeepService.PosteWeb.RolService;
using VecompSoftware.Services.Biblos.DocumentsService;

namespace VecompSoftware.JeepService.PosteWeb.Service
{
    public class WsRaccomandata
    {
        #region [ Fields ]

        private readonly RolService.RolService _rolService;
        private readonly PosteWebParameters _parameters;
        private DocumentsClient _biblosDSClient;

        #endregion

        #region [ Constructor ]

        public WsRaccomandata(string userName, string password, X509Certificate certificate, string url, double timeoutMilliseconds, PosteWebParameters parameters)
        {
            var objNetworkCredential = new NetworkCredential
            {
                UserName = userName,
                Password = password
            };
            _biblosDSClient = new DocumentsClient();
            _rolService = new RolService.RolService
            {
                Credentials = objNetworkCredential,
                Url = url
            };
            _rolService.ClientCertificates.Add(certificate);
            _rolService.Timeout = (int)timeoutMilliseconds;
            _parameters = parameters;
        }

        #endregion

        #region [ Methods ]

        public string GetIdRichiesta()
        {
            RecuperaIdRichiestaResult res = _rolService.RecuperaIdRichiesta();

            if (res == null || res.CEResult == null)
            {
                throw new PosteWebException("Messaggio con oggetto di ritorno vuoto.");
            }

            if (!res.CEResult.Type.Eq(GenericCeResult.Ok))
            {
                throw new PosteWebException("Impossibile creare richiesta raccomandata: ({0}) {1} - {2}.", res.CEResult.Type, res.CEResult.Code, res.CEResult.Description);
            }

            return res.IDRichiesta;
        }

        public bool RecipientsValidation(string idRichiesta, Destinatario[] recipients)
        {
            ValidaDestinatariResults res = _rolService.ValidaDestinatari(idRichiesta, recipients);

            if (res == null || res.CEResult == null)
            {
                throw new PosteWebException("Oggetto di ritorno della validazione destinatari nullo.");
            }

            if (res.CEResult.Type.Eq(GenericCeResult.Ok))
            {
                return true;
            }

            if (res.CEResult.Type.Eq(GenericCeResult.Error))
            {
                throw new PosteWebException("Impossibile validare i destinatari: Ricevuta risposta non attesa ({0}) {1} - {2}.",
                        res.CEResult.Type,
                        res.CEResult.Code,
                        res.CEResult.Description);
            }

            // Controllo incrociato validità dati ritornati
            IList<String> invalidNames = new List<String>();
            if (res.DatiDestinatari != null)
            {
                foreach (var vdr in res.DatiDestinatari)
                {
                    if (vdr.CEResult.Type.Eq(GenericCeResult.Ok))
                    {
                        continue;
                    }

                    var nominativo = "";
                    if (!vdr.Destinatari.IsNullOrEmpty() && vdr.Destinatari[0].Nominativo != null && !string.IsNullOrWhiteSpace(vdr.Destinatari[0].Nominativo.RagioneSociale))
                    {
                        nominativo = vdr.Destinatari[0].Nominativo.RagioneSociale;
                    }
                    invalidNames.Add(string.Format("'{0}': {1};", nominativo, vdr.CEResult.Description));
                }
            }

            if (invalidNames.Count > 0)
            {
                var names = new StringBuilder();
                foreach (String name in invalidNames)
                {
                    names.AppendFormat("({0}),", name);
                }

                throw new PosteWebException("Errore validazione [{0}]: Impossibile validare i destinatari [{1}].", res.CEResult.Description, names.Remove(names.Length - 1, 1));
            }

            throw new PosteWebException("Impossibile validare i destinatari: {0}.", res.CEResult.Description);
        }

        public InvioResult Send(string idRichiesta, string customer, ROLSubmit submission)
        {
            InvioResult res = _rolService.Invio(idRichiesta, customer, submission);

            if (res == null || res.CEResult == null)
            {
                throw new PosteWebException("Impossibile inviare: Oggetto di ritorno vuoto.");
            }

            if (res.CEResult.Type.Eq(GenericCeResult.Error))
            {
                throw new PosteWebException("Impossibile inviare: ({0}) {1} - {2}.", res.CEResult.Type, res.CEResult.Code, res.CEResult.Description);
            }

            return res;
        }

        /// <summary> Consente di conoscere importo, stato e documento di una richiesta. </summary>
        public ValorizzaSingleResult CheckRequest(ServizioSingleEnquirySubmit enquiry)
        {
            ValorizzaSingleResult result = _rolService.ValorizzaSingle(enquiry);

            if (result == null || result.ServizioEnquiryResponse == null || result.ServizioEnquiryResponse.CEResult == null)
            {
                throw new PosteWebException("Valorizzazione nulla.");
            }

            if (result.ServizioEnquiryResponse.CEResult.Type.Eq(GenericCeResult.Error))
            {
                throw new PosteWebException("Problema valorizzazione ({0}) {1} - {2}", result.ServizioEnquiryResponse.CEResult.Type, result.ServizioEnquiryResponse.CEResult.Code, result.ServizioEnquiryResponse.CEResult.Description);
            }

            return result;
        }

        public PreConfermaResult Confirm(Richiesta[] requests)
        {
            // Con l'autoconferma il preconferma diventa conferma
            PreConfermaResult result = _rolService.PreConferma(requests, true);

            if (result == null || result.CEResult == null)
            {
                throw new PosteWebException("Oggetto di ritorno della preconferma nullo.");
            }

            if (result.CEResult.Type.Eq(GenericCeResult.Error))
            {
                throw new PosteWebException("PreConferma errata ({0}) {1} - {2}", result.CEResult.Type, result.CEResult.Code, result.CEResult.Description);
            }

            return result;
        }

        public RecuperaDCSResult CheckDocument(Richiesta request)
        {
            RecuperaDCSResult res = _rolService.RecuperaDCS(request);

            if (res == null || res.CEResult == null)
            {
                throw new PosteWebException("Nessun documento recuperato.");
            }

            if (!res.CEResult.Type.Eq(GenericCeResult.Ok))
            {
                throw new PosteWebException("Problema in recupero documento ({0}) {1} - {2}", res.CEResult.Type, res.CEResult.Code, res.CEResult.Description);
            }
            return res;
        }

        /// <summary> Aggiorna per tutti i destinatari dei messaggi lo stato di ricezione. </summary>
        public StatoInviiPerIDResult StatoInviiPerId(Richiesta[] requests)
        {

            StatoInviiPerIDResult res = _rolService.StatoInviiPerID(requests);

            if (res == null || res.CeResult == null)
            {
                throw new PosteWebException("Errore aggiornamento stato di ricezione: Oggetto di ritorno nullo.");
            }

            if (res.CeResult.Type.Eq(GenericCeResult.Error))
            {
                throw new PosteWebException("Errore aggiornamento stato di ricezione: ({0}) {1} - {2}.", res.CeResult.Type, res.CeResult.Code, res.CeResult.Description);
            }

            return res;
        }

        public static Richiesta[] InstantiateRequests(IList<ROLRequest> raccomandate)
        {
            var requests = new List<Richiesta>(raccomandate.Count);
            requests.AddRange(raccomandate.Select(InstantiateRequest));
            return requests.ToArray();
        }

        public static Richiesta InstantiateRequest(ROLRequest raccomandata)
        {
            return new Richiesta { GuidUtente = raccomandata.GuidPoste, IDRichiesta = raccomandata.IdRichiesta };
        }

        public ROLSubmit CreaSubmit(ROLRequest message)
        {
            var documento = new Documento();
            documento.Firmatari = new string[0];

            Services.Biblos.DocumentsService.Document[] mergedDocument = _biblosDSClient.GetChainInfo(message.IdArchiveChain.Value, null, null);
            if (mergedDocument.Count() < 1)
            {
                throw new PosteWebException("Nessun documento trovato con Id = {0} ", message.IdArchiveChain.Value);
            }
            byte[] stream = _biblosDSClient.GetDocumentContentById(mergedDocument.First().IdDocument).Blob;

            documento.Immagine = stream;
            documento.MD5 = message.DocumentMD5;

            string documentExtension = Path.GetExtension(message.DocumentName);
            documento.TipoDocumento = !string.IsNullOrEmpty(documentExtension) && documentExtension.StartsWith(".") ? documentExtension.Remove(0, 1) : documentExtension;

            var submit = new ROLSubmit();
            submit.Mittente = CreaMittente(message.Sender);
            submit.NumeroDestinatari = message.Recipients.Count;
            submit.Destinatari = CreaDestinatari(message.Recipients);
            submit.Opzioni = new Opzioni
            {
                Archiviazione = false,
                DataStampa = DateTime.Now,
                DPM = false,
                FirmaElettronica = false,
                InserisciMittente = false,
                Inserti = new Inserti { InserisciMittente = false, Inserto = "" },
                OpzioniAggiuntive = new ServizioAggiuntivo[0],
                OpzionidiStampa = CreaOpzioniDiStampa()
            };
            submit.ServiziAggiuntivi = new ServizioAggiuntivo[0];
            submit.Documento = new[] { documento };

            submit.DatiRicevuta = new DatiRicevuta { Nominativo = submit.Mittente.Nominativo };

            return submit;
        }

        private Mittente CreaMittente(POLRequestContact contact)
        {
            return new Mittente { InviaStampa = false, Nominativo = CreaNominativo(contact) };
        }

        public Destinatario[] CreaDestinatari(IList<POLRequestRecipient> contacts)
        {
            var destinatari = new Destinatario[contacts.Count];
            for (int index = 0; index < contacts.Count; index++)
            {
                destinatari[index] = CreaDestinatario(contacts[index]);
            }

            return destinatari;
        }

        private Destinatario CreaDestinatario(POLRequestRecipient recipient)
        {
            var destinatario = new Destinatario
            {
                IdDestinatario = recipient.Id.ToString(),
                Nominativo = CreaNominativo(recipient)
            };

            return destinatario;
        }

        private OpzionidiStampa CreaOpzioniDiStampa()
        {
            OpzionidiStampa options = new OpzionidiStampa()
            {
                BW = _parameters.PrintBW.ToString(),
                FronteRetro = _parameters.PrintDoubleSided.ToString()
            };

            return options;
        }

        private Nominativo CreaNominativo(POLRequestContact contact)
        {
            string[] splittedName = Tools.SplitName(contact.Name);

            var nominativo = new Nominativo();
            nominativo.TipoIndirizzo = NominativoTipoIndirizzo.NORMALE;
            nominativo.Nome = "";
            nominativo.Cognome = "";
            nominativo.RagioneSociale = splittedName[0];
            if (splittedName.Length > 1)
            {
                nominativo.ComplementoNominativo = splittedName[1];
            }
            nominativo.Telefono = contact.PhoneNumber;
            nominativo.ForzaDestinazione = false;
            nominativo.Frazione = "";
            nominativo.Citta = contact.City;
            nominativo.Provincia = contact.Province;
            nominativo.CAP = contact.ZipCode;
            nominativo.Stato = "Italia";
            nominativo.ComplementoIndirizzo = "";
            nominativo.CasellaPostale = "";
            nominativo.UfficioPostale = "";
            nominativo.Zona = "";
            nominativo.Indirizzo = new Indirizzo
            {
                DUG = "",
                Toponimo = contact.Address,
                NumeroCivico = contact.CivicNumber,
                Esponente = ""
            };

            return nominativo;
        }

        #endregion
    }
}

