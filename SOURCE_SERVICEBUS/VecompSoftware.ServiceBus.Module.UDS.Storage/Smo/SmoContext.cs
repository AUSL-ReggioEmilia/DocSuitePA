using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Smo
{
    /// <summary>
    /// Classe di helper su oggetti del SQL Server Management Objects 
    /// </summary>
    public class SmoContext : IDisposable
    {
        private readonly ServerConnection conn = null;
        private readonly Server srv = null;
        private readonly string dbSchema = string.Empty;
        public Database DbInstace { get; set; }

        public SmoContext(string connStr, string dbSchema = "")
        {
            string _connStr = ConfigurationManager.ConnectionStrings[connStr].ConnectionString;
            conn = new ServerConnection(new SqlConnection(_connStr));
            srv = new Server(conn);

            SqlConnectionStringBuilder strBuilder = new SqlConnectionStringBuilder(_connStr);
            DbInstace = srv.Databases[strBuilder.InitialCatalog];

            this.dbSchema = dbSchema;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            if (conn != null)
            {
                conn.Disconnect();
            }
        }


        public Table GetTable(string tableName)
        {
            return TableExist(tableName) ? DbInstace.Tables[tableName, dbSchema] : null;
        }


        public Table GetNewTable(string tableName)
        {
            return new Table(DbInstace, tableName, dbSchema);
        }


        public bool TableExist(string tableName)
        {
            return DbInstace.Tables.Contains(tableName, dbSchema);
        }


        public void DropTable(string tableName)
        {
            Table tb = GetTable(tableName);
            if (tb != null)
            {
                tb.Drop();
            }
        }


        public SqlTransaction BeginTransaction()
        {
            return srv.ConnectionContext.SqlConnectionObject.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        }



        public bool ColumnExist(Table tb, string colName, bool ignoreCase = false)
        {
            foreach (Column col in tb.Columns)
            {
                if (string.Compare(col.Name, colName, ignoreCase) == 0)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
