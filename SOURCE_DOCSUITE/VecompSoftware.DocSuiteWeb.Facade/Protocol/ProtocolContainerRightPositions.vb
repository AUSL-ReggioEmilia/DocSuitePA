Imports System.ComponentModel

Public Enum ProtocolContainerRightPositions
    <Description("Inserimento")>
    Insert = 1

    <Description("Modifica")>
    Modify = 2

    <Description("Visualizzazione")>
    View = 3

    <Description("Sommario")>
    Preview = 4

    <Description("Annullamento")>
    Cancel = 5

    <Description("Interop. Ingresso")>
    InteropIn = 6

    <Description("Interop. Uscita")>
    InteropOut = 7

    <Description("PEC Ingresso")>
    PECIn = 8

    <Description("PEC Uscita")>
    PECOut = 9

    <Description("Distribuzione Doc.")>
    DocDistribution = 10

    <Description("Privacy")>
    Privacy = 11
End Enum