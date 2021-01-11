using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using BiblosDS.Library.Common.Enums;

namespace BiblosDS.Library.Common.Objects.Response
{
    [DataContract(Name = "PreservationResponse", Namespace = "http://BiblosDS/2009/10/PreservationResponse")]
    [KnownType(typeof(PreservationError))]
    public class PreservationResponse : ResponseBase
    {
        [DataMember]
        public override ResponseError Error
        {
            get { return (base.Error == null) ? null : base.Error as PreservationError; }

            set
            {
                base.Error = null;

                //if (value != null)
                if (value is PreservationError)
                {
                    if (!(value is PreservationError))
                        new PreservationError("Invalid PreservationError in PreservationResponse.", PreservationErrorCode.E_INVALID_CALL).ThrowsAsFaultException();

                    base.Error = value;
                }
            }
        }
    }

    [DataContract(Name = "PreservationError", Namespace = "http://BiblosDS/2009/10/PreservationError")]
    public class PreservationError : ResponseError
    {
        [DataMember]
        public override int ErrorCode
        {
            get
            {
                return (int)this.PreservationErrorCode;
            }

            set
            {
                try
                {
                    this.PreservationErrorCode = (PreservationErrorCode)Enum.Parse(typeof(PreservationErrorCode), value.ToString(), true);
                }
                catch
                {
                    this.PreservationErrorCode = PreservationErrorCode.E_USER_DEFINED_EXCEPTION;
                }
            }
        }

        [DataMember]
        public PreservationErrorCode PreservationErrorCode { get; set; }

        public PreservationError() : this(string.Empty, PreservationErrorCode.E_NO_ERROR) { /* EMPTY */ }

        public PreservationError(Exception ex) : this(ex, PreservationErrorCode.E_NO_ERROR) { /* EMPTY */ }

        public PreservationError(Exception ex, PreservationErrorCode errorCode)
            : base(ex, (int)errorCode) { /* EMPTY */ }

        public PreservationError(string message) : this(message, PreservationErrorCode.E_NO_ERROR) { /* EMPTY */ }

        public PreservationError(string message, PreservationErrorCode errorCode)
            : base(message, (int)errorCode) { /* EMPTY */}
    }
}
