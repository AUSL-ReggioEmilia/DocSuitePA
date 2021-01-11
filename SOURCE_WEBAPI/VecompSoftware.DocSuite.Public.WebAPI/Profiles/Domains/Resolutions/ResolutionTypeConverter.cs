using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Resolutions;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Resolutions
{
    public class ResolutionTypeConverter : ITypeConverter<byte, ResolutionType>
    {
        public ResolutionType Convert(byte idType, ResolutionType destination, ResolutionContext context)
        {
            return (ResolutionType)idType;
        }
    }
}