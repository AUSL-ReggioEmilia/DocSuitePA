using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using VecompSoftware.DocSuiteWeb.API;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService
{
    public class OrgDeptImporter : JeepModuleBase<OrgDeptImporterParameters>
    {

        #region [ Constants ]

        private const string SelectOrgDept = "select top {0} * from Org.ORG_Dept where DataUltimoJeep is null or isnull(DataUltimoUpdate, @defaultDataUltimoUpdate) > DataUltimoJeep order by isnull(DataUltimoUpdate, @defaultDataUltimoUpdate), isnull(DataCreazione, DataDismissione)";
        private const string UpdateOrgDept = "update Org.ORG_Dept set DataUltimoJeep = @dataUltimoJeep where Id in ({0})";

        #endregion

        #region [ Fields ]

        private DatabaseHelper _db;
        private OChartConnector _connector;

        #endregion

        #region [ Properties ]

        private string LoggerName
        {
            get { return Name; }
        }

        private DatabaseHelper Db
        {
            get { return _db ?? (_db = new DatabaseHelper(Parameters.ConnectionString)); }
        }

        private OChartConnector Connector
        {
            get { return _connector ?? (_connector = OChartConnector.For(Parameters.APIAddress)); }
        }

        #endregion

        #region [ Methods ]

        public override void SingleWork()
        {
            try
            {
                var transformers = GetTransformers();
                var success = Transform(transformers);
                if (!success)
                    return;

                MarkAsDone(transformers);
            }
            catch (Exception ex)
            {
                FileLogger.Error(LoggerName, ex.Message, ex);
            }
        }

        public override void Initialize(List<Parameter> parameters)
        {
            base.Initialize(parameters);
        }

        private IEnumerable<OrgDeptDTO> GetTransformers()
        {
            FileLogger.Info(LoggerName, "GetTable...");
            IEnumerable<OrgDeptDTO> transformers;
            var sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter("@defaultDataUltimoUpdate", SqlDateTime.MinValue));
            var formatted = string.Format(SelectOrgDept, Parameters.MaxResults);
            using (var table = Db.GetTable(formatted, sqlParameters))
                transformers = table.Rows.Cast<DataRow>().Select(r => new OrgDeptDTO(r));
            FileLogger.Info(LoggerName, string.Format("GetTable: {0}.", transformers.Count()));
            return transformers;
        }
        private bool Transform(IEnumerable<OrgDeptDTO> transformers)
        {
            FileLogger.Info(LoggerName, "Connector.Transform...");
            var success = Connector.Transform(transformers);
            FileLogger.Info(LoggerName, string.Format("Connector.Transform: {0}.", success));
            return success;
        }
        private int MarkAsDone(IEnumerable<OrgDeptDTO> transformers)
        {
            FileLogger.Info(LoggerName, "ExecuteNonQuery...");
            var sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter("@dataUltimoJeep", DateTime.Now));

            var i = 0;
            foreach (var value in transformers)
            {
                var parameterName = string.Concat("@Id", i);
                sqlParameters.Add(new SqlParameter(parameterName, value.Id));
                i++;
            }
            var updated = 0;
            if (transformers.Any())
            {
                var parameterNames = sqlParameters.Where(p => p.ParameterName.StartsWith("@Id")).Select(p => p.ParameterName);
                var joined = string.Join(", ", parameterNames);
                var formatted = string.Format(UpdateOrgDept, joined);
                updated = Db.ExecuteNonQuery(formatted, sqlParameters);
            }
            FileLogger.Info(LoggerName, string.Format("ExecuteNonQuery: {0}.", updated));
            return updated;
        }

        #endregion

    }
}
