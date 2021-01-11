<Serializable()> _
Public Class DocumentSeriesFamily
    Inherits DomainObject(Of Int32)

    Public Overridable Property Name() As String
    Public Overridable Property Description() As String
    Public Overridable Property SortOrder() As Integer?
    Public Overridable Property DocumentSeries As IList(Of DocumentSeries)
End Class
