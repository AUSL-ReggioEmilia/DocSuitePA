using VecompSoftware.StampaConforme.Models.Office;

namespace VecompSoftware.StampaConforme.Interfaces.Common.Converters
{
    public interface IToPdfConverter
    {
        bool Convert(string source, string destination, ConversionMode conversionMode = ConversionMode.Default);
    }
}
