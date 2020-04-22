Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class PecMailBoxSettings
    Inherits PECBasePage

#Region " Fields "

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
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Return
        End If

        Dim currentPecMailBox As PECMailBox = DirectCast(e.Item.DataItem, PECMailBox)

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
        
        Dim chkHumanEnabled As CheckBox = DirectCast(e.Item.FindControl("chkHumanEnabled"), CheckBox)
        chkHumanEnabled.Checked = currentPecMailBox.HumanEnabled

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
        If e.CommandName = RadGrid.FilterCommandName Then
            BindMailGrid()
        End If
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
        BindMailGrid()
    End Sub

    Private Sub BindMailGrid()
        Dim finder As NHibernatePECMailBoxFinder = New NHibernatePECMailBoxFinder("ProtDB")

        grdPecMailBoxes.PageSize = finder.PageSize
        grdPecMailBoxes.DataSource = finder.DoSearchHeader()
        grdPecMailBoxes.DataBind()
    End Sub
#End Region

End Class