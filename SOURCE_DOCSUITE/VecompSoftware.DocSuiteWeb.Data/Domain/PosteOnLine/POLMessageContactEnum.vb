Public Enum POLMessageContactEnum
    ''' <summary> In attesa di Conferma </summary>
    Created = 0
    ''' <summary> In Elaborazione dalle Poste (spedito ma in viaggio) </summary>
    Confirmed = 1
    WorkingInProgress = 2
    ''' <summary> Consegnato </summary>
    Received = 4
    ''' <summary> In restituzione </summary>
    Rejected = 10
End Enum