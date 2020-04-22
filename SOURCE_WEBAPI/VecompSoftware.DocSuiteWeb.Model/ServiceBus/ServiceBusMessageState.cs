namespace VecompSoftware.DocSuiteWeb.Model.ServiceBus
{
    public enum ServiceBusMessageState
    {
        //
        // Summary:
        //     The status of the messaging entity is active.
        Active = 0,
        //
        // Summary:
        //     The status of the messaging entity is disabled.
        Disabled = 1,
        //
        // Summary:
        //     Resuming the previous status of the messaging entity.
        Restoring = 2,
        //
        // Summary:
        //     The sending status of the messaging entity is disabled.
        SendDisabled = 3,
        //
        // Summary:
        //     The receiving status of the messaging entity is disabled.
        ReceiveDisabled = 4
    }
}
