Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers
Imports System.Collections.Generic
Imports System.Linq

Public Class CollaborationMailSender
    Inherits CommBasePage

#Region "Fields"
    Private Const CURRENT_PAGE_NAME As String = "GenericMailSender"
#End Region

#Region " Property "

    Private ReadOnly Property LoadRecipients As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("recipients", True)
        End Get
    End Property

    Private ReadOnly Property CollaborationId As Integer
        Get
            Return Request.QueryString.GetValueOrDefault("CollaborationId", 0)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then

            Title = $"{PECBasePage.EmailLabel} - Invio"


            Dim previous As ISendMail = DirectCast(PreviousPage, ISendMail)
            If previous IsNot Nothing Then
                MailSenderControl.SenderDescriptionValue = previous.SenderDescription

                MailSenderControl.SenderEmailValue = previous.SenderEmail
                MailSenderControl.SubjectValue = previous.Subject
                MailSenderControl.BodyValue = previous.Body

                MailSenderControl.Documents = previous.Documents
                If LoadRecipients Then
                    MailSenderControl.Recipients = previous.Recipients
                End If

                If ProtocolEnv.DeleteMultipleMailRecipientPages.Contains(CURRENT_PAGE_NAME) Then
                    MailSenderControl.EnableCheckBoxRecipients = True
                End If

                'verifico il tipo di collaborazione
                If ProtocolEnv.CheckResolutionCollaborationOriginEnabled AndAlso CollaborationId > 0 Then
                    Dim recipients As IList(Of ContactDTO) = New List(Of ContactDTO)
                    Dim currentCollaboration As Collaboration = Facade.CollaborationFacade.GetById(CollaborationId)
                    Dim draft As CollaborationXmlData = FacadeFactory.Instance.ProtocolDraftFacade.GetDataFromCollaboration(currentCollaboration)
                    If draft IsNot Nothing AndAlso draft.GetType() = GetType(ResolutionXML) Then
                        Dim resolutionXML As ResolutionXML = CType(draft, ResolutionXML)
                        If (resolutionXML IsNot Nothing) Then
                            Dim roleIds As IList(Of Integer) = currentCollaboration.CollaborationUsers.Where(Function(pr) pr.DestinationType = DestinatonType.S.ToString()).Select(Function(r) Convert.ToInt32(r.IdRole)).ToList()
                            Dim roleRecipients As IList(Of Role) = Facade.RoleFacade.GetByIds(roleIds)
                            MailSenderControl.Recipients = RoleFacade.GetValidContacts(roleRecipients)
                        End If
                    End If
                    MailSenderControl.EnableAttachment = DocSuiteContext.Current.GetEnableAttachmentByPage("CollaborationMailSender")
                    MailSenderControl.DataBind()
                End If
            Else
                MailSenderControl.EnableAttachment = DocSuiteContext.Current.GetEnableAttachmentByPage("CollaborationMailSender")
                MailSenderControl.SenderDescriptionValue = CommonInstance.UserDescription
                MailSenderControl.SenderEmailValue = CommonInstance.UserMail
            End If

        End If
    End Sub

    Private Sub MailSenderControl_CancelByUser(sender As Object, e As EventArgs) Handles MailSenderControl.CancelByUser, MailSenderControl.ConfirmByUser
        Response.Redirect(PreviousPageUrl)
    End Sub

#End Region

End Class