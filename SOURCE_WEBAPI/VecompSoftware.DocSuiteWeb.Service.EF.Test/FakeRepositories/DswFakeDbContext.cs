using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Repository.EF.DataContexts;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Service.EF.Test.FakeRepositories.Collaborations;
using VecompSoftware.DocSuiteWeb.Service.EF.Test.FakeRepositories.Desks;
using VecompSoftware.DocSuiteWeb.Service.EF.Test.FakeRepositories.Messages;

namespace VecompSoftware.DocSuiteWeb.Service.EF.Test.FakeRepositories
{
    public class DswFakeDbContext : FakeDbContext, IDSWDataContext
    {
        public DswFakeDbContext()
        {
            AddFakeDbSet<Desk, FakeDeskDbSet>();
            AddFakeDbSet<Collaboration, FakeCollaborationDbSet>();
            AddFakeDbSet<Message, FakeMessageDbSet>();
        }

        public IQueryable<T> DataSet<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TModel> ExecuteModelFunction<TModel>(string functionName, params IQueryParameter[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
