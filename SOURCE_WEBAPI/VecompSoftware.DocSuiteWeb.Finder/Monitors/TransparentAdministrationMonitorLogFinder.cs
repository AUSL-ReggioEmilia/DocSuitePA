using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Monitors;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Monitors
{
    public static class TransparentAdministrationMonitorLogFinder
    {
        public static IQueryable<TransparentAdministrationMonitorLog> GetByDateInterval(this IRepository<TransparentAdministrationMonitorLog> repository,
            DateTimeOffset dateFrom, DateTimeOffset dateTo, string userName, short? idContainer, int? environment)
        {
            dateTo = new DateTimeOffset(dateTo.ToLocalTime().Date.AddDays(1));
            IQueryable<TransparentAdministrationMonitorLog> results = repository
                .Query(x => x.Date >= dateFrom && x.Date <= dateTo, optimization: true)
                .Include(f => f.DocumentUnit)
                .Include(d => d.Role.TenantAOO)
                .SelectAsQueryable();

            if (!string.IsNullOrEmpty(userName))
            {
                results = results.Where(f => f.RegistrationUser.Contains(userName));
            }
            if (idContainer.HasValue)
            {
                results = results.Where(f => f.DocumentUnit.Container.EntityShortId == idContainer.Value);
            }
            if (environment.HasValue)
            {
                if (environment < 100)
                {
                    results = results.Where(f => f.DocumentUnit.Environment == environment.Value);
                }
                if (environment >= 100)
                {
                    results = results.Where(f => f.DocumentUnit.Environment >= environment.Value);
                }
            }
            return results;
        }
    }
}
