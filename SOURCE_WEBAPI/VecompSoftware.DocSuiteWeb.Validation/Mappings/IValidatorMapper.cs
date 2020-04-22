using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings
{
    public interface IValidatorMapper<T, E> : IMapper<T, E>
        where E : IObjectValidator<T>
    {
    }
}
