Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class PECMailBoxProfileSettings
    Inherits PECBasePage

#Region " Fields "

#End Region

#Region " Properties "
    Private ReadOnly Property SelectedRow As Integer?
        Get
            If Not grdPecMailBoxesConfigurations.SelectedItems.Count > 0 Then Return Nothing
            Dim grdItem As GridDataItem = grdPecMailBoxesConfigurations.SelectedItems(0).TryConvert(Of GridDataItem)
            Dim selectedIdHost As Integer = Integer.Parse(grdItem.GetDataKeyValue("Id").ToString())
            Return selectedIdHost
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
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub RadAjaxManagerAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        'todo: lasciato vuoto per future implementazioni
    End Sub

    Private Sub grdPecMailBoxesConfigurations_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grdPecMailBoxesConfigurations.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Return
        End If

        Dim mailBoxConfiguration As PECMailBoxConfiguration = DirectCast(e.Item.DataItem, PECMailBoxConfiguration)

        Dim lblId As Label = DirectCast(e.Item.FindControl("lblIdConf"), Label)
        lblId.Text = mailBoxConfiguration.Id.ToString()

        '' Gestione Zip
        Dim chkUnzipAttachments As CheckBox = DirectCast(e.Item.FindControl("chkUnzipAttachments"), CheckBox)
        chkUnzipAttachments.Checked = mailBoxConfiguration.UnzipAttachments

        '' Imap Search Flag
        Dim ddlImapSearchFlag As DropDownList = DirectCast(e.Item.FindControl("ddlImapSearchFlag"), DropDownList)
        ddlImapSearchFlag.DataSource = GetType(ImapFlag).EnumToDictionary()
        ddlImapSearchFlag.DataBind()

        ddlImapSearchFlag.SelectedValue = (CType(mailBoxConfiguration.ImapSearchFlag, Integer)).ToString()
    End Sub

    Private Sub cmdAddPECProfile_Click(sender As Object, e As EventArgs) Handles cmdAddPECProfile.Click
        Response.Redirect(String.Format("~/PEC/PECMailBoxProfileManage.aspx?Type=PEC&Action={0}", PECMailBoxProfileManage.INSERT_ACTION))
    End Sub

    Private Sub cmdUpdatePEC_Click(sender As Object, e As EventArgs) Handles cmdUpdatePECProfile.Click
        If Not SelectedRow.HasValue Then
            AjaxAlert("Nessun elemento selezionato per la modifica.")
            Exit Sub
        End If

        Response.Redirect(String.Format("~/PEC/PECMailBoxProfileManage.aspx?Type=PEC&Action={0}&idPECMailBoxConfiguration={1}", PECMailBoxProfileManage.EDIT_ACTION, SelectedRow.Value))
    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf RadAjaxManagerAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(grdPecMailBoxesConfigurations, grdPecMailBoxesConfigurations)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdUpdatePECProfile, pnlPageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAddPECProfile, pnlPageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        BindProfileGrid()
    End Sub

    Private Sub BindProfileGrid()
        grdPecMailBoxesConfigurations.DataSource = Facade.PECMailboxConfigurationFacade.GetAll()
        grdPecMailBoxesConfigurations.DataBind()
    End Sub
#End Region

End Class