Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao
Imports System.Linq

Public Class NHibernatePosteOnlineContactDao
    Inherits BaseNHibernateDao(Of POLRequestContact)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetRecipientWithRequestId(ByVal requestId As Guid) As POLRequestRecipient
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("RequestId", requestId))
        Dim contacts As IList(Of POLRequestContact) = criteria.List(Of POLRequestContact)()

        Return contacts.
           Where(Function(c As POLRequestContact) TypeOf c Is POLRequestRecipient).
           Cast(Of POLRequestRecipient)().
           FirstOrDefault()

    End Function
End Class
