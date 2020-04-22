Public Class PublicationResolutionDocumentModel

#Region " Constructor "

    Public Sub New()
        IdDocuments = New List(Of Guid)
    End Sub

#End Region

#Region " Properties "

    Public Property IdResolution As Integer

    Public Property IdDocuments As IList(Of Guid)

#End Region

End Class
