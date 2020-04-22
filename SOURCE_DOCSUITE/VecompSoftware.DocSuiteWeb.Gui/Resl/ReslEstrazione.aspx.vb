Imports System.IO
Imports System.Collections.Generic
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Linq
Partial Public Class ReslEstrazione
    Inherits ReslBasePage

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not IsPostBack Then
            Title = Facade.TabMasterFacade.TreeViewCaption & " - Estrazione"
        End If
    End Sub

    Private Sub Extract()
        Dim FileTemp As String = CommonUtil.UserDocumentName & "-Extraction-" & String.Format("{0:HHmmss}", Now()) & ".txt"
        Dim fDestination As String = CommonUtil.GetInstance().AppTempPath & FileTemp
        CommonUtil.GetInstance().UserDeleteTemp(TempType.P)

        Dim finder As NHibernateResolutionFinder = New NHibernateResolutionFinder("ReslDB")
        finder.AdoptionDateFrom = rdpAdoptionDateFrom.SelectedDate
        finder.AdoptionDateTo = rdpAdoptionDateTo.SelectedDate
        finder.EnablePaging = False
        finder.EnableStatus = False
        finder.EagerLog = False
        Dim reslList As IList(Of Resolution) = finder.DoSearch()

        If reslList.Count = 0 Then
            AjaxAlert("Ricerca Nulla")
            Exit Sub
        End If

        Const AP As String = """"

        Dim sw As StreamWriter = New StreamWriter(fDestination, True)
        'modifica per gestione codifica dei caratteri in formato html
        For Each resl As Resolution In reslList
            Dim exportRow As String = String.Empty
            exportRow &= AP & resl.Container.Name & AP & ","
            If resl.Year.HasValue And resl.Number.HasValue Then
                exportRow &= AP & resl.Year.Value & AP & "," & AP & resl.Number.Value & AP & ","
            End If
            exportRow &= AP & resl.AdoptionDate.Value & AP & ","
            exportRow &= AP & If(resl.ImmediatelyExecutive.GetValueOrDefault(False) = True, "S", "N") & AP & ","
            If resl.EffectivenessDate.HasValue Then
                exportRow &= AP & resl.EffectivenessDate.Value & AP & ","
            Else
                exportRow &= AP & String.Empty & AP & ","
            End If
            If resl.ResolutionContactProposers IsNot Nothing AndAlso resl.ResolutionContactProposers.Count > 0 Then
                exportRow &= AP & resl.ResolutionContactProposers.ElementAt(0).Contact.Code & AP & ","
            Else
                exportRow &= AP & String.Empty & AP & ","
            End If
            exportRow &= AP & StringHelper.ReplaceCrLf(resl.ResolutionObject) & AP
            sw.Write(exportRow & vbNewLine)
        Next
        sw.Close()
        Response.ContentType = "application/txt"
        Response.AppendHeader("Content-Disposition", "attachment; filename=Esportazione.txt")
        Response.WriteFile(fDestination)
        Response.End()
    End Sub

    Private Sub btnExtract_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnExtract.Click
        Extract()
    End Sub
End Class