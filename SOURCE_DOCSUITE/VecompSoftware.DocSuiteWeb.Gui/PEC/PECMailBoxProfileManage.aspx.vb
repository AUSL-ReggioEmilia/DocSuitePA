Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging

Public Class PECMailBoxProfileManage
    Inherits PECBasePage

#Region " Fields "

    Private _currentPecMailBoxConfiguration As PECMailBoxConfiguration
    Private Const PAGE_INSERT_TITLE As String = "Gestione profili caselle PEC - Inserimento"
    Private Const PAGE_EDIT_TITLE As String = "Gestione profili caselle PEC - Modifica"
#End Region

#Region " Properties "

    Public Const INSERT_ACTION As String = "PecProfileInsert"
    Public Const EDIT_ACTION As String = "PecProfileEdit"

    Private ReadOnly Property CurrentPECMailBoxConfiguration As PECMailBoxConfiguration
        Get
            If _currentPecMailBoxConfiguration IsNot Nothing Then Return _currentPecMailBoxConfiguration

            Dim _idPECMailBoxConfiguration As Integer = GetKeyValue(Of Integer)("idPECMailBoxConfiguration")
            _currentPecMailBoxConfiguration = Facade.PECMailboxConfigurationFacade.GetById(_idPECMailBoxConfiguration)
            Return _currentPecMailBoxConfiguration
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
        'todo: lasciato vuoto per implementazioni future
    End Sub

    Private Sub btnMailBoxAddProfile_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Not Page.IsValid Then
            AjaxAlert("Errore nella validazione dei dati inseriti.")
            Exit Sub
        End If

        Dim hasSaved As Boolean = False
        If Action.Eq(EDIT_ACTION) Then
            hasSaved = UpdatePecMailBoxConfiguration()
        Else
            hasSaved = InsertPecMailBoxConfiguration()
        End If

        If Not hasSaved Then
            AjaxAlert("Errore in fase di salvataggio configuratione PEC.")
            Exit Sub
        End If
        Response.Redirect("~/PEC/PECMailBoxProfileSettings.aspx?Type=PEC")
    End Sub

    Private Sub btnAnnulla_Click(sender As Object, e As EventArgs) Handles btnAnnulla.Click
        Response.Redirect("~/PEC/PECMailBoxProfileSettings.aspx?Type=PEC")
    End Sub


#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf RadAjaxManagerAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, pnlPageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnAnnulla, pnlPageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        If String.IsNullOrEmpty(Action) Then Throw New DocSuiteException("Nessuna Action specificata per la pagina.")

        If Not Action.Eq(EDIT_ACTION) AndAlso Not Action.Eq(INSERT_ACTION) Then
            Throw New DocSuiteException("Nessuna Action valida trovata.")
        End If

        drpSearchIMAP.DataSource = GetType(ImapFlag).EnumToDictionary()
        drpSearchIMAP.DataBind()

        If Action.Eq(EDIT_ACTION) Then
            Title = PAGE_EDIT_TITLE
            BindPageFromPecMailBoxConfiguration()
        Else
            Title = PAGE_INSERT_TITLE
        End If
    End Sub

    Private Sub BindPageFromPecMailBoxConfiguration()
        If CurrentPECMailBoxConfiguration Is Nothing Then Throw New DocSuiteException("Nessuna configuration PEC trovata per l'ID passato.")

        If Not String.IsNullOrEmpty(CurrentPECMailBoxConfiguration.Name) Then
            txtConfigurationName.Text = CurrentPECMailBoxConfiguration.Name
        End If

        txtMaxReadForSession.Text = CurrentPECMailBoxConfiguration.MaxReadForSession.ToString()
        txtMaxSendForSession.Text = CurrentPECMailBoxConfiguration.MaxSendForSession.ToString()
        chkZipExtract.Checked = CurrentPECMailBoxConfiguration.UnzipAttachments
        chkMarkAsRead.Checked = CurrentPECMailBoxConfiguration.MarkAsRead

        If Not String.IsNullOrEmpty(CurrentPECMailBoxConfiguration.MoveToFolder) Then
            txtMoveToFolder.Text = CurrentPECMailBoxConfiguration.MoveToFolder
        End If

        If Not String.IsNullOrEmpty(CurrentPECMailBoxConfiguration.MoveErrorToFolder) Then
            txtMoveErrorToFolder.Text = CurrentPECMailBoxConfiguration.MoveErrorToFolder
        End If

        If Not String.IsNullOrEmpty(CurrentPECMailBoxConfiguration.InboxFolder) Then
            txtInboxFolder.Text = CurrentPECMailBoxConfiguration.InboxFolder
        End If

        chkUploadSend.Checked = CurrentPECMailBoxConfiguration.UploadSent

        If Not String.IsNullOrEmpty(CurrentPECMailBoxConfiguration.FolderSent) Then
            txtFolderSent.Text = CurrentPECMailBoxConfiguration.FolderSent
        End If

        If Not String.IsNullOrEmpty(CurrentPECMailBoxConfiguration.NoSubjectDefaultText) Then
            txtNoSubjectDefaultText.Text = CurrentPECMailBoxConfiguration.NoSubjectDefaultText
        End If

        chkDeleteMailFromServer.Checked = CurrentPECMailBoxConfiguration.DeleteMailFromServer.GetValueOrDefault(False)
        txtMaxReceiveByteSize.Text = CurrentPECMailBoxConfiguration.MaxReceiveByteSize.ToString()
        txtMaxSendByteSize.Text = CurrentPECMailBoxConfiguration.MaxSendByteSize.ToString()
        drpSearchIMAP.SelectedValue = (DirectCast(CurrentPECMailBoxConfiguration.ImapSearchFlag, Integer)).ToString()
    End Sub

    Private Sub BindModelFromPage(ByRef model As PECMailBoxConfiguration)
        model.Name = txtConfigurationName.Text
        model.MaxReadForSession = Integer.Parse(txtMaxReadForSession.Text)
        model.MaxSendForSession = Integer.Parse(txtMaxSendForSession.Text)
        model.UnzipAttachments = chkZipExtract.Checked
        model.MarkAsRead = chkMarkAsRead.Checked
        model.MoveToFolder = txtMoveToFolder.Text
        model.MoveErrorToFolder = txtMoveErrorToFolder.Text
        model.InboxFolder = txtInboxFolder.Text
        model.UploadSent = chkUploadSend.Checked
        model.FolderSent = txtFolderSent.Text
        model.NoSubjectDefaultText = txtNoSubjectDefaultText.Text
        model.DeleteMailFromServer = chkDeleteMailFromServer.Checked
        model.MaxReceiveByteSize = Long.Parse(txtMaxReceiveByteSize.Text)
        model.MaxSendByteSize = Long.Parse(txtMaxSendByteSize.Text)
        model.ImapSearchFlag = DirectCast([Enum].Parse(GetType(ImapFlag), drpSearchIMAP.SelectedValue.ToString), ImapFlag)
    End Sub

    Protected Function UpdatePecMailBoxConfiguration() As Boolean
        Try
            Dim modelToSave As PECMailBoxConfiguration = CurrentPECMailBoxConfiguration
            BindModelFromPage(modelToSave)
            Facade.PECMailboxConfigurationFacade.Update(modelToSave)
            Return True
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            Return False
        End Try
    End Function

    Protected Function InsertPecMailBoxConfiguration() As Boolean
        Try
            Dim modelToSave As PECMailBoxConfiguration = New PECMailBoxConfiguration()
            BindModelFromPage(modelToSave)
            Facade.PECMailboxConfigurationFacade.Save(modelToSave)
            Return True
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            Return False
        End Try
    End Function
#End Region

End Class