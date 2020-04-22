Imports System.IO
Imports System.Linq
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models

Public Class BiblosPecMailWrapper

    Private ReadOnly _pec As PECMail
    Private _checkSignedEvaluateStream As Boolean
    Private _attachments As IList(Of BiblosPecMailAttachmentWrapper)
    Private _receipts As IList(Of BiblosPecMailReceiptWrapper)

    Private _postaCert As BiblosDocumentInfo
    Private _envelope As BiblosDocumentInfo
    Private _segnaturaInteroperabilita As BiblosDocumentInfo
    Private _datiCert As BiblosDocumentInfo
    Private _oChartCommunicationData As DocumentInfo

    Private _mailContent As BiblosDocumentInfo
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
                    Dim attachmentWrapper As BiblosPecMailAttachmentWrapper = New BiblosPecMailAttachmentWrapper(pecMailAttachment, _pec.Location.DocumentServer, _checkSignedEvaluateStream)
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

    Public ReadOnly Property PostaCert As BiblosDocumentInfo
        Get
            If _postaCert Is Nothing Then
                If Not _pec.IDPostacert.Equals(Guid.Empty) Then
                    _postaCert = New BiblosDocumentInfo(_pec.Location.DocumentServer, _pec.IDPostacert)
                    If (_postaCert IsNot Nothing) Then
                        _postaCert.CheckSignedEvaluateStream = _checkSignedEvaluateStream
                    End If
                End If
            End If
            Return _postaCert
        End Get
    End Property

    Public ReadOnly Property MailContent As BiblosDocumentInfo
        Get
            If _mailContent Is Nothing Then
                _mailContent = FacadeFactory.Instance.PECMailFacade.GetPecMailContent(_pec)
                If (_mailContent IsNot Nothing) Then
                    _mailContent.CheckSignedEvaluateStream = _checkSignedEvaluateStream
                End If
            End If
            Return _mailContent
        End Get
    End Property

    Public ReadOnly Property MailBody As FileDocumentInfo
        Get
            If _mailBody Is Nothing Then
                ' Salvo in TEMP un file di Testo con il corpo della MAIL
                Dim name As String = FileHelper.UniqueFileNameFormat("Corpo della mail.txt", DocSuiteContext.Current.User.UserName)
                Dim fullname As String = Path.Combine(CommonUtil.GetInstance().AppTempPath, name)
                File.WriteAllText(fullname, _pec.MailBody)
                _mailBody = New FileDocumentInfo(New FileInfo(fullname))
                _mailBody.Name = "Corpo della mail.txt"
                _mailBody.CheckSignedEvaluateStream = _checkSignedEvaluateStream
            End If
            Return _mailBody
        End Get
    End Property

    Public ReadOnly Property Envelope As BiblosDocumentInfo
        Get
            If _envelope Is Nothing AndAlso _pec.IDEnvelope <> Guid.Empty Then
                _envelope = New BiblosDocumentInfo(_pec.Location.DocumentServer, _pec.IDEnvelope)
                If (_envelope IsNot Nothing) Then
                    _envelope.CheckSignedEvaluateStream = _checkSignedEvaluateStream
                End If

            End If
            Return _envelope
        End Get
    End Property

    Public ReadOnly Property SegnaturaInteroperabilita As BiblosDocumentInfo
        Get
            If _segnaturaInteroperabilita Is Nothing AndAlso _pec.IDSegnatura <> Guid.Empty Then
                _segnaturaInteroperabilita = New BiblosDocumentInfo(_pec.Location.DocumentServer, _pec.IDSegnatura)
                If (_segnaturaInteroperabilita IsNot Nothing) Then
                    _segnaturaInteroperabilita.CheckSignedEvaluateStream = _checkSignedEvaluateStream
                End If

            End If
            Return _segnaturaInteroperabilita
        End Get
    End Property

    Public ReadOnly Property DatiCert As BiblosDocumentInfo
        Get
            If _datiCert Is Nothing AndAlso _pec.IDDaticert <> Guid.Empty Then
                _datiCert = New BiblosDocumentInfo(_pec.Location.DocumentServer, _pec.IDDaticert)
                If (_datiCert IsNot Nothing) Then
                    _datiCert.CheckSignedEvaluateStream = _checkSignedEvaluateStream
                End If

            End If
            Return _datiCert
        End Get
    End Property

    Public ReadOnly Property OChartCommunicationData As DocumentInfo
        Get
            If _oChartCommunicationData Is Nothing Then
                ''Cerco l'allegato con il nome richiesto
                Dim pecMailAttachment As PECMailAttachment = _pec.Attachments.ToList().Find(Function(x) x.AttachmentName = DocSuiteContext.Current.ProtocolEnv.OChartCommunicationDataName)

                If pecMailAttachment Is Nothing OrElse pecMailAttachment.IDDocument = Guid.Empty Then
                    Return Nothing
                End If

                _oChartCommunicationData = New BiblosDocumentInfo(_pec.Location.DocumentServer, pecMailAttachment.IDDocument)
                If (_oChartCommunicationData IsNot Nothing) Then
                    _oChartCommunicationData.CheckSignedEvaluateStream = _checkSignedEvaluateStream
                End If

            End If
            Return _oChartCommunicationData
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
