using System;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public class UDSFieldList : DSWBaseEntity
    {
        #region [ Constructor ]
        public UDSFieldList() : this(Guid.NewGuid()) { }
        public UDSFieldList(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public string FieldName { get; set; }
        public string Name { get; set; }
        public UDSFieldListStatus Status { get; set; }
        public int Environment { get; set; }
        public string UDSFieldListPath { get; set; }
        public short UDSFieldListLevel { get; set; }
        public Guid? ParentInsertId { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual UDSRepository Repository { get; set; }

        #endregion
    }
}
