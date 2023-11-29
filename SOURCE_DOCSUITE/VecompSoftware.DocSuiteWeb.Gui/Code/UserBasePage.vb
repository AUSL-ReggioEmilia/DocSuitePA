Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations

Public Class UserBasePage
    Inherits CommonBasePage

#Region " Fields "

    Private _currentCollaboration As Collaboration
    Private _collaborationId As Integer?
    Private _currentCollaborationSign As CollaborationSign
    Private _lastVersionings As IList(Of CollaborationVersioning)
    Private _workflowOperation As Boolean?
    Private _idWorkflowActivity As Guid?
    Private _templateModels As List(Of TemplateAuthorizationModel) = Nothing
    Protected Const _versioningGuidPattern As String = "[a-fA-F\d]{8}-([a-fA-F\d]{4}-){3}[a-fA-F\d]{12}"


#End Region

#Region " Properties "

    Protected Property CollaborationId As Integer?
        Get
            If Not _collaborationId.HasValue Then
                If ViewState("idCollaboration") Is Nothing Then
                    _collaborationId = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Integer?)("idCollaboration", Nothing)
                    ViewState("idCollaboration") = _collaborationId
                Else
                    _collaborationId = CType(ViewState("idCollaboration"), Integer?)
                End If
            End If
            Return _collaborationId
        End Get
        Set(value As Integer?)
            If Not _collaborationId.HasValue Then
                ViewState("idCollaboration") = value
                _collaborationId = value
                _currentCollaboration = Nothing
                _currentCollaborationSign = Nothing
                _lastVersionings = Nothing
            End If
        End Set
    End Property

    Protected ReadOnly Property CurrentCollaboration As Collaboration
        Get
            If _currentCollaboration Is Nothing AndAlso CollaborationId.HasValue Then
                _currentCollaboration = Facade.CollaborationFacade.GetById(CollaborationId.Value)
            End If
            Return _currentCollaboration
        End Get
    End Property

    Protected ReadOnly Property CurrentCollaborationSign As CollaborationSign
        Get
            If _currentCollaborationSign Is Nothing Then
                Dim availableSigns As IList(Of CollaborationSign) = Facade.CollaborationSignsFacade.SearchFull(CurrentCollaboration.Id, True)
                _currentCollaborationSign = availableSigns.First()
            End If
            Return _currentCollaborationSign
        End Get
    End Property

    Protected Property LastVersionings As IList(Of CollaborationVersioning)
        Get
            If _lastVersionings Is Nothing AndAlso CurrentCollaboration IsNot Nothing Then
                _lastVersionings = Facade.CollaborationVersioningFacade.GetLastVersionings(CurrentCollaboration)
            End If
            Return _lastVersionings
        End Get
        Set(value As IList(Of CollaborationVersioning))
            _lastVersionings = value
        End Set
    End Property

    Protected Overloads ReadOnly Property IsWorkflowOperation As Boolean
        Get
            If Not _workflowOperation.HasValue Then
                If ViewState("IsWorkflowOperation") Is Nothing Then
                    _workflowOperation = Request.QueryString.GetValueOrDefault("IsWorkflowOperation", False)
                    If (Not _workflowOperation AndAlso CurrentCollaboration IsNot Nothing) Then
                        _workflowOperation = CurrentCollaboration.IdWorkflowInstance.HasValue
                    End If
                    ViewState("IsWorkflowOperation") = _workflowOperation
                Else
                    _workflowOperation = DirectCast(ViewState("IsWorkflowOperation"), Boolean)
                End If
            End If
            Return _workflowOperation.Value
        End Get
    End Property

    Protected Overloads ReadOnly Property CurrentIdWorkflowActivity As Guid
        Get
            If Not _idWorkflowActivity.HasValue Then
                If ViewState("CurrentIdWorkflowActivity") Is Nothing Then
                    _idWorkflowActivity = Request.QueryString.GetValueOrDefault(Of Guid)("IdWorkflowActivity", Guid.Empty)
                    ViewState("CurrentIdWorkflowActivity") = _idWorkflowActivity
                Else
                    _idWorkflowActivity = DirectCast(ViewState("CurrentIdWorkflowActivity"), Guid)
                End If
            End If
            Return _idWorkflowActivity.Value
        End Get
    End Property

    Protected Function HasCheckOut() As Boolean
        Return HasCheckOut(CurrentCollaboration)
    End Function

    Protected Function HasCheckOut(ByVal collaboration As Collaboration) As Boolean
        If collaboration IsNot Nothing AndAlso Facade.CollaborationVersioningFacade.HasCheckOut(collaboration) Then
            AjaxAlert(String.Format("Attenzione! Ci sono dei file estratti in modifica nella collaborazione {0}. Effettuare l'archiviazione o annullare per proseguire.", collaboration.Id))
            Return True
        End If
        Return False
    End Function

    Public ReadOnly Property TemplateModels As List(Of TemplateAuthorizationModel)
        Get
            If _templateModels Is Nothing Then
                _templateModels = JsonConvert.DeserializeObject(Of List(Of TemplateAuthorizationModel))(DocSuiteContext.Current.ProtocolEnv.TemplatesAuthorizations)
            End If
            Return _templateModels
        End Get
    End Property
#End Region

#Region " Methods "
    Protected Function WorkflowBuildApprovedModel(currentSignPosition As Integer, isApproved As Boolean, collaborationSignerModels As List(Of CollaborationSignerWorkflowModel)) As List(Of CollaborationSignerWorkflowModel)
        Dim approvedModel As CollaborationSignerWorkflowModel = New CollaborationSignerWorkflowModel With {
                        .UserName = DocSuiteContext.Current.User.FullUserName,
                        .HasApproved = isApproved,
                        .ExecuteDate = DateTimeOffset.UtcNow}

        If collaborationSignerModels.Count < currentSignPosition Then
            collaborationSignerModels.Add(approvedModel)
        End If
        collaborationSignerModels(currentSignPosition - 1) = approvedModel
        Return collaborationSignerModels
    End Function
#End Region

End Class
