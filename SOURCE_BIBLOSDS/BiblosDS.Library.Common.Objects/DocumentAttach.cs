using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "DocumentAttach", Namespace = "http://BiblosDS/2009/10/DocumentAttach")]
    public partial class DocumentAttach : Document
    {       
        /// <summary>
        /// Id del documento
        /// </summary>
        [DataMember]
        public Guid IdDocumentAttach { get; set; }               
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int AttachOrder { get; set; }       

        [DataMember]
        public Document Document { get; set; }                
    }
   
}
