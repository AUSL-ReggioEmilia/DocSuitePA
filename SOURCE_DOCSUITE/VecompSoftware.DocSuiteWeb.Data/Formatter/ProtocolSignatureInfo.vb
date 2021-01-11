Namespace Formatter
    Public Class ProtocolSignatureInfo

#Region " Properties "

        Public DocumentType As ProtocolDocumentType
        Public AttachmentsCount As Integer?
        Public DocumentNumber As Integer?

#End Region

#Region " Constructors "

        Public Sub New()
            DocumentType = ProtocolDocumentType.None
        End Sub
        Public Sub New(attachmentCount As Integer)
            DocumentType = ProtocolDocumentType.None
            AttachmentsCount = attachmentCount
        End Sub

#End Region

    End Class
End NameSpace