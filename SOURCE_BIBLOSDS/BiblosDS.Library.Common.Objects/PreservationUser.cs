using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationUser", Namespace = "http://BiblosDS/2009/10/PreservationUser")]
    public class PreservationUser : BiblosDSObject
    {
        [DataMember]
        public Guid IdPreservationUser { get; set; }

        [DataMember]
        public BindingList<PreservationUserRole> UserRoles { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Surname { get; set; }

        [DataMember]
        public string FiscalId { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string EMail { get; set; }

        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public string DomainUser { get; set; }

        [DataMember]
        public string ArchiveName { get; set; }
    }
}
