using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Protocols
{
    /// <summary>
    /// Extending IRepository<ProtocolRole>
    /// </summary>
    public static class ProtocolRoleFinder
    {
        public static ICollection<RoleTableValuedModel> Get(this IRepository<ProtocolRole> repository, short year, int number)
        {
            ICollection<RoleTableValuedModel> results = repository.ExecuteModelFunction<RoleTableValuedModel>(CommonDefinition.SQL_FX_Protocol_ProtocolRoleModels,
                new QueryParameter(CommonDefinition.SQL_Param_Protocol_Year, year),
                new QueryParameter(CommonDefinition.SQL_Param_Protocol_Number, number));
            return results;
        }

    }
}
