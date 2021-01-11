<Serializable()> _
Public Class ContactName
    Inherits DomainObject(Of Int32)

#Region " Fields "

    Private _name As String
    Private _fromDate As DateTime?
    Private _toDate As DateTime?
    Private _contact As Contact

#End Region


    Public Overridable Property Name As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
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
    Public Overridable Property ToDate As DateTime?
        Get
            Return _toDate
        End Get
        Set(ByVal value As DateTime?)
            _toDate = value
        End Set
    End Property

    Public Overridable Property Contact() As Contact
        Get
            Return _contact
        End Get
        Set(ByVal value As Contact)
            _contact = value
        End Set
    End Property


End Class
