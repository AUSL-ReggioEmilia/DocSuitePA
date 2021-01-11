<Serializable()> _
Public Class ResolutionParer
    Inherits DomainObject(Of Int32)

#Region " Fields "

    Private _year As Short
    Private _number As Integer
    Private _archivedDate As DateTime?
    Private _parerUri As String
    Private _isForArchive As Boolean
    Private _hasError As Boolean
    Private _lastError As String
    Private _lastSendDate As DateTime?

#End Region

    Public Overridable Property Year() As Short
        Get
            Return _year
        End Get
        Set(ByVal value As Short)
            _year = value
        End Set
    End Property

    Public Overridable Property Number() As Integer
        Get
            Return _number
        End Get
        Set(ByVal value As Integer)
            _number = value
        End Set
    End Property

    Public Overridable Property ArchivedDate() As DateTime?
        Get
            Return _archivedDate
        End Get
        Set(ByVal value As DateTime?)
            _archivedDate = value
        End Set
    End Property

    Public Overridable Property ParerUri() As String
        Get
            Return _parerUri
        End Get
        Set(ByVal value As String)
            _parerUri = value
        End Set
    End Property

    Public Overridable Property IsForArchive() As Boolean
        Get
            Return _isForArchive
        End Get
        Set(ByVal value As Boolean)
            _isForArchive = value
        End Set
    End Property

    Public Overridable Property HasError() As Boolean
        Get
            Return _hasError
        End Get
        Set(ByVal value As Boolean)
            _hasError = value
        End Set
    End Property

    Public Overridable Property LastError() As String
        Get
            Return _lastError
        End Get
        Set(ByVal value As String)
            _lastError = value
        End Set
    End Property

    Public Overridable Property LastSendDate() As DateTime?
        Get
            Return _lastSendDate
        End Get
        Set(ByVal value As DateTime?)
            _lastSendDate = value
        End Set
    End Property

End Class
