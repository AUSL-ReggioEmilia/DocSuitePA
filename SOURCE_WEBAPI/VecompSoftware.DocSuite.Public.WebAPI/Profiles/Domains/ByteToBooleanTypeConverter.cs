using AutoMapper;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains
{
    public class ByteToBooleanTypeConverter : ITypeConverter<byte?, bool?>
    {
        public bool? Convert(byte? source, bool? destination, ResolutionContext context)
        {
            if (source.HasValue)
            {
                return System.Convert.ToBoolean(source.Value);
            }
            return null;
        }
    }
}