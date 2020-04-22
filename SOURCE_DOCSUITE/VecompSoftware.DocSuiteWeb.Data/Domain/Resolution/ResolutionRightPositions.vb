Imports System.ComponentModel

''' <summary> Enumeratore per la gestione dei diritti da Contenitore e da Role </summary>
Public Enum ResolutionRightPositions
    <Description("Proposta")>
    Insert = 1

    <Description("Uff. Dirigenziale")>
    Executive = 2

    <Description("Visualizzazione")>
    View = 3

    <Description("Sommario")>
    Preview = 4

    <Description("Cancellazione")>
    Cancel = 5

    <Description("Amministrazione")>
    Administration = 6

    <Description("Adozione")>
    Adoption = 7

    <Description("Allegati Riservati")>
    PrivacyAttachments = 8

End Enum