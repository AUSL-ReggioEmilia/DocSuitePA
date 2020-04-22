using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Resolutions
{
    public interface IResolutionModelMapper : IDomainMapper<Resolution, ResolutionModel>
    {
        FileResolution FileResolution { get; set; }

        IEnumerable<ResolutionRole> ResolutionRoles { get; set; }

    }
}
