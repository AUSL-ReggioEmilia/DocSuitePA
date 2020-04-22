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
        Return GetById(protocol.Id, False)
    End Function

    Public Function Exists(id As YearNumberCompositeKey) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "PP")

        criteria.Add(Restrictions.Eq("PP.Id.Year", id.Year))
        criteria.Add(Restrictions.Eq("PP.Id.Number", id.Number))

        Dim items As IList(Of ProtocolParer) = criteria.List(Of ProtocolParer)()
        If items.Count = 1 Then
            Return True
        End If
        If items.Count = 0 Then
            Return False
        End If

        ' In caso ci sia più di un record lancio l'eccezione
        Throw New DocSuiteException("ProtocolParer", String.Format("Errore in recupero ProtocolParer per Year [{0}] e Number [{1}].", id.Year, id.Number))
    End Function

End Class
