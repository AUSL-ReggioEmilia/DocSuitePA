Imports System.Collections.Generic
Imports VecompSoftware.Helpers.NHibernate
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Public Class DocmLog
    Inherits DocmBasePage

#Region " Fields "
    Dim Utente As String
    Private docmLogFinder As NHibernateDocumentLogFinder
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Utente = Request.QueryString("User")

        InitializeAjaxSettings()
        If Not Me.IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub RadGrid1_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadGrid1.Init
        For Each column As GridColumn In RadGrid1.Columns
            If TypeOf column Is SuggestFilteringColumn AndAlso String.Compare(column.UniqueName, "LogTypeDescription", True).Equals(0) Then

                Dim documentLog As New DocumentLog
                Dim types As New List(Of Duplet(Of String, String))
                For Each item As String In documentLog.TypeDescription.Keys
                    types.Add(New Duplet(Of String, String)(item, documentLog.TypeDescription(item)))
                Next
                Dim sfc As SuggestFilteringColumn = column
                sfc.DataSourceCombo = types
                sfc.DataFieldCombo = "First"
                sfc.DataTextCombo = "Second"
            End If
        Next
    End Sub

#End Region

#Region "Initialize"
    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(RadGrid1, RadGrid1, Me.MasterDocSuite.AjaxLoadingPanelSearch)
    End Sub

    Private Sub Initialize()
        Me.Title = "Pratica - Log"

        lblId.Text = DocumentUtil.DocmFull(CurrentDocumentYear, CurrentDocumentNumber)
        If String.IsNullOrEmpty(Utente) Then
            pnlUser.Visible = False
        Else
            pnlUser.Visible = True
            lblUser.Text = Utente
        End If

        docmLogFinder = Facade.DocumentLogFinder
        docmLogFinder.DocumentYear = CurrentDocumentYear
        docmLogFinder.DocumentNumber = CurrentDocumentNumber
        docmLogFinder.PageSize = DocumentEnv.SearchMaxRecords

        RadGrid1.Finder = docmLogFinder
        RadGrid1.PageSize = docmLogFinder.PageSize
        RadGrid1.MasterTableView.SortExpressions.AddSortExpression("Id DESC")
        RadGrid1.DataBindFinder()
    End Sub
#End Region

End Class

