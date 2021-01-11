Imports VecompSoftware.DocSuiteWeb.Data
Imports System.DirectoryServices
Imports System.Linq

''' <summary> Utente di Active Directory </summary>
Public Class ADContact
    Implements IComparable

#Region " Property "

    Public ReadOnly DisplayName As String

    Public ReadOnly Email As String

    Public ReadOnly DistinguishedName As String

    Public ReadOnly Property DisplayEmail As String
        Get
            Return String.Format("{0} <{1}>", DisplayName, Email)
        End Get

    End Property


    Public ReadOnly RawProperties As ResultPropertyCollection

#End Region

#Region " Constructor "

    ''' <summary> Inizializza un nuovo <see>ADUser</see> </summary>
    Public Sub New(ByRef properties As ResultPropertyCollection)
        DisplayName = If(properties("displayName").Count = 1 AndAlso properties("displayName")(0) IsNot Nothing, properties("displayName")(0).ToString(), String.Empty)
        Email = If(properties("mail").Count = 1 AndAlso properties("mail")(0) IsNot Nothing, properties("mail")(0).ToString(), String.Empty)
        DistinguishedName = If(properties("distinguishedName").Count = 1 AndAlso properties("distinguishedName")(0) IsNot Nothing, properties("distinguishedName")(0).ToString(), String.Empty)
        RawProperties = properties
    End Sub

#End Region

#Region " Methods "

    Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
        Return String.Compare(DistinguishedName, CType(obj, ADContact).DistinguishedName)
    End Function

    ''' <summary> Restituisce un oggetto normalizzato (con i campi troncati come sul DB) per la serializzazione </summary>
    Public Function GetContact() As Contact
        Dim contact As New Contact()
        contact.Description = DisplayName
        contact.EmailAddress = Email
        contact.ContactType = New ContactType(ContactType.Person)

        Return contact
    End Function

#End Region

End Class
