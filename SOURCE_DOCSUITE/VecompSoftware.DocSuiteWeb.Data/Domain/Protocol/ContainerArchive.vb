<Serializable()>
Public Class ContainerArchive
    Inherits DomainObject(Of Int32)

    Public Overridable Property Name As String
    Public Overridable Property Containers() As IList(Of Container)

End Class
