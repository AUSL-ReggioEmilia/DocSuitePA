using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Service
{
    public interface IServiceAsync<TContent, TResult> : IService<TContent, TResult>
          where TContent : class
          where TResult : class
    {
        Task<TResult> CreateAsync(TContent content);

        Task<TResult> UpdateAsync(TContent content);

        Task<bool> DeleteAsync(TContent content);

    }
}