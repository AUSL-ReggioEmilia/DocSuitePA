Imports VecompSoftware.Helpers.UDS

Public Interface IUDSStaticData

    ReadOnly Property Subject As String

    ReadOnly Property IdCategory As Integer?

    Property ActionType As String

    Sub ResetControls()

    Sub InitializeControls()

    Sub SetData(model As UDSModel)

    Sub SetUDSBehaviour(model As UDSModel)

End Interface
