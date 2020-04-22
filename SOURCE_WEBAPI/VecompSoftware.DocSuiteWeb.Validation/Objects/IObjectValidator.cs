using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects
{
    public interface IObjectValidator<T> : IValidator<T>
    {
        IObjectValidator<T> MappingFrom(T entity);
        IDataUnitOfWork UnitOfWork { get; }

        ISecurity CurrentSecurity { get; }

    }
}
