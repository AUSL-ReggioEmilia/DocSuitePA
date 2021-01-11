using System;

namespace VecompSoftware.DocSuiteWeb.Common.Infrastructures
{
    public class BuildActionModel
    {
        public Guid ReferenceId { get; set; }

        public string Model { get; set; }

        public BuildActionType BuildType { get; set; }
    }
}
