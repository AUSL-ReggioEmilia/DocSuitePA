using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Helpers.UDS;

namespace VecompSoftware.DocSuiteWeb.UDSDesigner
{
    public class SectionManager
    {
        public const string DefaultSectionName = "#default#";
        private List<SectionExt> sections = new List<SectionExt>();

        public SectionExt GetCurrent()
        {
            if (sections.Count == 0)
            {
                AddNew(DefaultSectionName);
            }

            return sections.Last();
        }

        public void AddNew(string sectionalabel)
        {
            this.sections.Add(new SectionExt
            {
                SectionLabel = sectionalabel,
                SectionId = string.Concat("sectionId_", sections.Count()),
                Ctrls = new List<FieldBaseType>()
            });
        }

        public void AddCtrl(FieldBaseType ctrl)
        {
            SectionExt section = GetCurrent();
            ctrl.ClientId = string.Concat("uds_", section.SectionId, "_", ctrl.ColumnName, "_", section.Ctrls.Count());
            section.Ctrls.Add(ctrl);
        }

        public Section[] GetSections()
        {
            return sections.Select(p => new Section
            {
                SectionLabel = p.SectionLabel,
                SectionId = p.SectionId,
                Items = p.Ctrls.ToArray() ?? new FieldBaseType[] {}
            }).ToArray();
        }
    }
}
