Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.Dossiers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class DossierMailSender
    Inherits DossierBasePage

#Region "Fields"
    Private Const CURRENT_PAGE_NAME As String = "DossierMailSender"
#End Region

#Region " Properties "

    Private ReadOnly Property SendToRoles As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("SendToRoles", False)
        End Get
    End Property
    Private ReadOnly Property SendToSecretaries As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("SendToSecretaries", False)
        End Get
    End Property
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            MasterDocSuite.TitleVisible = False
            Initialize()
        End If
    End Sub

    Private Sub MailSenderAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|", 2)


        Select Case arguments(0).ToUpper()
            Case "SENDMAIL"
                If arguments(1) IsNot Nothing Then
                    Dim emails() As String = arguments(1).Split(";"c, ","c)
                    Dim contacts As New List(Of ContactDTO)
                    For Each email As String In emails
                        If (String.IsNullOrEmpty(email) OrElse Not RegexHelper.IsValidEmail(email)) Then
                            Continue For
                        End If
                        contacts.Add(MailFacade.CreateManualContact(email, ContactType.Role, True))
                    Next
                    MailSenderControl.Recipients = contacts
                    MailSenderControl.DataBind()
                End If
        End Select


    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, MailSenderControl, MasterDocSuite.AjaxDefaultLoadingPanel)
        AddHandler AjaxManager.AjaxRequest, AddressOf MailSenderAjaxRequest
    End Sub

    Private Sub Initialize()
        Dim previous As ISendMail = DirectCast(PreviousPage, ISendMail)
        If previous IsNot Nothing Then
            ' Se arriva da pagina con implementazione
            MailSenderControl.SenderDescriptionValue = previous.SenderDescription
            MailSenderControl.SenderEmailValue = previous.SenderEmail
            MailSenderControl.SubjectValue = previous.Subject
            MailSenderControl.BodyValue = previous.Body
            MailSenderControl.Documents = previous.Documents
        Else
            MailSenderControl.SenderDescriptionValue = CommonInstance.UserDescription
            MailSenderControl.SenderEmailValue = CommonInstance.UserMail
            MailSenderControl.SubjectValue = MailFacade.GetDossierSubject(CurrentDossier)
            MailSenderControl.BodyValue = MailFacade.GetDossierBody(CurrentDossier)
        End If

        If ProtocolEnv.DeleteMultipleMailRecipientPages.Contains(CURRENT_PAGE_NAME) Then
            MailSenderControl.EnableCheckBoxRecipients = True
        End If

        MailSenderControl.Recipients = New List(Of ContactDTO)
        If SendToRoles Then
            MailSenderControl.Recipients = RoleFacade.CopyGetValidContacts(CurrentDossier.DossierRoles.Where(Function(f) f.Role IsNot Nothing AndAlso f.AuthorizationRoleType = Entity.Commons.AuthorizationRoleType.Accounted _
                                                                               AndAlso Not String.IsNullOrWhiteSpace(f.Role.EMailAddress)).Select(Function(f) f.Role).ToList())
        End If
        If SendToSecretaries Then
            Dim result As ICollection(Of WebAPIDto(Of Model.Entities.Commons.RoleUserModel)) _
            = WebAPIImpersonatorFacade.ImpersonateFinder(RoleUserFromDossierFinder,
                                                   Function(impersionationType As ImpersonationType, finder As RoleUserFromDossierFinder)
                                                       finder.ResetDecoration()
                                                       finder.EnablePaging = False
                                                       finder.RoleUserFinderModel = New Model.Entities.Commons.RoleUserFinderModel()
                                                       finder.RoleUserFinderModel.IdDossier = Me.CurrentDossier.UniqueId
                                                       Return finder.DoSearchHeader()
                                                   End Function)

            For Each item As WebAPIDto(Of Model.Entities.Commons.RoleUserModel) In result.Where(Function(f) Not String.IsNullOrWhiteSpace(f.Entity.Email))
                MailSenderControl.Recipients.Add(MailFacade.CreateManualContact(item.Entity.Account, item.Entity.Email, ContactType.Person, True, True))
            Next
        End If

        MailSenderControl.DataBind()
    End Sub

    Private Sub MailSenderControlEvent(sender As Object, e As EventArgs) Handles MailSenderControl.CancelByUser, MailSenderControl.ConfirmByUser
        Response.Redirect(PreviousPageUrl)
    End Sub

#End Region

End Class