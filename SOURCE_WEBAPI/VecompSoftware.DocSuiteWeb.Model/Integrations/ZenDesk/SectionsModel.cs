using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Integrations.ZenDesk
{
    public class SectionsModel : BaseZenDeskModel
    {
        public ICollection<SectionModel> Sections { get; set; }
    }
}
