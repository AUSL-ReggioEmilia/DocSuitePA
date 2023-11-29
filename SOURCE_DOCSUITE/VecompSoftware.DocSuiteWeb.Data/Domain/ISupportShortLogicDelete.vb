''' <summary> Interfaccia che permette la cancellazione logica dell'oggetto </summary>
''' <remarks> Se implementata la cancellazione fisica non avviene </remarks>
Public Interface ISupportShortLogicDelete
    ''' <summary> Indica se l'oggetto è cancellato o meno </summary>
    Property IsActive() As Short
End Interface
