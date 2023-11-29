using System;
using VecompSoftware.DocSuiteWeb.Model.Documents;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
{
    public class EvaluateFascicleModel
    {
        public Guid IdFascicle { get; set; }
        public ArchiveDocument ArchiveDocument { get; set; }
    }
}
