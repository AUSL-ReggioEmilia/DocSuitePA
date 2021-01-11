namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Collaborations
{
    public enum CollaborationFinderActionType
    {
        AtVisionSign = 0,
        ToVisionSign = 1,
        AtProtocolAdmission = 2 * ToVisionSign,
        Running = 2 * AtProtocolAdmission,
        ToManage = 2 * Running,
        Registered = 2 * ToManage,
        CollaborationIncremental = 2 * Registered,
        CheckedOut = 2 * CollaborationIncremental,
        ToDelegateVisionSign = 2 * CheckedOut
    }
}
