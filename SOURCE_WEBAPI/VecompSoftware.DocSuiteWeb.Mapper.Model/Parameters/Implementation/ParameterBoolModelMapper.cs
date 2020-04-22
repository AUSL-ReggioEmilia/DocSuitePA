namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Parameters
{
    public class ParameterBoolModelMapper : ParameterModelMapper<string, bool>, IParameterBoolModelMapper
    {
        public override bool Map(string value, bool result)
        {
            bool transformed;
            if (bool.TryParse(value, out transformed))
            {
                result = transformed;
            }
            else
            {
                if (value == "0" || value == "1")
                {
                    result = value == "1";
                }
            }
            return result;
        }

    }
}
