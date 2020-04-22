using System;
using System.Collections.Generic;
using System.Reflection;

namespace VecompSoftware.Commons.Interfaces.ODATA
{
    public interface IODATAModel
    {
        string EntityName { get; set; }
        Type EntityType { get; set; }
        PropertyInfo EntityKey { get; set; }

        ICollection<IODATAModel> NavigationProperties { get; set; }

    }
}