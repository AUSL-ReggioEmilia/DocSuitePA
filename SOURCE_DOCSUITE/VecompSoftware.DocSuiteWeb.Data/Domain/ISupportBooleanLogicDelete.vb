''' <summary> Interfaccia che permette la cancellazione logica dell'oggetto </summary>
''' <remarks> Se implementata la cancellazione fisica non avviene </remarks>
Public Interface ISupportBooleanLogicDelete
    ''' <summary> Indica se l'oggetto è cancellato o meno </summary>
    Property IsActive() As Boolean
End Interface