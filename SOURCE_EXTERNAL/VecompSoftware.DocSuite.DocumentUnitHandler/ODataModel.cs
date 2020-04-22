using System.Collections.Generic;

namespace VecompSoftware.DocSuite.DocumentUnitHandler
{
    public class ODataModel<T>
        where T:class
    {
        public ICollection<T> Value { get; set; }
    }
}