using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Conservations;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Conservations
{
    public static class ConservationFinder
    {
        public static ICollection<ConservationLogModel> AvailableProtocolLogs(this IRepository<Conservation> repository, int skip, int top)
        {
            return repository.ExecuteModelFunction<ConservationLogModel>(CommonDefinition.SQL_FX_Conservation_AvailableProtocolLogs,
                new QueryParameter(CommonDefinition.SQL_Param_Conservation_Skip, skip),
                new QueryParameter(CommonDefinition.SQL_Param_Conservation_Top, top));
        }

        public static int CountAvailableProtocolLogs(this IRepository<Conservation> repository)
        {
            return repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Conservation_CountAvailableProtocolLogs);
        }

        public static ICollection<ConservationLogModel> AvailableDocumentSeriesItemLogs(this IRepository<Conservation> repository, int skip, int top)
        {
            return repository.ExecuteModelFunction<ConservationLogModel>(CommonDefinition.SQL_FX_Conservation_AvailableDocumentSeriesItemLogs,
                new QueryParameter(CommonDefinition.SQL_Param_Conservation_Skip, skip),
                new QueryParameter(CommonDefinition.SQL_Param_Conservation_Top, top));
        }

        public static int CountAvailableDocumentSeriesItemLogs(this IRepository<Conservation> repository)
        {
            return repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Conservation_CountAvailableDocumentSeriesItemLogs);
        }

        public static ICollection<ConservationLogModel> AvailableDossierLogs(this IRepository<Conservation> repository, int skip, int top)
        {
            return repository.ExecuteModelFunction<ConservationLogModel>(CommonDefinition.SQL_FX_Conservation_AvailableDossierLogs,
                new QueryParameter(CommonDefinition.SQL_Param_Conservation_Skip, skip),
                new QueryParameter(CommonDefinition.SQL_Param_Conservation_Top, top));
        }

        public static int CountAvailableDossierLogs(this IRepository<Conservation> repository)
        {
            return repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Conservation_CountAvailableDossierLogs);
        }

        public static ICollection<ConservationLogModel> AvailableFascicleLogs(this IRepository<Conservation> repository, int skip, int top)
        {
            return repository.ExecuteModelFunction<ConservationLogModel>(CommonDefinition.SQL_FX_Conservation_AvailableFascicleLogs,
                new QueryParameter(CommonDefinition.SQL_Param_Conservation_Skip, skip),
                new QueryParameter(CommonDefinition.SQL_Param_Conservation_Top, top));
        }

        public static int CountAvailableFascicleLogs(this IRepository<Conservation> repository)
        {
            return repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Conservation_CountAvailableFascicleLogs);
        }

        public static ICollection<ConservationLogModel> AvailablePECMailLogs(this IRepository<Conservation> repository, int skip, int top)
        {
            return repository.ExecuteModelFunction<ConservationLogModel>(CommonDefinition.SQL_FX_Conservation_AvailablePECMailLogs,
                new QueryParameter(CommonDefinition.SQL_Param_Conservation_Skip, skip),
                new QueryParameter(CommonDefinition.SQL_Param_Conservation_Top, top));
        }

        public static int CountAvailablePECMailLogs(this IRepository<Conservation> repository)
        {
            return repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Conservation_CountAvailablePECMailLogs);
        }

        public static ICollection<ConservationLogModel> AvailableTableLogs(this IRepository<Conservation> repository, int skip, int top)
        {
            return repository.ExecuteModelFunction<ConservationLogModel>(CommonDefinition.SQL_FX_Conservation_AvailableTableLogs,
                new QueryParameter(CommonDefinition.SQL_Param_Conservation_Skip, skip),
                new QueryParameter(CommonDefinition.SQL_Param_Conservation_Top, top));
        }

        public static int CountAvailableTableLogs(this IRepository<Conservation> repository)
        {
            return repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Conservation_CountAvailableTableLogs);
        }

        public static ICollection<ConservationLogModel> AvailableUDSLogs(this IRepository<Conservation> repository, int skip, int top)
        {
            return repository.ExecuteModelFunction<ConservationLogModel>(CommonDefinition.SQL_FX_Conservation_AvailableUDSLogs,
                new QueryParameter(CommonDefinition.SQL_Param_Conservation_Skip, skip),
                new QueryParameter(CommonDefinition.SQL_Param_Conservation_Top, top));
        }

        public static int CountAvailableUDSLogs(this IRepository<Conservation> repository)
        {
            return repository.ExecuteModelScalarFunction<int>(CommonDefinition.SQL_FX_Conservation_CountAvailableUDSLogs);
        }

        public static IQueryable<Conservation> GetByIdWithStatus(this IRepository<Conservation> repository, Guid uniqueId, ConservationStatus status, bool optimization = true)
        {
            return repository.Query(x => x.UniqueId == uniqueId && x.Status == status, optimization: optimization)
                  .SelectAsQueryable();
        }
    }
}
