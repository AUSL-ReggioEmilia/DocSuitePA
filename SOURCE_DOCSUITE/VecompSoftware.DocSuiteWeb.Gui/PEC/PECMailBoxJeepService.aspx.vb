Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class PecMailBoxJeepService
    Inherits PECBasePage

#Region " Fields "

#End Region

#Region " Properties "

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            AjaxAlert("Sono necessari diritti amministrativi per vedere la pagina.")
            Exit Sub
        End If

        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub RadAjaxManagerAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        'todo: vuoto per implementazioni future
    End Sub

    Private Sub grdJeepService_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grdJeepService.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Return
        End If

        Dim currentJeepServiceHost As JeepServiceHost = DirectCast(e.Item.DataItem, JeepServiceHost)


        Dim lblId As Label = DirectCast(e.Item.FindControl("lblId"), Label)
        lblId.Text = currentJeepServiceHost.Id.ToString

        Dim lblHostname As Label = DirectCast(e.Item.FindControl("lblHostname"), Label)
        lblHostname.Text = currentJeepServiceHost.Hostname

        Dim chkIsActive As Image = DirectCast(e.Item.FindControl("chkIsActive"), Image)

        Dim active As Boolean = Convert.ToBoolean(currentJeepServiceHost.IsActive)
        If active Then
            chkIsActive.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/accept.png"
        Else
            chkIsActive.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/cancel.png"
        End If


        Dim chkIsDefault As Image = DirectCast(e.Item.FindControl("chkIsDefault"), Image)
        Dim defaultService As Boolean = currentJeepServiceHost.IsDefault
        If defaultService Then
            chkIsDefault.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/star.png"
            chkIsDefault.ToolTip = "Modulo di Default"
        End If

    End Sub

    Private Sub btnSetAsDefault_Click(sender As Object, e As EventArgs) Handles btnSetAsDefault.Click
        If Not grdJeepService.SelectedItems.Count > 0 Then
            AjaxAlert("Nessun elemento selezionato per l'azione richiesta.")
            Exit Sub
        End If

        If grdJeepService.SelectedItems.Count > 1 Then
            AjaxAlert("E' possibile impostare un solo elemento come default.")
            Exit Sub
        End If

        Dim grdItem As GridDataItem = grdJeepService.SelectedItems(0).TryConvert(Of GridDataItem)
        Dim selectedIdHost As Guid = Guid.Parse(grdItem.GetDataKeyValue("Id").ToString())
        CurrentJeepServiceHostFacade.SetDefaultHost(selectedIdHost)
        BindJeepServiceGrid()
    End Sub

    Private Sub btnActivate_Click(sender As Object, e As EventArgs) Handles btnActivate.Click
        Dim idHosts As IList(Of Guid) = GetSelectedIdHosts()
        If Not idHosts.Count > 0 Then Exit Sub
        CurrentJeepServiceHostFacade.ActivateJeepServiceHost(idHosts)
        BindJeepServiceGrid()
    End Sub

    Private Sub btnDeactivate_Click(sender As Object, e As EventArgs) Handles btnDeactivate.Click
        Dim idHosts As IList(Of Guid) = GetSelectedIdHosts()
        If Not idHosts.Count > 0 Then Exit Sub
        CurrentJeepServiceHostFacade.DisableJeepServiceHost(idHosts)
        BindJeepServiceGrid()
    End Sub

#End Region

#Region " Method "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf RadAjaxManagerAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(grdJeepService, grdJeepService, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSetAsDefault, pnlPageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnActivate, pnlPageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        BindJeepServiceGrid()
    End Sub

    Private Sub BindJeepServiceGrid()
        grdJeepService.DataSource = CurrentJeepServiceHostFacade.GetAll()
        grdJeepService.DataBind()
    End Sub

    Private Function GetSelectedIdHosts() As IList(Of Guid)
        If Not grdJeepService.SelectedItems.Count > 0 Then
            AjaxAlert("Nessun elemento selezionato per l'azione richiesta.")
            Return New List(Of Guid)()
        End If

        Dim idHosts As IList(Of Guid) = New List(Of Guid)
        For Each grdItem As GridDataItem In grdJeepService.SelectedItems
            idHosts.Add(Guid.Parse(grdItem.GetDataKeyValue("Id").ToString()))
        Next
        Return idHosts
    End Function
#End Region


End Class