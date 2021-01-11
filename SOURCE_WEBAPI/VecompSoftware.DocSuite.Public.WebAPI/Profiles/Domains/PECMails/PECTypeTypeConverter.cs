using AutoMapper;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.PECMails
{
    public class PECTypeTypeConverter : ITypeConverter<DocSuiteWeb.Entity.PECMails.PECType?, Core.Models.Domains.PECMails.PECType>
    {
        public Core.Models.Domains.PECMails.PECType Convert(DocSuiteWeb.Entity.PECMails.PECType? source, Core.Models.Domains.PECMails.PECType destination, ResolutionContext context)
        {
            Core.Models.Domains.PECMails.PECType ret = Core.Models.Domains.PECMails.PECType.Anomaly;
            if (source.HasValue)
            {
                ret = (Core.Models.Domains.PECMails.PECType)source.Value;
            }
            return ret;
        }
    }
}