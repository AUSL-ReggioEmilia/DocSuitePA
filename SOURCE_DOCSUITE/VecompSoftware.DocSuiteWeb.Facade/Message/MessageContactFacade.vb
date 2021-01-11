
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()>
Public Class MessageContactFacade
    Inherits BaseProtocolFacade(Of MessageContact, Integer, NHibernateMessageContactDAO)


    Public Overrides Sub Save(ByRef message As MessageContact)
        If message.ContactEmails IsNot Nothing Then
            For Each contactEmail As MessageContactEmail In message.ContactEmails
                contactEmail.Email = If(String.IsNullOrEmpty(contactEmail.Email), String.Empty, contactEmail.Email.Trim())
            Next
        End If
        MyBase.Save(message)
    End Sub

    Public Function GetByMessage(message As DSWMessage) As IList(Of MessageContact)
        Return _dao.GetByMessage(message)
    End Function

    Public Function GetByMessage(message As DSWMessage, position As MessageContact.ContactPositionEnum) As IList(Of MessageContact)
        Return _dao.GetByMessage(message, position)
    End Function

End Class
