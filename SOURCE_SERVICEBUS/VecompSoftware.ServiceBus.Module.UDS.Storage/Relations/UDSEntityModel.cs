using System;
using System.Collections.Generic;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Relations
{
    public class UDSEntityModel
    {
        public UDSEntityModel()
        {
            Logs = new HashSet<UDSLogModel>();
            Users = new HashSet<UDSUserModel>();
        }

        public Guid IdUDS { get; set; }

        public short? Year { get; set; }

        public int? Number { get; set; }

        public UDSRelations Relations { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public string LastChangedUser { get; set; }

        public string Subject { get; set; }

        public string Title { get; set; }

        public short? IdCategory { get; set; }

        public short? IdContainer { get; set; }

        public bool DocumentUnitSynchronizeEnabled { get; set; }

        public ICollection<UDSLogModel> Logs { get; set; }

        public ICollection<UDSUserModel> Users { get; set; }

        public string FullNumber => string.Format("{0}/{1:0000000}", Year, Number);
    }
}
