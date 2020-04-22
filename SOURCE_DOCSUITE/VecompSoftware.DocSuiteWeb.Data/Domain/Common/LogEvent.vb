Imports System.ComponentModel

<FlagsAttribute()>
Public Enum LogEvent As Short
    <Description("Eliminazione")>
    DL = 0
    <Description("Modifica")>
    UP = 1
    <Description("Inserimento")>
    INS = 2
    <Description("Privacy")>
    PR = 4
End Enum