Imports System.DirectoryServices
Imports System.DirectoryServices.AccountManagement
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data

''' <summary> Utente di Active Directory o di Workgroup di Windows</summary>
<Serializable()>
Public Class AccountModel
    Implements IComparable

#Region "[ Fields ]"
    Private Shared _serializerSettings As JsonSerializerSettings = New JsonSerializerSettings() With {
            .NullValueHandling = NullValueHandling.Ignore,
            .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            .PreserveReferencesHandling = PreserveReferencesHandling.All,
            .Formatting = Formatting.Indented}

    Private _jsonFormat As String
#End Region

#Region " Property "
    Public Property PrincipalName As String
    Public Property Domain As String
    Public Property FirstName As String
    Public Property LastName As String
    Public Property Name As String
    Public Property DisplayName As String
    Public Property Account As String
    Public Property TelephoneNumber As String
    Public Property Email As String
    Public Property SchemaClassName As String
    Public Property DistinguishedName As String
    Public Property Company As String
    Public Property Description As String
    Public Property RelatedGroupName As String

    Public ReadOnly Property JsonFormat As String
        Get
            Return _jsonFormat
        End Get
    End Property

#End Region

#Region " Constructor "

    Public Sub New()
    End Sub

    ''' <summary> Inizializza un nuovo <see>ADUser</see> </summary>
    Public Sub New(ByVal account As String, ByVal name As String, Optional domain As String = "", Optional displayName As String = "")
        _Account = account
        _Name = name
        If String.IsNullOrEmpty(_Name) Then
            _Name = _Account
        End If
        _Domain = domain
        _DisplayName = displayName
        _jsonFormat = String.Empty
    End Sub
    Public Sub New(ByRef user As UserPrincipal, Optional domain As String = "")
        _Domain = domain
        Account = If(user.SamAccountName, String.Empty)
        Name = If(user.Name, String.Empty)
        FirstName = If(user.GivenName, String.Empty)
        LastName = If(user.Surname, String.Empty)
        DisplayName = If(user.DisplayName, String.Empty)
        Email = If(user.EmailAddress, String.Empty)
        DistinguishedName = If(user.DistinguishedName, String.Empty)
        TelephoneNumber = If(user.VoiceTelephoneNumber, String.Empty)
        SchemaClassName = String.Empty
        Company = String.Empty
        Description = If(user.Description, String.Empty)
        PrincipalName = If(user.UserPrincipalName, String.Empty)
        _jsonFormat = JsonConvert.SerializeObject(user, _serializerSettings)
    End Sub

    Public Sub New(ByRef properties As ResultPropertyCollection, Optional domain As String = "")
        _Domain = domain
        Account = If(properties("samaccountname").Count = 1 AndAlso properties("samaccountname")(0) IsNot Nothing, properties("samaccountname")(0).ToString(), String.Empty)
        Name = If(properties("name").Count = 1 AndAlso properties("name")(0) IsNot Nothing, properties("name")(0).ToString(), String.Empty)
        FirstName = If(properties("givenName").Count = 1 AndAlso properties("givenName")(0) IsNot Nothing, properties("givenName")(0).ToString(), String.Empty)
        LastName = If(properties("sn").Count = 1 AndAlso properties("sn")(0) IsNot Nothing, properties("sn")(0).ToString(), String.Empty)
        DisplayName = If(properties("displayName").Count = 1 AndAlso properties("displayName")(0) IsNot Nothing, properties("displayName")(0).ToString(), String.Empty)
        Email = If(properties("mail").Count = 1 AndAlso properties("mail")(0) IsNot Nothing, properties("mail")(0).ToString(), String.Empty)
        DistinguishedName = If(properties("distinguishedName").Count = 1 AndAlso properties("distinguishedName")(0) IsNot Nothing, properties("distinguishedName")(0).ToString(), String.Empty)
        TelephoneNumber = If(properties("telephonenumber").Count = 1 AndAlso properties("telephonenumber")(0) IsNot Nothing, properties("telephonenumber")(0).ToString(), String.Empty)
        SchemaClassName = If(properties("schemaClassName").Count = 1 AndAlso properties("schemaClassName")(0) IsNot Nothing, properties("schemaClassName")(0).ToString(), String.Empty)
        Company = If(properties("company").Count = 1 AndAlso properties("company")(0) IsNot Nothing, properties("company")(0).ToString(), String.Empty)
        Description = If(properties("Description").Count = 1 AndAlso properties("Description")(0) IsNot Nothing, properties("Description")(0).ToString(), String.Empty)
        PrincipalName = If(properties("msDS-PrincipalName").Count = 1 AndAlso properties("msDS-PrincipalName")(0) IsNot Nothing, properties("msDS-PrincipalName")(0).ToString(), String.Empty)
        PrincipalName = If(String.IsNullOrEmpty(PrincipalName) AndAlso properties("userPrincipalName").Count = 1 AndAlso properties("userPrincipalName")(0) IsNot Nothing, properties("userPrincipalName")(0).ToString(), String.Empty)
        If properties.Contains(DocSuiteContext.Current.ProtocolEnv.ADDisplayProperty) Then
            Name = properties(DocSuiteContext.Current.ProtocolEnv.ADDisplayProperty)(0).ToString()
        End If
        _jsonFormat = JsonConvert.SerializeObject(properties, _serializerSettings)
    End Sub
#End Region

#Region " Methods "

    Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
        Return String.Compare(Account, CType(obj, AccountModel).Account)
    End Function

    ''' <summary> Restituisce un oggetto normalizzato (con i campi troncati come sul DB) per la serializzazione </summary>
    Public Function GetContact(ByVal addFiscal As Boolean) As Contact
        Dim contact As New Contact With {
            .Description = _Name,
            .EmailAddress = _Email,
            .TelephoneNumber = _TelephoneNumber,
            .Note = PrincipalName
        }
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
            Return $"{Domain}\{Account} - ({Name})"
        End If
        Return $"{Account} - ({Name})"
    End Function

    Public Function GetFullUserName() As String
        Return $"{Domain}\{Account}"
    End Function

#End Region

End Class
