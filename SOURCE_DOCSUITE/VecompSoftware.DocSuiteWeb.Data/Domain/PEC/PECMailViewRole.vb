<Serializable()>
Public Class PECMailViewRole
    Inherits DomainObject(Of Int32)

#Region " Fields "
#End Region

#Region " Properties "
    Public Overridable Property Role() As Role
    Public Overridable Property PECMailView() As PECMailView
#End Region

#Region " Constructor "
    Public Sub New()

    End Sub
#End Region

End Class