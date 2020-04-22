namespace VecompSoftware.DocSuiteWeb.Common.Infrastructures
{
    public enum BuildActionType : int
    {

        None = 0,
        Build = 1,
        Director = Build * 2,
        Evaluate = Director * 2,
        Synchronize = Evaluate * 2,
        Destroy = Synchronize * 2,
    }
}
