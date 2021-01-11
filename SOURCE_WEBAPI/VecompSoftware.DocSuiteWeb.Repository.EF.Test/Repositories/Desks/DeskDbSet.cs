using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.Repository.EF.Test.Repositories.Desks
{
    public class DeskDbSet : FakeDbSet<Desk>
    {
        public override Desk Find(params object[] keyValues)
        {
            return this.SingleOrDefault(t => t.UniqueId == (Guid)keyValues.FirstOrDefault());
        }

        public override Task<Desk> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return new Task<Desk>(() => Find(keyValues));
        }
    }
}
