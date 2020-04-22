using System;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity
{

    public abstract class DSWBaseEntity : BaseEntity, IDSWEntity
    {
        private string _timestampBase64 = string.Empty;

        private DSWBaseEntity() : base() { }

        protected DSWBaseEntity(Guid uniqueId) : this()
        {
            UniqueId = uniqueId;
        }

        public string TimestampBase64
        {
            get
            {
                if (string.IsNullOrEmpty(_timestampBase64))
                {
                    _timestampBase64 = Timestamp == null ? string.Empty : Convert.ToBase64String(Timestamp);
                }
                return _timestampBase64;
            }
        }

        public virtual string GetTitle()
        {
            return UniqueId.ToString();
        }
    }
}
