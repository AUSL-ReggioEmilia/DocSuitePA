Partial Public Class PecMailHeader

#Region " PECMail Properties "

    Public Property Id As Integer?
    Public Property Direction As Short?
    Public Property Year As Short?
    Public Property Number As Integer?
    Public Property MailSubject As String
    Public Property MailSenders As String
    Public Property MailRecipients As String
    Public Property MailRecipientsCc As String
    Public Property MailDate As Date?
    Public Property RegistrationDate As DateTimeOffset
    Public Property MailPriority As Short?
    Public Property XTrasporto As String
    Public Property XRiferimentoMessageID As String
    Public Property Segnatura As String
    Public Property RecordedInDocSuite As Short?
    Public Property MailBoxId As Short?
    Public Property MailBoxName As String
    Public Property IsToForward As Boolean?
    Public Property IsValidForInterop As Boolean?
    Public Property IsActive As Short?
    Public Property IsDestinated As Boolean?
    Public Property Handler As String
    Public Property Size As Int64?
    Public Property ReceivedAsCc As Boolean?
    Public Property PECType As PECMailType
    Public Property IdUDSRepository As Guid?
    Public Property IdDocumentUnit As Guid?
    Public Property DocumentUnitType As Integer?
    Public Property UDSAlias As String
#End Region

#Region " Properties "

    Public Property AttachmentsCount As Integer?
    Public ReadOnly Property HasAttachments As Boolean?
        Get
            Return AttachmentsCount.HasValue AndAlso AttachmentsCount > 0
        End Get
    End Property
    Public Property ReadCount As Integer?
    Public ReadOnly Property HasRead As Boolean?
        Get
            Return ReadCount.HasValue AndAlso ReadCount > 0
        End Get
    End Property
    Public Property MoveCount As Integer?
    Public ReadOnly Property HasMove As Boolean?
        Get
            Return MoveCount.HasValue AndAlso MoveCount > 0
        End Get
    End Property

    Public Property LastReplyMailId As Integer?
    Public Property LastForwardMailId As Integer?
    Public Property HasProtocol As Boolean?
    Public Property ExistSendRolesLog As Integer?
    Public ReadOnly Property IsSendRolesLog As Boolean
        Get
            Return ExistSendRolesLog.HasValue AndAlso ExistSendRolesLog.Value > 0
        End Get
    End Property
#End Region

End Class