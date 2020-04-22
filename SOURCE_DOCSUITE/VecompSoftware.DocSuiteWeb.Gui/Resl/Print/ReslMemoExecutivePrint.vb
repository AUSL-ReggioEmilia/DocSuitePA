Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ReslMemoExecutivePrint
    Inherits ReslMemoPrint

    Protected Overrides Sub CreatePrint()
        'Setto il titolo della stampa
        TitlePrint = "Elenco delle Delibere da Rendere Esecutive"

        Dim LastDate As Date = Nothing
        Dim reslList As IList(Of Resolution) = Nothing


        'Elenco delibere da pubblicare non inviate in regione
        reslList = ReslFacade.GetResolutionToEsecutive(False)
        If (reslList.Count > 0) Then
            CreateTitle(TablePrint, "Elenco delle Delibere da Pubblicare non inviate in Regione<BR><BR>")
            For Each resl As Resolution In reslList
                If (resl.PublishingDate <> LastDate) Then
                    'titolo
                    CreateSectionTitle(TablePrint, "PUBBLICATE ALL'ALBO IN DATA: " & resl.PublishingDate.Value)
                    LastDate = resl.PublishingDate
                    'linea divisoria
                    CreateDivindingLine(TablePrint)
                End If
                'crea intestazione
                CreateDataRow(TablePrint, "<I>PROT. PUBB.</I>", "<I>DELIBERA</I>", "<I>CONTENITORE</I>", True)
                'crea riga
                Dim s As String = "", s1 As String = ""
                Facade.ResolutionFacade.ReslFullNumber(resl, resl.Type.Id, s, s1)
                CreateDataRow(TablePrint, Resolution.FormatProtocolLink(resl.PublishingProtocolLink, ""), s1, resl.Container.Name, False)
            Next
        End If


        'Elenco delibere da rendere esecutive inviate in regione
        reslList = ReslFacade.GetResolutionToEsecutive(True)
        If (reslList.Count > 0) Then
            CreateTitle(TablePrint, "<BR><BR>Elenco delle Delibere da Rendere Esecutive inviate in Regione<BR><BR>")
            For Each resl As Resolution In reslList
                If (resl.WarningDate <> LastDate) Then
                    'linea divisoria
                    CreateDivindingLine(TablePrint, "3")
                    'titolo
                    CreateSectionTitle(TablePrint, "RICEVUTE IN REGIONE IN DATA: " & resl.ConfirmDate.Value)
                    LastDate = resl.ConfirmDate
                    'linea divisoria
                    CreateDivindingLine(TablePrint)
                End If
                'crea intestazione
                CreateDataRow(TablePrint, String.Empty, "<I>DELIBERA</I>", "<I>CONTENITORE</I>", True)
                'crea riga
                Dim s As String = "", s1 As String = ""
                Facade.ResolutionFacade.ReslFullNumber(resl, resl.Type.Id, s, s1)
                CreateDataRow(TablePrint, String.Empty, s1, resl.Container.Name, False)
            Next
        End If
    End Sub

End Class
