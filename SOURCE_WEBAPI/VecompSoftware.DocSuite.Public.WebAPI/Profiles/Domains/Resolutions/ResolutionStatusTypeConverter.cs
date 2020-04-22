using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Resolutions
{
    public class ResolutionStatusTypeConverter : ITypeConverter<ResolutionStatus, ResolutionStatusType>
    {
        public ResolutionStatusType Convert(ResolutionStatus status, ResolutionStatusType destination, ResolutionContext context)
        {
            return (ResolutionStatusType)status;
        }
    }
}