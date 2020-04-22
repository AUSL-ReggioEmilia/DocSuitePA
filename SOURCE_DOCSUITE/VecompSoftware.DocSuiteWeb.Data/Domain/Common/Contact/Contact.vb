Imports Newtonsoft.Json

<Serializable()>
Public Class Contact
    Inherits DomainObject(Of Int32)
    Implements IAuditable, ISupportLogicDelete, ISupportRangeDelete, ISupportChanged

#Region " Fields "

    Private _parent As Contact
    Private _description As String
    Private _code As String
    Private _searchCode As String
    Private _note As String
    Private _isLocked As Nullable(Of Short)
    Private _isNotExpandable As Nullable(Of Short)
    Private _role As Role
    Private _fullIncrementalPath As String
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _documents As IList(Of DocumentContact)
    Private _protocols As IList(Of ProtocolContact)
    Private _birthDate As Date?
    Private _birthPlace As String
    Private _contactTitle As ContactTitle
    Private _address As Address
    Private _fiscalCode As String
    Private _telephoneNumber As String
    Private _faxNumber As String
    Private _emailAddress As String
    Private _certifiedMail As String
    Private _children As IList(Of Contact)
    Private _roleUserIdRole As String
    Private _roleRootContact As Role
    Private _contactNames As IList(Of ContactName)
    Private _sdiIdentification As String
    Private _tenantContacts As IList(Of TenantContact)


#End Region

#Region " Properties "

    Public Overridable Property ContactType() As ContactType

    <JsonIgnore()>
    Public Overridable Property Parent() As Contact
        Get
            Return _parent
        End Get
        Set(ByVal value As Contact)
            _parent = value
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

    Public Overridable ReadOnly Property DescriptionFormatByContactType() As String
        Get
            If ContactType.Id = ContactType.Person Then
                Return Replace(Description, "|", " ")
            End If
            Return Description
        End Get
    End Property

    Public Overridable ReadOnly Property FullDescription As String
        Get
            Dim descr As String = If(String.IsNullOrEmpty(Code) OrElse Code.Contains("\"), String.Empty, Code)
            If Not String.IsNullOrEmpty(SearchCode) Then
                If Not String.IsNullOrEmpty(descr) Then
                    descr &= "-"
                End If
                descr &= SearchCode
            End If
            If Not String.IsNullOrEmpty(descr) Then
                descr = " (" & descr & ")"
            End If

            Return Replace(Description, "|", " ") & descr
        End Get
    End Property

    Public Overridable ReadOnly Property FullDescriptionWithFiscalCode As String
        Get
            Dim descr As String = FiscalCode
            If Not String.IsNullOrEmpty(descr) Then
                descr = String.Concat(" (", FiscalCode, ")")
            End If

            Return String.Concat(Replace(Description, "|", " "), descr)
        End Get
    End Property


    Public Overridable ReadOnly Property FullDescription(ByVal includePecAddress As Boolean) As String
        Get
            Dim tor As String = Me.FullDescription
            If includePecAddress AndAlso Not String.IsNullOrEmpty(CertifiedMail) Then
                tor = String.Format("{0} ({1})", tor, CertifiedMail)
            End If
            Return tor
        End Get
    End Property

    Public Overridable ReadOnly Property LastName As String
        Get
            Dim result As String = String.Empty
            If Not String.IsNullOrEmpty(Description) Then
                If Description.Contains("|") Then
                    result = Description.Split("|"c)(0)
                Else
                    result = Description
                End If
            End If
            Return result
        End Get
    End Property

    Public Overridable ReadOnly Property FirstName As String
        Get
            Dim result As String = String.Empty
            If Not String.IsNullOrEmpty(Description) AndAlso Description.Contains("|") Then
                result = Description.Split("|"c)(1)
            End If
            Return result
        End Get
    End Property

    Public Overridable Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            _code = value
        End Set
    End Property

    Public Overridable Property SearchCode() As String
        Get
            Return _searchCode
        End Get
        Set(ByVal value As String)
            _searchCode = value
        End Set
    End Property

    Public Overridable Property FiscalCode() As String
        Get
            Return _fiscalCode
        End Get
        Set(ByVal value As String)
            _fiscalCode = value
        End Set
    End Property

    Public Overridable Property TelephoneNumber() As String
        Get
            Return _telephoneNumber
        End Get
        Set(ByVal value As String)
            _telephoneNumber = value
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
            _emailAddress = If(String.IsNullOrEmpty(value), String.Empty, value.Trim())
        End Set
    End Property

    ''' <summary>Posta Elettronica Certificata</summary>
    Public Overridable Property CertifiedMail As String
        Get
            Return _certifiedMail
        End Get
        Set(ByVal value As String)
            _certifiedMail = If(String.IsNullOrEmpty(value), String.Empty, value.Trim())
        End Set
    End Property

    Public Overridable Property Note() As String
        Get
            Return _note
        End Get
        Set(ByVal value As String)
            _note = value
        End Set
    End Property

    Public Overridable Property IsActive() As Short Implements ISupportLogicDelete.IsActive
    <JsonIgnore()> _
    Public Overridable Property IsChanged As Short Implements ISupportChanged.IsChanged

    Public Overridable Property ActiveFrom() As DateTime? Implements ISupportRangeDelete.ActiveFrom

    Public Overridable Property ActiveTo() As DateTime? Implements ISupportRangeDelete.ActiveTo

    Public Overridable Property isLocked() As Nullable(Of Short)
        Get
            Return _isLocked
        End Get
        Set(ByVal value As Nullable(Of Short))
            _isLocked = value
        End Set
    End Property

    Public Overridable Property isNotExpandable() As Nullable(Of Short)
        Get
            Return _isNotExpandable
        End Get
        Set(ByVal value As Nullable(Of Short))
            _isNotExpandable = value
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

    <JsonIgnore()> _
    Public Overridable ReadOnly Property Level() As String
        Get
            Dim array As String() = Split(FullIncrementalPath, "|")
            Return CType((array.Length - 1), String)
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

    Public Overridable Property Role() As Role
        Get
            Return _role
        End Get
        Set(ByVal value As Role)
            _role = value
        End Set
    End Property

    Public Overridable Property StudyTitle() As ContactTitle
        Get
            Return _contactTitle
        End Get
        Set(ByVal value As ContactTitle)
            _contactTitle = value
        End Set
    End Property

    Public Overridable Property BirthDate() As Date?
        Get
            Return _birthDate
        End Get
        Set(ByVal value As Date?)
            _birthDate = value
        End Set
    End Property

    Public Overridable Property BirthPlace() As String
        Get
            Return _birthPlace
        End Get
        Set(ByVal value As String)
            _birthPlace = value
        End Set
    End Property

    Public Overridable Property Address() As Address
        Get
            Return _address
        End Get
        Set(ByVal value As Address)
            _address = value
        End Set
    End Property

    <JsonIgnore()>
    Public Overridable Property Protocols() As IList(Of ProtocolContact)
        Get
            Return _protocols
        End Get
        Set(ByVal value As IList(Of ProtocolContact))
            _protocols = value
        End Set
    End Property

    <JsonIgnore()>
    Public Overridable Property TenantContacts() As IList(Of TenantContact)
        Get
            Return _tenantContacts
        End Get
        Set(ByVal value As IList(Of TenantContact))
            _tenantContacts = value
        End Set
    End Property

    <JsonIgnore()>
    Public Overridable Property Documents() As IList(Of DocumentContact)
        Get
            Return _documents
        End Get
        Set(ByVal value As IList(Of DocumentContact))
            _documents = value
        End Set
    End Property

    <JsonIgnore()>
    Public Overridable Property Children() As IList(Of Contact)
        Get
            Return _children
        End Get
        Set(ByVal value As IList(Of Contact))
            _children = value
        End Set
    End Property

    <JsonIgnore()>
    Public Overridable ReadOnly Property HasChildren() As Boolean
        Get
            If Children Is Nothing Then
                Return False
            End If
            Return (Children.Count > 0)
        End Get
    End Property

    Public Overridable Property RoleUserIdRole() As String
        Get
            Return _roleUserIdRole
        End Get
        Set(ByVal value As String)
            _roleUserIdRole = value
        End Set
    End Property
    Public Overridable Property RoleRootContact() As Role
        Get
            Return _roleRootContact
        End Get
        Set(ByVal value As Role)
            _roleRootContact = value
        End Set
    End Property

    <JsonIgnore()>
    Public Overridable Property ContactNames() As IList(Of ContactName)
        Get
            Return _contactNames
        End Get
        Set(ByVal value As IList(Of ContactName))
            _contactNames = value
        End Set
    End Property

    <JsonIgnore()>
    Public Overridable Property ContactLists As IList(Of ContactList)
    Public Overridable Property SDIIdentification() As String
        Get
            Return _sdiIdentification
        End Get
        Set(ByVal value As String)
            _sdiIdentification = value
        End Set
    End Property
#End Region

#Region " Constructors "

    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub

#End Region

#Region " Methods "

    ''' <summary> Duplica alcuni campi del contatto restituendo un nuovo contatto senza ID. </summary>
    ''' <remarks>Da utilizzare per evitare problemi di serializzazione con JSON e i proxy NHibernate</remarks>
    Public Overridable Function Duplicate() As Contact
        Dim contact As New Contact
        contact.Address = Address
        contact.BirthDate = BirthDate
        contact.BirthPlace = BirthPlace
        contact.CertifiedMail = CertifiedMail
        contact.Code = Code
        contact.ContactType = New ContactType
        contact.ContactType.Id = ContactType.Id
        contact.ContactType.Description = ContactType.Description
        contact.Description = Description
        contact.Documents = Documents
        contact.EmailAddress = EmailAddress
        contact.FaxNumber = FaxNumber
        contact.FiscalCode = FiscalCode
        contact.FullIncrementalPath = FullIncrementalPath
        contact.IsActive = IsActive
        contact.isLocked = isLocked
        contact.isNotExpandable = isNotExpandable
        contact.LastChangedDate = LastChangedDate
        contact.LastChangedUser = LastChangedUser
        contact.Note = Note
        contact.RegistrationDate = If(RegistrationDate = DateTimeOffset.MinValue, DateTimeOffset.UtcNow, RegistrationDate)
        contact.RegistrationUser = RegistrationUser
        contact.SearchCode = SearchCode
        If StudyTitle IsNot Nothing Then
            contact.StudyTitle = New ContactTitle
            contact.StudyTitle.Id = StudyTitle.Id
            contact.StudyTitle.Code = StudyTitle.Code
            contact.StudyTitle.Description = StudyTitle.Description
            contact.StudyTitle.IsActive = StudyTitle.IsActive
            contact.StudyTitle.LastChangedDate = StudyTitle.LastChangedDate
            contact.StudyTitle.LastChangedUser = StudyTitle.LastChangedUser
            contact.StudyTitle.RegistrationUser = StudyTitle.RegistrationUser
            contact.StudyTitle.RegistrationDate = If(StudyTitle.RegistrationDate = DateTimeOffset.MinValue, DateTimeOffset.UtcNow, StudyTitle.RegistrationDate)
        End If
        contact.TelephoneNumber = TelephoneNumber

        Return contact
    End Function

    Public Overridable Function IsActiveRange() As Boolean Implements ISupportRangeDelete.IsActiveRange
        Return (Not ActiveFrom.HasValue AndAlso Not ActiveTo.HasValue) OrElse (ActiveFrom.Value < DateTime.Now AndAlso DateTime.Now < ActiveTo.Value)
    End Function

    Public Shared Function EscapingJSON(contact As Contact, escaping As Func(Of String, String)) As Contact
        contact.Description = escaping(contact.Description)
        contact.Note = escaping(contact.Note)
        contact.FaxNumber = escaping(contact.FaxNumber)
        contact.TelephoneNumber = escaping(contact.TelephoneNumber)
        contact.EmailAddress = escaping(contact.EmailAddress)
        contact.CertifiedMail = escaping(contact.CertifiedMail)
        contact.FiscalCode = escaping(contact.FiscalCode)
        contact.Code = escaping(contact.Code)

        If contact.Address IsNot Nothing Then
            contact.Address.Address = escaping(contact.Address.Address)
            contact.Address.City = escaping(contact.Address.City)
            contact.Address.CityCode = escaping(contact.Address.CityCode)
            contact.Address.ZipCode = escaping(contact.Address.ZipCode)
            contact.Address.CivicNumber = escaping(contact.Address.CivicNumber)
            If (contact.Address.PlaceName IsNot Nothing) Then
                contact.Address.PlaceName.Description = escaping(contact.Address.PlaceName.Description)
            End If
        End If
        Return contact
    End Function

#End Region

End Class
