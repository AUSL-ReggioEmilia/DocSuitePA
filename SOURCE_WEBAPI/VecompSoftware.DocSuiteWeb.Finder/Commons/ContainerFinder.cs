using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class ContainerFinder
    {
        public static int CountProtocolInsertRight(this IRepository<Container> repository, string userName, string domain)
        {

            return repository.Queryable(true).Count(x => x.ContainerGroups
                        .Any(s => s.SecurityGroup.SecurityUsers
                        .Any(a => a.Account.ToLower().Equals(userName) && a.UserDomain.ToLower().Equals(domain))
                            && s.ProtocolRights.StartsWith("1")
                        ) && x.isActive.HasValue && x.isActive.Value == 1
                    );
        }

        public static int CountDossierInsertRight(this IRepository<Container> repository, string userName, string domain, short idContainer)
        {
            return repository.Queryable(true).Count(x => x.EntityShortId == idContainer
            && x.DocmLocation != null && x.ContainerGroups
                .Any(s => s.SecurityGroup.SecurityUsers
                   .Any(a => a.Account.ToLower().Equals(userName) && a.UserDomain.ToLower().Equals(domain))
                    && s.DocumentRights.StartsWith("1"))
                 && x.isActive.HasValue && x.isActive.Value == 1
                 );
        }

        public static int CountDossierModifyRight(this IRepository<Container> repository, string userName, string domain, short idContainer)
        {
            return repository.Queryable(true).Count(x => x.EntityShortId == idContainer &&
            x.ContainerGroups
                .Any(s => s.SecurityGroup.SecurityUsers
                   .Any(a => a.Account.ToLower().Equals(userName) && a.UserDomain.ToLower().Equals(domain))
                    && ((s.DocumentRights.StartsWith("01")) || (s.DocumentRights.StartsWith("11"))))
                 && x.isActive.HasValue && x.isActive.Value == 1 && x.DocmLocation != null
         );
        }

        public static IQueryable<Container> GetDossierInsertContainers(this IRepository<Container> repository, string userName, string domain, bool optimization = true)
        {
            return repository.Query(x => x.DocmLocation != null && x.ContainerGroups
                   .Any(s => s.SecurityGroup.SecurityUsers
                     .Any(a => a.Account.ToLower().Equals(userName) && a.UserDomain.ToLower().Equals(domain))
                      && s.DocumentRights.StartsWith("1"))
                   && x.isActive.HasValue && x.isActive.Value == 1, optimization: optimization)
                   .SelectAsQueryable();
        }

        public static IQueryable<Container> GetAnyDossierContainers(this IRepository<Container> repository, bool optimization = true)
        {
            return repository.Query(x => x.DocmLocation != null && x.ContainerGroups
                   .Any(s => !s.DocumentRights.Equals("0000000000")), optimization: optimization)
                   .OrderBy(o => o.OrderByDescending(c => c.isActive).ThenBy(c => c.Name))
                   .SelectAsQueryable();
        }

        public static int CountContainerByName(this IRepository<Container> repository, string containerName, bool optimization = true)
        {
            return repository.Queryable(optimization).Count(x => x.Name == containerName);
        }

        public static IQueryable<Container> GetWithProtocolLocation(this IRepository<Container> repository, short idContainer)
        {
            return repository.Query(x => x.EntityShortId == idContainer)
                .Include(i => i.ProtLocation)
                .SelectAsQueryable();
        }

        public static IQueryable<Container> GetInsertAuthorizedContainers(this IRepository<Container> repository, string userName, string domain, bool optimization = true)
        {
            return repository.Query(x => x.ContainerGroups
                   .Any(s => s.SecurityGroup.SecurityUsers
                     .Any(a => a.Account.ToLower().Equals(userName) && a.UserDomain.ToLower().Equals(domain))
                      && (s.DocumentRights.StartsWith("1") || s.ProtocolRights.StartsWith("1") ||
                            s.ResolutionRights.StartsWith("1") || s.FascicleRights.StartsWith("1")
                            //|| s.UDSRights.StartsWith("1")
                            ))
                   && x.isActive.HasValue && x.isActive.Value == 1, optimization: optimization)
                   .OrderBy(o => o.OrderBy(c => c.Name))
                   .SelectAsQueryable();
        }

        public static ICollection<ContainerModel> GetFascicleInsertContainers(this IRepository<Container> repository, string userName, string domain, short? idCategory, FascicleType? fascicleType)
        {
            QueryParameter idCategoryParameter = new QueryParameter(CommonDefinition.SQL_Param_Container_IdCategory, DBNull.Value);
            QueryParameter fascicleTypeParameter = new QueryParameter(CommonDefinition.SQL_Param_Container_FascicleType, DBNull.Value);
            if (idCategory.HasValue)
            {
                idCategoryParameter.ParameterValue = idCategory;
            }
            if (fascicleType.HasValue)
            {
                fascicleTypeParameter.ParameterValue = fascicleType;
            }

            return repository.ExecuteModelFunction<ContainerModel>(CommonDefinition.SQL_FX_Container_FascicleInsertAuthorized,
                new QueryParameter(CommonDefinition.SQL_Param_Container_UserName, userName), new QueryParameter(CommonDefinition.SQL_Param_Container_Domain, domain),
                idCategoryParameter, fascicleTypeParameter);
        }
    }
}
