using AutoMapper;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains
{
    public class ProtocolTypeTypeConverter : ITypeConverter<DocSuiteWeb.Entity.Protocols.ProtocolType, Core.Models.Domains.Protocols.ProtocolType>
    {
        public Core.Models.Domains.Protocols.ProtocolType Convert(DocSuiteWeb.Entity.Protocols.ProtocolType source, Core.Models.Domains.Protocols.ProtocolType destination, ResolutionContext context)
        {
            if (source != null)
            {
                return (Core.Models.Domains.Protocols.ProtocolType)source.EntityShortId;
            }
            return Core.Models.Domains.Protocols.ProtocolType.Internal;
        }
    }
}