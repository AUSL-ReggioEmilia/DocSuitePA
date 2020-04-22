using BiblosDS.Library.Common.Objects;
using System.Runtime.Serialization;

namespace BiblosDS.API
{
    /// <summary>
    /// Metadato
    /// </summary>
    [DataContract]
    public sealed class MetadatoItem
    {
        /// <summary>
        /// Valore del metadato nel tipo definito nell'attributo        
        /// </summary>
        /// <remarks>
        /// I tipi gestiti sono:
        /// Int64
        /// Double
        /// DateTime
        /// String
        /// Per la verifica del tipo di attributo utlizzare la GetAttributes() parametro <see cref="DocumentAttribute">AttributeType</see> 
        /// </remarks>
        [DataMember]
        public string Tipo { get; set; }
        [DataMember]
        public short? Posizione { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public object Value { get; set; }
        [DataMember]
        public bool Obbligatorio { get; set; }
    }
}