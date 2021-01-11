using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Repository.EF.DataContexts;

namespace VecompSoftware.DocSuiteWeb.Service.EF.Test.FakeRepositories.Desks
{
    public class FakeDeskDbSet : FakeDbSet<Desk>
    {
        public override Desk Find(params object[] keyValues)
        {
            return this.SingleOrDefault(t => t.UniqueId == (Guid)keyValues.FirstOrDefault());
        }

        public override Task<Desk> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return Task.Factory.StartNew(() => Find(keyValues), cancellationToken);
        }
    }
}
