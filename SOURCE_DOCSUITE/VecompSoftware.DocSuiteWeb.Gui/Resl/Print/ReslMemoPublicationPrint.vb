Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ReslMemoPublicationPrint
    Inherits ReslMemoPrint

    Protected Overrides Sub CreatePrint()
        'Setto il titolo della stampa
        TitlePrint = "Elenco delle Delibere da Pubblicare"

        Dim LastDate As Date = Nothing
        Dim reslList As IList(Of Resolution) = Nothing

        'Elenco delibere da pubblicare non inviate in regione
        reslList = ReslFacade.GetResolutionToPublicate()
        If (reslList.Count > 0) Then
            For Each resl As Resolution In reslList
                If (resl.SupervisoryBoardWarningDate <> LastDate) Then
                    'linea divisoria
                    CreateDivindingLine(TablePrint, 3)
                    'titolo
                    CreateSectionTitle(TablePrint, "INVIATE AL COLLEGIO SINDACALE IN DATA: " & resl.SupervisoryBoardWarningDate.Value)
                    LastDate = resl.SupervisoryBoardWarningDate
                    'linea divisoria
                    CreateDivindingLine(TablePrint)
                End If
                'crea intestazione
                CreateDataRow(TablePrint, "<I>PROT. TRASM.</I>", "<I>DELIBERA</I>", "<I>CONTENITORE</I>", True)
                'crea riga
                Dim s As String = "", s1 As String = ""
                Facade.ResolutionFacade.ReslFullNumber(resl, resl.Type.Id, s, s1)
                CreateDataRow(TablePrint, Resolution.FormatProtocolLink(resl.SupervisoryBoardProtocolLink, ""), s1, resl.Container.Name, False)
            Next
        End If
    End Sub
End Class
