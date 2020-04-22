using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Commons
{
    public static class ContactFinder
    {
        public static ICollection<ContactTableValuedModel> FindContacts(this IRepository<Contact> repository, string userName, string domain, ContactFinderModel finderModel)
        {
            QueryParameter filterParameter = new QueryParameter(CommonDefinition.SQL_Param_Contact_Filter, DBNull.Value);
            QueryParameter applyAuthorizationsParameter = new QueryParameter(CommonDefinition.SQL_Param_Contact_ApplyAuthorizations, DBNull.Value);
            QueryParameter excludeRoleContactsParameter = new QueryParameter(CommonDefinition.SQL_Param_Contact_ExcludeRoleContacts, DBNull.Value);
            QueryParameter parentIdParameter = new QueryParameter(CommonDefinition.SQL_Param_Contact_ParentId, DBNull.Value);
            QueryParameter parentToExcludeParameter = new QueryParameter(CommonDefinition.SQL_Param_Contact_ParentToExclude, DBNull.Value);
            QueryParameter avcpParentIdParameter = new QueryParameter(CommonDefinition.SQL_Param_Contact_AVCPParentId, DBNull.Value);
            QueryParameter fascicleParentIdParameter = new QueryParameter(CommonDefinition.SQL_Param_Contact_FascicleParentId, DBNull.Value);

            if (!string.IsNullOrEmpty(finderModel.Filter))
            {
                filterParameter.ParameterValue = finderModel.Filter;
            }

            if (finderModel.ApplyAuthorizations.HasValue)
            {
                applyAuthorizationsParameter.ParameterValue = finderModel.ApplyAuthorizations.Value;
            }

            if (finderModel.ExcludeRoleContacts.HasValue)
            {
                excludeRoleContactsParameter.ParameterValue = finderModel.ExcludeRoleContacts.Value;
            }

            if (finderModel.ParentId.HasValue)
            {
                parentIdParameter.ParameterValue = finderModel.ParentId.Value;
            }

            if (finderModel.ParentToExclude.HasValue)
            {
                parentToExcludeParameter.ParameterValue = finderModel.ParentToExclude.Value;
            }

            return repository.ExecuteModelFunction<ContactTableValuedModel>(CommonDefinition.SQL_FX_Contact_FindContacts,
                new QueryParameter(CommonDefinition.SQL_Param_Contact_UserName, userName), new QueryParameter(CommonDefinition.SQL_Param_Contact_Domain, domain),
                filterParameter, applyAuthorizationsParameter, excludeRoleContactsParameter, parentIdParameter, parentToExcludeParameter, avcpParentIdParameter, fascicleParentIdParameter);
        }

        public static ICollection<ContactTableValuedModel> GetContactParents(this IRepository<Contact> repository, int idContact)
        {
            ICollection<ContactTableValuedModel> contacts = repository.ExecuteModelFunction<ContactTableValuedModel>(CommonDefinition.SQL_FX_Contact_GetContactParents,
                new QueryParameter(CommonDefinition.SQL_Param_Contact_IdContact, idContact));
            return contacts.OrderBy(x => x.Index).ToList();
        }

        public static ICollection<ContactTableValuedModel> GetAuthorizedRoleContacts(this IRepository<Contact> repository, string userName, string domain, string addressBookAdministratorGroups)
        {
            QueryParameter addressBookAdministratorGroupsParameter = new QueryParameter(CommonDefinition.SQL_Param_Contact_AddressBookAdministratorGroups, DBNull.Value);
            if (!string.IsNullOrEmpty(addressBookAdministratorGroups))
            {
                addressBookAdministratorGroupsParameter.ParameterValue = addressBookAdministratorGroups;
            }

            return repository.ExecuteModelFunction<ContactTableValuedModel>(CommonDefinition.SQL_FX_Contact_GetAuthorizedRoleContacts,
                new QueryParameter(CommonDefinition.SQL_Param_Contact_UserName, userName), new QueryParameter(CommonDefinition.SQL_Param_Contact_Domain, domain),
                addressBookAdministratorGroupsParameter);
        }

        public static int CountContactBySearchCode(this IRepository<Contact> repository, string searchCode, bool optimization = false)
        {
            return repository.Query(x => x.SearchCode == searchCode, optimization)
                .SelectAsQueryable()
                .Count();
        }

        public static IQueryable<Contact> FindContactsByDescriptionOrFiscalCode(this IRepository<Contact> repository, string description, string fiscalCode, bool optimization = false)
        {
            return repository.Query(x => x.Description == description || (!string.IsNullOrEmpty(fiscalCode) && x.FiscalCode == fiscalCode), optimization)
                .SelectAsQueryable();
        }
    }
}
