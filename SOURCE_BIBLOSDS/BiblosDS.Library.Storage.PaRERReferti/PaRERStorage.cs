using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;

using ICSharpCode.SharpZipLib.Zip;

using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.IStorage;
using BiblosDS.Library.Common;

using BiblosDS.Library.Storage.PaRERReferti.Entities;
using BiblosDS.Library.Storage.PaRERReferti.Util;

namespace BiblosDS.Library.Storage.PaRERReferti
{
    public class PaRERStorage : StorageBase
    {
        protected override bool StorageSupportVersioning
        {
            get
            {
                return false;
            }
        }

        #region IStorage Unsupported Interface

        /// <summary>
        /// Rimozione dei documenti dallo storage non è implementata 
        /// </summary>
        /// <param name="Document"></param>
        protected override void RemoveDocument(BiblosDS.Library.Common.Objects.Document Document)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// controllo corrispondenza metadati 
        /// </summary>
        /// <param name="DbAttributes"></param>
        /// <param name="StorageAttributes"></param>
        protected override void CheckAttribute(System.ComponentModel.BindingList<DocumentAttributeValue> DbAttributes, System.ComponentModel.BindingList<DocumentAttributeValue> StorageAttributes)
        {
            base.CheckAttribute(DbAttributes, StorageAttributes);
        }

        #endregion 

        #region IStorage Read Interface 

        /// <summary>
        /// Legge gli attributi da BiblodsDS, la chiamata a PaRER viene fatta solo quando esibisce il documento
        /// </summary>
        /// <param name="Document"></param>
        /// <returns></returns>
        protected override System.ComponentModel.BindingList<DocumentAttributeValue> LoadAttributes(Common.Objects.Document thisDocument)
        {
            BindingList<DocumentAttributeValue> thisAttributes = AttributeService.GetAttributeValues(thisDocument.IdDocument);

            // leggo gli attributi del padre e li aggiungo
            BindingList<DocumentAttributeValue> thisAttributesParent = AttributeService.GetAttributeValues(thisDocument.DocumentParent.IdDocument);
            foreach (DocumentAttributeValue thisParent in thisAttributesParent)
                thisAttributes.Add(thisParent);

            return thisAttributes;
        }

        protected override byte[] LoadAttach(BiblosDS.Library.Common.Objects.Document Document, string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// la lettura da PaRER corrisponde all'invocazione dei servizi di esibizione 
        /// </summary>

        /// <returns></returns>
        protected override byte[] LoadDocument(BiblosDS.Library.Common.Objects.Document thisDocument)
        {
            string valAnno = "";
            string valNumero = "";
            string valVersione = "";
            string valTipoRegistro = "";
            string valStruttura = "";

            // 20140519 si introduce la possibilità di gestire diverse tipologie di documenti oltre ai Referti 
            // l'estensione riguarda i documenti fiscali, ciascuno dei quali con un proprio XSD specifico e regole di compilazione specifiche
            string TipoDocumento = thisDocument.Archive.FiscalDocumentType.ToUpper();
            if (TipoDocumento == "")
                TipoDocumento = "REFERTO"; // per compatibilità pregressa 

            // 20140624 **REMOVE**
            // la nuova sintassi dell'attributo storage.MainPath è struttura#tiporegistro 
            // dove tipo registro è usato solo per i referti. 
            if (thisDocument.Storage.MainPath.Contains('#'))
            {
                try
                {
                    string[] PathValues = thisDocument.Storage.MainPath.Split(new char[] { '#' });
                    valStruttura = PathValues[0];
                    valTipoRegistro = PathValues[1];
                }
                catch
                {
                    throw new Exception("Storage.MainPath deve essere nel formato struttura#tiporegistro");
                }
            }
            else
            {
                throw new Exception("Storage.MainPath deve essere nel formato struttura#tiporegistro");
            }

            // legge i metadati del padre
            BindingList<DocumentAttributeValue> thisAttributes = AttributeService.GetAttributeValues(thisDocument.IdDocument);
            // carica eventuali metadati di chain 
            try
            {
                BindingList<DocumentAttributeValue> thisAttributesParent = AttributeService.GetAttributeValues(thisDocument.DocumentParent.IdDocument);
                foreach (DocumentAttributeValue thisParentValue in thisAttributesParent)
                    thisAttributes.Add(thisParentValue);
            }
            catch
            {
            }

            switch (TipoDocumento)
            {
                case "REFERTO":
                    // legge gli attributi del documento archiviato 
                    foreach (DocumentAttributeValue thisValue in thisAttributes)
                    {
                        if (thisValue.Attribute.Name.ToUpper() == "ANNO")
                        {
                            valAnno = thisValue.Value.ToString();
                            break;
                        }
                    }

                    // il numero nella chiave è l'IdBiblos
                    valNumero = thisDocument.IdDocument.ToString();

                    // valTipoRegistro è già letto sopra da Storage.MainPath, secondo parametro dopo il #
                    break;
                case "MODELLOF24":
                    valTipoRegistro = ""; // Lo legge dagli attributi
                    // legge gli attributi del documento archiviato 
                    foreach (DocumentAttributeValue thisValue in thisAttributes)
                    {
                        if (thisValue.Attribute.Name.ToUpper() == "TIPOREGISTRO")
                            valTipoRegistro = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "DATAVERSAMENTO")
                            valAnno = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "NUMERO")
                            valNumero = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "VERSIONE")
                            valVersione = thisValue.Value.ToString();

                        if ((valNumero != "") && (valTipoRegistro != "") && (valAnno != "") && (valVersione != ""))
                            break;
                    }
                    break;
                case "CUD":
                    valTipoRegistro = ""; // Lo legge dagli attributi 
                    foreach (DocumentAttributeValue thisValue in thisAttributes)
                    {
                        if (thisValue.Attribute.Name.ToUpper() == "TIPOREGISTRO")
                            valTipoRegistro = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "ANNO")
                            valAnno = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "MATRICOLA")
                            valNumero = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "VERSIONE")
                            valVersione = thisValue.Value.ToString();

                        if ((valNumero != "") && (valTipoRegistro != "") && (valAnno != "") && (valVersione != ""))
                            break;
                    }
                    break;
                case "MODELLO770":
                    // legge gli attributi del documento archiviato 
                    valTipoRegistro = ""; // Lo legge dagli attributi
                    foreach (DocumentAttributeValue thisValue in thisAttributes)
                    {
                        if (thisValue.Attribute.Name.ToUpper() == "TIPOREGISTRO")
                            valTipoRegistro = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "ANNO")
                            valAnno = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "NUMERO")
                            valNumero = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "VERSIONE")
                            valVersione = thisValue.Value.ToString();

                        if ((valNumero != "") && (valTipoRegistro != "") && (valAnno != "") && (valVersione != ""))
                            break;
                    }
                    break;
                case "CEDOLINO STIPENDIALE":
                    // legge gli attributi del documento archiviato 
                    string valMatricola = "";
                    string valMese = "";
                    valTipoRegistro = ""; // Lo legge dagli attributi

                    foreach (DocumentAttributeValue thisValue in thisAttributes)
                    {
                        if (thisValue.Attribute.Name.ToUpper() == "TIPOREGISTRO")
                            valTipoRegistro = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "ANNO")
                            valAnno = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "MATRICOLA")
                            valMatricola = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "MESE")
                            valMese = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "VERSIONE")
                            valVersione = thisValue.Value.ToString();

                        if ((valMese != "") && (valMatricola != "") && (valTipoRegistro != "") && (valAnno != "") && (valVersione != ""))
                            break;
                    }

                    valNumero = valMatricola + "-" + valMese;
                    break;
                case "CEDOLINO STIPENDIALE CUMULATIVO":
                    // legge gli attributi del documento archiviato 
                    valTipoRegistro = ""; // Lo legge dagli attributi

                    foreach (DocumentAttributeValue thisValue in thisAttributes)
                    {
                        if (thisValue.Attribute.Name.ToUpper() == "TIPOREGISTRO")
                            valTipoRegistro = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "ANNO")
                            valAnno = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "MESE")
                            valNumero = thisValue.Value.ToString();
                        if (thisValue.Attribute.Name.ToUpper() == "VERSIONE")
                            valVersione = thisValue.Value.ToString();

                        if ((valNumero != "") && (valTipoRegistro != "") && (valAnno != "") && (valVersione != ""))
                            break;
                    }

                    break;
            }

            // traduce il tipo registro
            switch (TipoDocumento)
            {
                case "REFERTO":
                    break;
                case "MODELLO770":
                    switch (valTipoRegistro)
                    {
                        case "1":
                            valTipoRegistro = "MODELLO770_AUSL_RE";
                            break;
                        case "2":
                            valTipoRegistro = "MODELLO770_AOSP_RE";
                            break;
                    }
                    break;
                case "MODELLOF24":
                    switch (valTipoRegistro)
                    {
                        case "1":
                            valTipoRegistro = "MODELLOF24_AUSL_RE";
                            break;
                        case "2":
                            valTipoRegistro = "MODELLOF24_AOSP_RE";
                            break;
                    }
                    break;
                case "CUD":
                    switch (valTipoRegistro)
                    {
                        case "1":
                            valTipoRegistro = "CUD_AUSL_RE";
                            break;
                        case "2":
                            valTipoRegistro = "CUD_AOSP_RE";
                            break;
                    }
                    break;
                case "CEDOLINO STIPENDIALE":
                case "CEDOLINO STIPENDIALE CUMULATIVO":
                    switch (valTipoRegistro)
                    {
                        case "1":
                            valTipoRegistro = "CEDOLINO_AUSL_RE_GUARDIAMEDICA";
                            break;
                        case "2":
                            valTipoRegistro = "CEDOLINO_AUSL_RE_SPECIALISTI";
                            break;
                        case "3":
                            valTipoRegistro = "CEDOLINO_AOSP_RE_SPECIALISTI";
                            break;
                        case "4":
                            valTipoRegistro = "CEDOLINO_AUSL_RE_LIBERAPROFESSIONE";
                            break;
                        case "5":
                            valTipoRegistro = "CEDOLINO_AOSP_RE_LIBERAPROFESSIONE";
                            break;
                        case "6":
                            valTipoRegistro = "CEDOLINO_AUSL_RE_DIPENDENTI";
                            break;
                        case "7":
                            valTipoRegistro = "CEDOLINO_AOSP_RE_DIPENDENTI";
                            break;
                    }
                    break;
            }

            // costruisce la richiesta della esibizione
            ParerStatus.Recupero request = new ParerStatus.Recupero();
            request.Chiave = new ParerStatus.ChiaveType();

            request.Chiave.Anno = valAnno;
            request.Chiave.TipoRegistro = valTipoRegistro;

            // **REMOVE** 2015 10 02 
            // la chiave PARER con numero_versione è presente solo per le registrazioni versate dall'Ottobre in poi 
            // mentre i versamenti pregressi che nelle serie documentali 
            if (valVersione == "0")
                request.Chiave.Numero = valNumero;
            else
                request.Chiave.Numero = valNumero + "_" + valVersione;

            request.Versatore = new ParerStatus.VersatoreType();
            request.Versatore.Ambiente = thisDocument.StorageArea.Path;
            request.Versatore.Ente = thisDocument.StorageArea.Name;
            request.Versatore.Struttura = valStruttura;
            // deve coincidere con l'utente che effettua la esibizione
            request.Versatore.UserID = thisDocument.Storage.AuthenticationKey;

            // deve coincidere con la versione del servizio di esibizione (no quello di versamento)
            request.Versione = "1.0";

            // invia la richiesta
            string xmlRequestStato = RequestFactory.GetXmlForStatusRequest(request);

            try
            {
                if (Properties.Settings.Default.TraceEnable == true)
                {
                    StreamWriter writer = File.CreateText(@"C:\BiblosDS2010\BiblosDSParerService\Request\Req_Status_" + TipoDocumento + "_" + valAnno + "_" + valNumero + ".xml");
                    writer.Write(xmlRequestStato);
                    writer.Close();
                }
            }
            catch
            {
                // none 
            }

            // elabora la risposta ed estrae lo stream 
            byte[] ZipResults = ParerEsibizione.Send(xmlRequestStato, thisDocument.Storage.AuthenticationKey, request.Versatore.Ambiente, request.Versatore.UserID);

            // usa il file della response per aprire lo zip 
            string sZipFilename = @"C:\BiblosDS2010\BiblosDSParerService\Response\Res_Status_" + TipoDocumento + "_" + valAnno + "_" + valNumero + ".zip";
            string sXmlFilename = @"C:\BiblosDS2010\BiblosDSParerService\Response\Res_Status_" + TipoDocumento + "_" + valAnno + "_" + valNumero + ".xml";

            FileStream writerZip = File.Create(sZipFilename);
            writerZip.Write(ZipResults, 0, ZipResults.Length);
            writerZip.Close();

            byte[] results = new byte[(int)thisDocument.Size];
            //ZipPackage
            try
            {
                FileStream zipStream = new FileStream(sZipFilename, FileMode.Open);
                using (ZipFile zipFile = new ZipFile(zipStream))
                {
                    foreach (ZipEntry thisEntry in zipFile)
                    {
                        if (thisEntry.Name.ToUpper().Contains("PRINCIPALE") == true)
                        {
                            Stream zipContentStream = zipFile.GetInputStream(thisEntry);
                            zipContentStream.Read(results, 0, (int)thisDocument.Size);
                            zipContentStream.Close();

                            break;
                        }
                    }
                    zipFile.Close();
                }
                zipStream.Close();
            }
            catch (Exception)
            {
                try
                {
                    if (Properties.Settings.Default.TraceEnable == true)
                    {
                        StreamWriter writerXmlResponse = File.CreateText(sXmlFilename);
                        writerXmlResponse.Write(System.Text.Encoding.UTF8.GetString(ZipResults));
                        writerXmlResponse.Close();
                    }
                }
                catch
                {
                    // none 
                }

                EsitoStatusFactory thisEsito;
                // analizza il risultato e se non è andato in parer lo lascia nel transito
                try
                {
                    thisEsito = new EsitoStatusFactory(System.Text.Encoding.UTF8.GetString(ZipResults));
                }
                catch (Exception ex)
                {
                    throw new Exception("Errore nel recupero del Documento presso Sacer", ex);
                }

                if (thisEsito.HasError == true)
                {
                    throw new Exception("Documento non disponibile presso Sacer: " + thisEsito.Errors);
                }
            }

            // se il trace non è attivo elimina il file zip
            try
            {
                if (Properties.Settings.Default.TraceEnable == false)
                    File.Delete(sZipFilename);
            }
            catch
            {
                // none 
            }

            return results;
        }

        #endregion 

        #region IStorage Save Interface

        protected override long WriteFile(string saveFileName, DocumentStorage storage, DocumentStorageArea storageArea, Common.Objects.Document document, DocumentContent content)
        {
            throw new NotImplementedException();
        }

        protected override void SaveAttributes(Common.Objects.Document Document)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Inserimento verso PaRER 
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <param name="storage"></param>
        /// <param name="storageArea"></param>
        /// <param name="document"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        protected override long SaveDocument(string localFilePath, DocumentStorage storage, DocumentStorageArea storageArea,
            BiblosDS.Library.Common.Objects.Document document, System.ComponentModel.BindingList<DocumentAttributeValue> attributeValue)
        {
            ParerConfig thisConfig = new ParerConfig();
            ParerContext thisContext = new ParerContext();
            BiblosDS.Library.Storage.PaRERReferti.Entities.Document thisDocument = new BiblosDS.Library.Storage.PaRERReferti.Entities.Document();
            string xmlthisDocumentParer;
            byte[] Content;

            // traccia il nome del file presente in transito 
            try
            {
                if (Properties.Settings.Default.TraceEnable == true)
                {
                    StreamWriter writer = File.CreateText(@"C:\BiblosDS2010\BiblosDSParerService\Request\Trace_" + document.IdBiblos.ToString() + ".txt");
                    writer.WriteLine("localFilePath : " + localFilePath);
                    if (attributeValue != null)
                        writer.WriteLine("n AttributeValue : " + attributeValue.Count.ToString());
                    if (document.AttributeValues != null)
                        writer.WriteLine("n document.AttributeValues : " + document.AttributeValues.Count.ToString());


                    writer.Close();
                }
            }
            catch
            {
            }

            // 20140519 si introduce la possibilità di gestire diverse tipologie di documenti oltre ai Referti 
            // l'estensione riguarda i documenti fiscali, ciascuno dei quali con un proprio XSD specifico e regole di compilazione specifiche
            string TipoDocumento = "";
            try
            {
                TipoDocumento = document.Archive.FiscalDocumentType;
                if (TipoDocumento == "")
                    TipoDocumento = "REFERTO"; // per compatibilità pregressa 
            }
            catch
            {
                throw new Exception("Tipo FiscalDocumentType non trovato");
            }

            try
            {
                thisConfig.ForzaAccettazione = Properties.Settings.Default.ParerForzaAccettazione;
                thisConfig.ForzaCollegamento = Properties.Settings.Default.ParerForzaCollegamento;
                thisConfig.ForzaConservazione = Properties.Settings.Default.ParerForzaConservazione;
                thisConfig.SimulaVersamento = Properties.Settings.Default.SimulaSalvataggioParer;

                thisContext.IdCliente = storage.AuthenticationKey; // "BIBLOSDS_AOSP_RE"      
                thisContext.UserPwd = storage.AuthenticationPassword; // qaz12PO
                thisContext.UserId = storage.AuthenticationKey;

                thisContext.Family = storageArea.Path;   // "PARER_PRE"; 
                thisContext.Organizzazione = storageArea.Name;  // "AOSP_RE" ; 

                // 20140624 **REMOVE**
                // la nuova sintassi dell'attributo storage.MainPath è struttura#tiporegistro 
                // dove tipo registro è usato solo per i referti. 
                string valStruttura = "";
                string valTipoRegistro = "";
                if (storage.MainPath.Contains('#'))
                {
                    try
                    {
                        string[] PathValues = storage.MainPath.Split(new char[] { '#' });
                        valStruttura = PathValues[0];
                        valTipoRegistro = PathValues[1];
                    }
                    catch
                    {
                        throw new Exception("Storage.MainPath deve essere nel formato struttura#tiporegistro");
                    }
                }
                else
                {
                    throw new Exception("Storage.MainPath deve essere nel formato struttura#tiporegistro");
                }

                thisContext.Struttura = valStruttura;  // 080-903


                // Costruisce il documento 
                if (document.AttributeValues.Count <= 2) // non ha i metadati di catena (ha solo filename e signature)  
                {
                    // leggo gli attributi del padre e li aggiungo
                    BindingList<DocumentAttributeValue> thisAttributesParent = AttributeService.GetAttributeValues(document.DocumentParent.IdDocument);
                    foreach (DocumentAttributeValue thisParentValue in thisAttributesParent)
                        attributeValue.Add(thisParentValue);
                }
                thisDocument.AttributesValue = attributeValue;

                // traccia i metadati dopo della memorizzazione
                try
                {
                    if (Properties.Settings.Default.TraceEnable == true)
                    {
                        StreamWriter writer = File.AppendText(@"C:\BiblosDS2010\BiblosDSParerService\Request\Trace_" + document.IdBiblos.ToString() + ".txt");
                        if (attributeValue != null)
                            writer.WriteLine("n thisDocument.AttributesValue : " + attributeValue.Count.ToString());

                        writer.Close();
                    }
                }
                catch
                {
                }

                // legge il contenuto del documento 
                Content = File.ReadAllBytes(localFilePath);
                BiblosDocument thisBiblosDSDocument = new BiblosDocument();
                thisBiblosDSDocument.BinaryContent = Content;
                thisBiblosDSDocument.IdDocument = document.IdDocument;
                thisBiblosDSDocument.IdBiblos = document.IdBiblos.ToString();
                thisBiblosDSDocument.NomeArchivioBiblos = document.Archive.Name;
                thisBiblosDSDocument.NomeDocumento = document.Name;

                switch (TipoDocumento)
                {
                    case "REFERTO":
                        thisBiblosDSDocument.TipoDocumento = TipoDocumento;
                        break;
                    case "MODELLOF24":
                    case "CUD":
                    case "MODELLO770":
                    case "CEDOLINO STIPENDIALE":
                    case "CEDOLINO STIPENDIALE CUMULATIVO":
                        thisBiblosDSDocument.TipoDocumento = TipoDocumento;
                        break;
                }

                if (document.IsSigned() == true)
                    thisBiblosDSDocument.TipoFirma = typeFirma.FIRMATO;

                thisDocument.Documento = thisBiblosDSDocument;

                try
                {
                    if (Properties.Settings.Default.TraceEnable == true)
                    {
                        StreamWriter writer = File.AppendText(@"C:\BiblosDS2010\BiblosDSParerService\Request\Trace_" + document.IdBiblos.ToString() + ".txt");
                        writer.WriteLine("Transit Path : " + localFilePath);
                        writer.WriteLine("Content Lenght : " + Content.Length.ToString());
                        writer.Close();
                    }
                }
                catch
                {
                }

                // verifica se già versato
                try
                {
                    foreach (DocumentAttributeValue thisValue in attributeValue)
                    {
                        if (thisValue.Attribute.Name.ToLower() == "pareruri")
                        {
                            if (thisValue.Value.ToString() != "")
                            {
                                // era un esito positivo o warning precedente rimasto in transito per problemi 
                                try
                                {
                                    if (Properties.Settings.Default.TraceEnable == true)
                                    {
                                        StreamWriter writer = File.AppendText(@"C:\BiblosDS2010\BiblosDSParerService\Request\Trace_" + document.IdBiblos.ToString() + ".txt");
                                        writer.WriteLine("Documento già versato : " + thisValue.Value.ToString());
                                        writer.WriteLine("-- END --");
                                        writer.Close();
                                    }
                                }
                                catch
                                {
                                }

                                // chiude il versamento come eseguito 
                                return Content.LongLength;
                            }
                            break;
                        }
                    }
                }
                catch
                {
                }

                // popola i metadati 
                switch (TipoDocumento)
                {
                    case "REFERTO":
                        thisDocument.Anno = thisDocument.GetAttributeValue("Anno");
                        break;
                    case "MODELLOF24":
                        // anno della data di versamento      
                        DateTime thisDate = DateTime.Parse(thisDocument.GetAttributeValue("DataVersamento"));
                        thisDocument.Anno = thisDate.Year.ToString();
                        break;
                    case "CUD":
                    case "MODELLO770":
                    case "CEDOLINO STIPENDIALE":
                    case "CEDOLINO STIPENDIALE CUMULATIVO":
                        // Anno è l'anno del cedolino 
                        thisDocument.Anno = thisDocument.GetAttributeValue("Anno");
                        break;
                }

                // thisDocument.Numero in chiave 
                switch (TipoDocumento)
                {
                    case "REFERTO":
                        thisDocument.Numero = document.IdDocument.ToString();
                        break;
                    case "MODELLO770":
                    case "MODELLOF24":
                        thisDocument.Numero = thisDocument.GetAttributeValue("Numero");
                        break;
                    case "CEDOLINO STIPENDIALE":
                        thisDocument.Numero = thisDocument.GetAttributeValue("Matricola") + "-" + thisDocument.GetAttributeValue("Mese") + "_" + thisDocument.GetAttributeValue("Versione");
                        break;
                    case "CEDOLINO STIPENDIALE CUMULATIVO":
                        thisDocument.Numero = thisDocument.GetAttributeValue("Mese") + "_" + thisDocument.GetAttributeValue("Versione");
                        break;
                    case "CUD":
                        thisDocument.Numero = thisDocument.GetAttributeValue("Matricola") + "_" + thisDocument.GetAttributeValue("Versione");
                        break;
                }

                switch (TipoDocumento)
                {
                    case "REFERTO":
                        thisDocument.TipoRegistro = valTipoRegistro; //  Storage.MainPath, secondo parametro dopo # 
                        break;
                    case "MODELLO770":
                        switch (thisDocument.GetAttributeValue("TipoRegistro"))
                        {
                            case "1":
                                thisDocument.TipoRegistro = "MODELLO770_AUSL_RE";
                                break;
                            case "2":
                                thisDocument.TipoRegistro = "MODELLO770_AOSP_RE";
                                break;
                        }
                        break;
                    case "MODELLOF24":
                        switch (thisDocument.GetAttributeValue("TipoRegistro"))
                        {
                            case "1":
                                thisDocument.TipoRegistro = "MODELLOF24_AUSL_RE";
                                break;
                            case "2":
                                thisDocument.TipoRegistro = "MODELLOF24_AOSP_RE";
                                break;
                        }
                        break;
                    case "CUD":
                        switch (thisDocument.GetAttributeValue("TipoRegistro"))
                        {
                            case "1":
                                thisDocument.TipoRegistro = "CUD_AUSL_RE";
                                break;
                            case "2":
                                thisDocument.TipoRegistro = "CUD_AOSP_RE";
                                break;
                        }
                        break;
                    case "CEDOLINO STIPENDIALE":
                    case "CEDOLINO STIPENDIALE CUMULATIVO":
                        switch (thisDocument.GetAttributeValue("TipoRegistro"))
                        {
                            case "1":
                                thisDocument.TipoRegistro = "CEDOLINO_AUSL_RE_GUARDIAMEDICA";
                                break;
                            case "2":
                                thisDocument.TipoRegistro = "CEDOLINO_AUSL_RE_SPECIALISTI";
                                break;
                            case "3":
                                thisDocument.TipoRegistro = "CEDOLINO_AOSP_RE_SPECIALISTI";
                                break;
                            case "4":
                                thisDocument.TipoRegistro = "CEDOLINO_AUSL_RE_LIBERAPROFESSIONE";
                                break;
                            case "5":
                                thisDocument.TipoRegistro = "CEDOLINO_AOSP_RE_LIBERAPROFESSIONE";
                                break;
                            case "6":
                                thisDocument.TipoRegistro = "CEDOLINO_AUSL_RE_DIPENDENTI";
                                break;
                            case "7":
                                thisDocument.TipoRegistro = "CEDOLINO_AOSP_RE_DIPENDENTI";
                                break;
                        }
                        break;
                }

                // Tipologia unità documentaria
                switch (TipoDocumento)
                {
                    case "REFERTO":
                        thisDocument.Serie = TipoDocumento;
                        break;
                    case "MODELLOF24":
                        thisDocument.Serie = "QUIETANZA VERSAMENTO MODELLOF24";
                        break;
                    case "CUD":
                    case "MODELLO770":
                    case "CEDOLINO STIPENDIALE":
                        thisDocument.Serie = TipoDocumento;
                        break;
                    case "CEDOLINO STIPENDIALE CUMULATIVO":
                        thisDocument.Serie = "CEDOLINO STIPENDIALE CUMULATIVO";
                        break;
                }


                switch (TipoDocumento)
                {
                    case "REFERTO":
                        // Denti 20140110 
                        // la stringa è composta dalla costante REFERTO_ seguita dal campo Erogatore 
                        // thisDocument.Oggetto = "ANALISI_LABORATORIO";
                        // thisDocument.Oggetto = "REFERTO_" + thisDocument.GetAttributeValue("Erogatore"); 
                        // Denti 20140127 
                        // la stringa per i referti di cartella clinica ausl-re contiene nel campo Erogatore il reparto e quindi non va bene
                        // si decide di usare il campo archivio 
                        thisDocument.Oggetto = "REFERTO_" + thisDocument.GetAttributeValue("Archivio").ToUpper().Replace(' ', '_');
                        break;
                    case "MODELLO770":
                        string TipoModello;
                        if (thisDocument.GetAttributeValue("TipoModello").ToString() == "1")
                            TipoModello = "ORDINARIO";
                        else
                            TipoModello = "SEMPLIFICATO";

                        thisDocument.Oggetto = "Modello 770 " + TipoModello + " " + thisDocument.GetAttributeValue("Anno") + " - Anno " + thisDocument.GetAttributeValue("AnnoDiCompetenza");
                        break;
                    case "MODELLOF24":
                        thisDocument.Oggetto = "Versamento del " + DateTime.Parse(thisDocument.GetAttributeValue("DataVersamento")).ToString("yyyy-MM-dd");
                        break;
                    case "CUD":
                        thisDocument.Oggetto = "CUD " + thisDocument.GetAttributeValue("Anno") + " REDDITI " + thisDocument.GetAttributeValue("AnnoDiCompetenza") + " " + thisDocument.GetAttributeValue("Cognome") + " " + thisDocument.GetAttributeValue("Nome");
                        break;
                    case "CEDOLINO STIPENDIALE":
                        thisDocument.Oggetto = "Cedolino Stipendiale di " + thisDocument.GetAttributeValue("Cognome") + " " + thisDocument.GetAttributeValue("Nome") + " " + thisDocument.GetAttributeValue("Mese") + "/" + thisDocument.GetAttributeValue("Anno");
                        break;
                    case "CEDOLINO STIPENDIALE CUMULATIVO":
                        switch (thisDocument.GetAttributeValue("TipoRegistro"))
                        {
                            case "1":
                                thisDocument.Oggetto = "CEDOLINO STIPENDIALE GUARDIA MEDICA" + " " + thisDocument.GetAttributeValue("Mese") + "/" + thisDocument.GetAttributeValue("Anno");
                                break;
                            case "2":
                            case "3":
                                thisDocument.Oggetto = "CEDOLINO STIPENDIALE SPECIALISTI" + " " + thisDocument.GetAttributeValue("Mese") + "/" + thisDocument.GetAttributeValue("Anno");
                                break;
                            case "4":
                            case "5":
                                thisDocument.Oggetto = "CEDOLINO STIPENDIALE LIBERA PROFESSIONE" + " " + thisDocument.GetAttributeValue("Mese") + "/" + thisDocument.GetAttributeValue("Anno");
                                break;
                            case "6":
                            case "7":
                                thisDocument.Oggetto = "CEDOLINO STIPENDIALE CUMULATIVO DIPENDENTI" + " " + thisDocument.GetAttributeValue("Mese") + "/" + thisDocument.GetAttributeValue("Anno");
                                break;
                        }
                        break;
                }

                switch (TipoDocumento)
                {
                    case "REFERTO":
                        thisDocument.Documento.Autore = thisDocument.GetAttributeValue("MedicoRefertante");
                        break;
                    case "MODELLO770":
                        thisDocument.Documento.Autore = "AUSL-RE";
                        break;
                    case "MODELLOF24":
                        thisDocument.Documento.Autore = "Agenzia delle Entrate";
                        thisDocument.Documento.Descrizione = "Quietanza telematica di versamento, documento redatto in applicazione di Provvedimento del Direttore dell'Agenzia delle Entrate";
                        break;
                    case "CUD":
                    case "CEDOLINO STIPENDIALE":
                    case "CEDOLINO STIPENDIALE CUMULATIVO":
                        switch (thisDocument.GetAttributeValue("TipoRegistro"))
                        {
                            case "1":
                            case "2":
                            case "4":
                            case "6":
                                thisDocument.Documento.Autore = "AUSL-RE";
                                break;
                            case "3":
                            case "5":
                            case "7":
                                thisDocument.Documento.Autore = "AOSP-RE";
                                break;
                        }
                        break;
                }
                thisDocument.Documento.Descrizione = ""; // il tag deve esserci : cfr. Alfier di PaRER


                switch (TipoDocumento)
                {
                    case "REFERTO":
                        // deve estrarre la data di firma del referto 
                        try
                        {
                            thisDocument.Data = DateTime.Parse(thisDocument.GetAttributeValue("DataOraReferto"));
                        }
                        catch
                        {
                            thisDocument.Data = DateTime.Now;

                            Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_WindowsService_WCFHost,
                                 "PaRERStorage",
                                  "Forza DataOraReferto alla data corrente",
                                  BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_InsertDocument,
                                  BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                        }
                        break;
                    case "MODELLOF24":
                        try
                        {
                            if (Properties.Settings.Default.TraceEnable == true)
                            {
                                StreamWriter writer = File.AppendText(@"C:\BiblosDS2010\BiblosDSParerService\Request\Trace_" + document.IdBiblos.ToString() + ".txt");
                                writer.WriteLine("data versamento : " + thisDocument.GetAttributeValue("DataVersamento"));
                                writer.Close();
                            }
                        }
                        catch
                        {
                        }
                        try
                        {
                            thisDocument.Data = DateTime.Parse(thisDocument.GetAttributeValue("DataVersamento"));
                        }
                        catch
                        {
                            thisDocument.Data = DateTime.Now;
                        }
                        break;
                    case "MODELLO770":
                    case "CUD":
                    case "CEDOLINO STIPENDIALE":
                        try
                        {
                            thisDocument.Data = DateTime.Parse(thisDocument.GetAttributeValue("DataProduzione"));
                        }
                        catch
                        {
                            thisDocument.Data = DateTime.Now;

                            Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_WindowsService_WCFHost,
                                 "PaRERStorage",
                                  "Forza DataProduzione alla data corrente",
                                  BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_InsertDocument,
                                  BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                        }
                        break;
                    case "CEDOLINO STIPENDIALE CUMULATIVO":
                        try
                        {
                            thisDocument.Data = DateTime.Parse(thisDocument.GetAttributeValue("Data Produzione"));
                        }
                        catch
                        {
                            thisDocument.Data = DateTime.Now;

                            Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_WindowsService_WCFHost,
                                 "PaRERStorage",
                                  "Forza DataProduzione alla data corrente",
                                  BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_InsertDocument,
                                  BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                        }
                        break;
                }

                thisDocument.DataInserimentoDocumento = document.DateCreated.GetValueOrDefault(DateTime.Now);

                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_WindowsService_WCFHost,
                     "PaRERStorage",
                      "Intestazione richiesta creata",
                      BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_InsertDocument,
                      BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Trace);

                // costruisce la richiesta per il Parer, specifica per la tipologia documentale corrente 
                xmlthisDocumentParer = RequestFactory.GetXmlRequestForParer(thisDocument, thisContext, TipoDocumento, storageArea.Path);

                try
                {
                    if (Properties.Settings.Default.TraceEnable == true)
                    {
                        StreamWriter writer = File.AppendText(@"C:\BiblosDS2010\BiblosDSParerService\Request\Trace_" + document.IdBiblos.ToString() + ".txt");
                        writer.WriteLine("XmlRequest : " + xmlthisDocumentParer);
                        writer.Close();
                    }
                }
                catch
                {
                }

                if (xmlthisDocumentParer.Length == 0)
                    throw new Exception("Xml risultante da GetXmlRequestParer vuoto");

            }
            catch (Exception e)
            {
                // invia email 
                try
                {
                    MailHelper.SendMail(thisDocument.GetAttributeValue("Anno"),
                        thisDocument.GetAttributeValue("Numero"),
                        thisDocument.TipoRegistro,
                        e.Message, true);
                }
                catch
                {
                }

                // invia l'eccezione in modo che il documento resti nel transito
                throw new Exception("Errore in predisposizione richiesta al PaRER", e);
            }

            // traccia le richieste 
            try
            {
                if (Properties.Settings.Default.TraceEnable == true)
                {
                    StreamWriter writer = File.CreateText(@"C:\BiblosDS2010\BiblosDSParerService\Request\Req_" + document.IdBiblos.ToString() + "_" + TipoDocumento + "_" + thisDocument.Anno + "_" + thisDocument.Numero + ".xml");
                    writer.Write(xmlthisDocumentParer);
                    writer.Close();
                }
            }
            catch
            {
                // none 
            }

            EsitoFactory thisEsito;
            string results;
            try
            {
                // invia la richiesta al Parer
                results = Parer.Send(xmlthisDocumentParer, thisDocument, thisContext, storageArea.Path);

                try
                {
                    if (Properties.Settings.Default.TraceEnable == true)
                    {
                        StreamWriter writer = File.AppendText(@"C:\BiblosDS2010\BiblosDSParerService\Request\Trace_" + document.IdBiblos.ToString() + ".txt");
                        writer.WriteLine("XmlResponse : " + results);
                        writer.Close();
                    }
                }
                catch
                {
                }

                // analizza il risultato e se non è andato in parer lo lascia nel transito
                thisEsito = new EsitoFactory(results);
            }
            catch (Exception e)
            {
                // invia email 
                try
                {
                    MailHelper.SendMail(thisDocument.GetAttributeValue("Anno"),
                        thisDocument.GetAttributeValue("Numero"),
                        thisDocument.TipoRegistro,
                        e.Message, true);
                }
                catch
                {
                }

                // invia l'eccezione in modo che il documento resti nel transito
                throw new Exception("Errore in invio richiesta al PaRER", e);
            }


            try
            {
                if (Properties.Settings.Default.TraceEnable == true)
                {
                    StreamWriter writer = File.CreateText(@"C:\BiblosDS2010\BiblosDSParerService\Response\Response_" + document.IdBiblos.ToString() + "_" + TipoDocumento + "_" + thisDocument.Anno + "_" + thisDocument.Numero + ".xml");
                    writer.Write(results);
                    writer.Close();

                    switch (TipoDocumento)
                    {
                        case "REFERTO":
                            // invia email 
                            try
                            {
                                MailHelper.SendMail(thisDocument.GetAttributeValue("Anno"),
                                    thisDocument.GetAttributeValue("Numero"),
                                    thisDocument.TipoRegistro,
                                    results, false);
                            }
                            catch
                            {
                            }
                            break;
                        default:
                            // nessuna email
                            break;
                    }
                }
            }
            catch
            {
                // none 
            }

            bool IsArchivedInPaRER = false;
            try
            {
                if (thisEsito.HasError == true)
                {
                    // in errore

                    // gestisce il caso di chiave già presente (OK solo per referti) 
                    // TODO : da migliorare andando a usare la chiamata a parer per recuperare l'uri della unità documentaria presente in PaRER
                    if (thisEsito.Errors.Contains("la chiave indicata corrisponde ad una Unità Documentaria già presente nel sistema") == true)
                    {

                        switch (TipoDocumento)
                        {
                            case "REFERTO":
                                // è un errore di chiave già presente, estrae il PaRERUri e si comporta come fosse archiviato 
                                IsArchivedInPaRER = true;

                                int startindex = thisEsito.Errors.IndexOf("REFERTI ");
                                int endindex = thisEsito.Errors.IndexOf(": la chiave ");

                                try
                                {
                                    if (Properties.Settings.Default.TraceEnable == true)
                                    {
                                        StreamWriter writer = File.AppendText(@"C:\BiblosDS2010\BiblosDSParerService\Request\Trace_" + document.IdBiblos.ToString() + ".txt");
                                        writer.WriteLine("-- POSITIVO PARER CHIAVE GIA' PRESENTE --");
                                        writer.Close();
                                    }
                                }
                                catch
                                {
                                }

                                foreach (DocumentAttribute thisAttribute in AttributeService.GetAttributesFromArchive(document.Archive.IdArchive))
                                {
                                    if (thisAttribute.Name.ToLower() == "pareruri")
                                    {
                                        DocumentAttributeValue thisParerUri = new DocumentAttributeValue(thisAttribute, thisEsito.Errors.Substring(startindex, endindex - startindex));
                                        thisParerUri.IdAttribute = thisParerUri.Attribute.IdAttribute;

                                        DocumentService.UpdateDocumentAttributeValue(document, thisParerUri);
                                        // DocumentService.UpdateDocumentAttributeValue(document.DocumentParent, thisParerUri);
                                    }
                                    else if (thisAttribute.Name.ToLower() == "parerdate")
                                    {
                                        DocumentAttributeValue thisParerDate = new DocumentAttributeValue(thisAttribute, DateTime.Now);
                                        thisParerDate.IdAttribute = thisParerDate.Attribute.IdAttribute;

                                        DocumentService.UpdateDocumentAttributeValue(document, thisParerDate);
                                    }
                                    else if (thisAttribute.Name.ToLower() == "pareresito")
                                    {
                                        DocumentAttributeValue thisParerEsito = new DocumentAttributeValue(thisAttribute, "POSITIVO");
                                        thisParerEsito.IdAttribute = thisParerEsito.Attribute.IdAttribute;

                                        DocumentService.UpdateDocumentAttributeValue(document, thisParerEsito);
                                    }
                                }

                                // invia email 
                                try
                                {
                                    MailHelper.SendMail(thisDocument.GetAttributeValue("Anno"),
                                        thisDocument.GetAttributeValue("Numero"),
                                        thisDocument.TipoRegistro,
                                        "Documento già versato, transito BiblosDS liberato.", false);
                                }
                                catch
                                {
                                }

                                break;
                            case "MODELLO770":
                            case "MODELLOF24":
                            case "CUD":
                            case "CEDOLINO STIPENDIALE":
                            case "CEDOLINO STIPENDIALE CUMULATIVO":
                                IsArchivedInPaRER = true;

                                try
                                {
                                    if (Properties.Settings.Default.TraceEnable == true)
                                    {
                                        StreamWriter writer = File.AppendText(@"C:\BiblosDS2010\BiblosDSParerService\Request\Trace_" + document.IdBiblos.ToString() + ".txt");
                                        writer.WriteLine("-- NEGATIVO PARER CHIAVE GIA' PRESENTE --");
                                        writer.Close();
                                    }
                                }
                                catch
                                {
                                }

                                // invia l'eccezione in modo che il documento resti nel transito o fallisca la memorizzazione (se non ci sono transiti)
                                throw new Exception("Errore in archiviazione al PaRER : " + thisEsito.Errors);
                        }
                    }
                    else
                    {
                        try
                        {
                            if (Properties.Settings.Default.TraceEnable == true)
                            {
                                StreamWriter writer = File.AppendText(@"C:\BiblosDS2010\BiblosDSParerService\Request\Trace_" + document.IdBiblos.ToString() + ".txt");
                                writer.WriteLine("-- ERRORE PARER --");
                                writer.Close();
                            }
                        }
                        catch
                        {
                        }

                        foreach (DocumentAttribute thisAttribute in AttributeService.GetAttributesFromArchive(document.Archive.IdArchive))
                        {
                            if (thisAttribute.Name.ToLower() == "pareruri")
                            {
                                DocumentAttributeValue thisParerUri = new DocumentAttributeValue(thisAttribute, thisEsito.Uri);
                                thisParerUri.IdAttribute = thisParerUri.Attribute.IdAttribute;
                                switch (TipoDocumento)
                                {
                                    case "REFERTO":
                                        DocumentService.UpdateDocumentAttributeValue(document, thisParerUri);
                                        break;
                                    case "MODELLO770":
                                    case "MODELLOF24":
                                    case "CUD":
                                    case "CEDOLINO STIPENDIALE":
                                    case "CEDOLINO STIPENDIALE CUMULATIVO":
                                        DocumentService.UpdateDocumentAttributeValue(document.DocumentParent, thisParerUri);
                                        break;
                                }
                            }
                            else if (thisAttribute.Name.ToLower() == "parerdate")
                            {
                                DocumentAttributeValue thisParerDate = new DocumentAttributeValue(thisAttribute, thisEsito.ArchiviedDate);
                                thisParerDate.IdAttribute = thisParerDate.Attribute.IdAttribute;
                                switch (TipoDocumento)
                                {
                                    case "REFERTO":
                                        DocumentService.UpdateDocumentAttributeValue(document, thisParerDate);
                                        break;
                                    case "MODELLO770":
                                    case "MODELLOF24":
                                    case "CUD":
                                    case "CEDOLINO STIPENDIALE":
                                    case "CEDOLINO STIPENDIALE CUMULATIVO":
                                        DocumentService.UpdateDocumentAttributeValue(document.DocumentParent, thisParerDate);
                                        break;
                                }
                            }
                            else if (thisAttribute.Name.ToLower() == "pareresito")
                            {
                                DocumentAttributeValue thisParerEsito = new DocumentAttributeValue(thisAttribute, thisEsito.Errors);
                                thisParerEsito.IdAttribute = thisParerEsito.Attribute.IdAttribute;
                                switch (TipoDocumento)
                                {
                                    case "REFERTO":
                                        DocumentService.UpdateDocumentAttributeValue(document, thisParerEsito);
                                        break;
                                    case "MODELLO770":
                                    case "MODELLOF24":
                                    case "CUD":
                                    case "CEDOLINO STIPENDIALE":
                                    case "CEDOLINO STIPENDIALE CUMULATIVO":
                                        DocumentService.UpdateDocumentAttributeValue(document.DocumentParent, thisParerEsito);
                                        break;
                                }
                            }
                        }

                        // invia email 
                        switch (TipoDocumento)
                        {
                            case "REFERTO":
                                try
                                {
                                    MailHelper.SendMail(thisDocument.GetAttributeValue("Anno"),
                                        thisDocument.GetAttributeValue("Numero"),
                                        thisDocument.TipoRegistro,
                                        thisEsito.Errors, true);
                                }
                                catch
                                {
                                }
                                break;
                            case "MODELLO770":
                            case "MODELLOF24":
                            case "CUD":
                            case "CEDOLINO STIPENDIALE":
                            case "CEDOLINO STIPENDIALE CUMULATIVO":
                                break;
                        }


                        // invia l'eccezione in modo che il documento resti nel transito o fallisca la memorizzazione (se non ci sono transiti)
                        throw new Exception("Errore in archiviazione al PaRER : " + thisEsito.Errors);
                    }
                }
                else if (thisEsito.HasWarning == true)
                {
                    // esito warning
                    IsArchivedInPaRER = true;

                    try
                    {
                        if (Properties.Settings.Default.TraceEnable == true)
                        {
                            StreamWriter writer = File.AppendText(@"C:\BiblosDS2010\BiblosDSParerService\Request\Trace_" + document.IdBiblos.ToString() + ".txt");
                            writer.WriteLine("-- WARNING PARER --");
                            writer.Close();
                        }
                    }
                    catch
                    {
                    }

                    foreach (DocumentAttribute thisAttribute in AttributeService.GetAttributesFromArchive(document.Archive.IdArchive))
                    {
                        if (thisAttribute.Name.ToLower() == "pareruri")
                        {
                            DocumentAttributeValue thisParerUri = new DocumentAttributeValue(thisAttribute, thisEsito.Uri);
                            thisParerUri.IdAttribute = thisParerUri.Attribute.IdAttribute;
                            switch (TipoDocumento)
                            {
                                case "REFERTO":
                                    DocumentService.UpdateDocumentAttributeValue(document, thisParerUri);
                                    break;
                                case "MODELLO770":
                                case "MODELLOF24":
                                case "CUD":
                                case "CEDOLINO STIPENDIALE":
                                case "CEDOLINO STIPENDIALE CUMULATIVO":
                                    DocumentService.UpdateDocumentAttributeValue(document.DocumentParent, thisParerUri);
                                    break;
                            }
                        }
                        else if (thisAttribute.Name.ToLower() == "parerdate")
                        {
                            DocumentAttributeValue thisParerDate = new DocumentAttributeValue(thisAttribute, thisEsito.ArchiviedDate);
                            thisParerDate.IdAttribute = thisParerDate.Attribute.IdAttribute;
                            switch (TipoDocumento)
                            {
                                case "REFERTO":
                                    DocumentService.UpdateDocumentAttributeValue(document, thisParerDate);
                                    break;
                                case "MODELLO770":
                                case "MODELLOF24":
                                case "CUD":
                                case "CEDOLINO STIPENDIALE":
                                case "CEDOLINO STIPENDIALE CUMULATIVO":
                                    DocumentService.UpdateDocumentAttributeValue(document.DocumentParent, thisParerDate);
                                    break;
                            }
                        }
                        else if (thisAttribute.Name.ToLower() == "pareresito")
                        {
                            DocumentAttributeValue thisParerEsito = new DocumentAttributeValue(thisAttribute, thisEsito.Errors);
                            thisParerEsito.IdAttribute = thisParerEsito.Attribute.IdAttribute;
                            switch (TipoDocumento)
                            {
                                case "REFERTO":
                                    DocumentService.UpdateDocumentAttributeValue(document, thisParerEsito);
                                    break;
                                case "MODELLO770":
                                case "MODELLOF24":
                                case "CUD":
                                case "CEDOLINO STIPENDIALE":
                                case "CEDOLINO STIPENDIALE CUMULATIVO":
                                    DocumentService.UpdateDocumentAttributeValue(document.DocumentParent, thisParerEsito);
                                    break;
                            }
                        }
                    }

                    // invia email 
                    switch (TipoDocumento)
                    {
                        case "REFERTO":
                            try
                            {
                                MailHelper.SendMail(thisDocument.GetAttributeValue("Anno"),
                                    thisDocument.GetAttributeValue("Numero"),
                                    thisDocument.TipoRegistro,
                                    thisEsito.Errors, false);
                            }
                            catch
                            {
                            }
                            break;
                        case "MODELLO770":
                        case "MODELLOF24":
                        case "CUD":
                        case "CEDOLINO STIPENDIALE":
                        case "CEDOLINO STIPENDIALE CUMULATIVO":
                            break;
                    }
                }
                else
                {
                    // esito positivo
                    IsArchivedInPaRER = true;

                    try
                    {
                        if (Properties.Settings.Default.TraceEnable == true)
                        {
                            StreamWriter writer = File.AppendText(@"C:\BiblosDS2010\BiblosDSParerService\Request\Trace_" + document.IdBiblos.ToString() + ".txt");
                            writer.WriteLine("-- POSITIVO PARER --");
                            writer.Close();
                        }
                    }
                    catch
                    {
                    }

                    foreach (DocumentAttribute thisAttribute in AttributeService.GetAttributesFromArchive(document.Archive.IdArchive))
                    {
                        if (thisAttribute.Name.ToLower() == "pareruri")
                        {
                            DocumentAttributeValue thisParerUri = new DocumentAttributeValue(thisAttribute, thisEsito.Uri);
                            thisParerUri.IdAttribute = thisParerUri.Attribute.IdAttribute;
                            switch (TipoDocumento)
                            {
                                case "REFERTO":
                                    DocumentService.UpdateDocumentAttributeValue(document, thisParerUri);
                                    break;
                                case "MODELLO770":
                                case "MODELLOF24":
                                case "CUD":
                                case "CEDOLINO STIPENDIALE":
                                case "CEDOLINO STIPENDIALE CUMULATIVO":
                                    DocumentService.UpdateDocumentAttributeValue(document.DocumentParent, thisParerUri);
                                    break;
                            }
                        }
                        else if (thisAttribute.Name.ToLower() == "parerdate")
                        {
                            DocumentAttributeValue thisParerDate = new DocumentAttributeValue(thisAttribute, thisEsito.ArchiviedDate);
                            thisParerDate.IdAttribute = thisParerDate.Attribute.IdAttribute;
                            switch (TipoDocumento)
                            {
                                case "REFERTO":
                                    DocumentService.UpdateDocumentAttributeValue(document, thisParerDate);
                                    break;
                                case "MODELLO770":
                                case "MODELLOF24":
                                case "CUD":
                                case "CEDOLINO STIPENDIALE":
                                case "CEDOLINO STIPENDIALE CUMULATIVO":
                                    DocumentService.UpdateDocumentAttributeValue(document.DocumentParent, thisParerDate);
                                    break;
                            }
                        }
                        else if (thisAttribute.Name.ToLower() == "pareresito")
                        {
                            DocumentAttributeValue thisParerEsito = new DocumentAttributeValue(thisAttribute, "POSITIVO");
                            thisParerEsito.IdAttribute = thisParerEsito.Attribute.IdAttribute;
                            switch (TipoDocumento)
                            {
                                case "REFERTO":
                                    DocumentService.UpdateDocumentAttributeValue(document, thisParerEsito);
                                    break;
                                case "MODELLO770":
                                case "MODELLOF24":
                                case "CUD":
                                case "CEDOLINO STIPENDIALE":
                                case "CEDOLINO STIPENDIALE CUMULATIVO":
                                    DocumentService.UpdateDocumentAttributeValue(document.DocumentParent, thisParerEsito);
                                    break;
                            }
                        }
                    }
                }

                try
                {
                    if (Properties.Settings.Default.TraceEnable == true)
                    {
                        StreamWriter writer = File.AppendText(@"C:\BiblosDS2010\BiblosDSParerService\Request\Trace_" + document.IdBiblos.ToString() + ".txt");
                        writer.WriteLine("-- Aggiornamento Metadati BiblosDS effettuato --");
                        writer.Close();
                    }
                }
                catch
                {
                }
            }
            catch (Exception e)
            {
                // al PaRER è andato 
                // invia la response via email 
                StreamWriter writer = File.CreateText(@"C:\BiblosDS2010\BiblosDSParerService\Response\Response_" + document.IdBiblos.ToString() + "_" + TipoDocumento + "_" + thisDocument.Anno + "_" + thisDocument.Numero + ".xml");
                writer.Write(results);
                writer.Close();

                // invia email 
                try
                {
                    MailHelper.SendMail(thisDocument.GetAttributeValue("Anno"),
                        thisDocument.GetAttributeValue("Numero"),
                        thisDocument.TipoRegistro,
                        results, false);
                }
                catch
                {
                }

                // se non archiviato lascia il documento in transito
                if (IsArchivedInPaRER == false)
                    throw e;
            }

            return Content.LongLength;
        }
        #endregion 
    }
}
