using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using VecompSoftware.Common;

namespace VecompSoftware.DataContract.Documents
{
    [DataContract(Name = "DocumentSignInfo", Namespace = "http://BiblosDS/2009/10/DocumentSignInfo")]
    public class DocumentSignInfo : IDataContract
    {
        /// <summary>
        /// Id del documento
        /// </summary>
        [DataMember]
        public Guid IdDocument { get; set; }

        /// <summary>
        /// Nome del documento
        /// </summary>
        [DataMember]
        public string DocumentName { get; set; }

        /// <summary>
        /// Data e ora di firma
        /// </summary>
        [DataMember]
        public DateTime? SignDate { get; set; }

        /// <summary>
        /// Tipo di Firma (CAdES o PAdES)
        /// </summary>
        [DataMember]
        public string SignType { get; set; }

        /// <summary>
        /// Nome e cognome del firmatario
        /// </summary>
        [DataMember]
        public string SignUser { get; set; }

        /// <summary>
        /// Lista di eventuali firme annidate
        /// </summary>
        [DataMember]
        public BindingList<DocumentSignInfo> ChildSignatures { get; set; }

        /// <summary>
        /// Numero Serie Carta
        /// </summary>
        [DataMember]
        public string SerialNumber { get; set; }
        public DocumentSignInfo() { }

        public DocumentSignInfo(SignInfo signInfo)
        {
            this.SignDate = signInfo.SignDate;
            this.SignType = signInfo.SignType;
            if (signInfo.HasCertificate)
            {
                this.SerialNumber = signInfo.Certificate.SerialNumber;
            }
            if (signInfo.HasCertificate && signInfo.Certificate.HasSubject)
            {
                this.SignUser = signInfo.Certificate.Subject.CommonName;
            }
            this.ChildSignatures = new BindingList<DocumentSignInfo>();
            if (signInfo.HasChildren)
            {
                this.ChildSignatures = new BindingList<DocumentSignInfo>(signInfo.Children.Select(f => new DocumentSignInfo(f)).ToList());
            }
        }
    }
}
