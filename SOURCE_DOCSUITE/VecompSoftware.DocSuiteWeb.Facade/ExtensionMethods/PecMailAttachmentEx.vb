Imports System.Runtime.CompilerServices
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.DTO.WorkflowsElsa
Imports System.Linq

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
                Dim useDocumentProxy As Boolean = pecAttachment.Mail.ProcessStatus = PECMailProcessStatus.StoredInDocumentManager OrElse pecAttachment.Mail.ProcessStatus = PECMailProcessStatus.ArchivedInDocSuiteNext
                If useDocumentProxy Then
                    Dim proxyDocumentInfo As DocumentProxyDocumentInfo = Nothing
                    If Not String.IsNullOrEmpty(pecAttachment.Mail.MailContent) Then
                        Dim idDocument As Guid = pecAttachment.IDDocument
                        Dim docs As List(Of DocumentInfoModel) = JsonConvert.DeserializeObject(Of List(Of DocumentInfoModel))(pecAttachment.Mail.MailContent)
                        Dim documentInfoModel As DocumentInfoModel = docs.Single(Function(f) f.DocumentId = idDocument)
                        proxyDocumentInfo = New DocumentProxyDocumentInfo(pecAttachment.Mail.UniqueId, documentInfoModel.DocumentId, DSWEnvironment.PECMail, documentInfoModel.Filename, documentInfoModel.FileExtension, documentInfoModel.Size, documentInfoModel.ReferenceType, documentInfoModel.VirtualPath)
                    Else
                        proxyDocumentInfo = New DocumentProxyDocumentInfo(pecAttachment.Mail.UniqueId, pecAttachment.IDDocument, DSWEnvironment.PECMail)
                    End If
                    pecAttachment.Size = proxyDocumentInfo.Size
                End If
                Dim attachment As New BiblosDocumentInfo(pecAttachment.IDDocument)
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
            Return New BiblosDocumentInfo(pecAttachment.IDDocument)
        End Function

        ''' <summary>
        ''' Trasforma una lista di allegati in una lista di DocumentInfo
        ''' </summary>
        ''' <param name="pecAttachments"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension()>
        Public Function ToDocumentInfoList(ByRef pecAttachments As IList(Of PECMailAttachment), action As Action(Of PECMail)) As IList(Of DocumentInfo)
            Dim documents As New List(Of DocumentInfo)
            For Each currentAttachment As PECMailAttachment In pecAttachments
                Dim useDocumentProxy As Boolean = currentAttachment.Mail.ProcessStatus = PECMailProcessStatus.StoredInDocumentManager OrElse currentAttachment.Mail.ProcessStatus = PECMailProcessStatus.ArchivedInDocSuiteNext
                Dim bdi As DocumentInfo = Nothing
                If useDocumentProxy Then
                    If Not String.IsNullOrEmpty(currentAttachment.Mail.MailContent) Then
                        Dim idDocument As Guid = currentAttachment.IDDocument
                        Dim docs As List(Of DocumentInfoModel) = JsonConvert.DeserializeObject(Of List(Of DocumentInfoModel))(currentAttachment.Mail.MailContent)
                        Dim documentInfoModel As DocumentInfoModel = docs.Single(Function(f) f.DocumentId = idDocument)
                        bdi = New DocumentProxyDocumentInfo(currentAttachment.Mail.UniqueId, documentInfoModel.DocumentId, DSWEnvironment.PECMail, documentInfoModel.Filename, documentInfoModel.FileExtension, documentInfoModel.Size, documentInfoModel.ReferenceType, documentInfoModel.VirtualPath) With {
                        .DelegateExternalInitializer = New Action(Sub() action(currentAttachment.Mail))
                    }
                    Else
                        bdi = New DocumentProxyDocumentInfo(currentAttachment.Mail.UniqueId, currentAttachment.IDDocument, DSWEnvironment.PECMail)
                    End If
                Else
                    bdi = New BiblosDocumentInfo(currentAttachment.IDDocument) With {
                        .Caption = currentAttachment.AttachmentName
                    }
                End If
                documents.Add(bdi)
            Next
            Return documents
        End Function
    End Module

End Namespace