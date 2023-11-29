Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.DTO.Desks
Imports VecompSoftware.Services.Logging
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.Entity.DocumentUnits

Public Class DeskInsert
    Inherits DeskBasePage

#Region "Fields"
    Private Const DESK_INSERT_TITLE As String = "Nome del tavolo"
    Private Const DESK_SUMMARY_PATH As String = "~/Desks/DeskSummary.aspx?Type=Desk&Action=View&DeskId={0}"
    Private _currentCollaborationId As Integer?
    Private _currentContainerId As Integer?
    Private _deskLocation As Location
#End Region

#Region "Properties"
    Private ReadOnly Property CurrentIdContainerSelected As Integer
        Get
            Dim idContainer As Integer
            If Integer.TryParse(ddlContainers.SelectedValue, idContainer) Then
                Return idContainer
            End If
            Return 0
        End Get
    End Property

    Public ReadOnly Property CurrentCollaborationId As Integer
        Get
            If Not _currentCollaborationId.HasValue Then
                _currentCollaborationId = Request.QueryString.GetValueOrDefault(Of Integer)("CollaborationId", -1)
            End If
            Return _currentCollaborationId.Value
        End Get
    End Property

    Public ReadOnly Property CurrentContainerId As Integer
        Get
            If Not _currentContainerId.HasValue Then
                _currentContainerId = Request.QueryString.GetValueOrDefault(Of Integer)("ContainerId", -1)
            End If
            Return _currentContainerId.Value
        End Get
    End Property

    Public ReadOnly Property CurrentDeskLocation As Location
        Get
            If _deskLocation Is Nothing Then
                _deskLocation = Facade.ContainerFacade.GetById(CurrentContainerId, False, "ProtDB").DeskLocation
            End If
            Return _deskLocation
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
        cvCompareData.ValueToCompare = DateTime.Today.ToShortDateString()
    End Sub

    Private Sub DeskInsert_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        'Vuota per implementazioni future
    End Sub

    Protected Sub BtnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        If (dtpDeskExpired.SelectedDate.HasValue AndAlso DateTime.Today > dtpDeskExpired.SelectedDate.Value) Then
            AjaxAlert("La data di scadenza deve essere maggiore della data odierna")
            Return
        End If
        Dim deskToInsert As Desk = BindDeskFromPage()
        Try
            CurrentDeskFacade.Save(deskToInsert)

            'Gestisco gli inviti
            SaveInvitedFromPage(deskToInsert)
            'Gestisco i documenti
            SaveDocumentsFromPage(deskToInsert)

            For Each userResult As DeskRoleUserResult In uscInvitationUser.GetUsers()
                SendInvitedMessage(deskToInsert, userResult)
            Next

            Response.Redirect(String.Format(DESK_SUMMARY_PATH, deskToInsert.Id))
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            AjaxAlert(String.Format("Errore in inserimento tavolo: {0}", ex.Message))
        End Try
    End Sub
#End Region

#Region "Methods"
    Public Sub Initialize()
        Title = DESK_INSERT_TITLE
        BindContainerControl()

        'Inizializzo lo user control degli inviti
        uscInvitationUser.InitializeUserControlSource()

        'Inizializzo lo user control dei documenti
        uscDeskDocument.InitializeUserControlSource()
        uscDeskDocument.BtnSelectTemplateVisible = GetControlTemplateDocumentVisibility(Entity.DocumentUnits.ChainType.MainChain)

        'Inizializzo lo user control dei documenti provenienti da collaboration
        InitializeDocumentFromCollaboration()
    End Sub

    Public Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf DeskInsert_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDeskDocument, uscDeskDocument)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscInvitationUser, uscInvitationUser)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, deskContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, btnSave, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub

    Private Sub BindContainerControl()
        Dim availableContainers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Desk, DeskRightPositions.Insert, True)
        If availableContainers.IsNullOrEmpty() Then
            Throw New DocSuiteException("Inserimento Tavoli", "Utente senza diritti di Inserimento.")
        End If

        ddlContainers.Enabled = True
        ddlContainers.Items.Clear()
        If availableContainers.Count.Equals(1) Then
            Dim itemText As String = availableContainers.Single().Name
            Dim value As String = availableContainers.Single().Id.ToString()
            ddlContainers.Items.Add(New DropDownListItem(itemText, value))
            ddlContainers.Enabled = False
        End If

        For Each container As Container In availableContainers
            Dim itemText As String = container.Name
            Dim value As String = container.Id.ToString()
            ddlContainers.Items.Add(New DropDownListItem(itemText, value))
        Next
    End Sub

    Private Function BindDeskFromPage() As Desk
        Dim deskItem As Desk = New Desk(DocSuiteContext.Current.User.FullUserName)
        deskItem.Name = txtDeskName.Text
        deskItem.Description = txtObject.Text
        deskItem.Container = Facade.ContainerFacade.GetById(CurrentIdContainerSelected)
        deskItem.ExpirationDate = dtpDeskExpired.SelectedDate

        Return deskItem
    End Function

    Private Sub SaveInvitedFromPage(ByRef desk As Desk)
        Dim users As IList(Of DeskRoleUserResult) = uscInvitationUser.GetUsers()

        'Inserisco l'utente owner del tavolo
        users.Add(New DeskRoleUserResult() With {.UserName = DocSuiteContext.Current.User.FullUserName, .PermissionType = DeskPermissionType.Admin})

        'Inserisco nuovi invitati
        If users.Count > 0 Then
            CurrentDeskRoleUserFacade.AddUsers(users, desk)
        End If
    End Sub

    Private Sub SaveDocumentsFromPage(ByRef desk As Desk)
        Dim documents As IList(Of DocumentInfo) = uscDeskDocument.GetDocuments()
        Dim deskLocation As Location = Facade.ContainerFacade.GetById(CurrentIdContainerSelected, False, "ProtDB").DeskLocation

        Dim documentsVersion As Decimal = If(uscDeskDocument.DeskDocumentDataSourceExternal.FirstOrDefault().LastVersion.HasValue(), uscDeskDocument.DeskDocumentDataSourceExternal.FirstOrDefault().LastVersion.Value, Nothing)

        MyBase.CurrentDeskDocumentFacade.AddNewDeskDocuments(desk, documents, deskLocation, documentsVersion)
    End Sub

    ''' <summary>
    ''' Inizializzo lo user control dei documenti con quelli di collaborazione
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeDocumentFromCollaboration()
        Dim desk As Desk = New Desk(DocSuiteContext.Current.User.FullUserName)
        If CurrentCollaborationId = -1 Then
            Exit Sub
        Else
            Dim collaboration As Collaboration = Facade.CollaborationFacade.GetById(CurrentCollaborationId)
            desk = CurrentDeskFacade.InsertDocumentNotSignedFromCollaboration(Facade.CollaborationVersioningFacade.GetLastVersionDocuments(collaboration), desk, CurrentDeskLocation)
        End If

        Dim documentDtos As IList(Of DeskDocumentResult) = New List(Of DeskDocumentResult)
        For Each deskDocument As DeskDocument In desk.DeskDocuments.Where(Function(x) x.IsActive)
            Dim docInfos As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(deskDocument.IdDocument.Value)
            If Not docInfos.Any() Then
                Exit Sub
            End If
            Dim docInfo As BiblosDocumentInfo = docInfos.OrderByDescending(Function(f) f.Version).First()
            Dim dto As DeskDocumentResult = CurrentDeskDocumentFacade.CreateDeskDocumentDto(deskDocument, docInfo, CurrentDeskLocation)

            documentDtos.Add(dto)
        Next

        uscDeskDocument.BindDeskDocuments(documentDtos, False)
    End Sub

    Private Sub UscDeskDocument_AddDocumentVersioning(sender As Object, e As DocumentEventArgs) Handles uscDeskDocument.DocumentVersioning
        uscDeskDocument.NewDocumentsVersioningControl()

    End Sub

    Private Sub UscDeskDocument_AddDocument(sender As Object, e As DocumentEventArgs) Handles uscDeskDocument.DocumentUploaded
        For Each doc As DeskDocumentResult In uscDeskDocument.DeskDocumentDataSourceExternal
            doc.LastVersion = e.Version
        Next

        uscDeskDocument.BindDeskDocuments(uscDeskDocument.DeskDocumentDataSourceExternal, False)
    End Sub

#End Region
End Class