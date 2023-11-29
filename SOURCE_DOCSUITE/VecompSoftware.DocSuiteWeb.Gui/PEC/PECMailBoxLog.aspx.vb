Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data.PEC.Finder

Public Class PECMailBoxLog
    Inherits PECBasePage

#Region " Fields "

    Private _mailBoxes As IList(Of PECMailBox)

#End Region

#Region " Properties "

    Private ReadOnly Property IntegratedMail As Boolean
        Get
            Dim iMail As Boolean
            Boolean.TryParse(Request.QueryString("IntegratedMail"), iMail)
            Return iMail
        End Get
    End Property

    Public ReadOnly Property Mailboxes As IList(Of PECMailBox)
        Get
            If _mailBoxes Is Nothing Then
                _mailBoxes = New List(Of PECMailBox)
            End If

            If _mailBoxes.Count = 0 Then
                _mailBoxes = Facade.PECMailboxFacade.GetVisibleMailBoxes(IntegratedMail)
            End If

            Return _mailBoxes
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            AjaxAlert("Sono necessari diritti amministrativi per vedere la pagina.")
            Exit Sub
        End If

        InitializeAjax()

        dgMail.MasterTableView.NoMasterRecordsText = String.Format("Nessuna {0} disponibile", PecLabel)
        If Not IsPostBack Then
            Title = String.Format("{0} - Log", PecLabel)
            InitializeFilters()

            DataBindMailBoxes(ddlMailBox)
            DataBindMailGrid()
        End If

    End Sub

    Private Sub RefreshGrid_Events(ByVal sender As Object, ByVal e As EventArgs) Handles ddlMailBox.SelectedIndexChanged, cmdRefreshGrid.Click
        DataBindMailGrid()
    End Sub

    Private Sub dgMail_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles dgMail.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Return
        End If

        Dim bound As Data.PECMailBoxLog = DirectCast(e.Item.DataItem, Data.PECMailBoxLog)

        With DirectCast(e.Item.FindControl("lblMailboxName"), Label)
            .Text = bound.MailBox.MailBoxName
        End With
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(dgMail, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlMailBox, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRefreshGrid, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRefreshGrid, dtpShowSentFrom)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRefreshGrid, dtpShowSentTo)
    End Sub

    Private Sub InitializeFilters()
        dtpShowSentFrom.SelectedDate = Today.AddDays(-15)
        dtpShowSentTo.SelectedDate = Today
        ddlType.SelectedIndex = 0
    End Sub

    Private Sub DataBindMailBoxes(ByVal ddlMBoxes As DropDownList)
        ddlMBoxes.Items.Clear()
        For Each mailBox As PECMailBox In MailBoxes
            If Facade.PECMailboxFacade.IsRealPecMailBox(mailBox) Then
                ddlMBoxes.Items.Add(New ListItem(Facade.PECMailboxFacade.MailBoxRecipientLabel(mailBox), mailBox.Id.ToString()))
            End If
        Next
        ddlMBoxes.Items.Add(New ListItem("Tutte", "ALL"))

        If CommonShared.HasGroupAdministratorRight Then
            ddlMBoxes.SelectedIndex = ddlMBoxes.Items.Count - 1
        End If
    End Sub

    Private Sub DataBindMailGrid()
        If (ddlMailBox.Items Is Nothing) OrElse ddlMailBox.Items.Count <= 1 OrElse String.IsNullOrEmpty(ddlMailBox.SelectedValue) OrElse ddlMailBox.SelectedValue.Equals("--") Then
            Exit Sub
        End If


        Dim pecMailBoxLogFinder As NHibernatePECMailBoxLogFinder = Facade.PECMailBoxLogFinder

        ' Filtro per casella di posta.
        If Not ddlMailBox.SelectedValue.Equals("ALL") Then
            pecMailBoxLogFinder.MailboxIds = New Short() {ddlMailBox.SelectedValue}
        Else
            Dim selectedMailBoxes(ddlMailBox.Items.Count - 2) As Short

            For i As Integer = 0 To ddlMailBox.Items.Count - 1
                If Not ddlMailBox.Items(i).Value.Equals("ALL") Then
                    selectedMailBoxes.SetValue(CShort(ddlMailBox.Items(i).Value), i)
                End If
            Next
            pecMailBoxLogFinder.MailboxIds = selectedMailBoxes
        End If

        ' Filtri per data spedizione (da - a e nulla).
        pecMailBoxLogFinder.LastOperationDateFrom = dtpShowSentFrom.SelectedDate
        pecMailBoxLogFinder.LastOperationDateTo = dtpShowSentTo.SelectedDate

        ' Tipo elemento
        If ddlType.SelectedIndex > 0 Then
            pecMailBoxLogFinder.ElemType = ddlType.SelectedValue
        End If

        ' Paginazione griglia.
        pecMailBoxLogFinder.EnablePaging = dgMail.AllowPaging

        ' DataBind del finder.
        dgMail.Finder = pecMailBoxLogFinder
        dgMail.DataBindFinder()
    End Sub

#End Region

End Class