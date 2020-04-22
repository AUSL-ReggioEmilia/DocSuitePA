using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Repository.EF.DataContexts;

namespace VecompSoftware.DocSuiteWeb.Service.EF.Test.FakeRepositories.Collaborations
{
    internal class FakeCollaborationDbSet : FakeDbSet<Collaboration>
    {
        public override Collaboration Find(params object[] keyValues)
        {
            return this.SingleOrDefault(t => t.EntityId == (long)keyValues.FirstOrDefault());
        }

        public override Task<Collaboration> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return Task.Factory.StartNew(() => Find(keyValues), cancellationToken);
        }
    }
}
