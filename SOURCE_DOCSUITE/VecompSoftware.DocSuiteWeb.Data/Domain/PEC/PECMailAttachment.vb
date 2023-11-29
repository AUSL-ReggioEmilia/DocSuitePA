
<Serializable()>
Public Class PECMailAttachment
    Inherits AuditableDomainObject(Of Int32)

#Region " Fields "

#End Region

#Region " Properties "

    Public Overridable Property Mail() As PECMail

    Public Overridable Property AttachmentName() As String

    Public Overridable Property IsMain() As Boolean

    ''' <summary> Identificativo del Documento in Biblos. </summary>
    Public Overridable Property IDDocument As Guid

    ''' <summary> Rappresenta l'allegato da cui è stato estratto (ZIP, EML, et.). </summary>
    Public Overridable Property Parent As PECMailAttachment

    ''' <summary> Elenco degli allegati che sono estati estratti dal presente elemento. </summary>
    Public Overridable Property Children As IList(Of PECMailAttachment)

    ''' <summary> Grandezza dell'allegato PEC (corrisponde al size del BiblosDocumentInfo) </summary>
    Public Overridable Property Size As Long?

#End Region

#Region " Methods "
    Public Overrides Function Clone() As Object
        Dim clonedPecMailAttachment As PECMailAttachment = CType(MyBase.Clone(), PECMailAttachment)
        '' Rimuovo l'id per evitare sovrapposizione
        clonedPecMailAttachment.Id = Nothing
        '' Reset dei figli
        clonedPecMailAttachment.Children = Nothing
        Return clonedPecMailAttachment
    End Function

#End Region

#Region " Constructor "
    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub
#End Region

End Class
