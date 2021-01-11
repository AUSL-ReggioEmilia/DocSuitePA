using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects.Response
{
    [DataContract(Name = "PreservationFileInfoResponse", Namespace = "http://BiblosDS/2009/10/PreservationFileInfoResponse")]
    public class PreservationFileInfoResponse : ResponseBase
    {
        [DataMember]
        public byte[] File { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string DigitalSignFileExtension { get; set; }

        [DataMember]
        public string InfoCamereFileExtension { get; set; }
    }
}
