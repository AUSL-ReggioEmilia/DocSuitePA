using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.UDS
{
    public class UDSRepositoryTableValuedModel : IContainerTableValuedModel
    {
        public Guid IdUDSRepository { get; set; }

        public string Name { get; set; }

        public short IdContainer { get; set; }

        public short SequenceCurrentYear { get; set; }

        public int SequenceCurrentNumber { get; set; }

        public short Version { get; set; }

        public DateTimeOffset ActiveDate { get; set; }

        public DateTimeOffset? ExpiredDate { get; set; }

        public UDSRepositoryStatus Status { get; set; }

        public string Alias { get; set; }

        public int DSWEnvironment { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public short? Container_IdContainer { get; set; }

        public string Container_Name { get; set; }
    }
}
