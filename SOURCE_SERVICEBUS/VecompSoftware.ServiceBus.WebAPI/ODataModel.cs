using System.Collections.Generic;

namespace VecompSoftware.ServiceBus.WebAPI
{
    public class ODataModel<T>
        where T : class
    {
        public ICollection<T> Value { get; set; }
    }
}
