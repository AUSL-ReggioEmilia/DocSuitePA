Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Desks
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Desks
Imports VecompSoftware.DocSuiteWeb.Gui.WebComponent.Grid
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Desks
Imports VecompSoftware.Services.Biblos.Models

Public Class uscDeskStoryBoard
    Inherits DocSuite2008BaseControl

#Region "Fields"
    Private _deskStoryBoardFacade As DeskStoryBoardFacade
    Private _storyBoardFinder As DeskStoryBoardFinder
    Private _genericStoryBoard As Boolean = False
    Private _currentDesk As Desk
    Private _currentDeskDocumentLastVersion As DeskDocumentVersion
    Private Const OPEN_WINDOW_SCRIPT As String = "return {0}_OpenWindow('{1}');"

#End Region

#Region "Properties"
    Protected Overridable ReadOnly Property CurrentDeskStoryBoardFacade As DeskStoryBoardFacade
        Get
            If _deskStoryBoardFacade Is Nothing Then
                _deskStoryBoardFacade = New DeskStoryBoardFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _deskStoryBoardFacade
        End Get
    End Property

    Public Property StoryBoardFinder As DeskStoryBoardFinder
        Get
            Return CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.DeskStoryBoardFinder), DeskStoryBoardFinder)
        End Get
        Set(value As DeskStoryBoardFinder)
            SessionSearchController.SaveSessionFinder(value, SessionSearchController.SessionFinderType.DeskStoryBoardFinder)
        End Set
    End Property

    Public Property GenericStoryBoard As Boolean
        Get
            Return _genericStoryBoard
        End Get
        Set(value As Boolean)
            _genericStoryBoard = value
        End Set
    End Property

    Public Property CurrentDeskId As Guid?
        Get
            If ViewState("StoryBoardDeskId") IsNot Nothing Then
                Return DirectCast(ViewState("StoryBoardDeskId"), Guid)
            End If
            Return Nothing
        End Get
        Set(ByVal value As Guid?)
            ViewState("StoryBoardDeskId") = value
        End Set
    End Property

    Private ReadOnly Property CurrentDesk As Desk
        Get
            If _currentDesk Is Nothing AndAlso CurrentDeskId.HasValue Then
                _currentDesk = New DeskFacade(DocSuiteContext.Current.User.FullUserName).GetById(CurrentDeskId.Value)
            End If
            Return _currentDesk
        End Get
    End Property

    Public Property CurrentDeskDocumentId As Guid?
        Get
            If ViewState("StoryBoardDeskDocumentId") IsNot Nothing Then
                Return DirectCast(ViewState("StoryBoardDeskDocumentId"), Guid)
            End If
            Return Nothing
        End Get
        Set(ByVal value As Guid?)
            ViewState("StoryBoardDeskDocumentId") = value
        End Set
    End Property

    Private ReadOnly Property CurrentDeskDocumentLastVersion As DeskDocumentVersion
        Get
            If _currentDeskDocumentLastVersion Is Nothing AndAlso CurrentDeskDocumentId.HasValue Then
                _currentDeskDocumentLastVersion = New DeskDocumentVersionFacade(DocSuiteContext.Current.User.FullUserName).GetLastVersionByIdDeskDocument(CurrentDeskDocumentId.Value)
            End If
            Return _currentDeskDocumentLastVersion
        End Get
    End Property

    Public ReadOnly Property AddCommentButton As RadToolBarButton
        Get
            Return DirectCast(storyBoardToolBar.FindButtonByCommandName("AddComment"), RadToolBarButton)
        End Get
    End Property

    Public ReadOnly Property ViewAllCommentsButton As RadToolBarButton
        Get
            Return DirectCast(storyBoardToolBar.FindButtonByCommandName("ViewAllComments"), RadToolBarButton)
        End Get
    End Property

    Public ReadOnly Property ViewDocCommentsButton As RadToolBarButton
        Get
            Return DirectCast(storyBoardToolBar.FindButtonByCommandName("ViewDocComments"), RadToolBarButton)
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub dgvStoryBoard_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles dgvStoryBoard.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim dto As DeskComment = DirectCast(e.Item.DataItem, DeskComment)

        With DirectCast(e.Item.FindControl("lblDeskCommentAuthor"), Label)
            .Text = dto.Author
        End With

        Dim lblDeskDocumentRef As Label = DirectCast(e.Item.FindControl("lblDeskDocumentRef"), Label)
        lblDeskDocumentRef.Visible = False
        If GenericStoryBoard AndAlso dto.IdDocument.HasValue Then
            lblDeskDocumentRef.Visible = True
            Dim docInfos As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(dto.IdDocument.Value)
            If Not docInfos.Any() Then
                Exit Sub
            End If
            Dim docInfo As BiblosDocumentInfo = docInfos.OrderByDescending(Function(f) f.Version).First()
            lblDeskDocumentRef.Text = String.Format("Documento - {0}", docInfo.Name)
        End If

        With DirectCast(e.Item.FindControl("lblDeskCommentAuthor"), Label)
            .Text = dto.Author
        End With

        If dto.CreationDate.HasValue Then
            With DirectCast(e.Item.FindControl("lblCommentDate"), Label)
                .Text = dto.CreationDate.Value.ToString("dd/MM/yyyy HH:mm:ss")
            End With
        End If

        With DirectCast(e.Item.FindControl("lblDeskComment"), Label)
            .Text = dto.Description
        End With
    End Sub

    Private Sub UscDeskStoryBoard_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument
        Select Case arg
            Case "AddComment"
                AddNewComment()
            Case "ViewDocComments"
                StoryBoardFinder.FindDocumentComments = ViewDocCommentsButton.Checked
                BindStoryBoardSource()
            Case "ViewAllComments"
                Dim fromDate As DateTime = Date.MinValue
                If ProtocolEnv.GenericStoryBoardMaxDay > 0 Then
                    fromDate = DateTime.Today.AddDays(-ProtocolEnv.GenericStoryBoardMaxDay)
                End If
                StoryBoardFinder.FindDocumentComments = ViewDocCommentsButton.Checked
                StoryBoardFinder.DateFrom = If(ViewAllCommentsButton.Checked, Nothing, fromDate)
                BindStoryBoardSource()
            Case "refreshStoryBoards"
                BindStoryBoardSource()
        End Select
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        'Inizializzo delegati per la griglia
        dgvStoryBoard = DelegateForGrid(Of DeskStoryBoard, DeskComment).Delegate(dgvStoryBoard)

        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub BtnSaveComment_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveComment.Click
        If String.IsNullOrEmpty(txtComment.Text) Then
            BasePage.AjaxAlert("Il campo Commento è obbligatorio")
            Exit Sub
        End If

        Dim lastVersion As DeskDocumentVersion = Nothing
        If Not GenericStoryBoard Then
            lastVersion = CurrentDeskDocumentLastVersion
        End If
        CurrentDeskStoryBoardFacade.AddCommentToStoryBoard(txtComment.Text, CurrentDesk, Nothing, lastVersion)

        AjaxManager.ResponseScripts.Add(String.Format("CloseWindowComment('{0}');", "refreshStoryBoards"))
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf UscDeskStoryBoard_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(dgvStoryBoard, dgvStoryBoard)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, dgvStoryBoard, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, windowAddCommentDocument)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSaveComment, pnlEditorWindow, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        trStoryBoardToolBar.Visible = False
        If GenericStoryBoard Then
            trStoryBoardToolBar.Visible = True
        End If

        AddCommentButton.ImageUrl = ImagePath.SmallAdd
        ViewAllCommentsButton.ImageUrl = "~/App_Themes/DocSuite2008/Images/search-transparent.png"
        ViewDocCommentsButton.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/comments.png"

        BindStoryBoardSource()
    End Sub

    ''' <summary>
    ''' Inizializzazione del finder in sessione per gestione visibilità
    ''' </summary>
    Public Sub InitializeUserControlFinder()
        StoryBoardFinder = Nothing
    End Sub

    Public Sub BindStoryBoardSource()
        If StoryBoardFinder Is Nothing Then
            StoryBoardFinder = New DeskStoryBoardFinder(New MapperDeskComment())
        End If

        dgvStoryBoard.Finder = StoryBoardFinder
        dgvStoryBoard.CurrentPageIndex = 0
        dgvStoryBoard.CustomPageIndex = 0

        dgvStoryBoard.DataBindFinder(Of DeskStoryBoard, DeskComment)()
    End Sub

    Public Sub AddNewComment()
        txtComment.Text = String.Empty
        AjaxManager.ResponseScripts.Add(String.Format(OPEN_WINDOW_SCRIPT, ID, windowAddCommentDocument.ClientID))
    End Sub
#End Region

End Class