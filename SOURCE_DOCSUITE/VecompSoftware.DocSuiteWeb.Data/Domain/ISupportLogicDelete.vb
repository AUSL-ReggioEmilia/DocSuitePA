''' <summary> Interfaccia che permette la cancellazione logica dell'oggetto </summary>
''' <remarks> Se implementata la cancellazione fisica non avviene </remarks>
Public Interface ISupportLogicDelete
    ''' <summary> Indica se l'oggetto � cancellato o meno </summary>
    Property IsActive() As Short
End Interface