using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Services.WebAPI
{
    public class ODataModel<T>
        where T : class
    {
        public ICollection<T> Value { get; set; }
    }
}
