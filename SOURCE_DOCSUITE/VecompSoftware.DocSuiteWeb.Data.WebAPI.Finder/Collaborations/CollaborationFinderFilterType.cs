namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Collaborations
{
    public enum CollaborationFinderFilterType
    {
        AllCollaborations = 0,
        ActiveCollaborations = 1,
        PastCollaborations = 2 * ActiveCollaborations,
        SignRequired = 2 * PastCollaborations,
        OnlyVision = 2 * SignRequired
    }
}
