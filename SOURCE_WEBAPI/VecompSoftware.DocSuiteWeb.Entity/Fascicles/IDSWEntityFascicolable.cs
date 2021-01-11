
namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public interface IDSWEntityFascicolable<T> : IDSWEntity
        where T : DSWBaseEntity
    {
        #region [ Properties ]

        ReferenceType ReferenceType { get; set; }

        #endregion

        #region [ Navigation Properties ]

        Fascicle Fascicle { get; set; }

        #endregion

    }
}
