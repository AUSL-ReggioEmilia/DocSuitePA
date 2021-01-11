Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.Facade

Namespace Viewers
    Public Class CollaborationViewer
        Inherits CommBasePage
        Implements ISendMail

#Region " Fields "

        Private _versioningId As Guid
        Private _action As String
        Private _versioningIdentifiers As IList(Of Guid)
        Private _collaborationId As Integer?
        Private _collaborationIdentifiers As IList(Of Integer)
        Private _collaborationList As IList(Of Collaboration)

#End Region

#Region " Properties "

        ' Le seguenti proprietà pilotano la tipologia di visualizzazione.
        ' Lo schema di inizializzazione a cascata va dal particolare di uno specifico CollaborationVersioning fino a un elenco di Collaboration.

        ' versioningId => versioningIdentifiers => collaborationId => collaborationIdentifiers => collaborationlist
        ' versioningIdentifiers => collaborationIdentifiers => collaborationlist
        ' collaborationId => collaborationIdentifiers => collaborationlist
        ' collaborationIdentifiers => collaborationlist

        ''' <summary> Richiesta di visualizzazione di un singolo CollaborationVersioning. </summary>
        Private ReadOnly Property VersioningId As Guid
            Get
                If _versioningId.IsEmpty Then
                    If ViewState("versioningid") Is Nothing Then
                        ' Valorizzo dalla querystring
                        Dim temp As String = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of String)("versioningid", Nothing)
                        If String.IsNullOrEmpty(temp) Then
                            Return Nothing
                        End If
                        Dim queryStringValue As String = Server.UrlDecode(temp)
                        _versioningId = JsonConvert.DeserializeObject(Of Guid)(queryStringValue)
                        ViewState("versioningid") = _versioningId
                    Else
                        _versioningId = DirectCast(ViewState("versioningid"), Guid)
                    End If
                End If
                Return _versioningId
            End Get
        End Property

        ''' <summary> Richiesta di visualizzazione di un elenco di CollaborationVersioning. </summary>
        Private ReadOnly Property VersioningIdentifiers As IList(Of Guid)
            Get
                If _versioningIdentifiers Is Nothing Then
                    If Not VersioningId.IsEmpty Then
                        ' Valorizzo dalla CollaborationVersioning singola
                        _versioningIdentifiers = New List(Of Guid) From {VersioningId}
                    ElseIf ViewState("versioningids") Is Nothing Then
                        ' Valorizzo dalla querystring
                        Dim temp As String = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of String)("versioningids", Nothing)
                        If String.IsNullOrEmpty(temp) Then
                            Return Nothing
                        End If
                        Dim queryStringValue As String = Server.UrlDecode(temp)
                        _versioningIdentifiers = JsonConvert.DeserializeObject(Of IList(Of Guid))(queryStringValue)
                    Else
                        ' Valorizzo dal viewstate
                        _versioningIdentifiers = DirectCast(ViewState("versioningids"), IList(Of Guid))
                    End If
                    ViewState("versioningids") = _versioningIdentifiers
                End If
                Return _versioningIdentifiers
            End Get
        End Property

        ''' <summary> Richiesta di visualizzazione di tutte le CollaborationVersioning di una singola Collaboration. </summary>
        Private ReadOnly Property CollaborationId As Integer?
            Get
                If Not _collaborationId.HasValue Then
                    If Not VersioningId.IsEmpty Then
                        _collaborationId = Facade.CollaborationVersioningFacade.GetById(VersioningId).Collaboration.Id
                    ElseIf ViewState("collaborationid") Is Nothing Then
                        ' Valorizzo dalla querystring
                        _collaborationId = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Integer?)("id", Nothing)
                    Else
                        ' Valorizzo dal viewstate
                        _collaborationId = DirectCast(ViewState("collaborationid"), Integer)
                    End If

                    ViewState("collaborationid") = _collaborationId
                End If
                Return _collaborationId
            End Get
        End Property

        ''' <summary> Richiesta di visualizzazione di tutte le CollaborationVersioning per un elenco di Collaboration. </summary>
        Private ReadOnly Property CollaborationIdentifiers As IList(Of Integer)
            Get
                If _collaborationIdentifiers Is Nothing Then
                    Dim ids As List(Of Integer)
                    If VersioningIdentifiers IsNot Nothing Then
                        ' Valorizzo dai CollaborationVersioningCompositeKey
                        ids = (From item In VersioningIdentifiers Select Facade.CollaborationVersioningFacade.GetById(item).Collaboration.Id).ToList()
                    ElseIf ViewState("collaborationids") Is Nothing Then
                        ' Valorizzo dalla querystring
                        Dim temp As String = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of String)("ids", Nothing)
                        If String.IsNullOrEmpty(temp) Then
                            Return Nothing
                        End If
                        Dim queryStringValue As String = Server.UrlDecode(temp)
                        ids = JsonConvert.DeserializeObject(Of List(Of Integer))(queryStringValue)
                    Else
                        ' Valorizzo dal viewstate
                        ids = DirectCast(ViewState("collaborationids"), List(Of Integer))
                    End If
                    _collaborationIdentifiers = ids
                    ViewState("collaborationids") = ids
                End If
                Return _collaborationIdentifiers
            End Get
        End Property

        ''' <summary> Elenco delle collaborazioni coinvolte nella visualizzazione. </summary>
        Private ReadOnly Property CollaborationList As IList(Of Collaboration)
            Get
                If CollaborationId.HasValue Then
                    _collaborationList = New List(Of Collaboration) From {Facade.CollaborationFacade.GetById(CollaborationId.Value)}
                ElseIf Not CollaborationIdentifiers.IsNullOrEmpty() Then
                    _collaborationList = Facade.CollaborationFacade.GetListByIds(CollaborationIdentifiers)
                End If
                Return _collaborationList
            End Get
        End Property

        Public ReadOnly Property SenderDescription() As String Implements ISendMail.SenderDescription
            Get
                Return CommonInstance.UserDescription
            End Get
        End Property

        Public ReadOnly Property SenderEmail() As String Implements ISendMail.SenderEmail
            Get
                Return CommonInstance.UserMail
            End Get
        End Property

        Public ReadOnly Property Recipients() As IList(Of ContactDTO) Implements ISendMail.Recipients
            Get
                Return New List(Of ContactDTO)()
            End Get
        End Property

        Public ReadOnly Property Documents() As IList(Of DocumentInfo) Implements ISendMail.Documents
            Get
                Dim list As List(Of DocumentInfo) = ViewerLight.CheckedDocuments
                If list.IsNullOrEmpty() Then
                    For Each item As Collaboration In CollaborationList
                        list.AddRange(Facade.CollaborationVersioningFacade.GetLastVersionFlatList(item))
                    Next
                End If
                Return list
            End Get
        End Property

        Public ReadOnly Property Subject() As String Implements ISendMail.Subject
            Get
                Return String.Empty
            End Get
        End Property

        Public ReadOnly Property Body() As String Implements ISendMail.Body
            Get
                Return String.Empty
            End Get
        End Property

#End Region

#Region " Events "

        Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
            MasterDocSuite.TitleVisible = False

            If CollaborationList.IsNullOrEmpty() Then
                Throw New ArgumentNullException("Nessuna collaborazione specificata.")
            End If

            If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
                Dim dataSource As New List(Of DocumentInfo)
                For Each item As Collaboration In CollaborationList
                    dataSource.Add(Facade.CollaborationVersioningFacade.GetCollaborationViewerSource(item))
                Next
                LogView()
                SetSendButton()
                ViewerLight.DataSource = dataSource
            End If
            ViewerLight.AlwaysDocumentTreeOpen = ProtocolEnv.ViewLightAlwaysOpenPages.Contains("CollaborationViewer")
        End Sub

#End Region

#Region " Methods "
        Protected Sub LogView()
            Dim message As String = String.Empty
            Dim id As Integer
            Dim versionings As IList(Of CollaborationVersioning) = New List(Of CollaborationVersioning)
            For Each collaborationSelected As Collaboration In CollaborationList
                id = collaborationSelected.Id
                versionings = Facade.CollaborationVersioningFacade.GetByCollaboration(id)
                For Each versioning As CollaborationVersioning In versionings
                    message = String.Format("Visualizzato il documento ""{0}"" della collaborazione numero "" [{1}]", versioning.DocumentName, id.ToString("N"))
                    Facade.CollaborationLogFacade.Insert(collaborationSelected, Nothing, Nothing, Nothing, CollaborationLogType.CD, message)
                Next
            Next

        End Sub
        Protected Sub SetSendButton()
            If CollaborationId > 0 Then
                btnSend.PostBackUrl = $"~/MailSenders/CollaborationMailSender.aspx?Type=Prot&CollaborationId={CollaborationId}"
            End If
        End Sub

#End Region

    End Class
End Namespace