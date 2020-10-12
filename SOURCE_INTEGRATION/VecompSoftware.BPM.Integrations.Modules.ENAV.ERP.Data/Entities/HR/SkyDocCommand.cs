using System;
using System.Collections.Generic;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.HR.Common;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.HR
{
    public class SkyDocCommand : BaseEntity
    {
        #region [ Constructor ]
        #endregion

        #region [ Properties ]
        public SkyDocCommandType CommandType { get; set; }
        public string DossierReference { get; set; }
        public string FascicleReference { get; set; }
        public string ResponsibleRoleMappingTag { get; set; }
        public string AuthorizedRoleMappingTag { get; set; }
        public string ContainerId { get; set; }
        public string CategoryId { get; set; }
        public string Object { get; set; }
        public string Contact1 { get; set; }
        public string Contact2 { get; set; }
        public string Contact3 { get; set; }
        public string Contact4 { get; set; }
        public DateTimeOffset InsertDate { get; set; }
        public short? Typology { get; set; }
        public Guid TenantId { get; set; }
        public Guid? WFSkyDocId { get; set; }
        public DateTimeOffset? WFSkyDocStarted { get; set; }
        public WorkflowStatus? WFSkyDocStatus { get; set; }
        #endregion


        #region [ Navigation Properties ]
        public virtual ICollection<SkyDocEvent> Events { get; set; }
        public virtual ICollection<SkyDocDocument> Documents { get; set; }
        #endregion
    }
}
