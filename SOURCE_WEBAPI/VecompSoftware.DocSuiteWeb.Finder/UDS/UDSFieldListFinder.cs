using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.UDS
{
    public static class UDSFieldListFinder
    {
        public static ICollection<UDSFieldListTableValuedModel> GetChildrenByParent(this IRepository<UDSFieldList> repository, Guid parentId)
        {
            return repository.ExecuteModelFunction<UDSFieldListTableValuedModel>(CommonDefinition.SQL_FX_UDSFieldList_GetChildrenByParent,
                new QueryParameter(CommonDefinition.SQL_Param_UDSFieldList_IdUDSFieldList, parentId));
        }

        public static UDSFieldListTableValuedModel GetParent(this IRepository<UDSFieldList> repository, Guid childId)
        {
            return repository.ExecuteModelFunction<UDSFieldListTableValuedModel>(CommonDefinition.SQL_FX_UDSFieldList_GetParent,
                new QueryParameter(CommonDefinition.SQL_Param_UDSFieldList_IdUDSFieldList, childId)).FirstOrDefault();
        }

        public static ICollection<UDSFieldListTableValuedModel> GetAllParents(this IRepository<UDSFieldList> repository, Guid childId)
        {
            return repository.ExecuteModelFunction<UDSFieldListTableValuedModel>(CommonDefinition.SQL_FX_UDSFieldList_GetAllParents,
                new QueryParameter(CommonDefinition.SQL_Param_UDSFieldList_IdUDSFieldList, childId));
        }
    }
}
