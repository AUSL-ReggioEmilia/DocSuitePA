Imports System.DirectoryServices
Imports System.DirectoryServices.AccountManagement
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data

''' <summary> Utente di Active Directory o di Workgroup di Windows</summary>
<Serializable()>
Public Class AccountModel
    Implements IComparable

#Region " Property "
    Public Property PrincipalName As String

    Public Property Domain As String

    ''' <summary> Gruppo Active Directory </summary>
    Public Property Groups As String()

    Public Property FirstName As String

    Public Property LastName As String

    ''' <summary> Nome utente </summary>
    Public Property Name As String

    Public Property DisplayName As String

    ''' <summary> Logon </summary>
    Public Property Account As String

    Public Property TelephoneNumber As String

    Public Property Email As String

    Public Property Parent As String

    Public Property SchemaClassName As String

    Public Property DistinguishedName As String

    Public Property Company As String

    Public Property Department As String

    Public Property Description As String

    Public Property Path As String

    Public Property RelatedGroupName As String

    Public ReadOnly Property RawProperties As Dictionary(Of String, String)
        Get
            Return _rawProperties
        End Get
    End Property

#End Region

#Region "[ Fields]"
    Private ReadOnly _rawProperties As Dictionary(Of String, String) = New Dictionary(Of String, String)()
#End Region

#Region " Constructor "

    Public Sub New()
    End Sub

    ''' <summary> Inizializza un nuovo <see>ADUser</see> </summary>
    Public Sub New(ByVal account As String, ByVal name As String, Optional domain As String = "")
        _Account = account
        _Name = name
        _Domain = domain
    End Sub
    Public Sub New(ByRef user As UserPrincipal, Optional domain As String = "")
        _Domain = domain
        Account = If(user.SamAccountName, String.Empty)
        _rawProperties.Add("samaccountname", Account)

        Name = If(user.Name, String.Empty)
        _rawProperties.Add("name", Name)

        FirstName = If(user.GivenName, String.Empty)
        _rawProperties.Add("givenName", Name)

        LastName = If(user.Surname, String.Empty)
        _rawProperties.Add("sn", Name)

        DisplayName = If(user.DisplayName, String.Empty)
        _rawProperties.Add("displayName", DisplayName)

        Email = If(user.EmailAddress, String.Empty)
        _rawProperties.Add("mail", Email)

        DistinguishedName = If(user.DistinguishedName, String.Empty)
        _rawProperties.Add("distinguishedName", DistinguishedName)

        TelephoneNumber = If(user.VoiceTelephoneNumber, String.Empty)
        _rawProperties.Add("telephonenumber", TelephoneNumber)

        SchemaClassName = String.Empty
        _rawProperties.Add("schemaClassName", SchemaClassName)

        Company = String.Empty
        _rawProperties.Add("company", Company)

        Description = If(user.Description, String.Empty)
        _rawProperties.Add("Description", Description)

        PrincipalName = If(user.UserPrincipalName, String.Empty)
        _rawProperties.Add("msDS-PrincipalName", PrincipalName)
    End Sub

    Public Sub New(ByRef properties As ResultPropertyCollection, Optional domain As String = "")
        _Domain = domain
        Account = If(properties("samaccountname").Count = 1 AndAlso properties("samaccountname")(0) IsNot Nothing, properties("samaccountname")(0).ToString(), String.Empty)
        _rawProperties.Add("samaccountname", Account)

        Name = If(properties("name").Count = 1 AndAlso properties("name")(0) IsNot Nothing, properties("name")(0).ToString(), String.Empty)
        _rawProperties.Add("name", Name)

        FirstName = If(properties("givenName").Count = 1 AndAlso properties("givenName")(0) IsNot Nothing, properties("givenName")(0).ToString(), String.Empty)
        _rawProperties.Add("givenName", Name)

        LastName = If(properties("sn").Count = 1 AndAlso properties("sn")(0) IsNot Nothing, properties("sn")(0).ToString(), String.Empty)
        _rawProperties.Add("sn", Name)

        DisplayName = If(properties("displayName").Count = 1 AndAlso properties("displayName")(0) IsNot Nothing, properties("displayName")(0).ToString(), String.Empty)
        _rawProperties.Add("displayName", DisplayName)

        Email = If(properties("mail").Count = 1 AndAlso properties("mail")(0) IsNot Nothing, properties("mail")(0).ToString(), String.Empty)
        _rawProperties.Add("mail", Email)

        DistinguishedName = If(properties("distinguishedName").Count = 1 AndAlso properties("distinguishedName")(0) IsNot Nothing, properties("distinguishedName")(0).ToString(), String.Empty)
        _rawProperties.Add("distinguishedName", DistinguishedName)

        TelephoneNumber = If(properties("telephonenumber").Count = 1 AndAlso properties("telephonenumber")(0) IsNot Nothing, properties("telephonenumber")(0).ToString(), String.Empty)
        _rawProperties.Add("telephonenumber", TelephoneNumber)

        SchemaClassName = If(properties("schemaClassName").Count = 1 AndAlso properties("schemaClassName")(0) IsNot Nothing, properties("schemaClassName")(0).ToString(), String.Empty)
        _rawProperties.Add("schemaClassName", SchemaClassName)

        Company = If(properties("company").Count = 1 AndAlso properties("company")(0) IsNot Nothing, properties("company")(0).ToString(), String.Empty)
        _rawProperties.Add("company", Company)

        Description = If(properties("Description").Count = 1 AndAlso properties("Description")(0) IsNot Nothing, properties("Description")(0).ToString(), String.Empty)
        _rawProperties.Add("Description", Description)

        PrincipalName = If(properties("msDS-PrincipalName").Count = 1 AndAlso properties("msDS-PrincipalName")(0) IsNot Nothing, properties("msDS-PrincipalName")(0).ToString(), String.Empty)
        _rawProperties.Add("msDS-PrincipalName", PrincipalName)

        If properties.Contains(DocSuiteContext.Current.ProtocolEnv.ADDisplayProperty) Then
            Name = properties(DocSuiteContext.Current.ProtocolEnv.ADDisplayProperty)(0).ToString()
        End If
    End Sub
#End Region

#Region " Methods "

    Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
        Return String.Compare(Account, CType(obj, AccountModel).Account)
    End Function

    ''' <summary> Restituisce un oggetto normalizzato (con i campi troncati come sul DB) per la serializzazione </summary>
    Public Function GetContact(ByVal addFiscal As Boolean) As Contact
        Dim contact As New Contact()
        contact.Description = _Name
        contact.EmailAddress = _Email
        contact.TelephoneNumber = _TelephoneNumber
        contact.Note = _Parent
        If addFiscal Then
            contact.FiscalCode = _Account
        Else
            contact.Code = _Account
        End If
        contact.ContactType = New ContactType(ContactType.AdAmPerson)

        Return contact
    End Function

    Public Function GetLabel() As String
        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled Then
            Return String.Format("{0}\{1} - ({2})", Domain, Account, Name)
        End If
        Return String.Format("{0} - ({1})", Account, Name)
    End Function

    Public Function GetFullUserName() As String
        Return String.Format("{0}\{1}", Domain, Account)
    End Function

#End Region

End Class
