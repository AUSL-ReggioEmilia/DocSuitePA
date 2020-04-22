Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class ProtJournal
    Inherits ProtBasePage

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        Dim finder As New NHibernateProtocolJournalLogFinder

        finder.StartDate = rdpDateFrom.SelectedDate
        finder.EndDate = rdpDateTo.SelectedDate

        finder.PageSize = ProtocolEnv.SearchMaxRecords
        finder.SortExpressions.Add("ProtocolJournalDate", "DESC")

        gvJournalLog.Finder = finder
        gvJournalLog.PageSize = finder.PageSize
        gvJournalLog.MasterTableView.AllowMultiColumnSorting = True
        gvJournalLog.MasterTableView.SortExpressions.AddSortExpression("ProtocolJournalDate DESC")
        gvJournalLog.DataBindFinder()
    End Sub

    Private Sub gvJournalLog_DataBound(ByVal sender As Object, ByVal e As EventArgs) Handles gvJournalLog.DataBound
        If gvJournalLog.DataSource IsNot Nothing Then
            lblHeader.Text = "Protocollo - Registri Giornalieri Protocollo - Risultati (" & gvJournalLog.DataSource.Count & "/" & gvJournalLog.VirtualItemCount & ")"
        Else
            lblHeader.Text = "Protocollo - Registri Giornalieri Protocollo"
        End If
    End Sub

    Private Sub gvJournalLog_ItemCommand(ByVal source As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs) Handles gvJournalLog.ItemCommand
        If e.CommandName.Eq("ShowDoc") Then
            Dim docInfo As String() = e.CommandArgument.ToString().Split("|"c)
            SetOpenDocument(docInfo(0), docInfo(1), docInfo(2))
        End If
    End Sub

    Protected Sub gvJournal_OnInit(source As Object, e As EventArgs) Handles gvJournalLog.Init
        gvJournalLog.EnableScrolling = False
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        MasterDocSuite.TitleVisible = False
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, gvJournalLog, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, lblHeader)
        AjaxManager.AjaxSettings.AddAjaxSetting(gvJournalLog, gvJournalLog, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        lblHeader.Text = "Protocollo - Registri Giornalieri Protocollo"
        'imposta data inizio ricerca ad inizio mese
        rdpDateFrom.SelectedDate = New Date(Date.Now.Year, Date.Now.Month, 1)
        'imposta data fine ricerca ad oggi
        rdpDateTo.SelectedDate = Date.Now
    End Sub

    Private Sub SetOpenDocument(ByVal BiblosDsServer As String, ByVal BiblosDsArchive As String, ByVal BiblosDsChain As String)
        Dim doc As New BiblosDocumentInfo(BiblosDsServer, BiblosDsArchive, BiblosDsChain)

        Dim parameters As String = "servername={0}&guid={1}&label={2}"
        parameters = String.Format(parameters, doc.Server, doc.ChainId, "Registro")
        Dim viewerUrl As String = "~/Viewers/BiblosViewer.aspx?" & CommonShared.AppendSecurityCheck(parameters)

        Response.Redirect(viewerUrl)
    End Sub

    Public Function SetDocumentInfo(ByRef Container As Object) As String
        Dim StrInfo As String

        StrInfo = DataBinder.Eval(Container.dataitem, "Location.DocumentServer")
        StrInfo &= "|" & DataBinder.Eval(Container.dataitem, "Location.ProtBiblosDSDB")
        StrInfo &= "|" & DataBinder.Eval(Container.dataitem, "IdDocument")

        Return StrInfo
    End Function

#End Region

End Class