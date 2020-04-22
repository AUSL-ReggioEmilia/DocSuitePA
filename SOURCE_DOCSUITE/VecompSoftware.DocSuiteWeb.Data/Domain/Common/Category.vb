Imports Newtonsoft.Json
Imports System.Text

<Serializable()> _
Partial Public Class Category
    Inherits DomainObject(Of Integer)
    Implements IAuditable

#Region " Fields "

    Private _name As String
    Private _parent As Category
    Private _isActive As Short
    Private _code As Integer
    Private _fullIncrementalPath As String
    Private _fullCode As String
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _children As IList(Of Category)

#End Region

#Region " Properties "

    Public Overridable Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Public Overridable Property FullSearchComputed() As String

    <JsonIgnore()>
    Public Overridable Property Parent() As Category
        Get
            Return _parent
        End Get
        Set(ByVal value As Category)
            _parent = value
        End Set
    End Property

    ''' <summary> Torna la categoria radice della sottocategoria. </summary>
    <JsonIgnore()>
    Public Overridable ReadOnly Property Root() As Category
        Get
            Return GetRoot()
        End Get
    End Property

    Public Overridable Property IsActive() As Short
        Get
            Return _isActive
        End Get
        Set(ByVal value As Short)
            _isActive = value
        End Set
    End Property

    Public Overridable Property Code() As Integer
        Get
            Return _code
        End Get
        Set(ByVal value As Integer)
            _code = value
        End Set
    End Property

    Public Overridable Property FullIncrementalPath() As String
        Get
            Return _fullIncrementalPath
        End Get
        Set(ByVal value As String)
            _fullIncrementalPath = value
        End Set
    End Property

    Public Overridable Property FullCode() As String
        Get
            Return _fullCode
        End Get
        Set(ByVal value As String)
            _fullCode = value
        End Set
    End Property

    <JsonIgnore()>
    Public Overridable ReadOnly Property FullCodeDotted() As String
        Get

            Dim sRet As New StringBuilder()
            Dim sArr As String() = _fullCode.Split("|"c)
            For Each s As String In sArr
                sRet.AppendFormat("{0}.", s.TrimStart("0"c))
            Next
            If (sRet.Length > 0) Then
                sRet.Remove(sRet.Length - 1, 1)
            End If
            Return sRet.ToString()
        End Get
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

    <JsonIgnore()>
    Public Overridable Property Children() As IList(Of Category)
        Get
            Return _children
        End Get
        Set(ByVal value As IList(Of Category))
            _children = value
        End Set
    End Property

    <JsonIgnore()>
    Public Overridable ReadOnly Property HasChildren() As Boolean
        Get
            Return (Not Children Is Nothing AndAlso Children.Count > 0)
        End Get
    End Property

    <JsonIgnore()>
    Public Overridable ReadOnly Property Level() As Integer
        Get
            Return FullIncrementalPath.Split("|"c).Length - 1
        End Get
    End Property

    'TODO: Questa gestione dovrebbe essere presente solo lato Web API
    Public Overridable Property IdMassimarioScarto As Guid?
    Public Overridable Property StartDate As DateTimeOffset
    Public Overridable Property EndDate As DateTimeOffset?
    Public Overridable Property CategorySchema As CategorySchema
    Public Overridable Property IdMetadataRepository As Guid?

#End Region

#Region " Constructors "

    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub

#End Region

#Region " Methods "

    Public Overridable Function GetFullName() As String
        Return String.Format("{0}.{1}", Code, Name)
    End Function

    Private Function GetRoot() As Category
        If Parent Is Nothing Then
            Return Me
        End If
        Return Parent.GetRoot()
    End Function

#End Region

End Class

