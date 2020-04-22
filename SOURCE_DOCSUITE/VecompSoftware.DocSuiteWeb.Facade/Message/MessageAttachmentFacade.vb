Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models

<ComponentModel.DataObject()> _
Public Class MessageAttachmentFacade
    Inherits BaseProtocolFacade(Of MessageAttachment, Integer, NHibernateMessageAttachmentDAO)


    Public Function GetByMessage(message As DSWMessage) As IList(Of MessageAttachment)
        Return _dao.GetByMessage(message)
    End Function

    ' Interroga biblos il quale ritorna il documento richiesto. Viene forzato lo stream reader per verificare che il file sia non corrotto.
    Private Function GetAttachment(ByRef messageAttachment As MessageAttachment, ByRef doc As BiblosDocumentInfo, Optional ByVal forceStreamReader As Boolean = False) As BiblosDocumentInfo
        If "pdf".Eq(messageAttachment.Extension) Then
            Dim docpdf As BiblosPdfDocumentInfo = New BiblosPdfDocumentInfo(doc)
            If (forceStreamReader) Then
                'Messo solo come metodo formale di accesso allo stream per determinare se stampaconforme genera errore
                Dim lenght As Integer = docpdf.Stream.Length
            End If
            Return docpdf
        End If
        Return doc
    End Function

    ' Estraggo i documenti da allegare
    Public Function GetByMessageAsDocumentInfoList(message As DSWMessage) As IList(Of DocumentInfo)
        Dim attachments As New List(Of DocumentInfo)
        For Each messageAttachment As MessageAttachment In message.Attachments
            Dim chainId As Guid = Service.GetChainGuid(New UIDDocument(messageAttachment.Server, messageAttachment.Archive, messageAttachment.ChainId))
            Dim docs As ICollection(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocumentChildren(messageAttachment.Server, chainId, True)
            Dim tempAttachment As DocumentInfo = Nothing
            For Each doc As BiblosDocumentInfo In docs
                tempAttachment = If(doc.IsRemoved, New BiblosDeletedDocumentInfo(doc.Server, doc.DocumentId), GetAttachment(messageAttachment, doc))
                attachments.Add(tempAttachment)
            Next
        Next
        Return attachments
    End Function

    ' Se il documento allegato non viene stampato correttamente dalla stampa conforme, ritorniamo il documento originale
    Public Function GetByMessageAsDocumentInfoListForceStream(message As DSWMessage) As IList(Of DocumentInfo)
        Dim attachments As New List(Of DocumentInfo)
        For Each messageAttachment As MessageAttachment In message.Attachments
            Dim doc As BiblosDocumentInfo = New BiblosDocumentInfo(messageAttachment.Server, messageAttachment.Archive, messageAttachment.ChainId, messageAttachment.DocumentEnum.GetValueOrDefault(0))
            Try
                Dim tempAttachment As BiblosDocumentInfo = GetAttachment(messageAttachment, doc, True)
                attachments.Add(tempAttachment)
            Catch ex As Exception
                attachments.Add(doc)
            End Try
        Next
        Return attachments
    End Function

End Class
