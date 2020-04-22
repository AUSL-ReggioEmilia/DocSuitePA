using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Finder.Protocols
{
    /// <summary>
    /// Extending IRepository<ProtocolContact>
    /// </summary>
    public static class ProtocolContactFinder
    {
        public static ICollection<ContactTableValuedModel> Get(this IRepository<ProtocolContact> repository, short year, int number)
        {
            ICollection<ContactTableValuedModel> results = repository.ExecuteModelFunction<ContactTableValuedModel>(CommonDefinition.SQL_FX_Protocol_ProtocolContactModels,
                new QueryParameter(CommonDefinition.SQL_Param_Protocol_Year, year),
                new QueryParameter(CommonDefinition.SQL_Param_Protocol_Number, number));
            return results;
        }

    }
}
