namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public interface IDSWUDSRelationEntity<T> : IDSWEntity
      where T : DSWBaseEntity
    {
        #region [ Properties ]

        UDSRelationType RelationType { get; set; }

        #endregion

        #region [ Navigation Properties ]

        T Relation { get; set; }
        UDSRepository Repository { get; set; }

        #endregion

    }
}
