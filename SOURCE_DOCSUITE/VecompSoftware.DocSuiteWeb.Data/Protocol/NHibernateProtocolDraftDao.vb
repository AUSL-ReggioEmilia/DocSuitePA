Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateProtocolDraftDao
    Inherits BaseNHibernateDao(Of ProtocolDraft)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByCollaboration(coll As Collaboration) As ProtocolDraft
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Collaboration.Id", coll.Id))
        Return criteria.UniqueResult(Of ProtocolDraft)()
    End Function

End Class
