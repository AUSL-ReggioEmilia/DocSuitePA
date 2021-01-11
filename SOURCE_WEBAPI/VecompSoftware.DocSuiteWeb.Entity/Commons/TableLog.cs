using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class TableLog : DSWBaseEntity
    {
        #region [ Constructor ]

        public TableLog() : this(Guid.NewGuid()) { }

        public TableLog(Guid uniqueId)
            : base(uniqueId)
        { }
        #endregion

        #region [ Properties ]

        public int? LoggedEntityId { get; set; }

        public Guid? LoggedEntityUniqueId { get; set; }

        public TableLogEvent LogType { get; set; }

        public string TableName { get; set; }

        public string SystemComputer { get; set; }

        public string LogDescription { get; set; }

        public string Hash { get; set; }

        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
