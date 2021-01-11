<Serializable()>
Public Class PECMailViewDefault
    Inherits DomainObject(Of Int32)

#Region " Fields "
#End Region

#Region " Properties "
    Public Overridable Property IdRole() As Integer
    Public Overridable Property Role() As Role
    Public Overridable Property DefaultView() As PECMailView
#End Region

#Region " Constructor "
    Public Sub New()

    End Sub
#End Region

End Class