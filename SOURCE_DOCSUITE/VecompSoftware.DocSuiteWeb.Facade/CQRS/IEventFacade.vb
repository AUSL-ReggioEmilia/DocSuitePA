Imports VecompSoftware.Services.Command.CQRS.Events

Public Interface IEventFacade(Of TEvent As IEvent)
    'Metodi per lettura da API
    Function Read(evt As TEvent) As TEvent
    Function Read(apiController As String, evt As TEvent) As TEvent

    'Metodi per l'invio di eventi alle API
    Sub Push(evt As TEvent)
End Interface
