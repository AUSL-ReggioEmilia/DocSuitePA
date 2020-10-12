Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateProtocolParerDao
    Inherits BaseNHibernateDao(Of ProtocolParer)

#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

    Public Function GetByProtocol(protocol As Protocol) As ProtocolParer
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Protocol", "P")
        criteria.Add(Restrictions.Eq("P.Id", protocol.Id))
        Return criteria.UniqueResult(Of ProtocolParer)()
    End Function

    Public Function GetByProtocol(year As Short, number As Integer) As ProtocolParer
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Protocol", "P")
        criteria.Add(Restrictions.Eq("P.Year", year))
        criteria.Add(Restrictions.Eq("P.Number", number))
        Return criteria.UniqueResult(Of ProtocolParer)()
    End Function

    Public Function ExistsProtocol(idProtocol As Guid) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "PP")
        criteria.CreateAlias("PP.Protocol", "P")
        criteria.Add(Restrictions.Eq("P.Id", idProtocol))
        criteria.SetProjection(Projections.Count("PP.Id"))
        Dim results As Integer = criteria.UniqueResult(Of Integer)()

        If results > 1 Then
            ' In caso ci sia più di un record lancio l'eccezione
            Throw New DocSuiteException("ProtocolParer", String.Format("Errore in recupero ProtocolParer per Id Protocollo [{0}].", idProtocol))
        End If
        Return results = 1
    End Function

    Public Function ExistsProtocol(year As Short, number As Integer) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "PP")
        criteria.CreateAlias("PP.Protocol", "P")
        criteria.Add(Restrictions.Eq("P.Year", year))
        criteria.Add(Restrictions.Eq("P.Number", number))
        criteria.SetProjection(Projections.Count("PP.Id"))
        Dim results As Integer = criteria.UniqueResult(Of Integer)()

        If results > 1 Then
            ' In caso ci sia più di un record lancio l'eccezione
            Throw New DocSuiteException("ProtocolParer", String.Format("Errore in recupero ProtocolParer per il Protocollo con anno [{0}] e numero [{1}].", year, number))
        End If
        Return results = 1
    End Function

End Class
