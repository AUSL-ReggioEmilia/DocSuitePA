namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Parameters
{
    public class ParameterShortModelMapper : ParameterModelMapper<string, short>, IParameterShortModelMapper
    {
        public override short Map(string value, short result)
        {
            short transformed;
            if (short.TryParse(value, out transformed))
            {
                result = transformed;
            }
            return result;
        }

    }
}
