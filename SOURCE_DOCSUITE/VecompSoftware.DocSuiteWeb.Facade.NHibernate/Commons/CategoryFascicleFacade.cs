using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Commons;
using VecompSoftware.NHibernateManager;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
{
    public class CategoryFascicleFacade : BaseProtocolFacade<CategoryFascicle, Guid, CategoryFascicleDao>
    {
        #region [ Fields ]

        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public CategoryFascicleFacade()
            : base()
        {

        }

        #endregion [ Constructor ]

        #region [ Methods ]

        public IList<int> GetNotFascicoled()
        {
            return _dao.GetNotFascicoled();
        }

        public IList<int> GetProcedureFascicles()
        {
            return _dao.GetProcedureFascicles();
        }

        public IList<CategoryFascicle> GetProcedureFascicles(int idCategory)
        {
            return _dao.GetProcedureFascicles(idCategory);
        }

        public ICollection<CategoryFascicle> GetByIdCategory(int idCategory)
        {
            return _dao.GetByIdCategory(idCategory);
        }

        public CategoryFascicle GetByIdCategory(int idCategory, int environment)
        {
            return _dao.GetByIdCategory(idCategory, environment);
        }

        public IList<CategoryFascicle> GetCategorySubFascicles()
        {
            return _dao.GetCategorySubFascicles();
        }

        public bool ExistFascicleProcedure(int idCategory)
        {
            return _dao.ExistFascicleProcedure(idCategory);
        }

        public bool ExistFascicleDefinition(int idCategory)
        {
            return _dao.ExistFascicleDefinition(idCategory);
        }

        public bool ExistProcedureSubCategories(int idCategory)
        {
            return _dao.ExistProcedureSubCategories(idCategory);
        }

        public IList<Category> GetFascicolableCategories()
        {
            return _dao.GetFascicolableCategories();
        }

        public IList<Category> GetFascicolableCategoriesByName(string name)
        {
            return _dao.GetFascicolableCategoriesByName(name);
        }

        public override void Save(ref CategoryFascicle model)
        {
            using (IStatelessSession session = NHibernateSessionManager.Instance.OpenStatelessSession("ProtDB"))
            using (ITransaction tx = session.BeginTransaction())
            {
                try
                {
                    SaveRecursive(ref model);
                    tx.Commit();
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw ex;
                }
            }
        }

        private void SaveRecursive(ref CategoryFascicle model)
        {
            if (model == null || model.Category == null)
            {
                return;
            }

            CategoryFascicle existingItem = GetByIdCategory(model.Category.Id, model.DSWEnvironment);
            //Verifico che non esista una tipologia di fascicolo associata al contenitore selezionato per quella UD
            //altrimenti ne eseguo l'update
            if (existingItem == null)
                base.SaveWithoutTransaction(ref model);
            else
            {
                if ((model.FascicleType != FascicleType.SubFascicle) || (existingItem.FascicleType == FascicleType.SubFascicle))
                {
                    existingItem.Category = model.Category;
                    existingItem.DSWEnvironment = model.DSWEnvironment;
                    existingItem.FasciclePeriod = model.FasciclePeriod;
                    existingItem.FascicleType = model.FascicleType;
                    existingItem.Manager = model.Manager;
                    UpdateWithoutTransaction(ref existingItem);
                }
            }

            //Ricorsivamente per ogni figlio
            if (model.Category.HasChildren)
            {
                foreach (Category child in model.Category.Children.Where(t => t.IsActive == 1))
                {
                    CategoryFascicle childModel = new CategoryFascicle(model.RegistrationUser);
                    childModel.Category = child;
                    childModel.DSWEnvironment = model.DSWEnvironment;
                    if (model.FascicleType == FascicleType.Period)
                        childModel.FasciclePeriod = model.FasciclePeriod;

                    childModel.FascicleType = FascicleType.SubFascicle;
                    childModel.Manager = model.Manager;
                    this.SaveRecursive(ref childModel);
                }
            }
        }

        public Contact GetCategoryFascicleManager(Guid categoryFascicleId)
        {
            CategoryFascicle categoryFascicle = GetById(categoryFascicleId);
            return categoryFascicle.Manager;
        }

        public IList<string> GetFirstIdCategoryWithProcedureCategoryFascicle(int idCategory)
        {          
            IList<string> results = new List<string>();
            IList<int> toConvert = _dao.GetFirstIdCategoryWithProcedureCategoryFascicle(idCategory);
            return toConvert.Select(x=>x.ToString()).ToList();
        }

        #endregion [ Methods ]

    }
}
