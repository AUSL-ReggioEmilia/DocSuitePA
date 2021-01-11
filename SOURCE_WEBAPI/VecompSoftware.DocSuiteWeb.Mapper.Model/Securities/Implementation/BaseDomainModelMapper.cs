using System.DirectoryServices.AccountManagement;
using VecompSoftware.DocSuiteWeb.Model.Securities;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Securities
{
    public abstract class BaseDomainModelMapper<TPrincipal, TDomainModel> : BaseModelMapper<TPrincipal, TDomainModel>, IDomainMapper<TPrincipal, TDomainModel>
        where TPrincipal : Principal
        where TDomainModel : DomainBaseModel, new()
    {
        public override TDomainModel Map(TPrincipal entity, TDomainModel entityTransformed)
        {
            entityTransformed.Description = entity.Description;
            entityTransformed.DisplayName = entity.DisplayName;
            entityTransformed.DistinguishedName = entity.DistinguishedName;
            entityTransformed.GUID = entity.Guid;
            entityTransformed.Name = entity.SamAccountName;
            entityTransformed.SDDL_SID = string.Empty;
            if (entity.Sid != null)
            {
                entityTransformed.SDDL_SID = entity.Sid.Value;
            }

            return entityTransformed;
        }

    }
}
