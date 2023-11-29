using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
{
    public class SynchronizeCollaborationModel
    {
        #region [ Constructor ]

        public SynchronizeCollaborationModel()
        {

        }

        #endregion

        #region [ Properties ]

        public ICollection<Guid> IdCollaborations { get; set; }

        public Guid IdTenantAOO { get; set; }

        #endregion
    }
}
