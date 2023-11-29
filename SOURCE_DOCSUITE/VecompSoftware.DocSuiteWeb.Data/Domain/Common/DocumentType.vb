Imports System.Collections.Generic

<Serializable()> _
Public Class DocumentType
    Inherits DomainObject(Of Int32)
    Implements IAuditable, ISupportBooleanLogicDelete

#Region " Fields "

    Private _code As String
    Private _commonUser As String
    Private _needPackage As Boolean
    Private _description As String
    Private _isActive As Boolean
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _hiddenFields As String

#End Region

#Region " Properties "

    Public Overridable Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            _code = value
        End Set
    End Property

    Public Overridable Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Public Overridable Property CommonUser() As String
        Get
            Return _commonUser
        End Get
        Set(ByVal value As String)
            _commonUser = value
        End Set
    End Property

    Public Overridable Property NeedPackage() As Boolean
        Get
            Return _needPackage
        End Get
        Set(ByVal value As Boolean)
            _needPackage = value
        End Set
    End Property

    Public Overridable Property IsActive() As Boolean Implements ISupportBooleanLogicDelete.IsActive
        Get
            Return _isActive
        End Get
        Set(ByVal value As Boolean)
            _isActive = value
        End Set
    End Property

    Public Overridable Property RegistrationUser() As String Implements IAuditable.RegistrationUser
        Get
            Return _registrationUser
        End Get
        Set(ByVal value As String)
            _registrationUser = value
        End Set
    End Property

    Public Overridable Property RegistrationDate() As DateTimeOffset Implements IAuditable.RegistrationDate
        Get
            Return _registrationDate
        End Get
        Set(ByVal value As DateTimeOffset)
            _registrationDate = value
        End Set
    End Property

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
        Get
            Return _lastChangedUser
        End Get
        Set(ByVal value As String)
            _lastChangedUser = value
        End Set
    End Property

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
        Get
            Return _lastChangedDate
        End Get
        Set(ByVal value As DateTimeOffset?)
            _lastChangedDate = value
        End Set
    End Property

    Public Overridable Property HiddenFields() As String
        Get
            Return _hiddenFields
        End Get
        Set(ByVal value As String)
            _hiddenFields = value
        End Set
    End Property


#End Region

#Region " Constructor "

    Public Sub New()
    End Sub

#End Region

End Class

