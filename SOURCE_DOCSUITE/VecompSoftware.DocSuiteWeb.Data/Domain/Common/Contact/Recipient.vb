<Serializable()> _
Public Class Recipient
    Inherits DomainObject(Of Int32)
    Implements ISupportLogicDelete

#Region "private data"

    Private _idType As Short
    Private _fullName As String
    Private _company As String
    Private _address As String
    Private _jobPosition As String
    Private _phoneNumber As String
    Private _faxNumber As String
    Private _emailAddress As String
    Private _isActive As Short
    Private _distributionLists As IList(Of DistributionList)
    Private _protocols As IList(Of Protocol)

#End Region

#Region "Properties"

    Public Overridable Property idType() As Short
        Get
            Return _idType
        End Get
        Set(ByVal value As Short)
            _idType = value
        End Set
    End Property

    Public Overridable Property FullName() As String
        Get
            Return _fullName
        End Get
        Set(ByVal value As String)
            _fullName = value
        End Set
    End Property

    Public Overridable Property Company() As String
        Get
            Return _company
        End Get
        Set(ByVal value As String)
            _company = value
        End Set
    End Property
    Public Overridable Property Address() As String
        Get
            Return _address
        End Get
        Set(ByVal value As String)
            _address = value
        End Set
    End Property
    Public Overridable Property JobPosition() As String
        Get
            Return _jobPosition
        End Get
        Set(ByVal value As String)
            _jobPosition = value
        End Set
    End Property
    Public Overridable Property PhoneNumber() As String
        Get
            Return _phoneNumber
        End Get
        Set(ByVal value As String)
            _phoneNumber = value
        End Set
    End Property
    Public Overridable Property FaxNumber() As String
        Get
            Return _faxNumber
        End Get
        Set(ByVal value As String)
            _faxNumber = value
        End Set
    End Property
    Public Overridable Property EmailAddress As String
        Get
            Return _emailAddress
        End Get
        Set(ByVal value As String)
            _emailAddress = value
        End Set
    End Property
    Public Overridable Property IsActive() As Short Implements ISupportLogicDelete.IsActive
        Get
            Return _isActive
        End Get
        Set(ByVal value As Short)
            _isActive = value
        End Set
    End Property

    Public Overridable Property DistributionLists() As IList(Of DistributionList)
        Get
            Return _distributionLists
        End Get
        Set(ByVal value As IList(Of DistributionList))
            _distributionLists = value
        End Set
    End Property

    Public Overridable Property Protocols() As IList(Of Protocol)
        Get
            Return _protocols
        End Get
        Set(ByVal value As IList(Of Protocol))
            _protocols = value
        End Set
    End Property

#End Region

#Region "Ctor/init"
    Public Sub New()
        _distributionLists = New List(Of DistributionList)
        _protocols = New List(Of Protocol)
    End Sub
#End Region

End Class

