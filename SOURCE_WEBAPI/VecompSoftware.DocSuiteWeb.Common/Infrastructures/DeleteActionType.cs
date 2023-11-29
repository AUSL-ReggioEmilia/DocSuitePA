using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Common.Infrastructures
{
    public enum DeleteActionType : int
    {
        [Description("None")]
        None = -1,

        [Description("DematerialisationLogDelete")]
        DematerialisationLogDelete = 0,

        [Description("SecureDocumentLogDelete")]
        SecureDocumentLogDelete = 1,

        [Description("DeleteCategory")]
        DeleteCategory = SecureDocumentLogDelete * 2,

        [Description("CancelFascicle")]
        CancelFascicle = DeleteCategory * 2,

        [Description("DeleteProtocol")]
        DeleteProtocol = CancelFascicle * 2,

        [Description("DelecteCategoryFascicle")]
        DeleteCategoryFascicle = DeleteProtocol * 2,

        [Description("CancelProcess")]
        CancelProcess = DeleteCategoryFascicle * 2,

        [Description("DeleteDocumentSeriesItem")]
        DeleteDocumentSeriesItem = CancelProcess * 2,

        [Description("DeleteUDSFieldList")]
        DeleteUDSFieldList = DeleteDocumentSeriesItem * 2,

        [Description("DeleteCollaboration")]
        DeleteCollaboration = DeleteUDSFieldList * 2,
    }
}
