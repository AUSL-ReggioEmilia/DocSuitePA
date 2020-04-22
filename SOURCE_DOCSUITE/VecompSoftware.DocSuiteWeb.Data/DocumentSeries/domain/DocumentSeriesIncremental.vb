<Serializable()> _
Public Class DocumentSeriesIncremental
    Inherits DomainObject(Of Int32)

    Public Overridable Property DocumentSeries() As DocumentSeries
    Public Overridable Property Year() As Integer?
    Public Overridable Property LastUsedNumber() As Integer?
    Public Overridable Property IsOpen As Boolean?
End Class
