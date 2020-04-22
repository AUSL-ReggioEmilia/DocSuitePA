<Serializable()> _
Public Class UserDiary
    Inherits DomainObject(Of Int32)

#Region " Fields "

    Private _year As Integer
    Private _number As Integer
    Private _object As String
    Private _codice As String
    Private _logDate As DateTime
    Private _type As String
    Private _pI As Integer
    Private _pS As Integer
    Private _pD As Integer
    Private _pZ As Integer
    Private _pM As Integer
    Private _adoptionDate As Date
    Private _isHandled As Integer

#End Region


#Region " Properties "

    Public Overridable Property Year() As Integer
        Get
            Return _year
        End Get
        Set(ByVal value As Integer)
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

    Public Overridable Property [Object]() As String
        Get
            Return _object
        End Get
        Set(ByVal value As String)
            _object = value
        End Set
    End Property

    Public Overridable Property Codice() As String
        Get
            Return _codice
        End Get
        Set(ByVal value As String)
            _codice = value
        End Set
    End Property

    Public Overridable Property LogDate() As DateTime
        Get
            Return _logDate
        End Get
        Set(ByVal value As DateTime)
            _logDate = value
        End Set
    End Property

    Public Overridable Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = value
        End Set
    End Property

    Public Overridable Property PI() As Integer
        Get
            Return _pI
        End Get
        Set(ByVal value As Integer)
            _pI = value
        End Set
    End Property

    Public Overridable Property PS() As Integer
        Get
            Return _pS
        End Get
        Set(ByVal value As Integer)
            _pS = value
        End Set
    End Property


    Public Overridable Property PD() As Integer
        Get
            Return _pD
        End Get
        Set(ByVal value As Integer)
            _pD = value
        End Set
    End Property
    Public Overridable Property PZ() As Integer
        Get
            Return _pZ
        End Get
        Set(ByVal value As Integer)
            _pZ = value
        End Set
    End Property
    Public Overridable Property PM() As Integer
        Get
            Return _pM
        End Get
        Set(ByVal value As Integer)
            _pM = value
        End Set
    End Property
    Public Overridable Property AdoptionDate() As Date
        Get
            Return _adoptionDate
        End Get
        Set(ByVal value As Date)
            _adoptionDate = value
        End Set
    End Property
    Public Overridable Property IsHandled() As Integer
        Get
            Return _isHandled
        End Get
        Set(ByVal value As Integer)
            _isHandled = value
        End Set
    End Property

#End Region

End Class
