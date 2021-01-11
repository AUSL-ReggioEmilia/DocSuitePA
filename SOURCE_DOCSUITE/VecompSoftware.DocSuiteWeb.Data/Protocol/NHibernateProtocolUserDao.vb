Imports System.Linq
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateProtocolUserDao
    Inherits BaseNHibernateDao(Of ProtocolUser)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetProtocolUserByProtocol(protocolId As Guid, accountUser As String) As ProtocolUser
        Dim puCriteria As ICriteria = NHibernateSession.CreateCriteria(Of ProtocolUser)
        puCriteria.Add(Restrictions.Eq("Protocol.Id", protocolId))
        puCriteria.Add(Restrictions.Eq("Account", accountUser))

        Dim pu As ProtocolUser = puCriteria.UniqueResult(Of ProtocolUser)()
        Return pu
    End Function

End Class
