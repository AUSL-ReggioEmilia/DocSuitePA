using System;
using System.ComponentModel.DataAnnotations;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.HR.Common
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
    }
}
