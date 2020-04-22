using System.Collections.Generic;

namespace VecompSoftware.DocSuite.DocumentHandler
{
    public class ODataModel<T>
        where T:class
    {
        public ICollection<T> Value { get; set; }
    }
}