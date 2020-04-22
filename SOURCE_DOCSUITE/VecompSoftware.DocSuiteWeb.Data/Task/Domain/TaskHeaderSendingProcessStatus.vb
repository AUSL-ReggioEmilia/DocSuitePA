Imports System.ComponentModel

Public Enum TaskHeaderSendingProcessStatus
    <Description("In coda")>
    Todo = 1
    <Description("In elaborazione")>
    InProgress = 2 * Todo
    <Description("Completato")>
    Complete = 2 * InProgress
End Enum
