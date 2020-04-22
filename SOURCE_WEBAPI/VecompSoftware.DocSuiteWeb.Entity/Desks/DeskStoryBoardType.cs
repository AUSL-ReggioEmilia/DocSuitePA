using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Desks
{
    public enum DeskStoryBoardType
    {
        [Description("Commento testuale")]
        TextComment = 1,
        [Description("Commento di archiviazione")]
        CheckInComment = 2,
        [Description("Commento di estrazione")]
        CheckOutComment = CheckInComment * 2,
        [Description("Commento risposta")]
        TextReply = CheckOutComment * 2,
        [Description("Documento approvato")]
        ApprovedDocument = TextReply * 2,
        [Description("Documento non approvato")]
        RejectDocument = ApprovedDocument * 2,
        [Description("Annulla estrazione")]
        UndoCheckout = RejectDocument * 2
    }
}
