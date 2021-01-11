using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;

namespace VecompSoftware.DocSuiteWeb.Data
{
    public interface IDataUnitOfWork : IUnitOfWorkAsync
    {
        Task<bool> SaveAsync();
    }
}
