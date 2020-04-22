
using System;
using VecompSoftware.DocSuiteWeb.Model.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers
{
    public class DossierRoleModel
    {
        public Guid UniqueId { get; set; }

        public AuthorizationRoleType Type { get; set; }

        public RoleModel Role { get; set; }

    }
}
