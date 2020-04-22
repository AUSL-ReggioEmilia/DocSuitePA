using NHibernate;
using NHibernate.Criterion;
using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using VecompSoftware.DocSuiteWeb.EntityMapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Commons
{
    public class CategoryFinder : BaseFinder<Category, CategoryModel>
    {
        #region [ Fields ]
        private DateTimeOffset _today = DateTimeOffset.UtcNow;
        #endregion [ Fields ]

        #region [ Properties ]

        public int? Year { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Name { get; set; }

        public bool? CheckFascicolable { get; set; }

        public bool? IsActive { get; set; }

        public int? ParentId { get; set; }

        public bool? IncludeChildren { get; set; }

        public bool? CheckFascicleProcedure { get; set; }

        public DateTimeOffset? CategorySchemaDate { get; set; }
        public Guid? CategorySchemaId { get; set; }

        public bool IsCurrentSchema { get; set; }

        public string FullCode { get; set; }

        public int? Environment { get; set; }

        public bool? ViewDisabled { get; set; }

        public bool? ViewActive { get; set; }

        private bool HasStatusFilter => !CategorySchemaDate.HasValue && ((ViewDisabled.HasValue) || (ViewActive.HasValue)) && CategorySchemaDate.HasValue;

        #endregion [ Properties ]

        #region [ Constructor ]
        public CategoryFinder(IEntityMapper<Category, CategoryModel> mapper, string currentUserName)
            : this(Enum.GetName(typeof(EnvironmentDataCode), EnvironmentDataCode.ProtDB), mapper, currentUserName)
        {
        }

        public CategoryFinder(string dbName, IEntityMapper<Category, CategoryModel> mapper, string currentUserName)
            : base(dbName, mapper)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]
        protected override IQueryOver<Category, Category> DecorateCriteria(IQueryOver<Category, Category> queryOver)
        {
            CategoryFascicle categoryFascicle = null;
            IQueryOver<Category, Category> query = queryOver;

            if (CategorySchemaId.HasValue)
            {
                query = query.Where(x => x.CategorySchema.Id == CategorySchemaId.Value);
            }

            if (IsActive.HasValue && IsActive.Value)
            {
                query = query.Where(x => x.IsActive == Convert.ToInt16(IsActive.Value));
            }

            if (CheckFascicolable.HasValue && CheckFascicolable.Value)
            {
                query = query.WithSubquery.WhereProperty(x => x.Id).In(SubQueryFascicle(QueryOver.Of(() => categoryFascicle)));

                if (!string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(FullCode))
                {
                    if (!string.IsNullOrEmpty(Name))
                    {
                        query = FilterByName(query);
                    }

                    if (!string.IsNullOrEmpty(FullCode))
                    {
                        query = query.Where(x => x.FullCode == FullCode);
                    }
                }
                else
                {
                    if (ParentId.HasValue)
                    {
                        if (IncludeChildren.HasValue && IncludeChildren.Value)
                        {
                            query = FilterByFullIncrementalPath(query);
                        }
                        else
                        {
                            query = FilterByParent(query);
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(FullCode))
                {
                    if (!string.IsNullOrEmpty(Name))
                    {
                        query = FilterByName(query);
                    }

                    if (!string.IsNullOrEmpty(FullCode))
                    {
                        query = query.Where(x => x.FullCode == FullCode);
                    }
                }
                else
                {
                    if (ParentId.HasValue)
                    {
                        query = FilterByParent(query);
                    }
                    else
                    {
                        query = query.Where(x => x.Parent == null);
                    }
                }
            }

            if (HasStatusFilter)
            {
                query = FilterByStatus(query);
            }
            else
            {
                if (Year.HasValue)
                {
                    query = FilterByYear(query);
                }
                else
                {
                    if (!CategorySchemaId.HasValue)
                    {
                        query = FilterByDate(query);
                    }
                }
            }

            return query;
        }

        private QueryOver<CategoryFascicle, CategoryFascicle> SubQueryFascicle(QueryOver<CategoryFascicle, CategoryFascicle> queryOver)
        {
            if (CheckFascicleProcedure.HasValue && CheckFascicleProcedure.Value)
            {
                queryOver = queryOver.Where(x => x.FascicleType == FascicleType.Procedure);
            }
            else
            {
                queryOver = queryOver.Where(x => x.FascicleType != FascicleType.SubFascicle);
            }

            if (Environment.HasValue)
            {
                queryOver = queryOver.Where(x => x.DSWEnvironment == Environment.Value);
            }

            return queryOver.Select(s => s.Category.Id);
        }

        private IQueryOver<Category, Category> FilterByName(IQueryOver<Category, Category> queryOver)
        {
            IQueryOver<Category, Category> query = queryOver.WhereRestrictionOn(x => x.Name).IsLike(Name, MatchMode.Anywhere);

            if (ParentId.HasValue)
            {
                query = FilterByParent(query);
            }

            return query;
        }

        private IQueryOver<Category, Category> FilterByParent(IQueryOver<Category, Category> queryOver)
        {
            return queryOver.Where(x => x.Parent.Id == ParentId);
        }

        private IQueryOver<Category, Category> FilterByFullIncrementalPath(IQueryOver<Category, Category> queryOver)
        {
            return queryOver.Where(x => x.FullIncrementalPath.IsLike(string.Concat(ParentId, "|"), MatchMode.Anywhere));
        }

        private IQueryOver<Category, Category> FilterByStatus(IQueryOver<Category, Category> queryOver)
        {
            if ((ViewDisabled.HasValue && ViewDisabled.Value) && (!ViewActive.HasValue || !ViewActive.Value))
            {
                //Classificatori disattivati
                if (IsCurrentSchema)
                {
                    queryOver = queryOver.Where(x => x.EndDate < DateTimeOffset.UtcNow);
                }
                else
                {
                    queryOver = queryOver.Where(x => x.EndDate < CategorySchemaDate.Value);
                }
            }
            else if ((!ViewDisabled.HasValue || !ViewDisabled.Value) && (ViewActive.HasValue && ViewActive.Value))
            {
                //Classificatori attivi              
                queryOver = queryOver.Where(x => x.StartDate < CategorySchemaDate.Value);
                if (!IsCurrentSchema)
                {
                    queryOver = queryOver.Where(x => x.EndDate == null);
                }
                else
                {
                    queryOver = queryOver.Where(x => x.EndDate == null || x.EndDate > DateTimeOffset.UtcNow);
                }
            }
            else if ((ViewDisabled.HasValue && ViewDisabled.Value) && (ViewActive.HasValue && ViewActive.Value))
            {
                //Classificatori disattivati e attivi
                queryOver = queryOver.Where(x => x.StartDate < CategorySchemaDate.Value && (x.EndDate == null || x.EndDate < CategorySchemaDate.Value));
            }
            return queryOver;
        }

        private IQueryOver<Category, Category> FilterByYear(IQueryOver<Category, Category> queryOver)
        {
            CategorySchema categorySchema = null;
            queryOver = queryOver.JoinAlias(x => x.CategorySchema, () => categorySchema)
                                 .Where(x => x.StartDate.Year <= Year.Value && (x.EndDate == null || x.EndDate.Value.Year >= Year.Value) && categorySchema.StartDate <= _today);
            return queryOver;
        }

        private IQueryOver<Category, Category> FilterByDate(IQueryOver<Category, Category> queryOver)
        {
            DateTimeOffset dateToCheck = _today;
            dateToCheck = StartDate.HasValue ? StartDate.Value : dateToCheck;
            dateToCheck = !StartDate.HasValue && EndDate.HasValue ? EndDate.Value : dateToCheck;
            CategorySchema categorySchema = null;
            queryOver = queryOver.JoinAlias(x => x.CategorySchema, () => categorySchema)
                                 .Where(x => x.StartDate <= dateToCheck && (x.EndDate == null || x.EndDate > dateToCheck) && categorySchema.StartDate <= _today);
            return queryOver;
        }

        #endregion [ Methods ]
    }
}
