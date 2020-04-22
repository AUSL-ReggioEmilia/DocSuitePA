using AutoMapper;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains
{
    public class ProtocolUserTypeTypeConverter : ITypeConverter<DocSuiteWeb.Entity.Protocols.ProtocolUserType, Core.Models.Domains.Protocols.ProtocolUserType>
    {
        public Core.Models.Domains.Protocols.ProtocolUserType Convert(DocSuiteWeb.Entity.Protocols.ProtocolUserType source, Core.Models.Domains.Protocols.ProtocolUserType destination, ResolutionContext context)
        {
            return (Core.Models.Domains.Protocols.ProtocolUserType)source;
        }
    }
}