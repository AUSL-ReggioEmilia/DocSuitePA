Imports System.IO
Imports System.Linq
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.WorkflowsElsa
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos.Models

Public Class BiblosPecMailWrapper

    Private ReadOnly _pec As PECMail
    Private _checkSignedEvaluateStream As Boolean
    Private _attachments As IList(Of BiblosPecMailAttachmentWrapper)
    Private _receipts As IList(Of BiblosPecMailReceiptWrapper)

    Private _postaCert As BiblosDocumentInfo
    Private _postaCertProxy As DocumentProxyDocumentInfo

    Private _envelope As BiblosDocumentInfo
    Private _envelopeProxy As DocumentProxyDocumentInfo

    Private _interopSignature As BiblosDocumentInfo
    Private _interopSignatureProxy As DocumentProxyDocumentInfo

    Private _datiCert As BiblosDocumentInfo
    Private _datiCertProxy As DocumentProxyDocumentInfo

    Private _mailContent As BiblosDocumentInfo
    Private _mailContentProxy As DocumentProxyDocumentInfo

    Private _mailBody As FileDocumentInfo

    Private _mailBox As PECMailBox

    ''' <summary>
    ''' Costruttore di base, richiede una PEC di riferimento e in modalità lazy calcola tutto il resto
    ''' </summary>
    ''' <param name="pec"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal pec As PECMail, checkSignedEvaluateStream As Boolean)
        _pec = pec
        _checkSignedEvaluateStream = checkSignedEvaluateStream
    End Sub

    Public ReadOnly Property Id As String
        Get
            Return _pec.Id.ToString
        End Get
    End Property

    Public ReadOnly Property Attachments As IList(Of BiblosPecMailAttachmentWrapper)
        Get
            If _attachments Is Nothing Then
                _attachments = New List(Of BiblosPecMailAttachmentWrapper)
                For Each pecMailAttachment As PECMailAttachment In _pec.Attachments
                    Dim attachmentWrapper As BiblosPecMailAttachmentWrapper = New BiblosPecMailAttachmentWrapper(pecMailAttachment, _checkSignedEvaluateStream)
                    _attachments.Add(attachmentWrapper)
                Next
            End If
            Return _attachments
        End Get
    End Property

    Public ReadOnly Property Receipts As IList(Of BiblosPecMailReceiptWrapper)
        Get
            If _receipts Is Nothing Then
                _receipts = New List(Of BiblosPecMailReceiptWrapper)
                For Each pecReceipt As PECMailReceipt In _pec.Receipts.OrderBy(Function(r) r.ReceiptDate)
                    _receipts.Add(New BiblosPecMailReceiptWrapper(pecReceipt, _checkSignedEvaluateStream))
                Next
            End If
            Return _receipts
        End Get
    End Property

    Public ReadOnly Property PostaCert As DocumentInfo
        Get
            If _pec.IDPostacert = Guid.Empty Then
                Return Nothing
            End If

            If _pec.ProcessStatus <> PECMailProcessStatus.StoredInDocumentManager Then
                If _postaCert Is Nothing Then
                    _postaCert = New BiblosDocumentInfo(_pec.IDPostacert)
                    If _postaCert IsNot Nothing Then
                        _postaCert.CheckSignedEvaluateStream = _checkSignedEvaluateStream
                    End If
                End If
                Return _postaCert
            End If
            If _pec.ProcessStatus = PECMailProcessStatus.StoredInDocumentManager Then
                If _postaCertProxy Is Nothing Then
                    If Not String.IsNullOrEmpty(_pec.MailContent) Then
                        Dim docs As List(Of DocumentInfoModel) = JsonConvert.DeserializeObject(Of List(Of DocumentInfoModel))(_pec.MailContent)
                        Dim documentInfoModel As DocumentInfoModel = docs.Single(Function(f) f.DocumentId = _pec.IDPostacert)
                        _postaCertProxy = New DocumentProxyDocumentInfo(_pec.UniqueId, documentInfoModel.DocumentId, DSWEnvironment.PECMail, documentInfoModel.Filename, documentInfoModel.FileExtension, documentInfoModel.Size, documentInfoModel.ReferenceType, documentInfoModel.VirtualPath)
                    Else
                        _postaCertProxy = New DocumentProxyDocumentInfo(_pec.UniqueId, _pec.IDPostacert, DSWEnvironment.PECMail)
                    End If
                End If
                Return _postaCertProxy
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property MailContent As DocumentInfo
        Get
            If _pec.IDMailContent = Guid.Empty Then
                Return Nothing
            End If

            If _pec.ProcessStatus <> PECMailProcessStatus.StoredInDocumentManager Then
                If _mailContent Is Nothing Then
                    _mailContent = FacadeFactory.Instance.PECMailFacade.GetPecMailContent(_pec)
                    If _mailContent IsNot Nothing Then
                        _mailContent.CheckSignedEvaluateStream = _checkSignedEvaluateStream
                    End If
                End If
                Return _mailContent
            End If
            If _pec.ProcessStatus = PECMailProcessStatus.StoredInDocumentManager Then
                If _mailContentProxy Is Nothing Then
                    If Not String.IsNullOrEmpty(_pec.MailContent) Then
                        Dim docs As List(Of DocumentInfoModel) = JsonConvert.DeserializeObject(Of List(Of DocumentInfoModel))(_pec.MailContent)
                        Dim documentInfoModel As DocumentInfoModel = docs.Single(Function(f) f.DocumentId = _pec.IDMailContent)
                        _mailContentProxy = New DocumentProxyDocumentInfo(_pec.UniqueId, documentInfoModel.DocumentId, DSWEnvironment.PECMail, documentInfoModel.Filename, documentInfoModel.FileExtension, documentInfoModel.Size, documentInfoModel.ReferenceType, documentInfoModel.VirtualPath)
                    Else
                        _mailContentProxy = New DocumentProxyDocumentInfo(_pec.UniqueId, _pec.IDMailContent, DSWEnvironment.PECMail)
                    End If
                End If
                Return _mailContentProxy
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property MailBody As FileDocumentInfo
        Get
            If _mailBody Is Nothing Then
                ' Salvo in TEMP un file di Testo con il corpo della MAIL
                Dim name As String = FileHelper.UniqueFileNameFormat("Corpo della mail.txt", DocSuiteContext.Current.User.UserName)
                Dim fullname As String = Path.Combine(CommonUtil.GetInstance().AppTempPath, name)
                File.WriteAllText(fullname, _pec.MailBody)
                _mailBody = New FileDocumentInfo(New FileInfo(fullname)) With {
                    .Name = "Corpo della mail.txt",
                    .CheckSignedEvaluateStream = _checkSignedEvaluateStream
                }
            End If
            Return _mailBody
        End Get
    End Property

    Public ReadOnly Property Envelope As DocumentInfo
        Get
            If _pec.IDEnvelope = Guid.Empty Then
                Return Nothing
            End If

            If _pec.ProcessStatus <> PECMailProcessStatus.StoredInDocumentManager Then
                If _envelope Is Nothing Then
                    _envelope = New BiblosDocumentInfo(_pec.IDEnvelope)
                    If _envelope IsNot Nothing Then
                        _envelope.CheckSignedEvaluateStream = _checkSignedEvaluateStream
                    End If

                End If
                Return _envelope
            End If
            If _pec.ProcessStatus = PECMailProcessStatus.StoredInDocumentManager Then
                If _envelopeProxy Is Nothing Then
                    If Not String.IsNullOrEmpty(_pec.MailContent) Then
                        Dim docs As List(Of DocumentInfoModel) = JsonConvert.DeserializeObject(Of List(Of DocumentInfoModel))(_pec.MailContent)
                        Dim documentInfoModel As DocumentInfoModel = docs.SingleOrDefault(Function(f) f.DocumentId = _pec.IDEnvelope)
                        _envelopeProxy = New DocumentProxyDocumentInfo(_pec.UniqueId, documentInfoModel.DocumentId, DSWEnvironment.PECMail, documentInfoModel.Filename, documentInfoModel.FileExtension, documentInfoModel.Size, documentInfoModel.ReferenceType, documentInfoModel.VirtualPath)
                    Else
                        _envelopeProxy = New DocumentProxyDocumentInfo(_pec.UniqueId, _pec.IDEnvelope, DSWEnvironment.PECMail)
                    End If
                End If
                Return _envelopeProxy
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property SegnaturaInteroperabilita As DocumentInfo
        Get
            If _pec.IDSegnatura = Guid.Empty Then
                Return Nothing
            End If

            If _pec.ProcessStatus <> PECMailProcessStatus.StoredInDocumentManager Then
                If _interopSignature Is Nothing Then
                    _interopSignature = New BiblosDocumentInfo(_pec.IDSegnatura)
                    If _interopSignature IsNot Nothing Then
                        _interopSignature.CheckSignedEvaluateStream = _checkSignedEvaluateStream
                    End If

                End If
                Return _interopSignature
            End If
            If _pec.ProcessStatus = PECMailProcessStatus.StoredInDocumentManager Then
                If _interopSignatureProxy Is Nothing Then
                    If Not String.IsNullOrEmpty(_pec.MailContent) Then
                        Dim docs As List(Of DocumentInfoModel) = JsonConvert.DeserializeObject(Of List(Of DocumentInfoModel))(_pec.MailContent)
                        Dim documentInfoModel As DocumentInfoModel = docs.SingleOrDefault(Function(f) f.DocumentId = _pec.IDSegnatura)
                        _interopSignatureProxy = New DocumentProxyDocumentInfo(_pec.UniqueId, documentInfoModel.DocumentId, DSWEnvironment.PECMail, documentInfoModel.Filename, documentInfoModel.FileExtension, documentInfoModel.Size, documentInfoModel.ReferenceType, documentInfoModel.VirtualPath)
                    Else
                        _interopSignatureProxy = New DocumentProxyDocumentInfo(_pec.UniqueId, _pec.IDSegnatura, DSWEnvironment.PECMail)
                    End If
                End If
                Return _interopSignatureProxy
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property DatiCert As DocumentInfo
        Get
            If _pec.IDDaticert = Guid.Empty Then
                Return Nothing
            End If

            If _pec.ProcessStatus <> PECMailProcessStatus.StoredInDocumentManager Then
                If _datiCert Is Nothing Then
                    _datiCert = New BiblosDocumentInfo(_pec.IDDaticert)
                    If _datiCert IsNot Nothing Then
                        _datiCert.CheckSignedEvaluateStream = _checkSignedEvaluateStream
                    End If

                End If
                Return _datiCert
            End If
            If _pec.ProcessStatus = PECMailProcessStatus.StoredInDocumentManager Then
                If _datiCertProxy Is Nothing Then
                    If Not String.IsNullOrEmpty(_pec.MailContent) Then
                        Dim docs As List(Of DocumentInfoModel) = JsonConvert.DeserializeObject(Of List(Of DocumentInfoModel))(_pec.MailContent)
                        Dim documentInfoModel As DocumentInfoModel = docs.SingleOrDefault(Function(f) f.DocumentId = _pec.IDDaticert)
                        _datiCertProxy = New DocumentProxyDocumentInfo(_pec.UniqueId, documentInfoModel.DocumentId, DSWEnvironment.PECMail, documentInfoModel.Filename, documentInfoModel.FileExtension, documentInfoModel.Size, documentInfoModel.ReferenceType, documentInfoModel.VirtualPath)
                    Else
                        _datiCertProxy = New DocumentProxyDocumentInfo(_pec.UniqueId, _pec.IDDaticert, DSWEnvironment.PECMail)
                    End If
                End If
                Return _datiCertProxy
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property MailBox As PECMailBox
        Get
            If _mailBox Is Nothing Then
                _mailBox = _pec.MailBox
            End If
            Return _mailBox
        End Get
    End Property

End Class
