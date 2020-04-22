''' <summary> Classe per la gestione delle PEC da inviare agli Organi di Controllo. </summary>
''' <remarks>Usata dal modulo jeep <see>CollegioSindacaleTorino</see></remarks>
<Serializable()> _
Public Class PECOC
    Inherits DomainObject(Of Int32)
    Implements IAuditable, ISupportLogicDelete

#Region " Fields "
    Private _resolutionType As ResolutionType
    Private _fromDate As DateTime
    Private _toDate As DateTime?
    Private _extractAttachments As Boolean
    Private _status As PECOCStatus
    Private _message As String
    Private _idMail As Integer?
    Dim _logEntries As IList(Of PECOCLog)
#End Region

#Region " Properties "
    Public Overridable Property ResolutionType() As ResolutionType
        Get
            Return _resolutionType
        End Get
        Set(ByVal value As ResolutionType)
            _resolutionType = value
        End Set
    End Property

    Public Overridable Property FromDate As DateTime
        Get
            Return _fromDate
        End Get
        Set(ByVal value As DateTime)
            _fromDate = value
        End Set
    End Property

    Public Overridable Property ToDate() As DateTime?
        Get
            Return _toDate
        End Get
        Set(ByVal value As DateTime?)
            _toDate = value
        End Set
    End Property

    Public Overridable Property ExtractAttachments() As Boolean
        Get
            Return _extractAttachments
        End Get
        Set(ByVal value As Boolean)
            _extractAttachments = value
        End Set
    End Property

    Public Overridable Property Status() As PECOCStatus
        Get
            Return _status
        End Get
        Set(ByVal value As PECOCStatus)
            _status = value
        End Set
    End Property

    Public Overridable Property Message() As String
        Get
            Return _message
        End Get
        Set(ByVal value As String)
            _message = value
        End Set
    End Property

    ''' <summary> Identificativo della PEC spedita. </summary>
    ''' <remarks>La mail non è referenziata perchè presente in un'altra session factory</remarks>
    Public Overridable Property IdMail() As Integer?
        Get
            Return _idMail
        End Get
        Set(ByVal value As Integer?)
            _idMail = value
        End Set
    End Property

    Public Overridable Property LogEntries() As IList(Of PECOCLog)
        Get
            Return _logEntries
        End Get
        Set(ByVal value As IList(Of PECOCLog))
            _logEntries = value
        End Set
    End Property

#End Region

#Region " IAuditable "
    Private _lastChangedDate As DateTimeOffset?
    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
        Get
            Return _lastChangedDate
        End Get
        Set(value As DateTimeOffset?)
            _lastChangedDate = value
        End Set
    End Property

    Private _lastChangedUser As String
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
        Get
            Return _lastChangedUser
        End Get
        Set(value As String)
            _lastChangedUser = value
        End Set
    End Property

    Private _registrationDate As DateTimeOffset
    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
        Get
            Return _registrationDate
        End Get
        Set(value As DateTimeOffset)
            _registrationDate = value
        End Set
    End Property

    Private _registrationUser As String
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser
        Get
            Return _registrationUser
        End Get
        Set(value As String)
            _registrationUser = value
        End Set
    End Property
#End Region

#Region " ISupportLogicDelete "
    Private _isActive As Short
    Public Overridable Property IsActive As Short Implements ISupportLogicDelete.IsActive
        Get
            Return _isActive
        End Get
        Set(value As Short)
            _isActive = value
        End Set
    End Property
#End Region

End Class