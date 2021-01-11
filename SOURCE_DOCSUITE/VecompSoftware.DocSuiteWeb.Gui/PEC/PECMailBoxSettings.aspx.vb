Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class PecMailBoxSettings
    Inherits PECBasePage

#Region " Fields "
    Private _currentFinder As NHibernatePECMailBoxFinder
    Private Const LOGIN_ERROR_FILTER As String = "LoginError"
    Private Const LOGIN_ERROR_CMBID As String = "cmbLoginErrorsFilter"
#End Region

#Region " Properties "

    Private ReadOnly Property SelectedRow As Integer?
        Get
            If Not grdPecMailBoxes.SelectedItems.Count > 0 Then Return Nothing
            Dim grdItem As GridDataItem = grdPecMailBoxes.SelectedItems(0).TryConvert(Of GridDataItem)
            Dim selectedIdPecBox As Integer = Integer.Parse(grdItem.GetDataKeyValue("Id").ToString())
            Return selectedIdPecBox
        End Get
    End Property

    Private ReadOnly Property CurrentFinder As NHibernatePECMailBoxFinder
        Get
            If _currentFinder Is Nothing Then
                _currentFinder = New NHibernatePECMailBoxFinder("ProtDB")
            End If

            Return _currentFinder
        End Get
    End Property

    Private ReadOnly Property ViewLoginError As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("ViewLoginError", False)
        End Get
    End Property
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub RadAjaxManagerAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        'todo: lasciato vuoto per future implementazioni
    End Sub

    Private Sub grdPecMailBoxes_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grdPecMailBoxes.ItemDataBound
        If e.Item.ItemType = GridItemType.FilteringItem Then
            Dim filterItem As GridFilteringItem = DirectCast(e.Item, GridFilteringItem)

            If CurrentFinder.FilterExpressions.Any(Function(x) x.Key.Eq(LOGIN_ERROR_FILTER)) Then
                Dim control As Control = filterItem.FindControl(LOGIN_ERROR_CMBID)
                Dim value As String = CurrentFinder.FilterExpressions(LOGIN_ERROR_FILTER).FilterValue.ToString()
                FilterHelper.SetFilterValue(control, value)
            End If
        End If

        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Return
        End If

        Dim currentPecMailBox As PECMailBox = DirectCast(e.Item.DataItem, PECMailBox)
        Dim currentRow As GridDataItem = DirectCast(e.Item, GridDataItem)

        '' ID
        Dim lblId As Label = DirectCast(e.Item.FindControl("lblId"), Label)
        lblId.Text = currentPecMailBox.Id.ToString()

        Dim lblPECCount As Label = DirectCast(e.Item.FindControl("lblPECCount"), Label)
        lblPECCount.Text = Facade.PECMailboxFacade.CountPECMails(currentPecMailBox.Id).ToString()

        '' Protocollo Posta in Arrivo
        Dim ddlProtocolIn As DropDownList = DirectCast(e.Item.FindControl("ddlProtocolIn"), DropDownList)
        If currentPecMailBox.IncomingServerProtocol.HasValue Then
            ddlProtocolIn.SelectedValue = (CType(currentPecMailBox.IncomingServerProtocol.Value, Integer)).ToString()
        End If

        '' Calcolo Profilo
        Dim ddlProfile As DropDownList = DirectCast(e.Item.FindControl("ddlProfile"), DropDownList)

        ddlProfile.DataSource = Facade.PECMailboxConfigurationFacade.GetAll().Select(Function(pmbc) New ListItem(pmbc.Name, pmbc.Id.ToString)).ToList()
        ddlProfile.DataValueField = "Value"
        ddlProfile.DataTextField = "Text"
        ddlProfile.DataBind()

        Dim lblModuleJeepServiceIncoming As Label = DirectCast(e.Item.FindControl("lblModuleJeepServiceIncoming"), Label)
        Dim lblModuleJeepServiceOutgoing As Label = DirectCast(e.Item.FindControl("lblModuleJeepServiceOutgoing"), Label)

        If currentPecMailBox.IdJeepServiceIncomingHost.HasValue Then
            Dim JeepServiceHost As JeepServiceHost = CurrentJeepServiceHostFacade.GetById(currentPecMailBox.IdJeepServiceIncomingHost.Value)
            lblModuleJeepServiceIncoming.Text = JeepServiceHost.Hostname
        End If

        If currentPecMailBox.IdJeepServiceOutgoingHost.HasValue Then
            Dim JeepServiceHost As JeepServiceHost = CurrentJeepServiceHostFacade.GetById(currentPecMailBox.IdJeepServiceOutgoingHost.Value)
            lblModuleJeepServiceOutgoing.Text = JeepServiceHost.Hostname
        End If

        If currentPecMailBox.Configuration IsNot Nothing Then
            ddlProfile.SelectedValue = currentPecMailBox.Configuration.Id.ToString()
        End If

        Dim chkLoginError As CheckBox = DirectCast(e.Item.FindControl("chkLoginError"), CheckBox)
        chkLoginError.Checked = currentPecMailBox.LoginError

        If currentPecMailBox.LoginError Then
            currentRow.BackColor = Drawing.Color.Red
        End If

    End Sub

    Private Sub cmdAddPECMailBox_Click(sender As Object, e As EventArgs) Handles cmdAddPECMailBox.Click
        Response.Redirect(String.Format("~/PEC/PECMailBoxManage.aspx?Type=PEC&Action={0}", PECMailBoxManage.INSERT_ACTION))
    End Sub

    Private Sub cmdUpdatePEC_Click(sender As Object, e As EventArgs) Handles cmdUpdatePEC.Click
        If Not SelectedRow.HasValue Then
            AjaxAlert("Nessun elemento selezionato per la modifica.")
            Exit Sub
        End If

        Response.Redirect(String.Format("~/PEC/PECMailBoxManage.aspx?Type=PEC&Action={0}&idPECMailBox={1}", PECMailBoxManage.EDIT_ACTION, SelectedRow.Value))
    End Sub

    Protected Sub grdPecMailBoxes_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles grdPecMailBoxes.ItemCommand
        If e.CommandName = "CustomFilter" AndAlso e.CommandArgument IsNot Nothing Then
            Dim filters As IList(Of IFilterExpression) = CType(e.CommandArgument, List(Of IFilterExpression))

            For Each filterExpression As IFilterExpression In filters
                If filterExpression.FilterExpression <> Data.FilterExpression.FilterType.NoFilter Then
                    CurrentFinder.FilterExpressions.Add(New KeyValuePair(Of String, IFilterExpression)(filterExpression.PropertyName, filterExpression))
                End If
            Next
        End If

        BindMailGrid()
    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf RadAjaxManagerAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(grdPecMailBoxes, grdPecMailBoxes, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdUpdatePEC, pnlPageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAddPECMailBox, pnlPageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        If ViewLoginError Then
            CurrentFinder.FilterExpressions.Add(LOGIN_ERROR_FILTER, New Data.FilterExpression(LOGIN_ERROR_FILTER, GetType(Boolean), "True", Data.FilterExpression.FilterType.EqualTo))
        End If
        BindMailGrid()
    End Sub

    Protected Sub cmbError_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        Dim comboBox As RadComboBox = DirectCast(sender, RadComboBox)

        Dim filterItem As GridFilteringItem = DirectCast(comboBox.NamingContainer, GridFilteringItem)
        Dim filters As IList(Of IFilterExpression) = New List(Of IFilterExpression)()
        Dim filterType As Data.FilterExpression.FilterType = Data.FilterExpression.FilterType.NoFilter
        If (Not String.IsNullOrEmpty(comboBox.SelectedValue)) Then
            filterType = Data.FilterExpression.FilterType.EqualTo
        End If
        filters.Add(New Data.FilterExpression(LOGIN_ERROR_FILTER, GetType(Boolean), comboBox.SelectedValue, filterType))
        filterItem.FireCommandEvent("CustomFilter", filters)
    End Sub

    Private Sub BindMailGrid()
        Dim result As IList(Of PECMailBox) = CurrentFinder.DoSearchHeader()
        grdPecMailBoxes.PageSize = CurrentFinder.PageSize
        grdPecMailBoxes.DataSource = result.OrderBy(Function(pecMailBox) pecMailBox.LoginError)
        grdPecMailBoxes.DataBind()
    End Sub
#End Region

End Class