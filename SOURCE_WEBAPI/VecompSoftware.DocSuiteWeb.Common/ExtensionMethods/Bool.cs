namespace VecompSoftware.DocSuiteWeb.Common.ExtensionMethods
{
    public static class Bool
    {
        public static bool TryParseDefault(this bool dfValue, string value)
        {
            bool tmp = dfValue;
            bool.TryParse(value, out tmp);
            return tmp;
        }

    }
}
