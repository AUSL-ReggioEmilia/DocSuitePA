using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Repository.EF.DataContexts;

namespace VecompSoftware.DocSuiteWeb.Service.EF.Test.FakeRepositories.Messages
{
    public class FakeMessageDbSet : FakeDbSet<Message>
    {
        public override Message Find(params object[] keyValues)
        {
            return this.SingleOrDefault(t => t.EntityId == (long)keyValues.FirstOrDefault());
        }

        public override Task<Message> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return Task.Factory.StartNew(() => Find(keyValues), cancellationToken);
        }
    }
}
