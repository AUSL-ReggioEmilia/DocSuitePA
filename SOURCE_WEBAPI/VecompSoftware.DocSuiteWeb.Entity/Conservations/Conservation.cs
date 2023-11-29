using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Conservations
{
    public class Conservation : DSWBaseEntity
    {
        #region [ Constructor ]
        public Conservation() : this(Guid.NewGuid()) { }
        public Conservation(Guid uniqueId)
            : base(uniqueId)
        {

        }
        #endregion

        #region [ Properties ]
        public string EntityType { get; set; }
        public ConservationStatus Status { get; set; }
        public string Message { get; set; }
        public ConservationType Type { get; set; }
        public DateTimeOffset? SendDate { get; set; }
        public DateTimeOffset? ConservationDate { get; set; }
        public string Uri { get; set; }
        #endregion
    }
}
