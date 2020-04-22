using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI
{
    public enum UpdateActionType : int
    {
        [Description("FascicleClose")]
        FascicleClose = 0,

        [Description("TemplateCollaborationPublish")]
        TemplateCollaborationPublish = 1,

        [Description("RoleUserTemplateCollaborationInvalid")]
        RoleUserTemplateCollaborationInvalid = 2,

        [Description("ActivityFascicleUpdate")]
        ActivityFascicleUpdate = RoleUserTemplateCollaborationInvalid * 2,

        [Description("ActivityFascicleClose")]
        ActivityFascicleClose = ActivityFascicleUpdate * 2,

        [Description("ProtocolArchivedUpdate")]
        ProtocolArchivedUpdate = ActivityFascicleClose * 2,

        [Description("AutomaticHandlingWorkflow")]
        HandlingWorkflow = ProtocolArchivedUpdate * 2,

        [Description("RelaseHandlingWorkflow")]
        RelaseHandlingWorkflow = HandlingWorkflow * 2
    }
}
