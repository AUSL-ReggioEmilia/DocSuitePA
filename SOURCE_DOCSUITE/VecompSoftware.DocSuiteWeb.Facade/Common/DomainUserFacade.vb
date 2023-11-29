Imports System.Linq
Imports System.Reflection

Public Class DomainUserFacade
    Public Const HasFascicleResponsibleRole As String = "HasFascicleResponsibleRole"
    Public Const HasInsertable As String = "HasInsertable"
    Public Const HasFascicleSecretaryRole As String = "HasFascicleSecretaryRole"
    Public Const HasSecretaryRole As String = "HasSecretaryRole"
    Public Const HasSignerRole As String = "HasSignerRole"
    Public Const HasPECSendableRight As String = "HasPECSendableRight"

    Private Function GetPropValue(ByVal obj As Object, ByVal name As String) As Object
        For Each part As String In name.Split("."c)
            If obj Is Nothing Then
                Return Nothing
            End If
            Dim type As Type = obj.[GetType]()
            Dim info As PropertyInfo = type.GetProperty(part)
            If info Is Nothing Then
                Return Nothing
            End If
            obj = info.GetValue(obj, Nothing)
        Next
        Return obj
    End Function
    Private Function GetPropValue(Of T)(ByVal obj As Object, ByVal name As String) As T
        Dim retval As Object = GetPropValue(obj, name)
        Dim ret As T
        If retval Is Nothing Then
            Return ret
        End If
        Return CType(retval, T)
    End Function

    Public Function HasCurrentRight(CurrentDomainUser As Model.Securities.DomainUserModel, env As Model.Entities.Commons.DSWEnvironmentType, thisProperty As String) As Boolean
        Dim value As Boolean = GetPropValue(Of Boolean)(CurrentDomainUser.Rights.Where(Function(x) x.Environment = env).FirstOrDefault(), thisProperty)
        Return value
    End Function
End Class
