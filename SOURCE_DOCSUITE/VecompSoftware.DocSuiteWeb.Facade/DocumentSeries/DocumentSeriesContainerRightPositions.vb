Imports System.ComponentModel

''' <summary> Posizione del diritto sulle serie documentali. </summary>
''' <remarks> Relativo alla colonna SeriesRights della ContainerGroup. </remarks>
Public Enum DocumentSeriesContainerRightPositions
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

    <Description("Bozze")>
    Draft = 6

    <Description("Vis. annullate")>
    ViewCanceled = 7

    <Description("Amministrazione")>
    Admin = 8

End Enum