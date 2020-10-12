Public Interface IUDSFinderStaticData
    Inherits IUDSStaticData

    ReadOnly Property Year As Double?

    ReadOnly Property DocumentName As String

    ReadOnly Property GenericDocument As Boolean

    ReadOnly Property Number As Double?

    ReadOnly Property RegistrationDateFrom As DateTimeOffset?

    ReadOnly Property RegistrationDateTo As DateTimeOffset?

    ReadOnly Property ViewDeletedUDS As Boolean

End Interface
