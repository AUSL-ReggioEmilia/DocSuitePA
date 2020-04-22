using AutoMapper;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.PECMails
{
    public class PECActiveTypeTypeConverter : ITypeConverter<byte, Core.Models.Domains.PECMails.PECActiveType>
    {
        public Core.Models.Domains.PECMails.PECActiveType Convert(byte source, Core.Models.Domains.PECMails.PECActiveType destination, ResolutionContext context)
        {
            return (Core.Models.Domains.PECMails.PECActiveType)source;
        }
    }
}