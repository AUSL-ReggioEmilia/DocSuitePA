''' <summary> Interfaccia per il binding. </summary>
''' <remarks> Contratto per blindare l'utilizzo delle colonne di audit negli oggetti delle tabelle. </remarks>
Public Interface IAuditable

    Property RegistrationUser As String
    Property RegistrationDate As DateTimeOffset
    Property LastChangedUser As String
    Property LastChangedDate As DateTimeOffset?

End Interface
