<Serializable()>
Public Class ContactList
    Inherits AuditableDomainObject(Of Guid)


#Region " Properties "

    Public Overridable Property Name As String

    Public Overridable Property Contacts As IList(Of Contact)

#End Region

#Region " Constructor "

    Public Sub New()
        Contacts = New List(Of Contact)
    End Sub
#End Region



End Class
