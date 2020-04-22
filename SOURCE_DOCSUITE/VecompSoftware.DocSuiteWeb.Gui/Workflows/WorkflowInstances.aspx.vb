Imports VecompSoftware.DocSuiteWeb.Entity.Workflows
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Public Class WorkflowInstances
    Inherits CommonBasePage

#Region " Fields "

#End Region

#Region " Properties "
    Protected ReadOnly Property WorkflowRepositoryName As String
        Get
            Return GetKeyValue(Of String)("WorkflowRepositoryName")
        End Get
    End Property

    Protected ReadOnly Property WorkflowRepositoryStatus As String
        Get
            Return GetKeyValue(Of String)("WorkflowRepositoryStatus")
        End Get
    End Property
#End Region

#Region "Events"
#End Region

#Region "Methods"
#End Region

End Class