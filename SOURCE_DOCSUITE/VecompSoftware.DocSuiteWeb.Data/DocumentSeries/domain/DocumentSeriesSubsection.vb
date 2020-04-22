
    <Serializable()> _
    Public Class DocumentSeriesSubsection
        Inherits DomainObject(Of Int32)

        Public Overridable Property DocumentSeries() As DocumentSeries
        Public Overridable Property Description() As String
        Public Overridable Property Notes() As String
        Public Overridable Property SortOrder() As Integer

    End Class