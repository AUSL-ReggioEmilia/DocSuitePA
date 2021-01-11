using System;
using System.Runtime.Serialization;
using System.Text;
using System.Reflection;
namespace BiblosDS.API
{
    [DataContract]
    public class RequestBase
    {
        /// <summary>
        /// Token di autenticazione 
        /// </summary>
        [DataMember]
        public Guid Token { get; set; }
        [DataMember]
        public string IdClient { get; set; }
        [DataMember]
        public string IdRichiesta { get; set; }
        [DataMember]
        public string IdCliente { get; set; }

        /// <summary>
        /// ClassCode presente nell'xml di configurazione
        /// </summary>
        [DataMember]
        public string TipoDocumento { get; set; }
        /// <summary>
        /// Chiave del documento
        /// </summary>
        /// <remarks>
        /// Valore calcolato a partire dagli attributi
        /// </remarks>
        [DataMember]
        public string Chiave { get; set; }

        private PropertyInfo[] _PropertyInfos = null;

        public override string ToString()
        {
            if (_PropertyInfos == null)
                _PropertyInfos = this.GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var info in _PropertyInfos)
            {
                sb.Append(" - " + info.Name + ": " + info.GetValue(this, null));
            }

            return sb.ToString();
        }
    }

    [DataContract]
    public class RequestIdBase : RequestBase
    {
        /// <summary>
        /// Id BiblosDS del documento
        /// </summary>
        [DataMember]
        public Guid IdDocumento { get; set; }
    }    

    [DataContract]    
    public class ResponseBase
    {
        [DataMember]
        public TokenInfo TokenInfo { get; set; }
        /// <summary>
        /// True se l'operazione è stata eseguita
        /// False altrimenti
        /// </summary>
        [DataMember]
        public bool Eseguito { get; set; }

        /// <summary>
        /// Id della richiesta
        /// </summary>
        [DataMember]
        public string IdRichiesta { get; set; }

        /// <summary>
        /// Codice di risposta
        /// </summary>
        [DataMember]
        public CodiceErrore CodiceEsito { get; set; }

        /// <summary>
        /// Descrizione estesa dell'errore
        /// </summary>
        [DataMember]
        public string MessaggioEsito { get; set; }

        /// <summary>
        /// Descrizione estesa dell'errore
        /// </summary>
        [DataMember]
        public string MessaggioErrore { get; set; }
        /// <summary>
        /// Id BiblosDS del documento
        /// </summary>
        [DataMember]
        public Guid IdDocumento { get; set; }

        private PropertyInfo[] _PropertyInfos = null;

        public override string ToString()
        {
            if (_PropertyInfos == null)
                _PropertyInfos = this.GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var info in _PropertyInfos)
            {
                sb.Append(" - " + info.Name + ": " + info.GetValue(this, null));
            }

            return sb.ToString();
        }
    }
    
    [DataContract]
    public class TokenInfo
    {
        /// <summary>
        /// Token utilizzato per l'autenticaziione
        /// </summary>
        [DataMember]
        public Guid Token { get; set; }
        /// <summary>
        /// Data/Ora di scadenza del token
        /// </summary>
        [DataMember]
        public DateTime DataScadenza { get; set; }
        
        internal string IdCliente { get; set; }
    }
}