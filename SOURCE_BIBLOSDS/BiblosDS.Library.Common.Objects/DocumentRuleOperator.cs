using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    /// <summary>
    /// Rule Operator associato ad uno storage area rule
    /// </summary>    
    [DataContract(Name = "RuleOperator", Namespace = "http://BiblosDS/2009/10/RuleOperator")]
    public partial class DocumentRuleOperator : BiblosDSObject
    {
        [DataMember]
        public int IdRuleOperator { get; set; }
        
        [DataMember]
        public string Descrizione { get; set; }

        #region Constructor

        public DocumentRuleOperator()
        {
        }

        public DocumentRuleOperator(string descrizione)
        {
            this.Descrizione = descrizione;
        }
        #endregion
    }
}
