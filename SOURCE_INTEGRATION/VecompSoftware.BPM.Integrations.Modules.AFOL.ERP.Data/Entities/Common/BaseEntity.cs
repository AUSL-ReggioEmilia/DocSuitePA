using System;
using System.ComponentModel.DataAnnotations;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities.Common
{
    public abstract class BaseEntity
    {
        public BaseEntity()
            : this(Guid.NewGuid())
        {

        }

        public BaseEntity(Guid uniqueId)
        {
            Id = uniqueId;
        }

        public Guid Id { get; set; }

        [Timestamp]
        [ConcurrencyCheck]
        public byte[] Timestamp { get; set; }
    }
}
