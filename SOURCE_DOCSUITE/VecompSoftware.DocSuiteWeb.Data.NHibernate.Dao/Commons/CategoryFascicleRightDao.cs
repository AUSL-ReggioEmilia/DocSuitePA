using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Commons
{
    public class CategoryFascicleRightDao : BaseNHibernateDao<CategoryFascicleRight>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public CategoryFascicleRightDao()
            : base()
        {
        }

        public CategoryFascicleRightDao(string sessionFactoryName) : base(sessionFactoryName)
        {
        }
        #endregion [ Constructor ]

        #region [ Methods ]
        public bool HasSpecialRole(int idCategory)
        {
            Category category = null;
            CategoryFascicle categoryFascicle = null;
            Role role = null;
            int result = NHibernateSession.QueryOver<CategoryFascicleRight>()
                .Inner.JoinAlias(x => x.CategoryFascicle, () => categoryFascicle)
                .Inner.JoinAlias(x => x.CategoryFascicle.Category, () => category)
                .Inner.JoinAlias(x => x.Role, () => role)
                .Where(() => category.Id == idCategory && role.UniqueId == Guid.Empty)
                .RowCount();

            return result > 0;
        }

        public bool HasRoles(int idCategory)
        {
            Category category = null;
            CategoryFascicle categoryFascicle = null;
            int result = NHibernateSession.QueryOver<CategoryFascicleRight>()
                .Inner.JoinAlias(x => x.CategoryFascicle, () => categoryFascicle)
                .Inner.JoinAlias(x => x.CategoryFascicle.Category, () => category)
                .Where(x => category.Id == idCategory && x.Role != null)
                .RowCount();

            return result > 0;
        }

        public IList<CategoryFascicleRight> GetByIdCategory(int idCategory)
        {
            Category category = null;
            CategoryFascicle categoryFascicle = null;
            IList<CategoryFascicleRight> result = NHibernateSession.QueryOver<CategoryFascicleRight>().Where(x => x.Role.Id != null)
                .Inner.JoinAlias(x => x.CategoryFascicle, () => categoryFascicle)
                .Inner.JoinAlias(x => x.CategoryFascicle.Category, () => category)
                .Where(() => category.Id == idCategory)
                .List<CategoryFascicleRight>();
            return result;
        }

        public IList<CategoryFascicleRight> GetByIdCategoryFascicle(Guid idCategoryFascicle)
        {
            IList<CategoryFascicleRight> result = NHibernateSession.QueryOver<CategoryFascicleRight>()
                .Where(x => x.CategoryFascicle.Id == idCategoryFascicle && x.Role.Id != null)
                .List<CategoryFascicleRight>();

            return result;
        }

        public IList<CategoryFascicleRight> GetByIdCategory(Guid idCategoryFascicle, int idRole)
        {
            IList<CategoryFascicleRight> result = NHibernateSession.QueryOver<CategoryFascicleRight>()
                .Where(x => x.CategoryFascicle.Id == idCategoryFascicle && x.Role.Id == idRole)
                .List<CategoryFascicleRight>();

            return result;
        }

        public bool HasCategoryFascicleRight(Guid idCategoryFascicle, int idRole)
        {
            int result = NHibernateSession.QueryOver<CategoryFascicleRight>()
                 .Where(x => x.CategoryFascicle.Id == idCategoryFascicle && x.Role.Id == idRole)
                 .RowCount();

            return result > 0;
        }

        public IList<CategoryFascicleRight> GetByIdCategoryRoleRestricted(int idCategory)
        {
            Category category = null;
            CategoryFascicle categoryFascicle = null;
            IList<CategoryFascicleRight> result = NHibernateSession.QueryOver<CategoryFascicleRight>()
                .Inner.JoinAlias(x => x.CategoryFascicle, () => categoryFascicle)
                .Inner.JoinAlias(x => x.CategoryFascicle.Category, () => category)
                .Where((x) => category.Id == idCategory && x.Role != null)
                .List<CategoryFascicleRight>();

            return result;
        }
        #endregion [ Methods ]
    }
}
