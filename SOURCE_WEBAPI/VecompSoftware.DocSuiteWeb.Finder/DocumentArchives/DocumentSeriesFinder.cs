using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Model.Entities.Monitors;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.DocumentArchives
{
    public static class DocumentSeriesFinder
    {
        public static ICollection<MonitoringSeriesSectionModel> GetMonitoringSeriesBySection(this IRepository<DocumentSeries> repository, DateTimeOffset dateFrom, DateTimeOffset dateTo)
        {
            return repository.ExecuteModelFunction<MonitoringSeriesSectionModel>(CommonDefinition.SQL_FX_TransparentAdministrationMonitors_MonitoringSeriesSection,
                new QueryParameter(CommonDefinition.SQL_Param_TransparentAdministrationMonitors_DateFrom, dateFrom),
                new QueryParameter(CommonDefinition.SQL_Param_TransparentAdministrationMonitors_DateTo, dateTo));
        }

        public static ICollection<MonitoringQualitySummaryModel> GetMonitoringQualitySummary(this IRepository<DocumentSeries> repository, DateTimeOffset dateFrom, DateTimeOffset dateTo, int idDocumentSeries)
        {
            QueryParameter IdDocumentSeries_Param = new QueryParameter(CommonDefinition.SQL_Param_TransparentAdministrationMonitors_IdDocumentSeries, idDocumentSeries);
            QueryParameter DBNull_Param = new QueryParameter(CommonDefinition.SQL_Param_TransparentAdministrationMonitors_IdDocumentSeries, DBNull.Value);
            return repository.ExecuteModelFunction<MonitoringQualitySummaryModel>(CommonDefinition.SQL_FX_TransparentAdministrationMonitors_MonitoringQualitySummary,
                idDocumentSeries == 0 ? DBNull_Param : IdDocumentSeries_Param,
                new QueryParameter(CommonDefinition.SQL_Param_TransparentAdministrationMonitors_DateFrom, dateFrom),
                new QueryParameter(CommonDefinition.SQL_Param_TransparentAdministrationMonitors_DateTo, dateTo));
        }

        public static ICollection<MonitoringQualityDetailsModel> GetMonitoringQualityDetails(this IRepository<DocumentSeries> repository, int idDocumentSeries, int? idRole, DateTimeOffset dateFrom, DateTimeOffset dateTo)
        {
            QueryParameter Param_IdRoleDBNull = new QueryParameter(CommonDefinition.SQL_Param_TransparentAdministrationMonitors_IdRole, DBNull.Value);
            QueryParameter Param_IdRole = new QueryParameter(CommonDefinition.SQL_Param_TransparentAdministrationMonitors_IdRole, idRole.HasValue ? idRole.Value : 0);
            return repository.ExecuteModelFunction<MonitoringQualityDetailsModel>(CommonDefinition.SQL_FX_TransparentAdministrationMonitors_MonitoringQualitySummaryDetails,
                new QueryParameter(CommonDefinition.SQL_Param_TransparentAdministrationMonitors_IdDocumentSeries, idDocumentSeries),
                idRole.HasValue ? Param_IdRole : Param_IdRoleDBNull,
                new QueryParameter(CommonDefinition.SQL_Param_TransparentAdministrationMonitors_DateFrom, dateFrom),
                new QueryParameter(CommonDefinition.SQL_Param_TransparentAdministrationMonitors_DateTo, dateTo));
        }

        public static ICollection<MonitoringSeriesRoleModel> GetMonitoringSeriesByRole(this IRepository<DocumentSeries> repository, DateTimeOffset dateFrom, DateTimeOffset dateTo)
        {
            return repository.ExecuteModelFunction<MonitoringSeriesRoleModel>(CommonDefinition.SQL_FX_TransparentAdministrationMonitors_MonitoringSeriesRole,
                new QueryParameter(CommonDefinition.SQL_Param_TransparentAdministrationMonitors_DateFrom, dateFrom),
                new QueryParameter(CommonDefinition.SQL_Param_TransparentAdministrationMonitors_DateTo, dateTo));
        }
    }
}
