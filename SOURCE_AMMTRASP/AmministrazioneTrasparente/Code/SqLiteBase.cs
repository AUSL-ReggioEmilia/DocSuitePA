using System.Configuration;
using System.Reflection;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator;
using FluentMigrator.Runner;
using VecompSoftware.Services.Logging;

namespace AmministrazioneTrasparente.Code
{
    public class MigrationOptions : IMigrationProcessorOptions
    {
        public bool PreviewOnly { get; set; }
        public string ProviderSwitches { get; set; }
        public int Timeout { get; set; }
    }

    public class SqLiteBase
    {
        #region Fields

        private readonly string _connection;
        private const string MIGRATION_NAMESPACE = "AmministrazioneTrasparente.SQLite.Migrations";

        #endregion

        #region Constructor
        public SqLiteBase()
        {
            this._connection = ConfigurationManager.ConnectionStrings["AmmTraspLite"].ConnectionString;
        }
        #endregion

        #region Methods

        public void Initialize()
        {
            var announcer = new TextWriterAnnouncer(s => FileLogger.Debug(BasePage.LoggerName, s));
            var assembly = Assembly.Load("AmministrazioneTrasparente.SQLite");

            var migrationContext = new RunnerContext(announcer)
            {
                Namespace = MIGRATION_NAMESPACE
            };

            var options = new MigrationOptions { PreviewOnly = false, Timeout = 60 };
            var factory = new FluentMigrator.Runner.Processors.SQLite.SQLiteProcessorFactory();
            using (var processor = factory.Create(this._connection, announcer, options))
            {
                var runner = new MigrationRunner(assembly, migrationContext, processor);
                runner.MigrateUp(true);
            }
        }
        #endregion
    }
}