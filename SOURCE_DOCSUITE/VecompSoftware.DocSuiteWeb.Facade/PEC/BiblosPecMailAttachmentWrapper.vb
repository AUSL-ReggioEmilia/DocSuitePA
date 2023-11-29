Imports System.Linq
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.WorkflowsElsa
Imports VecompSoftware.Services.Biblos.Models

Public Class BiblosPecMailAttachmentWrapper
    Private ReadOnly _pecAttachment As PECMailAttachment

    Private _biblosDocumentInfo As BiblosDocumentInfo
    Private _proxyDocumentInfo As DocumentProxyDocumentInfo

    Private _checkSignedEvaluateStream As Boolean
    Private _children As IList(Of BiblosPecMailAttachmentWrapper)
    Private _parent As BiblosPecMailAttachmentWrapper

    ''' <summary>
    ''' Costruttore di base, richiede un PECMailAttachment di riferimento e in modalità lazy calcola tutto il resto
    ''' </summary>
    ''' <param name="pecAttachment"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal pecAttachment As PECMailAttachment, checkSignedEvaluateStream As Boolean)
        _pecAttachment = pecAttachment
        _checkSignedEvaluateStream = checkSignedEvaluateStream
    End Sub

    Public ReadOnly Property Document As DocumentInfo
        Get
            Dim useDocumentProxy As Boolean = _pecAttachment.Mail.ProcessStatus = PECMailProcessStatus.StoredInDocumentManager OrElse _pecAttachment.Mail.ProcessStatus = PECMailProcessStatus.ArchivedInDocSuiteNext
            If useDocumentProxy = False Then
                If _biblosDocumentInfo Is Nothing Then
                    _biblosDocumentInfo = BiblosDocumentInfo.GetDocumentInfo(_pecAttachment.IDDocument, Nothing, True, True).SingleOrDefault()
                    If _biblosDocumentInfo IsNot Nothing AndAlso _biblosDocumentInfo.IsRemoved Then
                        _biblosDocumentInfo = New BiblosDeletedDocumentInfo(_pecAttachment.IDDocument)
                    End If
                    _biblosDocumentInfo.CheckSignedEvaluateStream = _checkSignedEvaluateStream
                End If
                Return _biblosDocumentInfo
            Else
                If _proxyDocumentInfo Is Nothing Then
                    If Not String.IsNullOrEmpty(_pecAttachment.Mail.MailContent) Then
                        Dim docs As List(Of DocumentInfoModel) = JsonConvert.DeserializeObject(Of List(Of DocumentInfoModel))(_pecAttachment.Mail.MailContent)
                        Dim documentInfoModel As DocumentInfoModel = docs.Single(Function(f) f.DocumentId = _pecAttachment.IDDocument)
                        _proxyDocumentInfo = New DocumentProxyDocumentInfo(_pecAttachment.Mail.UniqueId, documentInfoModel.DocumentId, DSWEnvironment.PECMail, documentInfoModel.Filename, documentInfoModel.FileExtension, documentInfoModel.Size, documentInfoModel.ReferenceType, documentInfoModel.VirtualPath)
                    Else
                        _proxyDocumentInfo = New DocumentProxyDocumentInfo(_pecAttachment.Mail.UniqueId, _pecAttachment.IDDocument, DSWEnvironment.PECMail)
                    End If
                End If
                Return _proxyDocumentInfo
            End If
        End Get
    End Property

    Public ReadOnly Property Children As IList(Of BiblosPecMailAttachmentWrapper)
        Get
            If _children Is Nothing Then
                _children = New List(Of BiblosPecMailAttachmentWrapper)
                If _pecAttachment.Children IsNot Nothing Then
                    For Each pecAttachment As PECMailAttachment In _pecAttachment.Children
                        _children.Add(New BiblosPecMailAttachmentWrapper(pecAttachment, _checkSignedEvaluateStream))
                    Next
                End If
            End If
            Return _children
        End Get
    End Property

    Public ReadOnly Property Parent As BiblosPecMailAttachmentWrapper
        Get
            If _parent Is Nothing AndAlso _pecAttachment.Parent IsNot Nothing Then
                _parent = New BiblosPecMailAttachmentWrapper(_pecAttachment.Parent, _checkSignedEvaluateStream)
            End If
            Return _parent
        End Get
    End Property

    Public Function IsGenericAttachment() As Boolean
        Return _pecAttachment.IDDocument <> _pecAttachment.Mail.IDSegnatura AndAlso
            _pecAttachment.IDDocument <> _pecAttachment.Mail.IDDaticert AndAlso
            _pecAttachment.IDDocument <> _pecAttachment.Mail.IDPostacert
    End Function
End Class
