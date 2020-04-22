<Serializable()> _
Public Class RoleName
    Inherits DomainObject(Of Integer)


#Region " Fields "

#End Region


    Public Overridable Property Name As String

    Public Overridable Property FromDate As DateTime

    Public Overridable Property ToDate As DateTime?


    Public Overridable Property Role() As Role

End Class
