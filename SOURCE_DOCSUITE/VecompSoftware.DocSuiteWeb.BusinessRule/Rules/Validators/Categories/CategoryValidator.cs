using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Validators.Categories
{
    public class CategoryValidator : IValidator
    {
        #region [Fields]
        private readonly Category _categoryToValidate;
        private readonly CategoryRuleset _ruleset;
        #endregion

        #region [Error Messages]
        private const string CATEGORY_SCHEMA_NULL_ERROR_MESSAGE = "Nessuna Versione del Classificatore definito per la Data di attivazione selezionata.";
        private const string START_DATE_PARENT_ERROR_MESSAGE = "La Data di attivazione non può essere inferiore alla Data di attivazione del nodo padre.";
        private const string END_DATE_PARENT_ERROR_MESSAGE = "La Data di attivazione non può essere superiore alla Data di Disattivazione del nodo padre.";
        private const string ANOTHER_SCHEMA_PARENT_ERROR_MESSAGE = "La Versione del Classificatore Padre è diversa dalla Versione del Classificatore che si sta gestendo.";
        private const string CATEGORY_USED_ERROR_MESSAGE = "Non è possibile modificare la Data di attivazione di un classificatore già utilizzato.";
        private const string ANOTHER_SCHEMA_ERROR_MESSAGE = "Non è possibile modificare la Versione del Classificatore.";
        private const string END_DATE_NULL_ERROR_MESSAGE = "La proprietà EndDate non può essere vuota.";
        private const string END_DATE_CHILD_ERROR_MESSAGE = "La Data di Disattivazione non può essere successiva alla data di Disattivazione del nodo figlio.";
        private const string START_DATE_CHILD_ERROR_MESSAGE = "La Data di attivazione non può essere successiva alla Data di attivazione del nodo figlio.";
        private const string END_DATE_SCHEMA_ERROR_MESSAGE = "La Data di Disattivazione non può essere successiva alla data di Disattivazione della Versione associata.";
        private const string RECOVER_ERROR_MESSAGE = "Non è possibile recuperare un classificatore di una Versione non attiva.";
        private const string CATEGORY_CODE_NOT_UNIQUE = "Il codice associato al classificatore risulta già utilizzato in un altro classificatore.";
        #endregion

        #region [Properties]
        private CategorySchema CurrentCategorySchema
        {
            get
            {
                return FacadeFactory.Instance.CategorySchemaFacade.GetActiveCategorySchema(this._categoryToValidate.StartDate);
            }
        }
        #endregion

        #region [Constructor]
        public CategoryValidator(Category categoryToValidate, CategoryRuleset ruleset)
        {
            this._categoryToValidate = categoryToValidate;
            this._ruleset = ruleset;
        }
        #endregion

        #region [Methods]
        public ValidatorResult Validate()
        {
            ValidatorResult resultModel = new ValidatorResult();
            switch (this._ruleset)
            {
                case CategoryRuleset.Insert:
                    resultModel = ValidateInsertRuleset(resultModel);
                    break;
                case CategoryRuleset.Update:
                    resultModel = ValidateEditRuleset(resultModel);
                    break;
                case CategoryRuleset.Delete:
                    resultModel = ValidateDeleteRuleset(resultModel);
                    break;
                case CategoryRuleset.Recover:
                    resultModel = ValidateRecoverRuleset(resultModel);
                    break;
            }

            return resultModel;
        }

        private ValidatorResult ValidateBase(ValidatorResult resultModel)
        {
            if (CurrentCategorySchema == null)
            {
                resultModel.Errors.Add(CATEGORY_SCHEMA_NULL_ERROR_MESSAGE);
            }

            if (this._categoryToValidate.Parent != null)
            {
                if (this._categoryToValidate.Parent.StartDate > this._categoryToValidate.StartDate)
                {
                    resultModel.Errors.Add(START_DATE_PARENT_ERROR_MESSAGE);
                }

                if (this._categoryToValidate.Parent.EndDate <= this._categoryToValidate.StartDate)
                {
                    resultModel.Errors.Add(END_DATE_PARENT_ERROR_MESSAGE);
                }

                if (CurrentCategorySchema != null)
                {
                    CategorySchema parentCategorySchema = FacadeFactory.Instance.CategorySchemaFacade.GetActiveCategorySchema(this._categoryToValidate.Parent.StartDate);
                    if (parentCategorySchema.Id != CurrentCategorySchema.Id)
                    {
                        resultModel.Errors.Add(ANOTHER_SCHEMA_PARENT_ERROR_MESSAGE);
                    }
                }
            }

            return resultModel;
        }

        private ValidatorResult ValidateInsertRuleset(ValidatorResult resultModel)
        {
           resultModel= ValidateBase(resultModel);

            bool isCategoryCodeUnique = FacadeFactory.Instance.CategoryFacade.IsCategoryCodeUnique(this._categoryToValidate);
            if (!isCategoryCodeUnique)
            {
                resultModel.Errors.Add(CATEGORY_CODE_NOT_UNIQUE);
            }
            return resultModel;
        }

        private ValidatorResult ValidateEditRuleset(ValidatorResult resultModel)
        {
            resultModel = ValidateBase(resultModel);
            Category dbCategory = FacadeFactory.Instance.CategoryFacade.GetById(this._categoryToValidate.Id);
            if (dbCategory.StartDate!= this._categoryToValidate.StartDate)
            {
                ICollection<WebAPIDto<DocumentUnitModel>> result = WebAPIImpersonatorFacade.ImpersonateFinder(new DocumentUnitModelFinder(DocSuiteContext.Current.Tenants),
                    (impersonationType, finder) =>
                    {
                        finder.DocumentUnitFinderAction = DocumentUnitFinderActionType.CategorizedUD;
                        finder.CategoryId = this._categoryToValidate.UniqueId;
                        return finder.DoSearch();
                    });

                if (FacadeFactory.Instance.CategoryFacade.IsUsed(ref dbCategory) || result.Count > 0)
                {
                    resultModel.Errors.Add(CATEGORY_USED_ERROR_MESSAGE);
                }
            }

            if (this._categoryToValidate.CategorySchema != null)
            {
                if (dbCategory.CategorySchema.Id != this._categoryToValidate.CategorySchema.Id)
                {
                    resultModel.Errors.Add(ANOTHER_SCHEMA_ERROR_MESSAGE);
                }
            }

            if (dbCategory.HasChildren)
            {
                if (dbCategory.Children.Any(x => x.StartDate < this._categoryToValidate.StartDate))
                {
                    resultModel.Errors.Add(START_DATE_CHILD_ERROR_MESSAGE);
                }
            }

            return resultModel;
        }

        private ValidatorResult ValidateDeleteRuleset(ValidatorResult resultModel)
        {
            if (!this._categoryToValidate.EndDate.HasValue)
            {
                resultModel.Errors.Add(END_DATE_NULL_ERROR_MESSAGE);
                return resultModel;
            }

            if (this._categoryToValidate.HasChildren)
            {
                if (this._categoryToValidate.Children.Any(x => x.EndDate > this._categoryToValidate.EndDate.Value))
                {
                    resultModel.Errors.Add(END_DATE_CHILD_ERROR_MESSAGE);
                }
            }

            if (this._categoryToValidate.CategorySchema.EndDate.HasValue
                && this._categoryToValidate.CategorySchema.EndDate.Value < this._categoryToValidate.EndDate.Value)
            {
                resultModel.Errors.Add(END_DATE_SCHEMA_ERROR_MESSAGE);
            }

            return resultModel;
        }

        private ValidatorResult ValidateRecoverRuleset(ValidatorResult resultModel)
        {
            CategorySchema currentSchema = FacadeFactory.Instance.CategorySchemaFacade.GetCurrentCategorySchema();
            if (currentSchema.Id != CurrentCategorySchema.Id)
            {
                resultModel.Errors.Add(ANOTHER_SCHEMA_PARENT_ERROR_MESSAGE);
            }

            return resultModel;
        }
        #endregion
    }
}
