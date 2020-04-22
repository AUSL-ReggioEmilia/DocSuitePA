Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Desks
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Desks
Imports VecompSoftware.Services.Biblos.Models

Public Class DeskViewStoryBoard
    Inherits DeskBasePage

#Region "Fields"
    Private Const PAGE_TITLE As String = "Tavoli - Lavagna"
    Private _currentDocumentInfo As BiblosDocumentInfo
    Private _currentDeskDocumentLastVersion As DeskDocumentVersion
#End Region

#Region "Properties"
    Private ReadOnly Property CurrentDeskDocumentLastVersion As DeskDocumentVersion
        Get
            If _currentDeskDocumentLastVersion Is Nothing AndAlso CurrentDeskDocumentId IsNot Nothing AndAlso CurrentDeskDocumentId.HasValue Then
                _currentDeskDocumentLastVersion = CurrentDeskDocumentVersionFacade.GetLastVersionByIdDeskDocument(CurrentDeskDocumentId.Value)
            End If
            Return _currentDeskDocumentLastVersion
        End Get
    End Property
    Private ReadOnly Property CurrentDocumentInfo As BiblosDocumentInfo
        Get
            If _currentDocumentInfo Is Nothing AndAlso CurrentDeskDocument IsNot Nothing AndAlso CurrentDeskDocument.IdDocument.HasValue Then
                Dim docInfos As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(CurrentDesk.Container.DeskLocation.DocumentServer, CurrentDeskDocument.IdDocument.Value)
                If Not docInfos.Any() Then
                    Return Nothing
                End If
                _currentDocumentInfo = docInfos.OrderByDescending(Function(f) f.Version).First()
            End If
            Return _currentDocumentInfo
        End Get
    End Property

    Public Property BtnAddCommentVisible As Boolean
        Get
            Return btnAddComment.Visible
        End Get
        Set(value As Boolean)
            btnAddComment.Visible = value
        End Set
    End Property

    Public Property BtnAddUserVisible As Boolean
        Get
            Return btnAddUser.Visible
        End Get
        Set(value As Boolean)
            btnAddUser.Visible = value
        End Set
    End Property

    Public Property BtnApproveVisible As Boolean
        Get
            Return btnApprove.Visible
        End Get
        Set(value As Boolean)
            btnApprove.Visible = value
        End Set
    End Property

    Public Property BtnRefuseVisible As Boolean
        Get
            Return btnRefuse.Visible
        End Get
        Set(value As Boolean)
            btnRefuse.Visible = value
        End Set
    End Property

    Public Property BtnViewDocumentVisible As Boolean
        Get
            Return btnViewDocument.Visible
        End Get
        Set(value As Boolean)
            btnViewDocument.Visible = value
        End Set
    End Property
#End Region

#Region "Events"
    Protected Sub DeskViewStoryBoard_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument
        Select Case arg
            Case "refreshStoryBoards"
                BindStoryBoardComments()
        End Select
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub BtnApprove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnApprove.Click
        CurrentDeskDocumentEndorsmentFacade.AddApprove(CurrentDeskDocumentId, MeDeskRoleUser)
        BindStoryBoardComments()
        InitializeButtons()
    End Sub

    Protected Sub BtnRefuse_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRefuse.Click
        CurrentDeskDocumentEndorsmentFacade.AddRefuse(CurrentDeskDocumentId, MeDeskRoleUser)
        BindStoryBoardComments()
        InitializeButtons()
    End Sub

    Protected Sub BtnViewDocument_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnViewDocument.Click
        If Not CurrentDeskDocument.IdDocument.HasValue Then
            AjaxAlert("Errore in visualizzazione documento")
            FileLogger.Error(LoggerName, "Errore in visualizzazione documento")
            Exit Sub
        End If

        Dim docInfos As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(CurrentDesk.Container.DeskLocation.DocumentServer, CurrentDeskDocument.IdDocument.Value)
        If docInfos.Any() Then
            AjaxAlert("Nessun documento presente per la visualizzazione")
            FileLogger.Error(LoggerName, "Nessun documento presente per la visualizzazione")
        End If
        Dim url As String = ResolveUrl("~/viewers/DocumentInfoViewer.aspx?" & CommonShared.AppendSecurityCheck(docInfos.OrderByDescending(Function(f) f.Version).First().ToQueryString().AsEncodedQueryString()))
        Response.Redirect(url)
    End Sub

    Protected Sub BtnAddComment_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddComment.Click
        uscDeskStoryBoard.AddNewComment()
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf DeskViewStoryBoard_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDeskStoryBoard, uscDeskStoryBoard)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscDeskStoryBoard, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnApprove, uscDeskStoryBoard, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnApprove, pnlButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRefuse, uscDeskStoryBoard, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRefuse, pnlButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnViewDocument, uscDeskStoryBoard, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        Title = PAGE_TITLE
        If CurrentDesk Is Nothing Then
            Throw New DocSuiteException(String.Format("Tavolo con ID {0} Inesistente", CurrentDeskId))
        End If

        If Not CurrentDeskRigths.IsSummaryViewable Then
            Throw New DocSuiteException(String.Format("Tavolo ID {0}", CurrentDeskId), "Non è possibile visualizzare il Tavolo richiesto. Verificare se si dispone di sufficienti autorizzazioni.")
        End If

        'Inizializzo il controllo dei story board
        uscDeskStoryBoard.InitializeUserControlFinder()
        uscDeskStoryBoard.CurrentDeskDocumentId = CurrentDeskDocumentId
        uscDeskStoryBoard.CurrentDeskId = CurrentDeskId

        lblDocumentVersion.Text = String.Format("Versione {0:00}", CurrentDeskDocumentLastVersion.Version)

        InitializeButtons()
        BindSelectedDocument()
        BindStoryBoardComments()
    End Sub

    Private Sub InitializeButtons()
        BtnAddCommentVisible = Not CurrentDeskRigths.IsClosed AndAlso CurrentDeskRigths.IsSummaryViewable
        BtnApproveVisible = CurrentDeskRigths.CanApproveDocument AndAlso CurrentDeskDocumentEndorsmentFacade.IsAlreadyApproved(CurrentDeskDocumentId.Value, DocSuiteContext.Current.User.FullUserName)
        BtnRefuseVisible = CurrentDeskRigths.CanRefuseDocument AndAlso CurrentDeskDocumentEndorsmentFacade.IsAlreadyApproved(CurrentDeskDocumentId.Value, DocSuiteContext.Current.User.FullUserName)
        BtnAddUserVisible = False
        'todo: da gestire gli inviti nella lavagna documento
        'BtnAddUserVisible = CurrentDeskRigths.IsEditable
        BtnViewDocumentVisible = CurrentDeskRigths.IsDocumentsViewable
    End Sub

    Private Sub BindSelectedDocument()
        If CurrentDocumentInfo Is Nothing Then
            Throw New DocSuiteException(String.Format("Tavolo ID {0}", CurrentDeskId), "Nessun documento disponibile per l'ID passato")
        End If
        imgDocumentType.ImageUrl = ImagePath.FromFile(CurrentDocumentInfo.Name, False)
        lblDocumentName.Text = CurrentDocumentInfo.Name
    End Sub

    Private Sub BindStoryBoardComments()
        Dim baseFinder As DeskStoryBoardFinder = New DeskStoryBoardFinder(New MapperDeskComment())
        baseFinder.DeskId = CurrentDeskId.Value
        baseFinder.FindDocumentComments = True
        baseFinder.DeskDocumentId = CurrentDeskDocumentId.Value
        baseFinder.SortExpressions.Add(New SortExpression(Of DeskStoryBoard)() With {.Direction = SortDirection.Descending, .Expression = Function(x) x.DateBoard})

        uscDeskStoryBoard.StoryBoardFinder = baseFinder
        uscDeskStoryBoard.BindStoryBoardSource()
    End Sub

#End Region

End Class