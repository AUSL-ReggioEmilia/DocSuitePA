
using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class CategoryFascicleRightFinder
    {
        public static IQueryable<CategoryFascicleRight> GetByProcedureCategoryId(this IRepository<CategoryFascicleRight> repository, short idCategory, bool optimization = false)
        {
            return repository.Query(x => x.CategoryFascicle.Category.EntityShortId == idCategory && x.CategoryFascicle.FascicleType == FascicleType.Procedure, optimization)
                .Include(i => i.Role)
                .SelectAsQueryable();
        }

        public static IQueryable<CategoryFascicleRight> GetPeriodicRightsByCategoryAndRole(this IRepository<CategoryFascicleRight> repository, short idCategory, short idRole, bool optimization = false)
        {
            return repository.Query(x => x.CategoryFascicle.Category.EntityShortId == idCategory && x.CategoryFascicle.FascicleType == FascicleType.Period && x.Role.EntityShortId == idRole, optimization)
                .SelectAsQueryable();
        }

        public static IQueryable<CategoryFascicleRight> GetByProcedureCategoryAndRole(this IRepository<CategoryFascicleRight> repository, short idCategory, short idRole, bool optimization = false)
        {
            return repository.Query(x => x.CategoryFascicle.Category.EntityShortId == idCategory && x.CategoryFascicle.FascicleType == FascicleType.Procedure && x.Role.EntityShortId == idRole, optimization)
                .SelectAsQueryable();
        }
    }
}