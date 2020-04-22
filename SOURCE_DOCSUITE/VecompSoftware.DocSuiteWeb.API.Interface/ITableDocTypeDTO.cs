using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.API
{
    public interface ITableDocTypeDTO : IAPIArgument
    {
        #region [ Properties ]
        int Id { get; set; }
        string Code { get; set; }
        string Description { get; set; }
        #endregion
    }
}
