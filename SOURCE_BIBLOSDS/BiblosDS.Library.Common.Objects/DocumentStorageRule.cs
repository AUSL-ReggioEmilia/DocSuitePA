using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    /// <summary>
    /// Ruolo associato ad uno storage
    /// </summary>    
    [DataContract(Name = "StorageRule", Namespace = "http://BiblosDS/2009/10/StorageRule")]
    public partial class DocumentStorageRule : BiblosDSObject
    {
        [DataMember]
        public DocumentStorage Storage { get; set; }

        [DataMember]
        public DocumentAttribute Attribute { get; set; }

        [DataMember]
        public short RuleOrder { get; set; }

        /// <summary>
        /// Filtro da applivare alla role.
        /// <example>
        /// L'archivio fatture
        /// sarà generalizzato in:
        /// Fatture 2007 avrà come KeyFilter 2007
        /// Fatture 2006 avrà come KeyFilter 2006
        /// </example>
        /// Con questo parametro si definiscono più
        /// rule automatiche per un determinato archivio
        /// <remarks>
        /// Costruire un indice sopra questo parametro 
        /// in modo che l'associazione di Archive, Storage e KeyFilter 
        /// sia univoca
        /// Valutare l'utilità di parametri automatici es: $Year per avere una role
        /// automatica sull'anno corrente.
        /// </remarks>        
        /// </summary>
        [DataMember]
        public string RuleFilter { get; set; }

        [DataMember]
        public string RuleFormat { get; set; }

        [DataMember]
        public DocumentRuleOperator RuleOperator { get; set; }

        #region Constructor

        public DocumentStorageRule()
        {
        }

        public DocumentStorageRule(DocumentAttribute attribute, short ruleOrder, string ruleFilter, string ruleFormat)
        {
            this.Attribute = attribute;
            this.RuleOrder = ruleOrder;
            this.RuleFilter = ruleFilter;
            this.RuleFormat = ruleFormat;
        }
        #endregion
    }
}
