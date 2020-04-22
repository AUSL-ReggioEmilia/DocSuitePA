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
    public static class CategoryFascicleFinder
    {

        public static IQueryable<CategoryFascicle> GetByCategory(this IRepository<CategoryFascicle> repository, short idCategory)
        {
            return repository.Query(x => x.Category.EntityShortId == idCategory)
                .Include(i => i.Category)
                .SelectAsQueryable();
        }

        public static IQueryable<CategoryFascicle> GetPeriodicByCategory(this IRepository<CategoryFascicle> repository, short idCategory, int env)
        {
            return repository.Query(x => x.Category.EntityShortId == idCategory && x.FascicleType == FascicleType.Period && x.DSWEnvironment == env)
                .SelectAsQueryable();
        }

        public static IQueryable<CategoryFascicle> GetAllPeriodicByCategory(this IRepository<CategoryFascicle> repository, short idCategory, bool optimization = false)
        {
            return repository.Query(x => x.Category.EntityShortId == idCategory && x.FascicleType == FascicleType.Period, optimization)
                .Include(i => i.CategoryFascicleRights.Select(s => s.Role))
                .SelectAsQueryable();
        }

        public static IQueryable<CategoryFascicle> GetByFascicleType(this IRepository<CategoryFascicle> repository, short idCategory, FascicleType fascicleType)
        {
            return repository.Query(x => x.Category.EntityShortId == idCategory && x.FascicleType == fascicleType)
                .Include(i => i.Category)
                .SelectAsQueryable();
        }

        public static IQueryable<CategoryFascicle> GetByEnvironment(this IRepository<CategoryFascicle> repository, short idCategory, int environmentType)
        {
            return repository.Query(x => x.Category.EntityShortId == idCategory && x.DSWEnvironment == environmentType)
                .Include(i => i.Category)
                .Include(i => i.FasciclePeriod)
                .SelectAsQueryable();
        }

        public static int CountByEnvironment(this IRepository<CategoryFascicle> repository, short idCategory, int environmentType)
        {
            return repository.Queryable(true)
                .Where(x => x.Category.EntityShortId == idCategory && ((x.DSWEnvironment == environmentType && x.FascicleType == FascicleType.Period) ||
                                                                        x.DSWEnvironment == 0))
                .Count();
        }

        public static ICollection<CategoryFascicleTableValuedModel> GetCategorySubFascicles(this IRepository<CategoryFascicle> repository, short idCategory)
        {
            return repository.ExecuteModelFunction<CategoryFascicleTableValuedModel>(CommonDefinition.SQL_FX_Category_CategorySubFascicles,
                new QueryParameter(CommonDefinition.SQL_Param_Category_IdCategory, idCategory));
        }

        public static ICollection<CategoryFascicle> GeAvailablePeriodicCategoryFascicles(this IRepository<CategoryFascicle> repository, short idCategory)
        {
            return repository.ExecuteModelFunction<CategoryFascicle>(CommonDefinition.SQL_FX_Category_GeAvailablePeriodicCategoryFascicles,
                new QueryParameter(CommonDefinition.SQL_Param_Category_IdCategory, idCategory));
        }

        public static IQueryable<CategoryFascicle> FindIncludeRights(this IRepository<CategoryFascicle> repository, Guid uniqueId)
        {
            return repository.Query(x => x.UniqueId == uniqueId)
                .Include(i => i.CategoryFascicleRights)
                .SelectAsQueryable();
        }

        public static IQueryable<CategoryFascicle> FindIncludeCategory(this IRepository<CategoryFascicle> repository, Guid uniqueId, bool optimization = false)
        {
            return repository.Query(x => x.UniqueId == uniqueId, optimization)
                .Include(i => i.Category)
                .SelectAsQueryable();
        }
    }
}
