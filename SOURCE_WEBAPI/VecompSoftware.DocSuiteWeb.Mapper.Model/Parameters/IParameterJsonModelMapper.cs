
namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Parameters
{
    public interface IParameterJsonModelMapper<TModel> : IDomainMapper<string, TModel>
        where TModel : class, new()
    {
    }
}
