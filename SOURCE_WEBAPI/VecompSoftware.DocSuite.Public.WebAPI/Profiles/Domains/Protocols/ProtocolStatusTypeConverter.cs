using AutoMapper;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains
{
    public class ProtocolStatusTypeConverter : ITypeConverter<short, Core.Models.Domains.Protocols.ProtocolStatusType>
    {
        public Core.Models.Domains.Protocols.ProtocolStatusType Convert(short idStatus, Core.Models.Domains.Protocols.ProtocolStatusType destination, ResolutionContext context)
        {
            return (Core.Models.Domains.Protocols.ProtocolStatusType)idStatus;
        }
    }
}