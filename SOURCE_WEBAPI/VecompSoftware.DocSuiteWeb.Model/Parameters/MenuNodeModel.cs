using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Parameters
{
    public class MenuNodeModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public Dictionary<string, MenuNodeModel> Nodes { get; set; }
    }
}
