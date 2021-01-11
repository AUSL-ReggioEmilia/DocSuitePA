using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BiblosDS.Library.Common;
using BiblosDS.Library.Common.Objects;

using BiblosDS.Library.Storage.PaRERReferti.Properties;

namespace BiblosDS.Library.Storage.PaRERReferti.Entities
{
    public class ParerConfig
    {
        private bool _ForzaConservazione;
        private bool _ForzaAccettazione;
        private bool _ForzaCollegamento;
        private string _TipoConservazione = "Sostitutiva";
        private bool _SimulaVersamento; 

        public ParerConfig()
        {
            _ForzaConservazione = Settings.Default.ParerForzaConservazione;
            _ForzaAccettazione = Settings.Default.ParerForzaAccettazione;
            _ForzaCollegamento = Settings.Default.ParerForzaCollegamento;
            _SimulaVersamento = Settings.Default.SimulaSalvataggioParer; 
        }

        public bool ForzaConservazione
        {
            get { return _ForzaConservazione; }
            set { _ForzaConservazione = value; } 
        }

        public bool ForzaAccettazione
        {
            get { return _ForzaAccettazione; }
            set { _ForzaAccettazione = value; }
        }

        public bool ForzaCollegamento
        {
            get { return _ForzaCollegamento; }
            set { _ForzaCollegamento = value; }
        }

        public string TipoConservazione
        {
            get { return _TipoConservazione; }
            set { _TipoConservazione = value; }
        }

        public bool SimulaVersamento
        {
            get { return _SimulaVersamento; }
            set { _SimulaVersamento = value; }
        }
    }

    public class ParerContext
    {
        private string _IdCliente;
        private string _Organizzazione;
        private string _Struttura;
        private string _user_id;
        private string _user_password;
        private string _Family;

        public string Family
        {
            get { return _Family; }
            set { _Family = value; }
        }

        public string IdCliente
        {
            get { return _IdCliente; }
            set { _IdCliente = value; }
        }

        public string Organizzazione
        {
            get { return _Organizzazione; }
            set { _Organizzazione = value; }
        }

        public string Struttura
        {
            get { return _Struttura; }
            set { _Struttura = value; }
        }

        public string UserId
        {
            get { return _user_id; }
            set { _user_id = value; }
        }

        public string UserPwd
        {
            get { return _user_password; }
            set { _user_password = value; }
        }
    }

    public enum ParerStatus : int { ERROR = 2, WARNING = 1, OK = 0, NEVER = -1 }

    public enum typeFirma : int { NON_FIRMATO = -1, FIRMATO = 1, FIRMATO_MARCATO, SOLO_MARCATO }

    public class Link
    {
        private string _Numero;
        private string _Anno;
        private string _Description; 
        public string _TipoRegistro ;

        /// <summary>
        /// Anno del documento
        /// </summary>
        public string Anno
        {
            get { return _Anno; }
            set { _Anno = value; }
        }

        /// <summary>
        /// Numero del documento
        /// </summary>
        public string Numero
        {
            get { return _Numero; }
            set { _Numero = value; }
        }

        /// <summary>
        /// Descrizione del collegamento / motivazione
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        /// <summary>
        /// serie = archivio di appartenenza 
        /// </summary>
        public string TipoRegistro
        {
            get { return _TipoRegistro ; }
            set { _TipoRegistro  = value; } 
        }
    }

    public class BiblosDocument
    {
        private string _idBiblos; 
        private string _NomeArchivioBiblos;
        private string  _TipoDocumento;
        private string _Descrizione;
        private string _Autore;
        private string _NomeDocumento;

        /// <summary>
        /// IdDocument BiblosDS 2010 
        /// </summary>
        private Guid _IdDocument;
        public Guid IdDocument
        {
            get { return _IdDocument; }
            set { _IdDocument = value; }
        }

        private byte[] _BinaryContent; 

        public byte[] BinaryContent
        {
            get
            {
                return _BinaryContent; 
            }
            set
            {
                _BinaryContent = value; 
            }
        }

        /// <summary>
        /// Id documento in Biblos
        /// </summary>
        /// <remarks>
        /// è una stringa per renderlo compatibile sia con Biblos DS sia con Biblos DS 2010
        /// </remarks>
        public string IdBiblos
        {
            get { return _idBiblos; }
            set { _idBiblos = value; }
        }

        /// <summary>
        /// Nome dell'archivio Biblos originario 
        /// </summary>
        public string NomeArchivioBiblos
        {
            get { return _NomeArchivioBiblos; }
            set { _NomeArchivioBiblos = value; }
        }

        /// <summary>
        /// chiave del contenuto documentale 
        /// </summary>
        /// <remarks>
        /// public perchè usato durante la trasmissione della richiesta 
        /// </remarks>
        public string IDDocumentKey(int Iteration)
        {
            return _NomeArchivioBiblos + "_" + _idBiblos + "_" + Iteration.ToString() ; 
        }

        /// <summary>
        /// Tipologia della firma digitale del documento 
        /// </summary>
        public typeFirma TipoFirma;

        internal string GetTipoFirma
        {
            get 
            {
                switch (TipoFirma) 
                {
                    case typeFirma.FIRMATO :
                        return "FIRMA"; 
                    case typeFirma.FIRMATO_MARCATO :
                        return "FIRMA MARCA"; 
                    case typeFirma.NON_FIRMATO :
                        return "CONTENUTO"; 
                    case typeFirma.SOLO_MARCATO :
                        return "MARCA"; 
                    default :
                        return "_undefined_"; 
                }
            }
        }

        /// <summary>
        /// tipo di documento (documento, allegato, atto...) 
        /// </summary>
        public string TipoDocumento
        {
            get { return _TipoDocumento; }
            set { _TipoDocumento = value; }
        }

        internal string GetTipoDocumento()
        {
            return _TipoDocumento; 
        }

        /// <summary>
        /// Anno del documento
        /// </summary>
        public string Descrizione
        {
            get { return _Descrizione; }
            set { _Descrizione = value; }
        }

        /// <summary>
        /// utente che ha inserito il documento
        /// </summary>
        public string Autore
        {
            get { return _Autore; }
            set { _Autore = value; } 
        }

        internal byte[] HashDocumento
        {
            get
            {
                if (_BinaryContent  == null)
                    return null ;
                if (_BinaryContent.Length  == 0)
                    return null ;

                System.Security.Cryptography.SHA1CryptoServiceProvider sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                sha1.Initialize();
                byte[] hash = sha1.ComputeHash(_BinaryContent);
                sha1.Clear();

                return hash; 
            }
        }

        public byte[] BytesDocumento
        {
            get
            {
                return _BinaryContent; 
            }
        }

        /// <summary>
        /// nome del file del documento
        /// </summary>
        public string NomeDocumento
        {
            get { return _NomeDocumento; }
            set { _NomeDocumento = value; }
        }

        /// <summary>
        /// estensione = formato del documento
        /// </summary>
        /// <remarks>
        /// venendo dall'estrattore della docsuiteweb o sono p7M , M7M , ...
        /// 20110228 per il parer estrae tutta l'intera estensione dei file se nel caso di doppia estensione
        /// 20111223 il parer vuole solo estensioni primarie, ovvero se .doc.p7m.p7m deve risultare come .doc.p7m 
        /// </remarks>
        internal string FormatoDocumento
        {
            get
            {
                if (_NomeDocumento == "")
                    return "";
                int npos = _NomeDocumento.LastIndexOf(".");
                if (npos == -1)
                    return "";

                string extensions = "";
                int lastpos = _NomeDocumento.Length; 
                do
                {
                    string thisEx = _NomeDocumento.Substring(npos, lastpos - npos).ToUpper(); // sempre maiuscolo 

                    // se è stampa conforme si ferma l'estrazione delle estensioni , perchè ciò che è a sinistra è stato convertito.
                    if (thisEx == ".SC") 
                        break ; 

                    if ((extensions.Contains(thisEx) == false) && ((thisEx.Length == 4)||(thisEx.Length == 5)))
                        extensions = thisEx + extensions;

                    // alcune estensioni chiudono l'estrazione delle successive a sinistra perchè non interessa o non possono avere tipi antecedenti (.doc, .docx,  .xls, .pdf) 
                    if (Properties.Settings.Default.LastCloseExtensions.Contains(thisEx))
                        break; 

                    lastpos = npos; 
                    npos = _NomeDocumento.Substring(0, lastpos - 1).LastIndexOf(".");
                }
                while (npos != -1);

                // toglie il primo . e fà ToUpper
                return extensions.Substring(1,extensions.Length-1).ToUpper()  ; 
            }
        }
    }

    public class Sign
    {
        public string Signature;
        public bool IsVicariale;
        public DateTime SignDate; 
    }

    public class Document
    {
        /// <summary>
        /// collezione degli attributi
        /// </summary>
        System.ComponentModel.BindingList<DocumentAttributeValue> _attributeValue = null;
        public System.ComponentModel.BindingList<DocumentAttributeValue> AttributesValue
        {
            set
            {
                _attributeValue = value;
            }
        }

        public string GetAttributeValue(string AttributeName)
        {
            try
            {
                foreach (DocumentAttributeValue item in _attributeValue)
                    if (item.Attribute.Name.ToUpper() == AttributeName.ToUpper())
                        return item.Value.ToString();

                return ""; 
            }
            catch
            {
                return "";
            }
        }

        internal List<Link> _Collegati = new List<Link>() ;

        /// <summary>
        /// aggiunge un link 
        /// </summary>
        /// <param name="thisCollegato"></param>
        public void AddCollegato(Link thisCollegato)
        {
            _Collegati.Add(thisCollegato);
        }

        /// <summary>
        /// Documento principale
        /// </summary>
        private BiblosDocument _Documento = new BiblosDocument();
        /// <summary>
        /// Documento principale
        /// </summary>
        public BiblosDocument Documento
        {
            get { return _Documento; }
            set { _Documento = value; }
        }

        /// <summary>
        /// tipologia della serie documentale di appartenenza
        /// </summary>
        public string _Serie;
        public string Serie
        {
            get { return _Serie; }
            set { _Serie = value; }
        }

        private string[] _EsitiParer;
        private ParerStatus _StatoParer;
        private string _KeyParer;

        /// <summary>
        /// chiave univoca di conservazione presso il parer
        /// </summary>
        public string KeyParer
        {
            get { return _KeyParer; }
            set { _KeyParer = value; }
        }

        /// <summary>
        /// stato dell'invio al parer 
        /// </summary>
        public ParerStatus StatoParer
        {
            get { return _StatoParer; }
            set { _StatoParer = value; }
        }

        /// <summary>
        /// array degli esiti di invii al parer
        /// </summary>
        /// <remarks>
        /// può essere null 
        /// </remarks>
        public string[] EsitiParer
        {
            get { return _EsitiParer; }
            set { _EsitiParer = value; }
        }

        
        /// <summary>
        /// Anno del documento
        /// </summary>
        string _Anno; 
        public string Anno
        {
            get { return _Anno; }
            set { _Anno = value; }
        }

        /// <summary>
        /// numero di atto / protocollo associato al documento
        /// </summary>
        /// <remarks>
        /// quando imposta il numero se è un service number lo estre
        /// </remarks>
        string _Numero; 
        public string Numero
        {
            get { return _Numero; }
            set { _Numero = value; }
        }

        /// <summary>
        /// Data del documento
        /// </summary>
        DateTime _Data; 
        public DateTime Data
        {
            get { return _Data; }
            set { _Data = value; }
        }

        /// <summary>
        /// data inserimento e data di protocollazione
        /// </summary>
        DateTime _DataInserimentoDocumento; 
        public DateTime DataInserimentoDocumento
        {
            get { return _DataInserimentoDocumento; }
            set { _DataInserimentoDocumento = value; }
        }

        /// <summary>
        /// tipologia del registro 
        /// </summary>
        string _TipoRegistro; 
        public string TipoRegistro
        {
            get { return _TipoRegistro; }
            set { _TipoRegistro = value; }
        }

        string _Oggetto;
        public string Oggetto
        {
            get { return _Oggetto; }
            set { _Oggetto = value; } 
        }
    }

    public class Documents
    {
        private List<Document> _DocumentList = new List<Document>();

        /// <summary>
        /// lista dei documenti 
        /// </summary>
        public List<Document> DocumentList
        {
            get { return _DocumentList; }
            set { _DocumentList = value; }
        }

        public Document[] DocumentArray
        {
            get { return _DocumentList.ToArray(); }
            set
            {
                Document[] docArray = value;
                for (int iDoc = 0; iDoc < docArray.Length; iDoc++)
                {
                    _DocumentList.Add(docArray[iDoc]);
                }
            }
        }
    }
}
