using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "PreservationStorageDevice", Namespace = "http://BiblosDS/2009/10/PreservationStorageDevice")]
    public class PreservationStorageDevice : BiblosDSObject
    {
        [DataMember]
        public Guid IdPreservationStorageDevice { get; set; }

        [DataMember]
        public PreservationStorageDevice OriginalPreservationStorageDevice { get; set; }

        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public PreservationStorageDeviceStatus Status { get; set; }

        [DataMember]
        public Nullable<DateTime> DateStorageDevice { get; set; }

        [DataMember]
        public Nullable<DateTime> DateCreated { get; set; }

        [DataMember]
        public Nullable<DateTime> LastVerifyDate { get; set; }

        [DataMember]
        public Nullable<DateTime> MinDate { get; set; }

        [DataMember]
        public Nullable<DateTime> MaxDate { get; set; }

        [DataMember]
        public PreservationUser User { get; set; }

        [DataMember]
        public BindingList<PreservationInStorageDevice> PreservationsInDevice { get; set; }

        [DataMember]
        public string EntratelCompleteFileName { get; set; }

        [DataMember]
        public DateTime? EntratelUploadDate { get; set; }

        [DataMember]
        public string Company { get; set; }

        [DataMember]
        public Guid? IdCompany { get; set; }
    }
}
