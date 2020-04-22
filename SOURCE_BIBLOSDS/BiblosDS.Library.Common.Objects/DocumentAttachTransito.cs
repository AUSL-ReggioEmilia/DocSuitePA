using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Enums;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "DocumentAttachTransito", Namespace = "http://BiblosDS/2009/10/DocumentAttachTransito")]
    public class DocumentAttachTransito : DocumentTransito
    {
        public Guid IdDocumentAttach { get; set; }

        public DocumentAttach Attach { get; set; }
    }
}
