using System.Collections.Generic;

namespace VecompSoftware.DocSuite.SPID.Model.Common
{
    public class ODataResult<T>
    {
        public ICollection<T> value { get; set; }
    }
}
