using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Formatter;
using VecompSoftware.DocSuiteWeb.Facade.Common.Interfaces;

namespace VecompSoftware.DocSuiteWeb.Facade.Common
{
    class ProtocolImporter : IProtocolImporter
    {
        public Protocol Import(XmlDocument xml, FileInfo pdf, ProtocolTemplate template)
        {
            // Istanze delle Facade per accesso ed inserimento Dati
            var logFacade = new ProtocolLogFacade();
            var facade = new ProtocolFacade();

            // Verifico se il documento PDF esiste. In caso negativo lancio una eccezione.
            if (!pdf.Exists)
                throw new Exception("Documento non trovato.");

            // Extraggo i metadata dall'XML
            var data = ExtractMetadata(xml);

            // Aggiungo il filenamee
            data.Add("FILENAME", pdf.Name);
            if (data.ContainsKey("SEZIONALENUMERICO"))
            {
                switch (data["SEZIONALENUMERICO"].ToLower())
                {
                    case "fattura":
                        data.Add("REGISTROIVA", "1");
                        break;
                    case "fattura accompagnatoria":
                        data.Add("REGISTROIVA", "2");
                        break;
                    case "nota di credito":
                        data.Add("REGISTROIVA", "3");
                        break;
                    case "bolletta doganale":
                        data.Add("REGISTROIVA", "4");
                        break;
                    case "fattura di acquisto":
                        data.Add("REGISTROIVA", "5");
                        break;
                    case "fattura di acquisto CEE":
                        data.Add("REGISTROIVA", "6");
                        break;
                    case "fattura di acquisto in reverse charge":
                        data.Add("REGISTROIVA", "7");
                        break;
                    case "nota di credito da fornitore":
                        data.Add("REGISTROIVA", "8");
                        break;
                    default:
                        data.Add("REGISTROIVA", "101");
                        break;
                }
            }
            else
                data.Add("REGISTROIVA", "100");

            // Estraggo il contatto dai metadata
            var contact = ExtractContact(data);

            // Verifico se la fattura è già stata inserita in DocSuite. In caso positivo lancio una eccezione.
            if (GetProtocolByAccountFields(template, data) != null)
            {
                throw new Exception("Fattura già presente.");
            }

            // Creazione oggetto del protocollo
            var protocolobject = GetProtocolObject(data, contact);

            CheckData(data);

            var prot = CreateFormTemplate(template);
            prot.ProtocolObject = protocolobject;
            prot.DocumentCode = pdf.Name;

            // Riverso i valori dai Dati al Protocollo
            UpdateProtocolFromData(prot, data);

            // Associo il contatto al protocollo
            if (prot.Container.Name.ToLower() == "protocollo fatture passive")
                prot.AddSenderManual(ref contact, false); // Aggiungo il sender
            else
                prot.AddRecipientManual(ref contact, false); // Aggiungo il mittente

            facade.Save(ref prot);

            // Recupero la signature del protocollo e la aggiungo alla collection di dati
            data.Add("SIGNATURE", facade.GenerateSignature(prot, prot.RegistrationDate.ToLocalTime().DateTime, new ProtocolSignatureInfo()));

            // Registro l'inserimento in LOG
            logFacade.Insert(ref prot, ProtocolLogEvent.PI, "Inserimento da modulo importazione");

            return prot;
        }

        public bool ClassificationManaged
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Estrae i dati dall'XML e li riversa su una Dictionary generica (chiave-valore)
        /// Le chiavi vengono impostate ad UPPER-CASE per omogeneità.
        /// </summary>
        /// <param name="xml">XmlDocument di importazione.</param>
        /// <returns>Restituisce la Collection dei dati recuperati da XML</returns>
        private Dictionary<string, string> ExtractMetadata(XmlDocument xml)
        {
            var xmlNode = xml.SelectSingleNode("/Document/Page[contains(@DocumentFileName,'.pdf')]");
            if (xmlNode == null) xmlNode = xml.SelectSingleNode("/Document/Page[contains(@DocumentFileName,'.PDF')]");
            if (xmlNode == null) xmlNode = xml.SelectSingleNode("/Document/Page[contains(@DocumentFileName,'.Pdf')]");
            if (xmlNode == null) return null; // PDF Non indicato

            var data = new Dictionary<string, string>();

            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                if (child.Name.ToLower() == "zone")
                {
                    var item = child.Attributes["Name"];
                    if (item != null && item.Value != null && item.Value.Length > 0)
                    {
                        XmlNode val = child.SelectSingleNode("./Row[@Y=\"0\"]");
                        if (val != null && val.InnerText != null && val.InnerText.Length > 0)
                        {
                            // Inserisco il valore nei Metadata se non presente
                            if (!data.ContainsKey(item.Value.ToUpper()))
                                data.Add(item.Value.ToUpper(), val.InnerText);
                        }
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Estrae il contatto dai metadati
        /// </summary>
        /// <param name="data">Collection dei dati recuperati da XML</param>
        /// <returns>Restituisce l'istanza di Contact estratta dei dati dell'XML</returns>
        private Contact ExtractContact(IDictionary<string, string> data)
        {
            var contact = new Contact();
            if (data.ContainsKey("PIVA"))
            {
                // Si tratta di una persona giuridica
                if (!data.ContainsKey("DENOMINAZIONE"))
                {
                    throw new DocSuiteException("Estrazione contatto da Metadati", "Denominazione non valida nei Metadati su File (Manca campo DENOMINAZIONE)");
                }
                contact.ContactType = new ContactType(ContactType.Aoo);
                contact.Description = data["DENOMINAZIONE"];
                contact.FiscalCode = data["PIVA"];
            }
            else
            {
                contact.ContactType = new ContactType(ContactType.Person);

                if (data.ContainsKey("NOME") && data.ContainsKey("COGNOME"))
                    contact.Description = data["COGNOME"] + data["NOME"];
                else
                {
                    contact.Description = data["DENOMINAZIONE"];
                    string[] tokens = data["DENOMINAZIONE"].Split(" |-".ToCharArray());
                    data.Add("NOME", tokens[0]);
                    data.Add("COGNOME", tokens[1]);
                }
                contact.FiscalCode = data["CFIS"];

            }
            return contact;
        }

        /// <summary>
        /// Verifica se il Protocollo è già stato inserito.
        /// </summary>
        /// <param name="template">Struttura template di protocollo</param>
        /// <param name="data">Collection dei dati recuperati da XML</param>
        /// <returns>Restituisce true se viene trovata una corrispondenza</returns>
        private Protocol GetProtocolByAccountFields(ProtocolTemplate template, IDictionary<string, string> data)
        {
            // Verifica Dati già presenti in DataBase
            var facade = new ProtocolFacade();
            var prots = facade.SerachInvoiceAccountingDouble(template.Container.Id, data["REGISTROIVA"], Convert.ToInt16(data["ANNOIVA"]), Convert.ToInt32(data["PROTOCOLLOIVA"]));
            if (prots != null && prots.Count > 0)
                return prots[0].Protocol; // i tre campi dovrebbero identificare sempre un unico protocollo

            return null;
        }

        private string GetProtocolObject(IDictionary<string, string> data, Contact contact)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Fattura N. {0} del {1}", data["NUMEROFATTURA"], data["DATAFATTURA"]);
            sb.AppendFormat(" {0}", contact.Description.Replace("|", " "));

            return sb.ToString();
        }

        private void CheckData(IDictionary<string, string> data)
        {
            //int tempInt;
            //if (!int.TryParse(data["NUMEROFATTURA"], out tempInt)) throw new ProtocolImportException("Errore in fase di recupero numero fattura: " + data["NUMEROFATTURA"]);

            DateTime tempDt;
            if (!DateTime.TryParseExact(data["DATAFATTURA"], "dd/MM/yyyy", new CultureInfo("it-IT"), DateTimeStyles.None, out tempDt)) throw new Exception("Errore in fase di recupero data fattura: " + data["DATAFATTURA"]);
        }

        private Protocol CreateFormTemplate(ProtocolTemplate template)
        {
            var facade = new ProtocolFacade();
            Protocol prot = facade.CreateProtocol();
            if (prot == null)
            {
                throw new DocSuiteException("Creazione Protocollo Da Template", "Errore in fase di creazione protocollo");
            }

            // Imposto i valori provenienti dal TEMPLATE (valori globali per l'importazione)
            prot.Category = template.Category;
            prot.Type = template.Type;
            if (DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled && template.Status != null)
                prot.Status = template.Status;
            else
                prot.IdStatus = (int)ProtocolStatusId.Errato;
            prot.Container = template.Container;
            prot.Location = template.Location;
            prot.AttachLocation = template.AttachLocation;
            prot.RegistrationDate = DateTimeOffset.UtcNow;
            prot.Note = template.Note;
            prot.DocumentType = template.DocumentType;

            return prot;
        }

        private void UpdateProtocolFromData(Protocol prot, IDictionary<string, string> data)
        {
            // Numero e data della Fattura (verifico correttezza formato numeri e date)
            int tempInt;
            if (!int.TryParse(data["NUMEROFATTURA"], out tempInt)) 
                throw new DocSuiteException("Recupero Fattura", string.Format("Errore in fase di recupero numero fattura [{0}]", data["NUMEROFATTURA"]));

            prot.InvoiceNumber = tempInt.ToString();

            DateTime tempDt;
            if (!DateTime.TryParseExact(data["DATAFATTURA"], "dd/MM/yyyy", new CultureInfo("it-IT"), DateTimeStyles.None, out tempDt))
            {
                throw new DocSuiteException("Recupero Fattura", string.Format("Errore in fase di recupero data fattura [{0}]", data["DATAFATTURA"]));
            }
            prot.InvoiceDate = tempDt;

            // Dati IVA documento inserito
            prot.AccountingSectional = data["REGISTROIVA"];
            prot.AccountingYear = short.Parse(data["ANNOIVA"]);
            prot.AccountingNumber = int.Parse(data["PROTOCOLLOIVA"]);

            if (!DateTime.TryParseExact(data["DATAIVA"], "dd/MM/yyyy", new CultureInfo("it-IT"), DateTimeStyles.None, out tempDt)) 
                throw new DocSuiteException("Recupero Fattura",string.Format("Errore in fase di recupero data fattura [{0}]", data["DATAFATTURA"]));
            prot.AccountingDate = tempDt;
        }
    }
}
