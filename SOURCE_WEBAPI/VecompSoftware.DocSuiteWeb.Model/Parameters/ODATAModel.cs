using System;
using System.Collections.Generic;
using System.Reflection;
using VecompSoftware.Commons.Interfaces.ODATA;

namespace VecompSoftware.DocSuiteWeb.Model.Parameters
{
    public class ODATAModel : IODATAModel
    {
        public ODATAModel()
        {
            NavigationProperties = new List<IODATAModel>();
        }

        public string EntityName { get; set; }
        public Type EntityType { get; set; }
        public PropertyInfo EntityKey { get; set; }
        public ICollection<IODATAModel> NavigationProperties { get; set; }
    }
}
