Imports System.Collections.Generic
Imports System.Text
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.Helpers.UDS
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Gui.MailSenders
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS

Public Class UDSMailSender
    Inherits UDSBasePage

#Region "Fields"
    Private _udsSource As UDSDto
    Private Const CURRENT_PAGE_NAME As String = "UDSMailSender"
    Private Const UDS_SUMMARY_PATH As String = "~/UDS/UDSView.aspx?Type=UDS&IdUDS={0}&IdUDSRepository={1}"
    Public Const NOTIFICATION_ERROR_ICON As String = "delete"
    Public Const NOTIFICATION_SUCCESS_ICON As String = "ok"
    Public Const COMMAND_SUCCESS As String = "Attendere il termine dell'attività di {0}."
    Private Const ON_ERROR_FUNCTION As String = "onError('{0}')"
    Public Const CONFIRM_UDS_CLICK As String = "confirmUds"
    Public Const SUBMIT_SENDING_WORKAROUND As String = "submitSending()"
#End Region

#Region "Properties"

    Private ReadOnly Property UDSSource As UDSDto
        Get
            If _udsSource Is Nothing AndAlso CurrentIdUDSRepository.HasValue AndAlso CurrentIdUDS.HasValue Then
                _udsSource = GetSource()
            End If
            Return _udsSource
        End Get
    End Property

    Private ReadOnly Property LoadRecipients As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("recipients", True)
        End Get
    End Property

    Private ReadOnly Property OverridePreviousPageUrl As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("overridepreviouspageurl", False)
        End Get
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            MasterDocSuite.TitleVisible = False
            HFSubmit.SetDisplay(False)
            If UDSSource IsNot Nothing Then
                MailSenderControl.ButtonToolBar.OnClientButtonClicking = CONFIRM_UDS_CLICK
            End If
            BindMailSenderControl()
        End If
    End Sub

    Protected Sub HFSubmit_Click(sender As Object, e As EventArgs) Handles HFSubmit.Click
        AjaxManager.ResponseScripts.Add(SUBMIT_SENDING_WORKAROUND)
    End Sub

    Private Sub MailSenderCancelEvent(sender As Object, e As EventArgs) Handles MailSenderControl.CancelByUser
        Response.Redirect(PreviousPageUrl)
    End Sub

    Private Sub MailSenderErrorEvent(sender As Object, e As MailErrorEventArgs) Handles MailSenderControl.MailError
        AjaxManager.ResponseScripts.Add(String.Format(ON_ERROR_FUNCTION, HttpUtility.JavaScriptStringEncode(e.Message)))
    End Sub

    Private Sub MailSenderConfirmEvent(sender As Object, e As MailSenders.MailSentEventArgs) Handles MailSenderControl.MailSent
        If UDSSource IsNot Nothing AndAlso e.Mail IsNot Nothing AndAlso e.Mail.Message IsNot Nothing Then
            Try
                Dim correlationId As Guid = Guid.Empty
                If (Not Guid.TryParse(HFcorrelatedCommandId.Value, correlationId)) Then
                    AjaxManager.ResponseScripts.Add(String.Format(ON_ERROR_FUNCTION, "Errore generale, contattare assistenza : CorrelationId is not Valid."))
                    Exit Sub
                End If

                Dim udsModel As UDSModel = UDSSource.UDSModel
                If udsModel.Model.Messages Is Nothing Then
                    udsModel.FillMessages(New List(Of ReferenceModel) From {New ReferenceModel() With {.EntityId = e.Mail.Message.Id, .UniqueId = e.Mail.UniqueId}})
                Else
                    Dim newMessage As MessageInstance = New MessageInstance() With {.IdMessage = e.Mail.Message.Id, .UniqueId = e.Mail.UniqueId.ToString()}
                    udsModel.Model.Messages.Instances = If(udsModel.Model.Messages.Instances, Enumerable.Empty(Of MessageInstance)).Concat(New MessageInstance() {newMessage}).ToArray()
                End If

                Dim sendedCommandId As Guid = CurrentUDSRepositoryFacade.SendCommandUpdateData(CurrentIdUDSRepository.Value, CurrentIdUDS.Value, correlationId, udsModel)
                FileLogger.Info(LoggerName, String.Format("Command sended with Id {0} and CorrelationId {0}", sendedCommandId, correlationId))
            Catch ex As Exception
                Dim exceptionMessage As String = String.Format("Errore nella fase di salvataggio: {0}", ProtocolEnv.DefaultErrorMessage)
                FileLogger.Error(LoggerName, ex.Message, ex)
                AjaxManager.ResponseScripts.Add(String.Format(ON_ERROR_FUNCTION, exceptionMessage))
            End Try
        End If
    End Sub

    Private Sub MailSenderControlEvent(sender As Object, e As EventArgs) Handles MailSenderControl.ConfirmByUser
        If UDSSource Is Nothing Then
            Response.Redirect(PreviousPageUrl)
        End If
    End Sub

    Protected Sub UDSMailSender_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument.Replace("~", "'")
        Dim arguments As Object() = arg.Split("|"c)
        If arguments.Length = 0 Then
            Exit Sub
        End If

        Dim argumentName As String = arguments(0).ToString()

        Select Case argumentName
            Case "callback"
                Dim udsId As Guid = Guid.Parse(arguments(1).ToString())
                Dim udsRepositoryId As Guid = Guid.Parse(arguments(2).ToString())
                If Not udsId.Equals(Guid.Empty) AndAlso Not udsRepositoryId.Equals(Guid.Empty) Then
                    Dim url As String = UDS_SUMMARY_PATH
                    Response.Redirect(String.Format(url, udsId, udsRepositoryId))
                End If
        End Select
    End Sub
#End Region

#Region "Methods"

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf UDSMailSender_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(HFSubmit, HFSubmit)
    End Sub


    Private Sub BindMailSenderControl()
        Try
            Dim previous As ISendMail = TryCast(PreviousPage, ISendMail)
            If previous IsNot Nothing Then
                MailSenderControl.SenderDescriptionValue = previous.SenderDescription
                MailSenderControl.SenderEmailValue = previous.SenderEmail
                MailSenderControl.SubjectValue = previous.Subject
                MailSenderControl.BodyValue = previous.Body

                SetMailRecipients()
                If OverridePreviousPageUrl Then
                    MailSenderControl.Documents = previous.Documents
                    MailSenderControl.EnableAttachment = DocSuiteContext.Current.GetEnableAttachmentByPage("UDSMailSender")
                End If
                MailSenderControl.DataBind()
                Exit Sub
            End If

            If UDSSource IsNot Nothing Then
                MailSenderControl.SenderDescriptionValue = CommonInstance.UserDescription
                MailSenderControl.SenderEmailValue = CommonInstance.UserMail
                MailSenderControl.SubjectValue = UDSFacade.GetUDSMailSubject(UDSSource)
                MailSenderControl.BodyValue = UDSFacade.GetUDSMailBody(UDSSource)

                SetMailRecipients()
                If UDSSource.UDSModel.Model.Documents.Document.Instances IsNot Nothing AndAlso OverridePreviousPageUrl Then
                    MailSenderControl.Documents = UDSFacade.GetAllDocuments(UDSSource.UDSModel)
                    MailSenderControl.EnableAttachment = DocSuiteContext.Current.GetEnableAttachmentByPage("UDSMailSender")
                End If
                MailSenderControl.DataBind()
            End If

        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore pagina di invio mail", ex)
            Throw New DocSuiteException("Errore pagina di invio mail", "Impossibile inizializzare la mail di invio")
        End Try
    End Sub

    Private Sub SetMailRecipients()
        If ProtocolEnv.DeleteMultipleMailRecipientPages.Contains(CURRENT_PAGE_NAME) Then
            MailSenderControl.EnableCheckBoxRecipients = True
        End If

        If UDSSource IsNot Nothing AndAlso LoadRecipients AndAlso UDSSource.UDSModel.Model.Authorizations.Instances IsNot Nothing Then
            Dim roleIds As IList(Of Integer) = New List(Of Integer)
            For Each item As AuthorizationInstance In UDSSource.UDSModel.Model.Authorizations.Instances
                roleIds.Add(Convert.ToInt32(item.IdAuthorization))
            Next

            If ProtocolEnv.MailRecipientsSelectionEnabled AndAlso roleIds.Count > 0 Then
                Dim ids As String = String.Join("|", roleIds)
                Dim url As String = String.Concat("../UserControl/CommonSelMailRecipients.aspx?Type=", Type, "&Roles=", ids)
                AjaxManager.ResponseScripts.Add(String.Concat("return OpenWindowMailSettori('", url, "');"))
            Else
                Dim roles As IList(Of Role) = Facade.RoleFacade.GetByIds(roleIds)
                MailSenderControl.Recipients = RoleFacade.GetValidContacts(roles)
            End If
        End If
    End Sub
#End Region

End Class