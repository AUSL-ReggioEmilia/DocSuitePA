Imports System.Collections.Generic
Imports System.Text
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports System.Linq

Public Class SeriesMailSender
    Inherits CommonBasePage

#Region "Fields"
    Private Const CURRENT_PAGE_NAME As String = "SeriesMailSender"
#End Region

#Region "Properties"

    Private ReadOnly Property IdItem As Integer
        Get
            Return Request.QueryString.GetValue(Of Integer)("idSeriesItem")
        End Get
    End Property

    Private ReadOnly Property LoadRecipients As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("recipients", True)
        End Get
    End Property

    Private _currentItem As DocumentSeriesItem
    Private ReadOnly Property CurrentItem As DocumentSeriesItem
        Get
            If _currentItem Is Nothing Then
                _currentItem = Facade.DocumentSeriesItemFacade.GetById(IdItem)
            End If
            Return _currentItem
        End Get
    End Property
#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            MasterDocSuite.TitleVisible = False
            BindMailSenderControl()
        End If
    End Sub

    Private Sub MailSenderControlEvent(sender As Object, e As EventArgs) Handles MailSenderControl.CancelByUser, MailSenderControl.ConfirmByUser
        Response.Redirect(PreviousPageUrl)
    End Sub

    Private Sub MailSenderControlMailSent(sender As Object, e As MailSenders.MailSentEventArgs) Handles MailSenderControl.MailSent

        If CurrentItem IsNot Nothing Then
            Dim message As New DocumentSeriesItemMessage(CurrentItem, e.Mail.Message)
            Facade.DocumentSeriesItemMessageFacade.Save(message)
        End If

        Dim destinatari As IList(Of MessageContact) = Facade.MessageContactFacade.GetByMessage(e.Mail.Message, MessageContact.ContactPositionEnum.Recipient)

        For Each contact As MessageContact In destinatari
            Dim item As MessageContactEmail = Facade.MessageContactEmailFacade.GetByContact(contact)
            Facade.DocumentSeriesItemLogFacade.AddLog(CurrentItem, DocumentSeriesItemLogType.Mail, String.Format("Spedito a {0}({1})", item.Description, item.Email))
        Next

    End Sub
#End Region

#Region "Methods"

    Private Sub BindMailSenderControl()

        MailSenderControl.SenderDescriptionValue = CommonInstance.UserDescription
        MailSenderControl.SenderEmailValue = CommonInstance.UserMail

        If CurrentItem.Status = DocumentSeriesItemStatus.Draft OrElse (CurrentItem.Year Is Nothing AndAlso CurrentItem.Number Is Nothing) Then
            MailSenderControl.SubjectValue = String.Format("{3}: Bozza N {0} del {1:dd/MM/yyyy} registrata da {2}", CurrentItem.Id, CurrentItem.RegistrationDate, CurrentItem.RegistrationUser, ProtocolEnv.DocumentSeriesName)
        Else
            MailSenderControl.SubjectValue = String.Format("{2}: {0}/{1:0000000}", CurrentItem.Year, CurrentItem.Number, ProtocolEnv.DocumentSeriesName)
        End If

        Dim tempBody As New StringBuilder()
        tempBody.Append(MailSenderControl.SubjectValue)

        Dim retireDate As String = If(CurrentItem.RetireDate.HasValue, CurrentItem.RetireDate.Value.ToString("dd/MM/yyyy"), "")
        Dim publicationDate As String = If(CurrentItem.PublishingDate.HasValue, CurrentItem.PublishingDate.Value.ToString("dd/MM/yyyy"), "")
        Dim link As String = String.Format("<a href=""{0}?Tipo=DocumentSeries&Azione=Apri&IdDocumentSeriesItem={1}"">{2}</a>", DocSuiteContext.Current.CurrentTenant.DSWUrl, CurrentItem.Id, MailSenderControl.SubjectValue)

        tempBody.AppendFormat("<br>Contenitore: {0} <br> Classificazione: {1} <br> Oggetto: {2} <br> Pubblicata: {3} <br> Ritirata: {4} <br><br> {5} <br>", CurrentItem.DocumentSeries.Container.Name, CurrentItem.Category.Name, CurrentItem.Subject, publicationDate, retireDate, link)

        MailSenderControl.BodyValue = tempBody.ToString()

        If ProtocolEnv.DeleteMultipleMailRecipientPages.Contains(CURRENT_PAGE_NAME) Then MailSenderControl.EnableCheckBoxRecipients = True

        If LoadRecipients AndAlso CurrentItem.DocumentSeriesItemRoles IsNot Nothing AndAlso CurrentItem.DocumentSeriesItemRoles.Count > 0 Then
            If ProtocolEnv.MailRecipientsSelectionEnabled Then
                Dim roleIds As String = String.Join("|", CurrentItem.DocumentSeriesItemRoles.Select(Function(r) r.Role.Id))
                Dim url As String = String.Concat("../UserControl/CommonSelMailRecipients.aspx?Type=", Type, "&Roles=", roleIds)
                AjaxManager.ResponseScripts.Add(String.Concat("return OpenWindowMailSettori('", url, "');"))
            Else
                Dim roles As List(Of Role) = CurrentItem.DocumentSeriesItemRoles.Select(Function(rr) rr.Role).ToList()
                MailSenderControl.Recipients = RoleFacade.GetValidContacts(roles)
            End If
        End If
        MailSenderControl.DataBind()

    End Sub
#End Region

End Class