using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using ProtocolUserType = VecompSoftware.DocSuiteWeb.Entity.Protocols.ProtocolUserType;

namespace VecompSoftware.DocSuiteWeb.Finder.Protocols
{
    /// <summary>
    /// Extending IRepository<Protocol>
    /// </summary>
    public static class ProtocolFinder
    {
        public static ICollection<ProtocolTableValuedModel> GetAuthorized(this IRepository<Protocol> repository, string username, string domain, DateTimeOffset dateFrom, DateTimeOffset dateTo)
        {
            ICollection<ProtocolTableValuedModel> results = repository.ExecuteModelFunction<ProtocolTableValuedModel>(CommonDefinition.SQL_FX_Protocol_AuthorizedProtocols, new QueryParameter(CommonDefinition.SQL_Param_Protocol_UserName, username), new QueryParameter(CommonDefinition.SQL_Param_Protocol_Domain, domain),
                new QueryParameter(CommonDefinition.SQL_Param_Protocol_DateFrom, dateFrom), new QueryParameter(CommonDefinition.SQL_Param_Protocol_DateTo, dateTo));
            return results;
        }

        public static IQueryable<Protocol> GetByUniqueIds(this IRepository<Protocol> repository, ICollection<Guid> ids)
        {
            return repository.Query(x => ids.Contains(x.UniqueId))
                .SelectAsQueryable();
        }

        public static int CountByUniqueId(this IRepository<Protocol> repository, Guid uniqueId)
        {
            return repository.Queryable(true)
                .Where(x => x.UniqueId == uniqueId)
                .Count();
        }

        public static IQueryable<Protocol> GetByUniqueId(this IRepository<Protocol> repository, Guid uniqueId)
        {
            return repository.Query(x => x.UniqueId == uniqueId)
                .Include(i => i.Category)
                .Include(i => i.ProtocolRoles.Select(p => p.Role.TenantAOO))
                .Include(i => i.ProtocolRoleUsers)
                .Include(i => i.ProtocolType)
                .Include(i => i.Container.ProtLocation)
                .Include(i => i.TenantAOO)
                .SelectAsQueryable();
        }

        public static IQueryable<Protocol> GetByUniqueIdWithRole(this IRepository<Protocol> repository, Guid uniqueIdProtocol)
        {
            return repository.Query(x => x.UniqueId == uniqueIdProtocol)
                .Include(t => t.Category.CategoryFascicles.Select(f => f.FasciclePeriod))
                .Include(i => i.ProtocolRoles.Select(p => p.Role.TenantAOO))
                .Include(i => i.Container.ProtLocation)
                .Include(i => i.ProtocolUsers)
                .Include(i => i.TenantAOO)
                .SelectAsQueryable();
        }

        public static IQueryable<Protocol> GetByUniqueIdWithRoleAndContact(this IRepository<Protocol> repository, Guid uniqueIdProtocol)
        {
            return repository.Query(x => x.UniqueId == uniqueIdProtocol)
                .Include(t => t.Category.CategoryFascicles.Select(f => f.FasciclePeriod))
                .Include(i => i.ProtocolRoles.Select(p => p.Role.TenantAOO))
                .Include(i => i.ProtocolContacts.Select(p => p.Contact))
                .Include(i => i.ProtocolContactManuals)
                .Include(i => i.Container.ProtLocation)
                .Include(i => i.ProtocolUsers)
                .Include(i => i.TenantAOO)
                .Include(i => i.ProtocolType)
                .SelectAsQueryable();
        }

        public static IQueryable<Protocol> GetByCompositeKey(this IRepository<Protocol> repository, short year, int number)
        {

            return repository.Query(x => x.Year == year && x.Number == number)
                .Include(i => i.Category)
                .Include(i => i.ProtocolType)
                .Include(i => i.Container.ProtLocation)
                .SelectAsQueryable();
        }
        public static IQueryable<Protocol> GetByCompositeKey(this IRepository<Protocol> repository, Guid uniqueIdProtocol)
        {
            return repository.Query(x => x.UniqueId == uniqueIdProtocol)
                .Include(i => i.Category)
                .Include(i => i.Container.ProtLocation)
                .SelectAsQueryable();
        }

        public static IQueryable<Protocol> GetUserAuthorizedByUniqueId(this IRepository<Protocol> repository, Guid uniqueIdProtocol, string account, bool optimization = true)
        {
            return repository.Query(x => x.UniqueId == uniqueIdProtocol && x.ProtocolUsers.Any(u => u.Type == ProtocolUserType.Authorization && u.Account == account), optimization)
                .Include(i => i.Category)
                .Include(i => i.ProtocolRoles)
                .Include(i => i.Container)
                .Include(i => i.ProtocolType)
                .Include(i => i.ProtocolUsers)
                .SelectAsQueryable();
        }

        public static IQueryable<Protocol> GetUserAuthorized(this IRepository<Protocol> repository, int skip, int top, string account, string subject, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, string contact, bool optimization = true)
        {
            IQueryable<Protocol> results = repository.Query(x => x.ProtocolUsers.Any(u => u.Type == ProtocolUserType.Authorization && u.Account == account
                && (!dateFrom.HasValue || u.RegistrationDate >= dateFrom) && (!dateTo.HasValue || u.RegistrationDate <= dateTo)), optimization)
                .Include(i => i.Category)
                .Include(i => i.Container)
                .OrderBy(o => o.OrderByDescending(c => c.ProtocolUsers.Where(u => u.Type == ProtocolUserType.Authorization && u.Account == account).Max(f => f.RegistrationDate)))
                .SelectAsQueryable();

            if (!string.IsNullOrEmpty(subject))
            {
                results = results.Where(x => x.Object.Contains(subject));
            }

            if (!string.IsNullOrEmpty(contact))
            {
                results = results.Where(x => x.ProtocolContactManuals.Any(pcm => pcm.Description.Contains(contact)) || x.ProtocolContacts.Any(pc => pc.Contact.Description.Contains(contact)));
            }

            return results
                .Skip(skip)
                .Take(top);
        }

        public static int CountUserAuthorized(this IRepository<Protocol> repository, string account, string subject, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, string contact, bool optimization = true)
        {
            IQueryable<Protocol> results = repository.Queryable(optimization).Where(x => x.ProtocolUsers.Any(u => u.Type == ProtocolUserType.Authorization && u.Account == account
                && (!dateFrom.HasValue || u.RegistrationDate >= dateFrom) && (!dateTo.HasValue || u.RegistrationDate <= dateTo)));

            if (!string.IsNullOrEmpty(subject))
            {
                results = results.Where(x => x.Object.Contains(subject));
            }

            if (!string.IsNullOrEmpty(contact))
            {
                results = results.Where(x => x.ProtocolContactManuals.Any(pcm => pcm.Description.Contains(contact)) || x.ProtocolContacts.Any(pc => pc.Contact.Description.Contains(contact)));
            }

            return results.Count();
        }

        public static IQueryable<Protocol> GetByContacts(this IRepository<Protocol> repository, string searchCode, DateTimeOffset? dateFrom, DateTimeOffset? dateTo,
            bool optimization = true)
        {
            IQueryable<Protocol> results = repository.Queryable(optimization).Where(f => f.ProtocolContacts.Any(c => c.Contact.SearchCode == searchCode));
            if (dateFrom.HasValue)
            {
                results = results.Where(f => f.RegistrationDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                results = results.Where(f => f.RegistrationDate <= dateTo.Value);
            }

            return results;
        }


        public static long CountProtocolToRead(this IRepository<Protocol> repository, string account, bool optimization = true)
        {
            //TODO: Implement protocols to read here..
            return 0;
        }

    }
}
