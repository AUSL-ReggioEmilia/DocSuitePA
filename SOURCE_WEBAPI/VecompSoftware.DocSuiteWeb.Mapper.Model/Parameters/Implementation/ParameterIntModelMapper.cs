namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Parameters
{
    public class ParameterIntModelMapper : ParameterModelMapper<string, int>, IParameterIntModelMapper
    {
        public override int Map(string value, int result)
        {
            int transformed;
            if (int.TryParse(value, out transformed))
            {
                result = transformed;
            }
            return result;
        }

    }
}
