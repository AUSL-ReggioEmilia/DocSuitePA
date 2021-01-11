Imports VecompSoftware.Services.Command.CQRS.Commands

Public Interface ICommandFacade(Of TComand As ICommand)
    'Metodi per lettura da API
    Function Read(command As TComand) As TComand
    Function Read(apiController As String, command As TComand) As TComand

    'Metodi per l'invio di comandi alle API
    Sub Push(command As TComand)

End Interface
