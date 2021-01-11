using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Repository.DataContext;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;

namespace VecompSoftware.DocSuiteWeb.Data
{
    public interface IDSWDataContext : IDataContextAsync
    {
        IQueryable<T> DataSet<T>() where T : class;

        IEnumerable<TModel> ExecuteModelFunction<TModel>(string functionName, params IQueryParameter[] parameters);
    }
}
