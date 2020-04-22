<Serializable()> _
Public Class PECMailBoxConfiguration
    Inherits DomainObject(Of Integer)

    Public Overridable Property Name As String

    Public Overridable Property MaxReadForSession As Integer

    Public Overridable Property MaxSendForSession As Integer

    Public Overridable Property UnzipAttachments As Boolean

    Public Overridable Property MarkAsRead As Boolean

    Public Overridable Property MoveToFolder As String

    Public Overridable Property MoveErrorToFolder As String

    Public Overridable Property InboxFolder As String

    Public Overridable Property UploadSent As Boolean

    Public Overridable Property FolderSent As String

    Public Overridable Property ImapSearchFlag As ImapFlag

    Public Overridable Property ImapStartDate As Date?

    Public Overridable Property ImapEndDate As Date?

    Public Overridable Property NoSubjectDefaultText As String

    Public Overridable Property DeleteMailFromServer As Boolean?

    Public Overridable Property MaxReceiveByteSize As Long

    Public Overridable Property MaxSendByteSize As Long

End Class
