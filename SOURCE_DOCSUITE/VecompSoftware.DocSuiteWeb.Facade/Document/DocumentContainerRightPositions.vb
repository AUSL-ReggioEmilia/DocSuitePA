Imports System.ComponentModel

''' <summary> Flag dei diritti sulle pratiche. </summary>
Public Enum DocumentContainerRightPositions
    <Description("Inserimento")>
    Insert = 1

    <Description("Modifica")>
    Modify = 2

    <Description("Visualizzazione")>
    View = 3

    <Description("Sommario")>
    Preview = 4

    <Description("Cancellazione")>
    Cancel = 5

    <Description("WorkFlow")>
    Workflow = 6
End Enum