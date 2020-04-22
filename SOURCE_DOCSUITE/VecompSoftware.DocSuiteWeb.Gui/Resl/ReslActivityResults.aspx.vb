Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports System.Collections.Generic

Public Class ReslActivityResults
    Inherits ReslBasePage

#Region " Fields"

    Private _finder As NHibernateResolutionActivityFinder

#End Region

#Region " Properties "

    Public ReadOnly Property CurrentResolutionActivityFinder As NHibernateResolutionActivityFinder
        Get
            If _finder Is Nothing Then
                _finder = New NHibernateResolutionActivityFinder()
            End If
            Return _finder
        End Get
    End Property

    Public ReadOnly Property Status As ResolutionActivityStatus?
        Get
            Return Request.QueryString.GetValueOrDefault(Of ResolutionActivityStatus?)("Status", Nothing)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        Title = String.Format("{0} - Controllo Automatismi Ricerca Flusso", Facade.TabMasterFacade.TreeViewCaption)
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub BtnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        LoadActivities()
    End Sub

    Protected Sub ReslActivityResults_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        If e.Argument IsNot Nothing AndAlso e.Argument.Eq("RefreshResults") Then
            LoadActivities()
        End If
    End Sub

    Private Sub dgTaskHeaders_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles dgTaskHeaders.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim bound As ResolutionActivityHeader = DirectCast(e.Item.DataItem, ResolutionActivityHeader)
        Dim cmdSummary As Image = DirectCast(e.Item.FindControl("cmdViewSummary"), Image)

        Select Case bound.Status
            Case ResolutionActivityStatus.Processed
                cmdSummary.ImageUrl = ImagePath.SmallFlagGreen
            Case ResolutionActivityStatus.ProcessedWithErrors
                cmdSummary.ImageUrl = ImagePath.SmallFlagRed
            Case ResolutionActivityStatus.ToBeProcessed
                cmdSummary.ImageUrl = ImagePath.SmallFlagBlue
        End Select

        DirectCast(e.Item, GridDataItem)("Description").Text = bound.Description
        DirectCast(e.Item, GridDataItem)("ActivityType").Text = bound.Type.GetDescription()
        DirectCast(e.Item, GridDataItem)("Status").Text = bound.Status.GetDescription()
        DirectCast(e.Item, GridDataItem)("R.ResolutionObject").Text = bound.ResolutionObject
        Dim typeImage As Image = DirectCast(e.Item.FindControl("imgTipoAtto"), Image)
        typeImage.ImageUrl = DefineIcon(bound.ResolutionTypeId, bound.ResolutionStatusId, False)
        typeImage.ToolTip = Facade.ResolutionTypeFacade.GetDescription(bound.ResolutionTypeId)

        With DirectCast(e.Item.FindControl("lnkResolution"), HyperLink)
            .NavigateUrl = String.Concat("../Resl/ReslVisualizza.aspx?", CommonShared.AppendSecurityCheck(String.Concat("IdResolution=", bound.ResolutionId, "&Type=Resl")))
            .Text = bound.ResolutionInclusiveNumber
        End With

        Dim activityDate As LinkButton = DirectCast(e.Item.FindControl("lnkActivityDate"), LinkButton)
        activityDate.Text = bound.ActivityDate.ToString("dd/MM/yyyy")
        activityDate.Enabled = False
        If bound.Type = ResolutionActivityType.Effectiveness AndAlso bound.Status = ResolutionActivityStatus.ToBeProcessed AndAlso Not bound.ResolutionEffectivenessDate.HasValue Then
            If bound.HasResolutionExecutiveRight Then
                activityDate.Enabled = True
                activityDate.ToolTip = "Modifica data"
                activityDate.OnClientClick = String.Concat("return OpenWindowEdit('", bound.Id.ToString(), "')")
            End If
        End If

        Dim OCLabel As Label = DirectCast(e.Item.FindControl("lblTipOC"), Label)
        SetOCLabel(bound, OCLabel)
    End Sub

    Private Sub RdpDateChangeOnSelected(sender As Object, e As EventArgs) Handles rdpDateTo.SelectedDateChanged, rdpDateFrom.SelectedDateChanged
        LoadActivities()
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()

        rblStatus.Items.Add(New ListItem(ResolutionActivityStatus.ToBeProcessed.GetDescription(), ResolutionActivityStatus.ToBeProcessed.ToString()))
        rblStatus.Items.Add(New ListItem(ResolutionActivityStatus.Processed.GetDescription(), ResolutionActivityStatus.Processed.ToString()))
        rblStatus.Items.Add(New ListItem(ResolutionActivityStatus.ProcessedWithErrors.GetDescription(), ResolutionActivityStatus.ProcessedWithErrors.ToString()))
        rblStatus.Items.Add(New ListItem("Tutte", "Tutte"))
        rblStatus.SelectedValue = "Tutte"

        rblActivity.Items.Add(New ListItem(ResolutionActivityType.Publication.GetDescription(), ResolutionActivityType.Publication.ToString()))
        rblActivity.Items.Add(New ListItem(ResolutionActivityType.Effectiveness.GetDescription(), ResolutionActivityType.Effectiveness.ToString()))
        rblActivity.Items.Add(New ListItem("Tutte", "Tutte"))
        rblActivity.SelectedValue = "Tutte"
        rdpDateFrom.SelectedDate = Date.Today.Date.AddDays(-10)
        rdpDateTo.SelectedDate = Date.Today.Date.AddDays(ProtocolEnv.DesktopDayDiff)

        cbCS.Visible = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "CS") OrElse Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "CS")
        cbCC.Visible = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "CC") OrElse Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "CC")
        cbR.Visible = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "REG") OrElse Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "REG")
        cbA.Visible = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "ALTRO") OrElse Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "ALTRO")

        If Status.HasValue Then
            rblStatus.SelectedValue = Status.ToString()
            If Status.Equals(ResolutionActivityStatus.ProcessedWithErrors) Then
                rdpDateFrom.SelectedDate = Nothing
                rdpDateTo.SelectedDate = Nothing
            End If
        End If

        SetFinder(Status, Nothing)
        dgTaskHeaders.CurrentPageIndex = 0
        dgTaskHeaders.CustomPageIndex = 0
        dgTaskHeaders.PageSize = dgTaskHeaders.Finder.PageSize

    End Sub

    Private Sub LoadActivities()
        Dim status As ResolutionActivityStatus? = Nothing
        If Not rblStatus.SelectedValue.Equals("Tutte") Then
            status = CType([Enum].Parse(GetType(ResolutionActivityStatus), rblStatus.SelectedValue), ResolutionActivityStatus)
        End If
        Dim type As ResolutionActivityType? = Nothing
        If Not rblActivity.SelectedValue.Equals("Tutte") Then
            type = CType([Enum].Parse(GetType(ResolutionActivityType), rblActivity.SelectedValue), ResolutionActivityType)
        End If
        SetFinder(status, type)
    End Sub
    Private Sub InitializeAjax()
        AddHandler MasterDocSuite.AjaxManager.AjaxRequest, AddressOf ReslActivityResults_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(dgTaskHeaders, dgTaskHeaders, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, dgTaskHeaders, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, dgTaskHeaders, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub SetFinder(status As ResolutionActivityStatus?, type As ResolutionActivityType?)
        dgTaskHeaders.Finder = CurrentResolutionActivityFinder
        CurrentResolutionActivityFinder.Username = DocSuiteContext.Current.User.UserName
        CurrentResolutionActivityFinder.Domain = DocSuiteContext.Current.User.Domain
        CurrentResolutionActivityFinder.CheckExecutiveRight = True

        If status.HasValue Then
            CurrentResolutionActivityFinder.Status = status.Value
        End If

        If type.HasValue Then
            CurrentResolutionActivityFinder.ActivityType = type.Value
        End If

        If rdpDateFrom.SelectedDate.HasValue Then
            CurrentResolutionActivityFinder.ActivityDateFrom = rdpDateFrom.SelectedDate.Value.Date
        End If

        If rdpDateTo.SelectedDate.HasValue Then
            CurrentResolutionActivityFinder.ActivityDateTo = rdpDateTo.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1)
        End If

        CurrentResolutionActivityFinder.OCSupervisoryBoard = cbCS.Visible AndAlso cbCS.Checked
        CurrentResolutionActivityFinder.OCCorteConti = cbCC.Visible AndAlso cbCC.Checked
        CurrentResolutionActivityFinder.OCRegion = cbR.Visible AndAlso cbR.Checked
        CurrentResolutionActivityFinder.OCOther = cbA.Visible AndAlso cbA.Checked

        dgTaskHeaders.DataBindFinder()
    End Sub

    Private Sub SetOCLabel(header As ResolutionActivityHeader, label As Label)
        Dim ocs As New List(Of String)
        Dim ocTooltip As New List(Of String)
        If header.ResolutionOCSupervisoryBoard Then
            ocs.Add("CS")
            ocTooltip.Add("Collegio Sindacale")
        End If
        If header.ResolutionOCRegion Then
            ocs.Add("R")
            ocTooltip.Add("Regione")
        End If
        If header.ResolutionOCManagement Then
            ocs.Add("CG")
            ocTooltip.Add("Controllo Gestione")
        End If
        If header.ResolutionOCCorteConti Then
            ocs.Add("CC")
            ocTooltip.Add("Corte dei Conti")
        End If
        If header.ResolutionOCOther Then
            ocs.Add("A")
            ocTooltip.Add("Altro")
        End If
        label.Text = String.Join("-", ocs)
        label.ToolTip = String.Join("-", ocTooltip)
    End Sub

#End Region

End Class