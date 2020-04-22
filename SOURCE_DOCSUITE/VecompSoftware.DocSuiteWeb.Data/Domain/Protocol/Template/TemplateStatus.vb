Imports System.ComponentModel

<Flags()>
Public Enum TemplateStatus As Short
    <Description("Template disattivato")>
    Disabled = 0
    <Description("Template attivo")>
    Active = 1
    <Description("Template in errore")>
    Fault = 2
End Enum
