Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateProtocolLinkDao
    Inherits BaseNHibernateDao(Of ProtocolLink)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetProtocolLink(protocolParent As Protocol, protocolChild As Protocol) As ProtocolLink
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Protocol", "P")
        criteria.CreateAlias("ProtocolLinked", "PL")
        criteria.Add(Restrictions.Eq("P.Id", protocolParent.Id))
        criteria.Add(Restrictions.Eq("PL.Id", protocolChild.Id))
        Return criteria.UniqueResult(Of ProtocolLink)()
    End Function

End Class

