using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.DocumentUnits
{
    public sealed class DocumentUnitFascicleHistoricizedCategoryRulesetDefinition : IDocumentUnitFascicleHistoricizedCategoryRuleset
    {
        public string READ => "DocumentUnitFascicleHistoricizedCategoryRead";

        public string INSERT => "DocumentUnitFascicleHistoricizedCategoryInsert";

        public string UPDATE => "DocumentUnitFascicleHistoricizedCategoryUpdate";

        public string DELETE => "DocumentUnitFascicleHistoricizedCategoryDelete";
    }
}
