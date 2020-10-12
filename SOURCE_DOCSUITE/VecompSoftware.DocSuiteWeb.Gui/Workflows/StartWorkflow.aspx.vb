Imports VecompSoftware.DocSuiteWeb.Data

Public Class StartWorkflow
    Inherits CommonBasePage

#Region " Fields "
    Private _environment As DSWEnvironment?
    Private _roleProposerRequired As Boolean?
#End Region

    Protected ReadOnly Property Environment As DSWEnvironment
        Get
            Return GetKeyValue(Of DSWEnvironment)("DSWEnvironment")
        End Get
    End Property

    Public ReadOnly Property WindowsCallback As String
        Get
            Return GetKeyValue(Of String)("Callback")
        End Get
    End Property

    Public ReadOnly Property ShowOnlyNoInstanceWorkflows As Boolean
        Get
            Return GetKeyValueOrDefault(Of Boolean)("ShowOnlyNoInstanceWorkflows", False)
        End Get
    End Property

    Public ReadOnly Property ShowOnlyHasIsFascicleClosedRequired As Boolean
        Get
            Return GetKeyValueOrDefault(Of Boolean)("ShowOnlyHasIsFascicleClosedRequired", False)
        End Get
    End Property
#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region " Methods "

    Private Sub Initialize()
        uscWorkflowId.Environment = Environment
        uscWorkflowId.TenantName = CurrentTenant.TenantName
        uscWorkflowId.TenantId = CurrentTenant.UniqueId.ToString()
        uscWorkflowId.ShowOnlyNoInstanceWorkflows = ShowOnlyNoInstanceWorkflows
        uscWorkflowId.ShowOnlyHasIsFascicleClosedRequired = ShowOnlyHasIsFascicleClosedRequired
    End Sub
#End Region
End Class