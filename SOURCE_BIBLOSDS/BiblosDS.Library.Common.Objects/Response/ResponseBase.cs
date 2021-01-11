using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace BiblosDS.Library.Common.Objects.Response
{
    [DataContract(Name = "ResponseBase", Namespace = "http://BiblosDS/2009/10/ResponseBase")]
    public abstract class ResponseBase
    {
        /// <summary>
        /// Exception throw
        /// </summary>
        [DataMember]
        public virtual ResponseError Error { get; set; }
        /// <summary>
        /// Numero di record risultato della richiesta
        /// </summary>
        [DataMember]
        public long TotalRecords { get; set; }
        [DataMember]
        public bool HasErros
        {
            get { return this.Error != null; }
            set { /*EMPTY*/ }
        }
    }

    [DataContract(Name = "ResponseError", Namespace = "http://BiblosDS/2009/10/ResponseError")]
    public class ResponseError
    {
        /// <summary>
        /// Codice di errore predefinito in caso di successo (operazione request\response eseguita senza errori).
        /// </summary>
        public const int E_NO_ERROR = 0;
        [DataMember]
        public virtual int ErrorCode { get; set; }
        [DataMember]
        public string StackTrace { get; set; }
        [DataMember]
        public string Message { get; set; }

        public ResponseError() : this(string.Empty, E_NO_ERROR) { /* EMPTY */ }

        public ResponseError(Exception ex) : this(ex, E_NO_ERROR) { /* EMPTY */ }

        public ResponseError(Exception ex, int errorCode)
        {
            if (ex != null)
            {
                StackTrace = ex.StackTrace;
                Message = ex.Message;
            }

            this.ErrorCode = errorCode;
        }

        public ResponseError(string message) : this(message, E_NO_ERROR) { /* EMPTY */ }

        public ResponseError(string message, int errorCode)
        {
            this.Message = message;
            this.ErrorCode = errorCode;
            this.StackTrace = string.Empty;
        }

        public void ThrowsAsFaultException()
        {
            throw new FaultException<ResponseError>(this, this.Message ?? string.Empty);
        }
    }
}
