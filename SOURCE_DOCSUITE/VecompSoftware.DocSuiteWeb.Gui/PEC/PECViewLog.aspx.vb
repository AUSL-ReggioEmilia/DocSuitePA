Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade
'Imports VecompSoftware.DocSuiteWeb.Gui.WebComponent.Grid
'Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.PECMails
'Imports VecompSoftware.DocSuiteWeb.Data
'Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder

Public Class PecViewLog
    Inherits PECBasePage

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        ''''  I metodi commentati sono da applicare alla nuova griglia
        'dgLog = DelegateForGrid(Of PECMailLog, PECMailLogHeader).Delegate(dgLog)

        'If Not IsPostBack Then
        '    Dim finder As New PECMailLogFinder()

        '    If CurrentPecMailList.Count = 1 Then
        '        finder.PECMail = CurrentPecMailList.Single()
        '    Else
        '        finder.PECMailIDIn = CurrentPecMailList.Select(Function(p) p.Id).ToList()
        '    End If

        '    Dim sort As SortExpression(Of PECMailLog) = New SortExpression(Of PECMailLog)()
        '    sort.Direction = True
        '    sort.Expression = Function(f) f.Date

        '    finder.SortExpressions.Add(sort)
        '    finder.EnablePaging = True
        '    dgLog.Finder = finder

        '    dgLog.CurrentPageIndex = 0
        '    dgLog.CustomPageIndex = 0
        '    dgLog.PageSize = dgLog.Finder.PageSize

        '    dgLog.DataBindFinder(Of PECMailLog, PECMailLogHeader)()
        'End If
        If Not IsPostBack Then
            Dim finder As New PECMailLogFinder

            If CurrentPecMailList.Count = 1 Then
                finder.PECMail = CurrentPecMailList.Single()
            Else
                finder.PECMailIDIn = CurrentPecMailList.Select(Function(p) p.Id).ToList()
            End If

            finder.SortExpressions.Add("Date", "DESC")

            dgLog.Finder = finder
            dgLog.CurrentPageIndex = 0
            dgLog.CustomPageIndex = 0
            dgLog.PageSize = dgLog.Finder.PageSize
            dgLog.DataBindFinder()
        End If

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(dgLog, dgLog, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

#End Region

End Class