using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.DocumentUnits
{
    public sealed class DocumentUnitFascicleCategoryRulesetDefinition : IDocumentUnitFascicleCategoryRuleset
    {

        public string READ => "DocumentUnitFascicleCategoryRead";

        public string INSERT => "DocumentUnitFascicleCategoryInsert";

        public string UPDATE => "DocumentUnitFascicleCategoryUpdate";

        public string DELETE => "DocumentUnitFascicleCategoryDelete";
    }
}
