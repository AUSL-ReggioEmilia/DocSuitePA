Public Class ResolutionActivityDocumentModel

#Region " Constructor "

    Public Sub New()
        Ids = New List(Of Guid)
    End Sub

#End Region

#Region " Properties "

    Public Property Ids As IList(Of Guid)

    Public Property IsPrivacy As Boolean

#End Region


End Class
