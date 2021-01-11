Imports System.Linq
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Desks
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.DTO.Desks
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Desks
Imports VecompSoftware.DocSuiteWeb.Gui
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports VecompSoftware.Services.Biblos.Models

Public Class DeskSummary
    Inherits DeskBasePage
    Implements ISignMultipleDocuments

#Region "Fields"
    Private Const ACTION_VIEW_NAME As String = "View"
    Private Const ACTION_MODIFY_NAME As String = "Modify"
    Private Const PAGE_TITLE As String = "Tavoli - Gestione"
    Private Const APPROVE_MANAGER_PATH_FORMAT As String = "~/Desks/DeskApproveManager.aspx?Type=Desk&DeskId={0}"
    Private Const TO_COLLABORATION_PATH_FORMAT As String = "~/Desks/DeskToCollaboration.aspx?Type=Desk&DeskId={0}"
    Private Const TO_PROTOCOL_PATH_FORMAT As String = "~/Desks/DeskToProtocol.aspx?Type=Desk&DeskId={0}"
    Private Const DESK_SUMMARY_PATH As String = "~/Desks/DeskSummary.aspx?Type=Desk&Action=View&DeskId={0}"
    Private Const COLLABORATION_PATH_FORMAT As String = "~/User/UserCollGestione.aspx?Type={0}&Titolo=Visualizzazione&Action={1}&idCollaboration={2}&Action2={3}&Title2=Visualizzazione"
    Private _currentDeskCollaborationFacade As DeskCollaborationFacade
#End Region

#Region "Properties"
    Public Property BtnSaveVisible As Boolean
        Get
            Return btnSave.Visible
        End Get
        Set(value As Boolean)
            btnSave.Visible = value
        End Set
    End Property

    Public ReadOnly Property CurrentDeskCollaborationFacade As DeskCollaborationFacade
        Get
            If _currentDeskCollaborationFacade Is Nothing Then
                _currentDeskCollaborationFacade = New DeskCollaborationFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _currentDeskCollaborationFacade
        End Get
    End Property

    Public ReadOnly Property DocumentsToSign As IList(Of MultiSignDocumentInfo) Implements ISignMultipleDocuments.DocumentsToSign
        Get
            Return uscDeskDocument.DocumentsToSign
        End Get
    End Property

    Public ReadOnly Property ReturnUrl As String Implements ISignMultipleDocuments.ReturnUrl
        Get
            Return String.Format(DESK_SUMMARY_PATH, CurrentDeskId)
        End Get
    End Property
    Public ReadOnly Property SignAction As String Implements ISignMultipleDocuments.SignAction
        Get
            Return String.Empty
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
            InitializeUserControls()
        End If

    End Sub

    Private Sub DeskSummary_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument
        Select Case arg
            Case "initializeDataControls"
                InitializeUserControls()
        End Select
    End Sub

    Protected Sub BtnModify_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnModify.Click
        InitializeModifyPageControl()
        InitializeUserControls()

        btnSave.Visible = True
        btnCancel.Visible = True
        btnModify.Visible = False
        btnApprove.Visible = False
        btnCloseDesk.Visible = False
        btnOpenDesk.Visible = Not btnCloseDesk.Visible AndAlso (CurrentDeskRigths.CurrentUserIsManager OrElse CurrentDeskRigths.CurrentUserIsOwner)
        btnApproveManager.Visible = False
        btnCollaboration.Visible = False
        btnViewDocument.Visible = False
    End Sub

    Protected Sub BtnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        BindDeskFromPage()

        InitializeViewPageControl()
        InitializeUserControls()

        InitializeButtons()
    End Sub
    Protected Sub BtnApprove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnApprove.Click
        If Not CurrentDeskId.HasValue OrElse Not CurrentDeskRoleUserFacade.HasUsersApprovers(CurrentDeskId.Value) Then
            AjaxAlert("Non sono presenti utenti a cui richiedere approvazione.")
            Exit Sub
        End If

        If CurrentDesk.DeskDocuments Is Nothing OrElse Not CurrentDesk.DeskDocuments.Any() Then
            AjaxAlert("Non sono presenti documenti da approvare.")
            Exit Sub
        End If

        CurrentDesk.Status = DeskState.Approve
        CurrentDeskFacade.Update(CurrentDesk)

        Dim userAdded As IList(Of DeskRoleUserResult) = uscInvitationUser.GetUsers()
        If userAdded.Count > 0 Then
            For Each userResult As DeskRoleUserResult In userAdded
                If String.Equals(userResult.PermissionType, DeskPermissionType.Approval.ToString) OrElse String.Equals(userResult.PermissionType, DeskPermissionType.Admin.ToString) Then
                    SendApprovalRequested(CurrentDesk, userResult)
                End If
            Next
        End If

        InitializeButtons()
        lblDeskStatusExteded.Text = CurrentDesk.Status.Value.GetDescription()
        lblDeskStatusRestricted.Text = String.Concat(CurrentDesk.Status.Value.GetDescription(), " - ", CurrentDesk.ExpirationDate.Value.ToString("dd/MM/yyyy"))
        lblRegistrationUser.Text = CommonAD.GetDisplayName(CurrentDesk.RegistrationUser)
        lblRegistrationUser2.Text = lblRegistrationUser.Text
    End Sub

    Protected Sub BtnCloseDesk_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCloseDesk.Click
        CurrentDesk.Status = DeskState.Closed
        CurrentDeskFacade.Update(CurrentDesk)

        InitializeButtons()
        lblDeskStatusExteded.Text = CurrentDesk.Status.Value.GetDescription()
        lblDeskStatusRestricted.Text = String.Concat(CurrentDesk.Status.Value.GetDescription(), " - ", CurrentDesk.ExpirationDate.Value.ToString("dd/MM/yyyy"))
        lblRegistrationUser.Text = CommonAD.GetDisplayName(CurrentDesk.RegistrationUser)
        lblRegistrationUser2.Text = lblRegistrationUser.Text
    End Sub

    Protected Sub BtnOpenDesk_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnOpenDesk.Click
        CurrentDesk.Status = DeskState.Open
        CurrentDeskFacade.Update(CurrentDesk)

        InitializeButtons()
        lblDeskStatusExteded.Text = CurrentDesk.Status.Value.GetDescription()
        lblDeskStatusRestricted.Text = String.Concat(CurrentDesk.Status.Value.GetDescription(), " - ", CurrentDesk.ExpirationDate.Value.ToString("dd/MM/yyyy"))
        lblRegistrationUser.Text = CommonAD.GetDisplayName(CurrentDesk.RegistrationUser)
        lblRegistrationUser2.Text = lblRegistrationUser.Text
    End Sub

    Protected Sub BtnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        InitializeViewPageControl()
        InitializeUserControls()

        InitializeButtons()
    End Sub

    Protected Sub BtnApproveManager_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnApproveManager.Click

        Response.Redirect(String.Format(APPROVE_MANAGER_PATH_FORMAT, CurrentDesk.Id))
    End Sub

    Protected Sub BtnCollaboration_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCollaboration.Click
        Response.Redirect(String.Format(TO_COLLABORATION_PATH_FORMAT, CurrentDesk.Id))
    End Sub
    Protected Sub BtnProtocol_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnProtocol.Click
        Response.Redirect(String.Format(TO_PROTOCOL_PATH_FORMAT, CurrentDesk.Id))
    End Sub
    Private Sub UscDeskDocument_DocumentsDataSourceUpdate(sender As Object, e As DeskDocumentResultEventArgs) Handles uscDeskDocument.DocumentCheckIn, uscDeskDocument.DocumentCheckOut, uscDeskDocument.DocumentUndoCheckOut, uscDeskDocument.DocumentSigned
        InitializeDocuments()
    End Sub
    Private Sub UscDeskDocument_AddDocument(sender As Object, e As DocumentEventArgs) Handles uscDeskDocument.DocumentUploaded
        UpdateDocumentsFromPage()
        InitializeDocuments()
    End Sub
    Private Sub UscDeskDocument_ReloadAllDocument(sender As Object, e As DocumentEventArgs) Handles uscDeskDocument.DocumentReloaded
        InitializeDocuments()
    End Sub
    Private Sub UscInvitationUser_AddUser(sender As Object, e As DeskRoleUserEventArgs) Handles uscInvitationUser.UserAdded
        If e.FromAddDomain Then
            CurrentDeskRoleUserFacade.AddUser(e.DeskRoleUser, CurrentDesk)
            Dim userAdded As IList(Of DeskRoleUserResult) = uscInvitationUser.GetAddedUsers()
            If userAdded.Count > 0 Then
                For Each userResult As DeskRoleUserResult In userAdded
                    SendInvitedMessage(CurrentDesk, userResult)
                Next
            End If
        End If
    End Sub

    Private Sub UscInvitationUser_RemoveUser(sender As Object, e As DeskRoleUserEventArgs) Handles uscInvitationUser.UserDeleted
        CurrentDeskRoleUserFacade.RemoveUser(e.DeskRoleUser, CurrentDesk)
    End Sub

    Private Sub UscInvitationUser_ChangeRoleUser(sender As Object, e As DeskRoleUserEventArgs) Handles uscInvitationUser.UserChangeRole
        CurrentDeskRoleUserFacade.UpdateUser(e.DeskRoleUser, CurrentDesk)
    End Sub

    Protected Sub BtnViewDocument_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnViewDocument.Click
        If CurrentDesk.DeskDocuments.IsNullOrEmpty() Then
            AjaxAlert("Errore in visualizzazione documento")
            FileLogger.Error(LoggerName, "Errore in visualizzazione documento")
            Exit Sub
        End If

        Dim parameters As String = String.Format("DeskId={0}", CurrentDesk.Id)
        Dim url As String = String.Format("~/viewers/DeskViewer.aspx?{0}", CommonShared.AppendSecurityCheck(parameters))
        Response.Redirect(url)
    End Sub

    Private Sub btnCollapse_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCollapse.Click
        If btnCollapse.CssClass.Contains("arrow-down") Then
            pnlContents.Visible = False
            btnCollapse.CssClass = btnCollapse.CssClass.Replace("arrow-down", "arrow-up")
        Else
            pnlContents.Visible = True
            btnCollapse.CssClass = btnCollapse.CssClass.Replace("arrow-up", "arrow-down")
        End If
    End Sub


#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf DeskSummary_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDeskDocument, uscDeskDocument)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscInvitationUser, uscInvitationUser)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDeskStoryBoard, uscDeskStoryBoard)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, deskContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, pnlContainers)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, pnlDeskData)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, pnlDeskInformationExtended)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, pnlDeskInformationRestricted)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, pnlDeskSubject)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, pnlActionButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, pnlStoryBoard)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnCancel, deskContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCancel, pnlContainers)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCancel, pnlDeskData)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, pnlDeskInformationExtended)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, pnlDeskInformationRestricted)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCancel, pnlDeskSubject)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCancel, pnlActionButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCancel, pnlStoryBoard)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, deskContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, pnlContainers)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, pnlDeskData)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, pnlDeskInformationExtended)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, pnlDeskInformationRestricted)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, pnlDeskSubject)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, pnlActionButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, pnlStoryBoard)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnCloseDesk, deskContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCloseDesk, pnlActionButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnOpenDesk, deskContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnOpenDesk, pnlActionButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, pnlDeskInformationExtended)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, pnlDeskInformationRestricted)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnApprove, deskContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnApprove, pnlActionButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, pnlDeskInformationExtended)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModify, pnlDeskInformationRestricted)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnCollaboration, deskContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCollaboration, pnlActionButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnProtocol, deskContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnProtocol, pnlActionButtons)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnApproveManager, deskContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnViewDocument, deskContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        If CurrentDesk Is Nothing Then
            Throw New DocSuiteException(String.Format("Tavolo con ID {0} Inesistente", CurrentDeskId))
        End If

        If Not CurrentDeskRigths.IsSummaryViewable Then
            Throw New DocSuiteException(String.Format("Tavolo ID {0}", CurrentDeskId), "Non è possibile visualizzare il Tavolo richiesto. Verificare se si dispone di sufficienti autorizzazioni.")
        End If
        Title = CurrentDesk.Name

        'Inizializzo lo user control degli inviti
        uscInvitationUser.InitializeUserControlSource()

        'Inizializzo lo user control dei documenti        
        uscDeskDocument.InitializeUserControlSource()

        Dim collaborations As ICollection(Of Collaboration) = CurrentDeskCollaborationFacade.GetActiveCollaborationByIdDesk(CurrentDesk.Id)
        If collaborations.Any() Then
            pnlDeskCollaborationLink.Visible = True
            Dim dict As Dictionary(Of String, String) = collaborations.ToDictionary(Function(x) x.CollaborationObject, Function(x) String.Format(COLLABORATION_PATH_FORMAT, CollaborationFacade.GetPageTypeFromDocumentType(x.DocumentType), CollaborationSubAction.ProtocollatiGestiti, x.Id, CollaborationMainAction.ProtocollatiGestiti))
            links.DataSource = dict
            links.DataBind()
        End If

        'Contenitore
        lblDeskContainer.Text = CurrentDesk.Container.Name

        pnlDeskInformationRestricted.Visible = True
        pnlDeskInformationExtended.Visible = False
        lblDeskStatusExteded.Text = CurrentDesk.Status.Value.GetDescription()
        lblDeskStatusRestricted.Text = String.Concat(CurrentDesk.Status.Value.GetDescription(), " - ", CurrentDesk.ExpirationDate.Value.ToString("dd/MM/yyyy"))
        lblRegistrationUser.Text = CommonAD.GetDisplayName(CurrentDesk.RegistrationUser)
        lblRegistrationUser2.Text = lblRegistrationUser.Text

        'Inizializzo il controllo dei story board
        uscDeskStoryBoard.InitializeUserControlFinder()
        uscDeskStoryBoard.CurrentDeskId = CurrentDeskId
        uscDeskDocument.ParentPageUrl = ReturnUrl

        'Visualizzazione link verso collaborazione
        pnlDeskCollaborationLink.Visible = ProtocolEnv.DeskCollaborationLinkVisible

        If CurrentDeskRigths.IsClosed Then
            InitializeViewPageControl(False)
        Else
            InitializePageControlFromAction(False)
        End If

        InitializeButtons()
    End Sub

    Private Sub InitializePageControlFromAction(Optional needRefresh As Boolean = True)
        Select Case Action
            Case ACTION_VIEW_NAME
                InitializeViewPageControl(needRefresh)

            Case ACTION_MODIFY_NAME
                If Not CurrentDeskRigths.IsEditable Then
                    Throw New DocSuiteException(String.Format("Tavolo ID {0}", CurrentDeskId), "Non è possibile visualizzare il Tavolo richiesto in modifica. Verificare se si dispone di sufficienti autorizzazioni.")
                End If
                InitializeModifyPageControl()

            Case Else
                Throw New DocSuiteException("Errore visualizzazione tavolo", "Action non riconosciuta come valida")
        End Select
    End Sub

    Private Sub InitializeViewPageControl(Optional needRefresh As Boolean = True)
        'Oggetto del tavolo
        lblDeskObject.Visible = True
        txtObject.Text = String.Empty
        txtObject.Visible = False
        lblDeskObject.Text = CurrentDesk.Description

        'Scadenza del tavolo
        dtpDeskExpired.SelectedDate = Nothing
        dtpDeskExpired.Visible = False

        'Documenti
        uscDeskDocument.DeskId = CurrentDeskId
        uscDeskDocument.ButtonStoryBoardVisible = CurrentDeskRigths.IsStoryBoardViewable
        uscDeskDocument.IsReadOnly = CurrentDeskRigths.CurrentUserIsReader
        uscDeskDocument.ButtonDeleteEnable = CurrentDeskRigths.CanLogicDeleteDocument()
        uscDeskDocument.BtnSelectTemplateVisible = GetControlTemplateDocumentVisibility(Entity.DocumentUnits.ChainType.MainChain) AndAlso Not CurrentDeskRigths.CurrentUserIsReader
        uscDeskDocument.Refresh()

        'Inviti
        uscInvitationUser.Type = uscInvitationUser.TYPE_MODIFY_NAME
        uscInvitationUser.IsReadOnly = CurrentDeskRigths.IsClosed OrElse Not (CurrentDeskRigths.CurrentUserIsManager OrElse CurrentDeskRigths.CurrentUserIsOwner)
        uscInvitationUser.ButtonUserDeleteEnabled = CurrentDeskRigths.IsUserButtonDeleteVisible
        uscInvitationUser.Refresh()

        'Commenti
        uscDeskStoryBoard.Visible = True

        'Information 
        pnlDeskInformationRestricted.Visible = True
        pnlDeskInformationExtended.Visible = False
        lblDeskStatusExteded.Text = CurrentDesk.Status.Value.GetDescription()
        lblDeskStatusRestricted.Text = String.Concat(CurrentDesk.Status.Value.GetDescription(), " - ", CurrentDesk.ExpirationDate.Value.ToString("dd/MM/yyyy"))
    End Sub

    Private Sub InitializeModifyPageControl()
        'Oggetto del tavolo
        lblDeskObject.Visible = False
        txtObject.Text = CurrentDesk.Description
        txtObject.Visible = True
        lblDeskObject.Text = String.Empty

        'Scadenza del tavolo
        dtpDeskExpired.SelectedDate = CurrentDesk.ExpirationDate.Value
        dtpDeskExpired.Visible = True

        'Documenti
        uscDeskDocument.DeskId = CurrentDeskId
        uscDeskDocument.ButtonStoryBoardVisible = CurrentDeskRigths.IsStoryBoardViewable
        uscDeskDocument.IsReadOnly = False
        uscDeskDocument.ButtonDeleteEnable = CurrentDeskRigths.CanLogicDeleteDocument()
        uscDeskDocument.BtnSelectTemplateVisible = GetControlTemplateDocumentVisibility(Entity.DocumentUnits.ChainType.MainChain)
        uscDeskDocument.Refresh()

        'Inviti
        uscInvitationUser.Type = uscInvitationUser.TYPE_MODIFY_NAME
        uscInvitationUser.IsReadOnly = CurrentDeskRigths.IsClosed
        uscInvitationUser.ButtonUserDeleteEnabled = CurrentDeskRigths.IsUserButtonDeleteVisible
        uscInvitationUser.Refresh()

        'Commenti
        uscDeskStoryBoard.Visible = False

        'Information 
        pnlDeskInformationRestricted.Visible = False
        pnlDeskInformationExtended.Visible = True
        lblDeskStatusExteded.Text = CurrentDesk.Status.Value.GetDescription()
        lblDeskStatusRestricted.Text = String.Concat(CurrentDesk.Status.Value.GetDescription(), " - ", CurrentDesk.ExpirationDate.Value.ToString("dd/MM/yyyy"))
        lblRegistrationUser.Text = CommonAD.GetDisplayName(CurrentDesk.RegistrationUser)
        lblRegistrationUser2.Text = lblRegistrationUser.Text
    End Sub

    Private Sub InitializeButtons()
        btnSave.Visible = False
        btnCancel.Visible = False
        btnApprove.Visible = CurrentDeskRigths.IsEditable AndAlso CurrentDeskRigths.IsOpen
        btnCloseDesk.Visible = CurrentDeskRigths.IsCloseable
        btnOpenDesk.Visible = Not btnCloseDesk.Visible AndAlso (CurrentDeskRigths.CurrentUserIsManager OrElse CurrentDeskRigths.CurrentUserIsOwner)
        btnModify.Visible = CurrentDeskRigths.IsEditable
        btnApproveManager.Visible = CurrentDeskRigths.IsEditable
        btnCollaboration.Visible = CurrentDeskRigths.IsProtocollable
        btnProtocol.Visible = CurrentDeskRigths.IsProtocollable
        btnViewDocument.Visible = CurrentDeskRigths.IsSummaryViewable
    End Sub

    Private Sub InitializeInvitationUsers()
        'Inizializzo user control inviti
        Dim users As IEnumerable(Of DeskRoleUser) = CurrentDesk.DeskRoleUsers
        uscInvitationUser.BindUser(users, True)
    End Sub

    Private Sub InitializeDocuments()
        'Inizializzo user control documenti
        Dim documentDtos As IList(Of DeskDocumentResult) = New List(Of DeskDocumentResult)
        For Each deskDocument As DeskDocument In CurrentDesk.DeskDocuments.Where(Function(x) x.IsActive = 0)
            Dim docInfos As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(deskDocument.IdDocument.Value)
            If Not docInfos.Any() Then
                Exit Sub
            End If
            Dim docInfo As BiblosDocumentInfo = docInfos.OrderByDescending(Function(f) f.Version).First()
            Dim dto As DeskDocumentResult = CurrentDeskDocumentFacade.CreateDeskDocumentDto(deskDocument, docInfo, True)

            documentDtos.Add(dto)
        Next

        uscDeskDocument.BindDeskDocuments(documentDtos, False)
    End Sub

    Private Sub InitializeStoryBoards()
        Dim baseFinder As DeskStoryBoardFinder = New DeskStoryBoardFinder(New MapperDeskComment())
        baseFinder.DeskId = CurrentDeskId.Value
        If ProtocolEnv.GenericStoryBoardMaxDay > 0 Then
            baseFinder.DateFrom = DateTime.Today.AddDays(-ProtocolEnv.GenericStoryBoardMaxDay)
        End If
        baseFinder.FindDocumentComments = False
        baseFinder.SortExpressions.Add(New SortExpression(Of DeskStoryBoard)() With {.Direction = WebControls.SortDirection.Descending, .Expression = Function(x) x.DateBoard})

        uscDeskStoryBoard.StoryBoardFinder = baseFinder
        uscDeskStoryBoard.BindStoryBoardSource()
    End Sub

    Private Sub InitializeUserControls()
        InitializeInvitationUsers()
        InitializeDocuments()
        InitializeStoryBoards()
    End Sub

    Private Sub BindDeskFromPage()
        If Not CurrentDesk.Description.Eq(txtObject.Text) Then
            CurrentDesk.Description = txtObject.Text
        End If

        If CurrentDesk.ExpirationDate.HasValue AndAlso Not CurrentDesk.ExpirationDate.Value.Date.Equals(dtpDeskExpired.SelectedDate) AndAlso dtpDeskExpired.SelectedDate IsNot Nothing Then
            CurrentDesk.ExpirationDate = dtpDeskExpired.SelectedDate
        End If

        Try
            UpdateInvitedFromPage()
            UpdateDocumentsFromPage()

            CurrentDeskFacade.Update(CurrentDesk)
            Dim userAdded As IList(Of DeskRoleUserResult) = uscInvitationUser.GetAddedUsers()
            If userAdded.Count > 0 Then
                For Each userResult As DeskRoleUserResult In userAdded
                    SendInvitedMessage(CurrentDesk, userResult)
                Next
            End If

        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            AjaxAlert(String.Format("Errore in modifica tavolo: {0}", ex.Message))
            Exit Sub
        End Try
    End Sub

    Private Sub UpdateDocumentsFromPage()
        Dim documents As IList(Of DocumentInfo) = uscDeskDocument.GetAddedDocuments()
        CurrentDeskDocumentFacade.AddNewDeskDocuments(CurrentDesk, documents, CurrentDesk.Container.DeskLocation)
    End Sub

    Private Sub UpdateInvitedFromPage()
        Dim users As IList(Of DeskRoleUserResult) = uscInvitationUser.GetAddedUsers()
        Dim userChanged As IList(Of DeskRoleUserResult) = uscInvitationUser.GetChangedUsers()

        'Rimuovo eventuali utenti
        If uscInvitationUser.RemovedUser.Count > 0 Then
            CurrentDeskRoleUserFacade.RemoveUsers(uscInvitationUser.RemovedUser, CurrentDesk)
        End If

        'Inserisco nuovi invitati
        If users.Count > 0 Then
            CurrentDeskRoleUserFacade.AddUsers(users, CurrentDesk)
        End If

        'Modifico eventuali utenti già presenti nel tavolo
        If userChanged.Count > 0 Then
            CurrentDeskRoleUserFacade.UpdateUsers(userChanged, CurrentDesk)
        End If
    End Sub


#End Region

End Class