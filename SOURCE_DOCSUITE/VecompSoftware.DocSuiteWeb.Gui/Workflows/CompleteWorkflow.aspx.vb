Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class CompleteWorkflow
    Inherits CommonBasePage

#Region "Properties"

    Private ReadOnly Property CurrentWorkflowActivityId As String
        Get
            Dim wfaId As Guid = GetKeyValue(Of Guid)("IdWorkflowActivity")
            If wfaId = Guid.Empty Then
                Return String.Empty
            End If
            Return wfaId.ToString()
        End Get
    End Property
#End Region

#Region "Methods"
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub Initialize()
        uscCompleteWorkflowId.WorkflowActivityId = CurrentWorkflowActivityId
    End Sub

    Public Overloads Function GetKeyValue(Of T)(key As String) As T
        Return GetKeyValue(Of T, StartWorkflow)(key)
    End Function

    Public Overloads Function GetKeyValueOrDefault(Of T)(key As String, defaultValue As T) As T
        Return Context.Request.QueryString.GetValueOrDefault(key, defaultValue)
    End Function
#End Region

End Class