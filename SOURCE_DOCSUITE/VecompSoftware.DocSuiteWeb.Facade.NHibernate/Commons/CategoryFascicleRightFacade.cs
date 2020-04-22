using NHibernate;
using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Commons;
using VecompSoftware.NHibernateManager;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
{
    public class CategoryFascicleRightFacade : BaseProtocolFacade<CategoryFascicleRight, Guid, CategoryFascicleRightDao>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public CategoryFascicleRightFacade()
            : base()
        {

        }

        #endregion [ Constructor ]

        #region [ Methods ]
        public bool HasSpecialRole(int idCategory)
        {
            return _dao.HasSpecialRole(idCategory);
        }

        public bool HasRoles(int idCategory)
        {
            return _dao.HasRoles(idCategory);
        }

        public IList<CategoryFascicleRight> GetByIdCategory(int idCategory)
        {
            return _dao.GetByIdCategory(idCategory);
        }

        public ICollection<CategoryFascicleRight> GetByIdCategoryFascicle(Guid idCategoryFascicle)
        {
            return _dao.GetByIdCategoryFascicle(idCategoryFascicle);
        }

        public ICollection<CategoryFascicleRight> GetByIdCategoryRole(Guid idCategoryFascicle, int idRole)
        {
            return _dao.GetByIdCategory(idCategoryFascicle, idRole);
        }

        public bool HasCategoryFascicleRight(Guid idCategoryFascicle, int idRole)
        {
            return _dao.HasCategoryFascicleRight(idCategoryFascicle, idRole);
        }

        public ICollection<CategoryFascicleRight> GetByIdCategoryRoleRestricted(int idCategory)
        {
            return _dao.GetByIdCategoryRoleRestricted(idCategory);
        }

        public override void Save(ref CategoryFascicleRight model)
        {
            using (IStatelessSession session = NHibernateSessionManager.Instance.OpenStatelessSession("ProtDB"))
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {
                    if (model == null || model.CategoryFascicle.Id == null)
                    {
                        return;
                    }

                    base.Save(ref model);
                    tx.Commit();
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw ex;
                }
            }
        }
 


 




        #endregion [ Methods ]
    }
}