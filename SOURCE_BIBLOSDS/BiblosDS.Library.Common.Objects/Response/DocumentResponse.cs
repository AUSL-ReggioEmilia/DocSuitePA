using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects.Response
{
    [DataContract(Name = "DocumentResponse", Namespace = "http://BiblosDS/2009/10/DocumentResponse")]
    [KnownType(typeof(ResponseBase))]
    public class DocumentResponse: ResponseBase
    {
        /// <summary>
        /// Documenti ritornati dalla ricerca
        /// </summary>
        [DataMember]
        public BindingList<Document> Documents { get; set; }
    }
}
