Imports Newtonsoft.Json

<Serializable()>
Public Class Contact
    Inherits DomainObject(Of Int32)
    Implements IAuditable, ISupportBooleanLogicDelete, ISupportRangeDelete

#Region " Fields "
    Private _emailAddress As String
    Private _certifiedMail As String
#End Region

#Region " Properties "

    Public Overridable Property ContactType As ContactType

    <JsonIgnore()>
    Public Overridable Property Parent As Contact

    Public Overridable Property Description As String

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

    Public Overridable Property Code As String

    Public Overridable Property SearchCode As String

    Public Overridable Property FiscalCode As String

    Public Overridable Property TelephoneNumber As String

    Public Overridable Property FaxNumber As String

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

    Public Overridable Property Note As String

    Public Overridable Property IsActive As Boolean Implements ISupportBooleanLogicDelete.IsActive

    Public Overridable Property isLocked As Short?

    Public Overridable Property isNotExpandable As Short?

    Public Overridable Property FullIncrementalPath As String

    <JsonIgnore()>
    Public Overridable ReadOnly Property Level As String
        Get
            Dim array As String() = Split(FullIncrementalPath, "|")
            Return CType((array.Length - 1), String)
        End Get
    End Property

    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate

    Public Overridable Property Role As Role

    Public Overridable Property StudyTitle As ContactTitle

    Public Overridable Property BirthDate As Date?

    Public Overridable Property BirthPlace As String

    Public Overridable Property Address As Address

    <JsonIgnore()>
    Public Overridable Property Protocols As IList(Of ProtocolContact)

    <JsonIgnore()>
    Public Overridable Property Documents As IList(Of DocumentContact)

    <JsonIgnore()>
    Public Overridable Property Children As IList(Of Contact)

    <JsonIgnore()>
    Public Overridable ReadOnly Property HasChildren As Boolean
        Get
            If Children Is Nothing Then
                Return False
            End If
            Return (Children.Count > 0)
        End Get
    End Property

    Public Overridable Property RoleUserIdRole As String

    Public Overridable Property RoleRootContact As Role

    <JsonIgnore()>
    Public Overridable Property ContactLists As IList(Of ContactList)
    Public Overridable Property SDIIdentification As String
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
        Return IsActive = True
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
