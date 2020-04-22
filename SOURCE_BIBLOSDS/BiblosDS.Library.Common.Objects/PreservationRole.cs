using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects
{

    [DataContract(Name = "PreservationRole", Namespace = "http://BiblosDS/2009/10/PreservationRole")]
    public class PreservationRole : BiblosDSObject
    {
        [DataMember]
        public Guid IdPreservationRole { get; set; }

        [DataMember]
        public int KeyCode { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public bool AlertEnabled { get; set; }

        [DataMember]
        public BindingList<PreservationUserRole> UserRoles { get; set; }
    }
}
