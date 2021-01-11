
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class MessageContactEmailFacade
    Inherits BaseProtocolFacade(Of MessageContactEmail, Integer, NHibernateMessageContactEmailDAO)

    Public Overrides Sub Save(ByRef contactEmail As MessageContactEmail)
        contactEmail.Email = If(String.IsNullOrEmpty(contactEmail.Email), String.Empty, contactEmail.Email.Trim())
        MyBase.Save(contactEmail)
    End Sub

    Public Function GetByContact(contact As MessageContact) As MessageContactEmail
        Return _dao.GetByContact(contact)
    End Function

    ''' <summary> Genera un contatto e-mail per l'invio tramite il modulo messaggi </summary>
    ''' <param name="description">Descrizione del contatto</param>
    ''' <param name="user">Utente che ha registrato l'attività</param>
    ''' <param name="email">E-mail del contatto</param>
    ''' <param name="position">Tipologia di contatto e-mail</param>
    Public Function CreateEmailContact(description As String, user As String, email As String, position As MessageContact.ContactPositionEnum, Optional type As MessageContact.ContactTypeEnum = MessageContact.ContactTypeEnum.User) As MessageContactEmail
        Dim contact As New MessageContact(type, description, position)
        Return New MessageContactEmail(contact, user, email, description)
    End Function

    Function GetByMessages(messages As IList(Of DSWMessage), ByVal contactPositions As IList(Of MessageContact.ContactPositionEnum)) As IList(Of MessageContactEmailHeader)
        Return _dao.GetByMessages(messages, contactPositions)
    End Function


End Class
