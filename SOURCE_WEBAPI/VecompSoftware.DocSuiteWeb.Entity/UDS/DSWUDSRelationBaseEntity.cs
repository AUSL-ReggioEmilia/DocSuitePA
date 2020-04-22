using System;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public abstract class DSWUDSRelationBaseEntity<T> : DSWUDSBaseEntity, IDSWUDSRelationEntity<T>
         where T : DSWBaseEntity
    {
        protected DSWUDSRelationBaseEntity(Guid uniqueId) : base(uniqueId)
        {
        }

        #region [ Properties ]
        public UDSRelationType RelationType { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual T Relation { get; set; }
        #endregion

    }
}
