using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Parameters
{
    public class GridViewModel
    {
        public string ViewName { get; set; }
        public Dictionary<string, bool> ColumnsVisibility { get; set; }
    }
}
