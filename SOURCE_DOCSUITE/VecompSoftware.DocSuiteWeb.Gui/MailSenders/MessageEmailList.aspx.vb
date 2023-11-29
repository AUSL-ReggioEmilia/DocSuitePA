Imports System.Net.Mail
Imports System.Linq
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web
Imports System.Web

Public Class MessageEmailList
    Inherits CommBasePage

#Region " Filters "

    ''' <summary> Numero di giorni da sottrarre alla data odierna nella preimpostazione del filtro. </summary>
    Private Const DaysToAddInitialized As Double = -15
    Private _pagedCurrentRecipients As IList(Of MessageContactEmailHeader)
    Private _finder As NHMessageEmailFinder

#End Region

#Region " Properties "

    Private ReadOnly Property PagedCurrentContacts() As IList(Of MessageContactEmailHeader)
        Get
            If _pagedCurrentRecipients Is Nothing Then
                ' Estraggo i messaggi attualmente bindati alle righe
                Dim messages As IList(Of MessageEmail) = DirectCast(dgMessageEmail.DataSource, IList(Of MessageEmail))
                _pagedCurrentRecipients = Facade.MessageContactEmailFacade.GetByMessages(messages.Select(Function(messageEmail) messageEmail.Message).Distinct().ToList(), {MessageContact.ContactPositionEnum.Sender, MessageContact.ContactPositionEnum.Recipient, MessageContact.ContactPositionEnum.RecipientCc})
            End If
            Return _pagedCurrentRecipients
        End Get
    End Property

    Private ReadOnly Property Finder As NHMessageEmailFinder
        Get
            If _finder Is Nothing Then
                _finder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.MessageEmailFinderType), NHMessageEmailFinder)
            End If
            Return _finder
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        dgMessageEmail.MasterTableView.NoMasterRecordsText = "Nessuna email disponibile"
        If Not IsPostBack Then
            Initialize()
            If ddlSender.Enabled Then
                InitializeColumns()
                DataBindMessageGrid()
            End If
        End If
    End Sub

    Private Sub cmdRefreshGrid_Click(sender As Object, e As EventArgs) Handles cmdRefreshGrid.Click
        dgMessageEmail.DiscardFinder()
        DataBindMessageGrid()
    End Sub

    Private Sub cmdClearFilters_Click(sender As Object, e As EventArgs) Handles cmdClearFilters.Click
        InitializeFilters()
        If ddlSender.Items.Any() Then
            ddlSender.Items.FirstOrDefault().Selected = True
        End If
    End Sub

    Private Sub dgMessageEmail_ItemCommand(sender As Object, e As GridCommandEventArgs) Handles dgMessageEmail.ItemCommand
        If e.CommandName.Eq("Log") Then
            Response.Redirect(String.Format("MessageViewLog.aspx?Type=Comm&MessageEmailId={0}", DirectCast(e.Item, GridDataItem).GetDataKeyValue("Id")))
        End If
    End Sub

    Private Sub dgMessageEmail_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles dgMessageEmail.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim gridDataItem As GridDataItem = DirectCast(e.Item, GridDataItem)
        Dim item As MessageEmail = DirectCast(e.Item.DataItem, MessageEmail)

        With DirectCast(e.Item.FindControl("imgPriority"), Image)
            Select Case item.Priority
                Case MailPriority.High
                    .ImageUrl = "../Comm/Images/Mails/highimportance.gif"
                    .AlternateText = "Priorità alta"
                Case MailPriority.Low
                    .ImageUrl = "../Comm/Images/Mails/lowimportance.gif"
                    .AlternateText = "Priorità bassa"
                Case Else
                    .Visible = False
                    gridDataItem.Item("cPriority").Text = WebHelper.Space
            End Select
        End With

        With DirectCast(e.Item.FindControl("lblSenders"), Label)
            .Text = String.Join("; ", PagedCurrentContacts.Where(Function(mce) mce.IdMessage = item.Message.Id AndAlso mce.ContactPosition = MessageContact.ContactPositionEnum.Sender).Select(Function(mce) String.Format(HttpUtility.HtmlEncode("{0} <{1}>"), mce.Description, mce.Email)))
        End With

        With DirectCast(e.Item.FindControl("lblSubject"), Label)
            If DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsObjectInGridPec > 0 AndAlso item.Subject.Length > DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsObjectInGridPec Then
                .Text = item.Subject.Remove(DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsObjectInGridPec) & "  [...]"
                .ToolTip = item.Subject
            Else
                .Text = item.Subject
            End If
        End With

        With DirectCast(e.Item.FindControl("lblRecipients"), Label)
            .Text = String.Join("; ", PagedCurrentContacts.Where(Function(mce) mce.IdMessage = item.Message.Id AndAlso mce.ContactPosition = MessageContact.ContactPositionEnum.Recipient).Select(Function(mce) String.Format(HttpUtility.HtmlEncode("{0} <{1}>"), mce.Description, mce.Email)))
        End With

        With DirectCast(e.Item.FindControl("lblRecipientsCc"), Label)
            .Text = String.Join("; ", PagedCurrentContacts.Where(Function(mce) mce.IdMessage = item.Message.Id AndAlso mce.ContactPosition = MessageContact.ContactPositionEnum.RecipientCc).Select(Function(mce) String.Format(HttpUtility.HtmlEncode("{0} <{1}>"), mce.Description, mce.Email)))
        End With
    End Sub

    Private Sub ddlSender_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlSender.TextChanged
        Dim isValidEmail As Boolean = ddlSender.Items.Any(Function(x) x.Text.Equals(sender.Text))
        SetFiltersState(Not isValidEmail)
    End Sub

    Private Sub ddlSender_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlSender.SelectedIndexChanged
        SetFiltersState()
    End Sub
#End Region

#Region " Methods "
    Private Sub SetFiltersState(Optional isErrorState As Boolean = False)
        errorDDLSender.Visible = isErrorState
        cmdRefreshGrid.Enabled = Not isErrorState
        cmdClearFilters.Enabled = Not isErrorState
    End Sub
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(dgMessageEmail, dgMessageEmail, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRefreshGrid, dgMessageEmail, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, pnlFilters, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        If Not DocSuiteContext.Current.ProtocolEnv.EnableMessageView Then
            Throw New DocSuiteException(DocSuiteContext.Current.ProtocolEnv.MessageViewName, "Visualizzazione non abilitata")
        End If
        Title = DocSuiteContext.Current.ProtocolEnv.MessageViewName

        InitializeFilters()
        ' Se trovo finder imposto i filtri
        If Finder IsNot Nothing Then
            dtpSentFrom.SelectedDate = Finder.SentFrom
            dtpSentTo.SelectedDate = Finder.SentTo
            chkUnsent.Checked = Finder.Unsent
            If ProtocolEnv.ViewAuthorizedRoleEmailsEnabled Then
                ddlSender.Text = Finder.SenderEmail
            Else
                txtSender.Text = Finder.SenderEmail
            End If
            txtRecipient.Text = Finder.RecipientEmail
        End If
    End Sub

    Private Sub InitializeColumns()
        dgMessageEmail.Columns.FindByUniqueName("colRecipientsCc").Visible = ProtocolEnv.EnableMessageViewCcColumn
    End Sub

    Private Sub InitializeFilters()
        dtpSentFrom.SelectedDate = Today.AddDays(DaysToAddInitialized)
        dtpSentTo.SelectedDate = Today
        chkUnsent.Checked = True
        SetFiltersState()

        If ProtocolEnv.ViewAuthorizedRoleEmailsEnabled Then
            inputSender.Visible = False
            dropdownSender.Visible = True

            Dim roles As IList(Of Role) = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Any, Nothing, True, CurrentTenant.TenantAOO.UniqueId)

            ddlSender.Items.Clear()
            If roles IsNot Nothing Then
                For Each role As Role In roles
                    If Not String.IsNullOrEmpty(role.EMailAddress) Then
                        Dim emailRoles As String() = role.EMailAddress.Split(";"c)
                        For Each emailRole As String In emailRoles
                            ddlSender.Items.Add(New RadComboBoxItem(emailRole, emailRole))
                        Next
                    End If
                Next
            End If


            If Finder Is Nothing AndAlso ddlSender.Items.Any() Then
                ddlSender.Items.FirstOrDefault().Selected = True
            Else
                ddlSender.Text = Finder.SenderEmail
            End If

            If Not ddlSender.Items.Any() Then
                ddlSender.Enabled = False
                cmdRefreshGrid.Enabled = False
                cmdClearFilters.Enabled = False
            End If

            ddlSender.DataBind()
        Else
            inputSender.Visible = True
            dropdownSender.Visible = False
            txtSender.Text = Facade.UserLogFacade.EmailOfUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain, True)
        End If
        txtRecipient.Text = String.Empty
    End Sub

    Private Sub DataBindMessageGrid()
        If dgMessageEmail.Finder Is Nothing Then
            Dim messageEmailFinder As New NHMessageEmailFinder()
            SetFinderProperties(messageEmailFinder)

            dgMessageEmail.Finder = messageEmailFinder

            ' Paginazione griglia.
            messageEmailFinder.EnablePaging = dgMessageEmail.AllowPaging
            dgMessageEmail.CurrentPageIndex = 0
            dgMessageEmail.CustomPageIndex = 0
        End If

        dgMessageEmail.PageSize = dgMessageEmail.Finder.PageSize

        ' Salvo il Finder in Sessione (gestione BACK)
        SessionSearchController.SaveSessionFinder(dgMessageEmail.Finder, SessionSearchController.SessionFinderType.MessageEmailFinderType)
        ' Salvo in sessione l'ora di registrazione
        Session.Add("DSW_MessageEmailFinderType", DateTime.Now)

        ' DataBind del finder.
        dgMessageEmail.DataBindFinder()
    End Sub

    Private Sub SetFinderProperties(ByVal finder As NHMessageEmailFinder)
        finder.SentFrom = dtpSentFrom.SelectedDate
        finder.SentTo = dtpSentTo.SelectedDate
        finder.Unsent = chkUnsent.Checked
        finder.SenderEmail = If(ProtocolEnv.ViewAuthorizedRoleEmailsEnabled, If(ddlSender.SelectedItem IsNot Nothing, ddlSender.SelectedItem.Text, ddlSender.Text), txtSender.Text)
        finder.RecipientEmail = txtRecipient.Text
    End Sub

#End Region

End Class