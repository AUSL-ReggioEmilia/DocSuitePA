Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion

Public Class NHibernateCollaborationLogDao
    Inherits BaseNHibernateDao(Of CollaborationLog)

    Public Function GetMaxId() As Integer
        Dim query As String = "SELECT MAX(CL.Id) FROM CollaborationLog AS CL"

        Try
            Return NHibernateSession.CreateQuery(query).UniqueResult(Of Integer)()
        Catch ex As Exception
            Return 0
        End Try

    End Function

    Public Function IsInsertedByCurrentProgram(collaboration As Collaboration) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of CollaborationLog)()
        criteria.Add(Restrictions.Eq("IdCollaboration", collaboration.Id))
        criteria.Add(Restrictions.Eq("Program", DocSuiteContext.Program))
        criteria.Add(Restrictions.Eq("LogDescription", "Insert"))
        criteria.SetProjection(Projections.Constant(True))
        criteria.SetMaxResults(1)
        Return criteria.List(Of Boolean).Count > 0
    End Function

End Class
