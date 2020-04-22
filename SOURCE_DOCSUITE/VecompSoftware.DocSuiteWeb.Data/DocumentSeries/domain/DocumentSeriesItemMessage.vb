Public Class DocumentSeriesItemMessage
    Inherits DomainObject(Of Integer)


#Region " Properties "
    Public Overridable Property DocumentSeriesItem As DocumentSeriesItem
    Public Overridable Property Message As DSWMessage
    Public Overridable Property UniqueIdDocumentSeriesItem As Guid

#End Region

#Region " Constructor "

    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub

    Public Sub New(ByRef documentSeriesItem As DocumentSeriesItem, ByRef message As DSWMessage)
        Me.DocumentSeriesItem = documentSeriesItem
        UniqueIdDocumentSeriesItem = documentSeriesItem.UniqueId
        Me.Message = message
        UniqueId = Guid.NewGuid()
    End Sub

#End Region
End Class
