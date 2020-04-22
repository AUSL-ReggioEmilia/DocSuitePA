<Serializable()>
Public Class ParameterEnv
    Inherits AuditableDomainObject(Of String)


    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub

    Public Overridable Property Value As String

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Try
            Dim compareTo As ParameterEnv = DirectCast(obj, ParameterEnv)
            Return compareTo.Id = Me.Id
        Catch
            Return False
        End Try
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Id.GetHashCode()
    End Function

End Class
