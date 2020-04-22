using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VecompSoftware.DocSuiteWeb.Repository.Infrastructure;

namespace VecompSoftware.DocSuiteWeb.Repository.Entity
{
    [Serializable]
    public abstract class BaseEntity : IEntity
    {
        [NotMapped]
        public ObjectState ObjectState { get; set; }

        public int EntityId { get; set; }

        public short EntityShortId { get; set; }

        public Guid UniqueId { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        [Timestamp]
        [ConcurrencyCheck]
        public byte[] Timestamp { get; set; }

    }
}