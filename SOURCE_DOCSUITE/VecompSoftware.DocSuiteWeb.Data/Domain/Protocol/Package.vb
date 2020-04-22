<Serializable()> _
Public Class Package
    Inherits DomainObject(Of PackageCompositeKey)

    Private _account As String
    Private _lot As Integer
    Private _incremental As Integer
    Private _totalIncremental As Integer
    Private _maxDocuments As Integer
    Private _state As Char

    Public Overridable Property Origin() As Char
        Get
            Return Id.Origin
        End Get
        Set(ByVal value As Char)
            Id.Origin = value
        End Set
    End Property

    Public Overridable Property Package() As Integer
        Get
            Return Id.Package
        End Get
        Set(ByVal value As Integer)
            Id.Package = value
        End Set
    End Property

    Public Overridable Property Account() As String
        Get
            Return _account
        End Get
        Set(ByVal value As String)
            _account = value
        End Set
    End Property

    Public Overridable Property Lot() As Integer
        Get
            Return _lot
        End Get
        Set(ByVal value As Integer)
            _lot = value
        End Set
    End Property

    Public Overridable Property Incremental() As Integer
        Get
            Return _incremental
        End Get
        Set(ByVal value As Integer)
            _incremental = value
        End Set
    End Property

    Public Overridable Property TotalIncremental() As Integer
        Get
            Return _totalIncremental
        End Get
        Set(ByVal value As Integer)
            _totalIncremental = value
        End Set
    End Property

    Public Overridable Property MaxDocuments() As Integer
        Get
            Return _maxDocuments
        End Get
        Set(ByVal value As Integer)
            _maxDocuments = value
        End Set
    End Property

    Public Overridable Property State() As Char
        Get
            Return _state
        End Get
        Set(ByVal value As Char)
            _state = value
        End Set
    End Property
End Class
