using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity
{
    public interface IDSWLogEntity<TLogEntity, TType> : IDSWEntity
        where TLogEntity : DSWBaseEntity
    {
        #region [ Properties ]

        string SystemComputer { get; set; }

        TType LogType { get; set; }

        string LogDescription { get; set; }

        SeverityLog? Severity { get; set; }

        #endregion

        #region [ Navigation Properties ]

        TLogEntity Entity { get; set; }

        #endregion
    }
}
