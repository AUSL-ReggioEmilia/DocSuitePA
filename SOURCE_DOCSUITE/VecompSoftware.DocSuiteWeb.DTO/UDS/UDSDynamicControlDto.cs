using System;
using System.Collections;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    [Serializable()]
    public class UDSDynamicControlDto
    {
        public UDSDynamicControlDto()
        {
            CustomProperties = new Dictionary<string, object>();
        }

        public string IdControl { get; set; }
        public string DynamicControlName { get; set; }
        public object Value { get; set; }
        public IDictionary<string, object> CustomProperties { get; set; }
    }
}
