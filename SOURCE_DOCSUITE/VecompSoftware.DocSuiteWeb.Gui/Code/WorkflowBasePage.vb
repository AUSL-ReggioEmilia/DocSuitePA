Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Workflows
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Workflows

Public Class WorkflowBasePage
    Inherits CommonBasePage

#Region "Fields"
    ''' <summary><see cref="CommonBasePage.Type"/> da usare come default</summary>
    Public Const DefaultType As String = "Workflow"
    Private _facade As FacadeFactory
    Private _currentWorkflowInstanceId As Guid?
    Private _currentWorkflowInstance As WorkflowInstance
    Private _currentWorkflowActivity As WorkflowActivity
    Private _currentWorkflowActivityId As Guid?
    Private _currentFinderQueryString As String
    Private _workflowInstanceFacade As WorkflowInstanceFacade
    Private _workflowActivityFacade As WorkflowActivityFacade

#End Region

#Region "Properties"
    Protected Overridable ReadOnly Property CurrentWorkflowInstanceFacade As WorkflowInstanceFacade
        Get
            If _workflowInstanceFacade Is Nothing Then
                _workflowInstanceFacade = New WorkflowInstanceFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _workflowInstanceFacade
        End Get
    End Property


    Protected Overridable ReadOnly Property CurrentWorkflowActivityFacade As WorkflowActivityFacade
        Get
            If _workflowActivityFacade Is Nothing Then
                _workflowActivityFacade = New WorkflowActivityFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _workflowActivityFacade
        End Get
    End Property

    Public Overrides ReadOnly Property Facade As FacadeFactory
        Get
            If _facade Is Nothing Then
                _facade = New FacadeFactory("ProtDB")
            End If
            Return _facade
        End Get
    End Property


    Protected ReadOnly Property CurrentWorkflowInstanceId As Guid?
        Get
            If Not _currentWorkflowInstanceId.HasValue Then
                _currentWorkflowInstanceId = GetKeyValue(Of Guid?)("WorkflowInstanceId")
            End If
            Return _currentWorkflowInstanceId
        End Get
    End Property

    Protected ReadOnly Property CurrentFinderQueryString As String
        Get
            If String.IsNullOrEmpty(_currentFinderQueryString) Then
                _currentFinderQueryString = GetKeyValue(Of String)("Finder")
            End If
            Return _currentFinderQueryString
        End Get
    End Property

    Protected ReadOnly Property CurrentWorkflowInstance As WorkflowInstance
        Get
            If _currentWorkflowInstance Is Nothing AndAlso CurrentWorkflowInstanceId.HasValue Then
                _currentWorkflowInstance = CurrentWorkflowInstanceFacade.GetById(CurrentWorkflowInstanceId.Value)
            End If
            Return _currentWorkflowInstance
        End Get
    End Property

    Protected ReadOnly Property CurrentWorkflowActivityId As Guid?
        Get
            If Not _currentWorkflowActivityId.HasValue Then
                _currentWorkflowActivityId = GetKeyValue(Of Guid?)("WorkflowActivityId")
            End If
            Return _currentWorkflowActivityId
        End Get
    End Property

    Protected ReadOnly Property CurrentWorkflowActivity As WorkflowActivity
        Get
            If _currentWorkflowActivity Is Nothing AndAlso CurrentWorkflowActivityId.HasValue Then
                _currentWorkflowActivity = CurrentWorkflowActivityFacade.GetById(CurrentWorkflowActivityId.Value)
            End If
            Return _currentWorkflowActivity
        End Get
    End Property


#End Region

#Region "Constructor"
    Public Sub New()

    End Sub
#End Region

#Region "Methods"

    Public Overloads Function GetKeyValue(Of T)(key As String) As T
        Return GetKeyValue(Of T, WorkflowBasePage)(key)
    End Function



#End Region
End Class
