using System;
using System.Collections.Generic;
using System.ComponentModel;
using VecompSoftware.Common;

namespace VecompSoftware.DataContract.BiblosDS.Documents
{
    public interface IDocumentSignInfo : IDataContract
    {
        Guid IdDocument { get; set; }
        Guid IdChain { get; set; }
        BindingList<SignInfo> XmlSignatureInfo { get; set; }
    }
}
