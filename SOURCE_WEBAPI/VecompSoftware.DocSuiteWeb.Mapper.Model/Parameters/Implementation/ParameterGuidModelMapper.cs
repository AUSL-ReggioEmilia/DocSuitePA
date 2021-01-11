using System;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Parameters
{
    public class ParameterGuidModelMapper : ParameterModelMapper<string, Guid>, IParameterGuidModelMapper
    {
        public override Guid Map(string value, Guid result)
        {
            Guid transformed = Guid.Empty;
            if (Guid.TryParse(value, out transformed))
            {
                result = transformed;
            }
            return result;
        }

    }
}
