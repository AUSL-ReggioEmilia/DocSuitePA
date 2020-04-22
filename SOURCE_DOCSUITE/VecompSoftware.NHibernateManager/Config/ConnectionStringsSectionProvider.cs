using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Connection;

namespace VecompSoftware.NHibernateManager.Config
{
    public class ConnectionStringsSectionConnectionProvider : ConnectionProvider
    {

        public override DbConnection GetConnection()
        {
            ConnectionStringSettings settings = NHibernateSessionManager.Instance.SessionConnectionStrings[this.ConnectionString];
            if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
            {
                return this.Driver.CreateConnection();
            }

            DbConnection conn = this.Driver.CreateConnection();
            conn.ConnectionString = settings.ConnectionString;
            conn.Open();
            return conn;
        }

        public override async Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken)
        {
            return await Task.Run(() => GetConnection());
        }
    }
}
