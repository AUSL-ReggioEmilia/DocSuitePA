using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Metadata;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class MetadataRepositoryModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<MetadataModel> Metadata { get; set; }
    }
}
