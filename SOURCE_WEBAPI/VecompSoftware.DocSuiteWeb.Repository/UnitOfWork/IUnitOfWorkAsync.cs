using System.Threading;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Repository.UnitOfWork
{
    public interface IUnitOfWorkAsync : IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}