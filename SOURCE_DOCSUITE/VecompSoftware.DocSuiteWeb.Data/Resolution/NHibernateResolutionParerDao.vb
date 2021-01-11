Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateResolutionParerDao
    Inherits BaseNHibernateDao(Of ResolutionParer)


    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function Exists(id As Integer) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "RP")

        criteria.Add(Restrictions.Eq("RP.Id", id))

        Dim items As IList(Of ResolutionParer) = criteria.List(Of ResolutionParer)()

        If items.Count = 1 Then
            Return True
        End If
        If items.Count = 0 Then
            Return False
        End If

        ' In caso ci sia più di un record lancio l'eccezione
        Throw New DocSuiteException("Errore in recupero ResolutionParer per Id=" & id, "Sono presenti piu' record di ResolutionParer")

    End Function

    Public Function GetByResolution(resolution As Resolution) As ResolutionParer
        Return GetById(resolution.Id, False)
    End Function

End Class
