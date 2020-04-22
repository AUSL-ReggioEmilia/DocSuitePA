Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate.Transform
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate.Criterion

Public Class NHibernateMessageContactEmailDAO
    Inherits BaseNHibernateDao(Of MessageContactEmail)
    Implements IMessageContactEmailDao

    Public Function GetByContact(contact As MessageContact) As MessageContactEmail Implements IMessageContactEmailDao.GetByContact
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("MessageContact", contact))
        Return criteria.UniqueResult(Of MessageContactEmail)()

    End Function
    Public Function GetByMessage(ByVal message As DSWMessage, ByVal contactPosition As MessageContact.ContactPositionEnum) As MessageContactEmailHeader
        Dim messages As IList(Of DSWMessage) = New List(Of DSWMessage)(1)
        Dim contactPositions As IList(Of MessageContact.ContactPositionEnum) = New List(Of MessageContact.ContactPositionEnum)(1)
        messages.Add(message)
        contactPositions.Add(contactPosition)
        Return GetByMessages(messages, contactPositions).FirstOrDefault()
    End Function

    Public Function GetByMessages(ByVal messages As IList(Of DSWMessage), ByVal contactPositions As IList(Of MessageContact.ContactPositionEnum)) As IList(Of MessageContactEmailHeader)
        criteria = NHibernateSession.CreateCriteria(persitentType, "MCE")
        criteria.CreateAlias("MCE.MessageContact", "MC")
        If Not messages.IsNullOrEmpty() Then
            criteria.Add(Restrictions.In("MC.Message.Id", messages.Select(Function(m) m.Id).ToArray()))
        End If
        If Not contactPositions.IsNullOrEmpty() Then
            criteria.Add(Restrictions.In("MC.ContactPosition", contactPositions.ToArray()))
        End If
        criteria.SetProjection(Projections.ProjectionList() _
                               .Add(Projections.Property("MCE.Email"), "Email") _
                               .Add(Projections.Property("MCE.Description"), "Description") _
                               .Add(Projections.Property("MC.Message.Id"), "IdMessage") _
                               .Add(Projections.Property("MC.ContactPosition"), "ContactPosition"))
        criteria.SetResultTransformer(Transformers.AliasToBean(Of MessageContactEmailHeader))
        Return criteria.List(Of MessageContactEmailHeader)()
    End Function
End Class
