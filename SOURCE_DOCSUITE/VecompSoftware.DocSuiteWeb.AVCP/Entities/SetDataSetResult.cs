using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.Services.Biblos.Models;

namespace VecompSoftware.DocSuiteWeb.AVCP
{
    public class SetDataSetResult
    {
        public DocumentSeriesItem Item { get; set; }
        public BiblosChainInfo Chain { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool Saved { get; set; }
        public bool Updated { get; set; }
        public bool Flushed { get; set; }
        public bool Validated { get; set; }
        public List<string> ValidationErrors { get; set; }
        public string SerializedDataSet { get; set; }

        public int Step { get; set; }

    }
}
