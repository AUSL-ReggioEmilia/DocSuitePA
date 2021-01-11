Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports System.Collections.Generic

Public Class PECMailBoxManage
    Inherits PECBasePage

#Region " Fields "

    Private _currentPecMailBox As PECMailBox
    Private _encHelper As Helpers.Signer.Security.EncryptionHelper
    Const PAGE_INSERT_TITLE As String = "PEC - Gestione caselle PEC - Inserimento"
    Private Const PAGE_EDIT_TITLE As String = "PEC - Gestione caselle PEC - Modifica"
#End Region

#Region " Properties "


    Private ReadOnly Property EncHelper As Helpers.Signer.Security.EncryptionHelper
        Get
            If _encHelper Is Nothing Then
                _encHelper = New Helpers.Signer.Security.EncryptionHelper()
            End If
            Return _encHelper
        End Get
    End Property

    Private ReadOnly Property CurrentPecMailBox As PECMailBox
        Get
            If _currentPecMailBox IsNot Nothing Then
                Return _currentPecMailBox
            End If

            Dim _idPecMailBox As Short = GetKeyValue(Of Short)("idPECMailBox")
            _currentPecMailBox = Facade.PECMailboxFacade.GetById(_idPecMailBox)
            Return _currentPecMailBox
        End Get
    End Property

    Private ReadOnly Property EncryptionRequired As Boolean
        Get
            Return Not String.IsNullOrEmpty(ProtocolEnv.PasswordEncryptionKey) AndAlso ProtocolEnv.PasswordEncryptionKey.Length = 32
        End Get
    End Property


    Public Const INSERT_ACTION As String = "PecInsert"
    Public Const EDIT_ACTION As String = "PecEdit"
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub RadAjaxManagerAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        'todo: vuoto per future implementazioni
    End Sub

    Private Sub btnSaveMailBox_Click(sender As Object, e As EventArgs) Handles saveBtn.Click
        If Not Page.IsValid Then
            AjaxAlert("Errore nella validazione dei dati inseriti.")
            Exit Sub
        End If

        Dim hasSaved As Boolean = False
        If Action.Eq(EDIT_ACTION) Then
            hasSaved = UpdatePecMailBox()
        Else
            hasSaved = InsertPecMailBox()
        End If

        If Not hasSaved Then
            AjaxAlert("Errore in fase di salvataggio casella PEC.")
            Exit Sub
        End If
        Response.Redirect("~/PEC/PECMailBoxSettings.aspx?Type=PEC")
    End Sub

    Private Sub btnAnnulla_Click(sender As Object, e As EventArgs) Handles cancelBtn.Click
        Response.Redirect("~/PEC/PECMailBoxSettings.aspx?Type=PEC")
    End Sub

#End Region

#Region "Methods"

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf RadAjaxManagerAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(saveBtn, pnlPageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cancelBtn, pnlPageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        If String.IsNullOrEmpty(Action) Then
            Throw New DocSuiteException("Nessuna Action specificata per la pagina.")
        End If

        If Not Action.Eq(EDIT_ACTION) AndAlso Not Action.Eq(INSERT_ACTION) Then
            Throw New DocSuiteException("Nessuna Action valida trovata.")
        End If

        ddlProfileAdd.DataSource = Facade.PECMailboxConfigurationFacade.GetAll()
        ddlProfileAdd.DataBind()

        If (CurrentPecMailBox IsNot Nothing) Then
            If (CurrentPecMailBox.IncomingServerProtocol.HasValue) Then
                ddlINServerType.SelectedValue = (CType(CurrentPecMailBox.IncomingServerProtocol.Value, Integer)).ToString()
            End If
        End If

        ddlLocation.DataSource = Facade.LocationFacade.GetAll()
        ddlLocation.DataBind()

        Dim hosts As IList(Of JeepServiceHost) = CurrentJeepServiceHostFacade.GetAll()

        ddlJeepServiceIn.DataSource = hosts
        ddlJeepServiceIn.DataBind()

        ddlJeepServiceOut.DataSource = hosts
        ddlJeepServiceOut.DataBind()

        If Action.Eq(EDIT_ACTION) Then
            Title = PAGE_EDIT_TITLE
            txtPasswordRequireValidator.Enabled = False
            BindPageFromPecMailBox()
        Else
            Title = PAGE_INSERT_TITLE
        End If

        Dim itemValues As Array = System.Enum.GetValues(GetType(InvoiceType))
        Dim itemNames As Array = System.Enum.GetNames(GetType(InvoiceType))
        For i As Integer = 0 To itemNames.Length - 1
            Dim item As New ListItem(DirectCast(i, InvoiceType).GetDescription(), itemValues(i))
            ddlInvoiceType.Items.Add(item)
        Next

        If CurrentPecMailBox IsNot Nothing Then
            If CurrentPecMailBox.InvoiceType.HasValue Then
                ddlInvoiceType.SelectedValue = (CType(CurrentPecMailBox.InvoiceType.Value, Integer)).ToString()
            End If
            chkLoginError.Checked = CurrentPecMailBox.LoginError
        End If
    End Sub

    Private Sub BindPageFromPecMailBox()
        If CurrentPecMailBox Is Nothing Then
            Throw New DocSuiteException("Nessuna casella PEC configurata per l'ID passato.")
        End If

        If CurrentPecMailBox.IdJeepServiceOutgoingHost.HasValue Then
            Dim jsHostExist As Boolean = CurrentJeepServiceHostFacade.HostExist(CurrentPecMailBox.IdJeepServiceOutgoingHost.Value)
            If jsHostExist Then
                ddlJeepServiceOut.SelectedValue = CurrentPecMailBox.IdJeepServiceOutgoingHost.Value.ToString()
            End If
        End If

        If CurrentPecMailBox.IdJeepServiceIncomingHost.HasValue Then
            Dim jsHostExist As Boolean = CurrentJeepServiceHostFacade.HostExist(CurrentPecMailBox.IdJeepServiceIncomingHost.Value)
            If jsHostExist Then
                ddlJeepServiceIn.SelectedValue = CurrentPecMailBox.IdJeepServiceIncomingHost.Value.ToString()
            End If
        End If

        If Not String.IsNullOrEmpty(CurrentPecMailBox.MailBoxName) Then
            txtMailboxName.Text = CurrentPecMailBox.MailBoxName
        End If

        If Not String.IsNullOrEmpty(CurrentPecMailBox.Username) Then
            txtUsername.Text = CurrentPecMailBox.Username
        End If

        txtPassword.Text = String.Empty

        chkIsInterop.Checked = CurrentPecMailBox.IsForInterop
        chkIsProtocol.Checked = CurrentPecMailBox.IsProtocolBox.GetValueOrDefault(False)
        chkIsPublicProtocol.Checked = CurrentPecMailBox.IsProtocolBoxExplicit.GetValueOrDefault(False)
        If CurrentPecMailBox.IncomingServerProtocol.HasValue AndAlso Not ddlINServerType.SelectedValue.Equals("-1") Then
            ddlINServerType.SelectedValue = CurrentPecMailBox.IncomingServerProtocol.Value.ToString()
        Else
            ddlINServerType.SelectedValue = "-1"
        End If

        If CurrentPecMailBox.IncomingServerProtocol.HasValue Then
            DirectCast(CurrentPecMailBox.IncomingServerProtocol.Value, Integer).ToString()
        End If
        txtINServerName.Text = CurrentPecMailBox.IncomingServerName
        txtINPort.Text = CurrentPecMailBox.IncomingServerPort.GetValueOrDefault(0).ToString()
        chkINSsl.Checked = CurrentPecMailBox.IncomingServerUseSsl.GetValueOrDefault(False)
        txtOUTServerName.Text = CurrentPecMailBox.OutgoingServerName
        txtOUTPort.Text = CurrentPecMailBox.OutgoingServerPort.GetValueOrDefault(0).ToString()
        chkOUTSsl.Checked = CurrentPecMailBox.OutgoingServerUseSsl.GetValueOrDefault(False)
        chkManaged.Checked = CurrentPecMailBox.Managed
        chkUnmanaged.Checked = CurrentPecMailBox.Unmanaged
        chkIsHandleEnabled.Checked = CurrentPecMailBox.IsHandleEnabled.GetValueOrDefault(False)

        If CurrentPecMailBox.Location IsNot Nothing Then
            ddlLocation.SelectedValue = CurrentPecMailBox.Location.Id.ToString()
        End If

        If CurrentPecMailBox.Configuration IsNot Nothing Then
            ddlProfileAdd.SelectedValue = CurrentPecMailBox.Configuration.Id.ToString()
        End If
    End Sub

    Private Sub BindModelFromPage(ByRef model As PECMailBox)
        model.MailBoxName = txtMailboxName.Text
        model.Username = txtUsername.Text

        If Not String.IsNullOrEmpty(txtPassword.Text) Then
            model.Password = txtPassword.Text
            If EncryptionRequired Then
                Try
                    model.Password = EncHelper.EcryptString(txtPassword.Text, ProtocolEnv.PasswordEncryptionKey)
                Catch ex As Exception
                    AjaxAlert("Errore durante la criptazione della password. Ritentare o contattare assistenza.")
                    Return
                End Try
            End If
        End If

        model.IsForInterop = chkIsInterop.Checked
        model.IsProtocolBox = chkIsProtocol.Checked
        model.IsProtocolBoxExplicit = chkIsPublicProtocol.Checked
        If Not String.IsNullOrEmpty(ddlINServerType.SelectedValue) Then
            model.IncomingServerProtocol = DirectCast([Enum].Parse(GetType(IncomingProtocol), ddlINServerType.SelectedValue.ToString()), IncomingProtocol)
        End If
        model.IncomingServerName = txtINServerName.Text

        Dim port As Integer
        If Integer.TryParse(txtINPort.Text, port) Then
            model.IncomingServerPort = port
        End If
        model.IncomingServerUseSsl = chkINSsl.Checked
        model.OutgoingServerName = txtOUTServerName.Text

        If Integer.TryParse(txtOUTPort.Text, port) Then
            model.OutgoingServerPort = port
        End If
        model.OutgoingServerUseSsl = chkOUTSsl.Checked
        model.Configuration = Facade.PECMailboxConfigurationFacade.GetById(CType(ddlProfileAdd.SelectedValue, Integer))
        model.Location = Facade.LocationFacade.GetById(Integer.Parse(ddlLocation.SelectedValue))
        model.IsHandleEnabled = chkIsHandleEnabled.Checked
        model.IsDestinationEnabled = False
        model.Managed = chkManaged.Checked
        model.Unmanaged = chkUnmanaged.Checked

        If Not String.IsNullOrEmpty(ddlJeepServiceOut.SelectedValue) Then
            model.IdJeepServiceOutgoingHost = Guid.Parse(ddlJeepServiceOut.SelectedValue)
        End If
        If Not String.IsNullOrEmpty(ddlJeepServiceIn.SelectedValue) Then
            model.IdJeepServiceIncomingHost = Guid.Parse(ddlJeepServiceIn.SelectedValue)
        End If

        If Not String.IsNullOrEmpty(ddlInvoiceType.SelectedItem.Text) Then
            model.InvoiceType = CType(ddlInvoiceType.SelectedItem.Value, InvoiceType?)
        End If
        model.LoginError = False
    End Sub

    Protected Function UpdatePecMailBox() As Boolean
        Try
            Dim modelToSave As PECMailBox = CurrentPecMailBox
            BindModelFromPage(modelToSave)
            Facade.PECMailboxFacade.Update(modelToSave)
            Return True
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            Return False
        End Try
    End Function

    Protected Function InsertPecMailBox() As Boolean
        Try
            Dim modelToSave As PECMailBox = New PECMailBox()
            BindModelFromPage(modelToSave)
            Facade.PECMailboxFacade.Save(modelToSave)
            Return True
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            Return False
        End Try
    End Function
#End Region
End Class