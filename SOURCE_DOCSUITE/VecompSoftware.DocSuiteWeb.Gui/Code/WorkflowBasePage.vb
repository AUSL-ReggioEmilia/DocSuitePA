Imports VecompSoftware.DocSuiteWeb.Facade

Public Class WorkflowBasePage
    Inherits CommonBasePage

#Region "Fields"
    ''' <summary><see cref="CommonBasePage.Type"/> da usare come default</summary>
    Public Const DefaultType As String = "Workflow"
    Private _facade As FacadeFactory

#End Region

#Region "Properties"

    Public Overrides ReadOnly Property Facade As FacadeFactory
        Get
            If _facade Is Nothing Then
                _facade = New FacadeFactory("ProtDB")
            End If
            Return _facade
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
