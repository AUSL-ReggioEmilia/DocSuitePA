Public Class TaskParameter
    Inherits DomainObject(Of Integer)

    Public Sub New()
    End Sub

    Public Sub New(parameterKey As String, parameterValue As String)
        Me.ParameterKey = parameterKey
        Me.Value = parameterValue
    End Sub

    Public Overridable Property TaskHeader As TaskHeader

    Public Overridable Property ParameterKey As String

    Public Overridable Property Value As String

End Class
