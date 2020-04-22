<Serializable()> _
Public Class DocumentVersioning
    Inherits DomainObject(Of YearNumberIncrCompositeKey)

#Region "private data"
    Private _incrementalObject As Short
    Private _checkOutUser As String
    Private _checkOutDate As Date?
    Private _checkInUser As String
    Private _checkInDate As Date?
    Private _checkDir As String
    Private _checkSystemComputer As String
    Private _checkStatus As String
    Private _documentObject As DocumentObject

#End Region

#Region "Properties"
    Public Overridable Property Year() As Short
        Get
            Return Id.Year
        End Get
        Set(ByVal value As Short)
            Id.Year = value
        End Set
    End Property
    Public Overridable Property Number() As Integer
        Get
            Return Id.Number
        End Get
        Set(ByVal value As Integer)
            Id.Number = value
        End Set
    End Property
    Public Overridable Property Incremental() As Short
        Get
            Return Id.Incremental
        End Get
        Set(ByVal value As Short)
            Id.Incremental = value
        End Set
    End Property
    Public Overridable Property IncrementalObject() As Short
        Get
            Return _incrementalObject
        End Get
        Set(ByVal value As Short)
            _incrementalObject = value
        End Set
    End Property
    Public Overridable Property CheckOutUser() As String
        Get
            Return _checkOutUser
        End Get
        Set(ByVal value As String)
            _checkOutUser = value
        End Set
    End Property
    Public Overridable Property CheckOutDate() As Date?
        Get
            Return _checkOutDate
        End Get
        Set(ByVal value As Date?)
            _checkOutDate = value
        End Set
    End Property
    Public Overridable Property CheckInUser() As String
        Get
            Return _checkInUser
        End Get
        Set(ByVal value As String)
            _checkInUser = value
        End Set
    End Property
    Public Overridable Property CheckInDate() As Date?
        Get
            Return _checkInDate
        End Get
        Set(ByVal value As Date?)
            _checkInDate = value
        End Set
    End Property
    Public Overridable Property CheckDir() As String
        Get
            Return _checkDir
        End Get
        Set(ByVal value As String)
            _checkDir = value
        End Set
    End Property
    Public Overridable Property CheckSystemComputer() As String
        Get
            Return _checkSystemComputer
        End Get
        Set(ByVal value As String)
            _checkSystemComputer = value
        End Set
    End Property
    Public Overridable Property CheckStatus() As String
        Get
            Return _checkStatus
        End Get
        Set(ByVal value As String)
            _checkStatus = value
        End Set
    End Property

    Public Overridable Property DocumentObject() As DocumentObject
        Get
            Return _documentObject
        End Get
        Set(ByVal value As DocumentObject)
            _documentObject = value
        End Set
    End Property

    Public Overridable ReadOnly Property CheckInUserDate() As String
        Get
            Return _checkInUser & "<br>" & _checkInDate.ToString()
        End Get
    End Property

    Public Overridable ReadOnly Property CheckOutUserDate() As String
        Get
            Return _checkOutUser & "<br>" & _checkOutDate.ToString()
        End Get
    End Property

#End Region

#Region "Descriptions"

#End Region

#Region "Ctor/init"
    Public Sub New()
        Id = New YearNumberIncrCompositeKey()
    End Sub
#End Region

End Class

