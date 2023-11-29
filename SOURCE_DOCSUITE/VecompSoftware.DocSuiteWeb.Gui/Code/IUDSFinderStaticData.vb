Public Interface IUDSFinderStaticData
    Inherits IUDSStaticData

    ReadOnly Property Year As Double?

    ReadOnly Property DocumentName As String

    ReadOnly Property GenericDocument As String

    ReadOnly Property Number As Double?

    ReadOnly Property RegistrationDateFrom As DateTimeOffset?

    ReadOnly Property RegistrationDateTo As DateTimeOffset?

    ReadOnly Property ViewDeletedUDS As String

End Interface
