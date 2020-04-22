Imports VecompSoftware.DocSuiteWeb.Data

<Serializable()>
Public Class MessageContactEmail
    Inherits AuditableDomainObject(Of Int32)

#Region " Properties "

    Public Overridable Property MessageContact As MessageContact

    Public Overridable Property User As String

    Public Overridable Property Email As String

    Public Overridable Property Description As String

#End Region

#Region " Constructors "

    Public Sub New()

    End Sub

    Public Sub New(messageContact As MessageContact, user As String, email As String, description As String)
        Me.MessageContact = messageContact
        Me.User = user
        If String.IsNullOrEmpty(email) Then
            Me.Email = String.Empty
        Else
            Me.Email = email.Trim()
        End If

        Me.Description = description
    End Sub

#End Region

#Region " Methods "

    Public Overrides Function ToString() As String
        Return String.Format("{0} <{1}>", If(Not String.IsNullOrEmpty(Description), Description, String.Empty), Email)
    End Function

#End Region

End Class
