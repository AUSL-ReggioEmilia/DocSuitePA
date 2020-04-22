Imports System.Runtime.CompilerServices
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models

Namespace ExtensionMethods

    Public Module PecMailAttachmentEx

        ''' <summary>
        ''' Ritorna la grandezza (se già calcolata) o alternativamente la calcola
        ''' di uno specifico allegato PEC
        ''' </summary>
        ''' <param name="pecAttachment"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension()>
        Public Function GetSize(ByRef pecAttachment As PECMailAttachment) As Long
            '' Se esiste già un valore ed è valido allora lo uso
            If pecAttachment.Size.HasValue AndAlso pecAttachment.Size.Value > 0 Then
                Return pecAttachment.Size.Value
            End If

            '' Altrimenti lo ricalcolo
            If pecAttachment.IDDocument <> Guid.Empty Then
                Dim attachment As New BiblosDocumentInfo(pecAttachment.Mail.Location.DocumentServer, pecAttachment.IDDocument)
                pecAttachment.Size = attachment.Size
                Return pecAttachment.Size.Value
            End If
            Return -1
        End Function

        ''' <summary>
        ''' Ritorno direttamente il BiblosDocumentInfo relativo ad un particolare allegato
        ''' </summary>
        ''' <param name="pecAttachment"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension()>
        Public Function Document(ByRef pecAttachment As PECMailAttachment) As BiblosDocumentInfo
            Return New BiblosDocumentInfo(pecAttachment.Mail.Location.DocumentServer, pecAttachment.IDDocument)
        End Function

        ''' <summary>
        ''' Trasforma una lista di allegati in una lista di DocumentInfo
        ''' </summary>
        ''' <param name="pecAttachments"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension()>
        Public Function ToDocumentInfoList(ByRef pecAttachments As IList(Of PECMailAttachment)) As IList(Of DocumentInfo)
            Dim documents As New List(Of DocumentInfo)
            For Each currentAttachment As PECMailAttachment In pecAttachments
                Dim bdi As New BiblosDocumentInfo(currentAttachment.Mail.Location.DocumentServer, currentAttachment.IDDocument)
                bdi.Caption = currentAttachment.AttachmentName
                documents.Add(bdi)
            Next
            Return documents
        End Function
    End Module

End Namespace