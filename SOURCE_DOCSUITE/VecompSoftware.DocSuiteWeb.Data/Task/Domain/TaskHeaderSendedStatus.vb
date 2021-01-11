Imports System.ComponentModel

Public Enum TaskHeaderSendedStatus
    <Description("Terminato correttamente")>
    Successfully = 1
    <Description("Terminato con errori")>
    Errors = 2 * Successfully
End Enum
