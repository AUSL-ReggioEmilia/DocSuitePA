using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.DTO.OData
{
    public class ODataModel<T>
    {
        public T Value { get; set; }
    }
}
