namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Relations
{
    public enum UDSLogType
    {
        Insert = 0,
        Modify = 1,
        AuthorizationModify = Modify * 2,
        DocumentModify = AuthorizationModify * 2,
        ObjectModify = DocumentModify * 2,
        Delete = ObjectModify * 2
    }
}
