Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateResolutionWorkflowDao
    Inherits BaseNHibernateDao(Of ResolutionWorkflow)

#Region " Constructor "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    ''' <summary> Torna l'incrementale massimo correntemente legato all'atto. </summary>
    ''' <param name="idResolution">Atto per il quale ritirare l'incrementale</param>
    ''' <param name="isactive">Indica se cercare tra gli incrementali attivi o meno</param>
    ''' <returns>Se non trova nulla torna 0</returns>
    Public Function GetActiveIncremental(ByVal idResolution As Integer, ByVal isactive As Short) As Short
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Id.IdResolution", idResolution))
        criteria.Add(Restrictions.Eq("IsActive", isactive))
        criteria.SetProjection(Projections.Max("Id.Incremental"))

        Dim result As Object = criteria.UniqueResult()
        If result IsNot Nothing Then
            Return CType(result, Short)
        End If
        Return 0S
    End Function

    Public Function GetActiveStep(ByVal idResolution As Integer) As Short
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Id.IdResolution", idResolution))
        criteria.Add(Restrictions.Eq("IsActive", 1S))
        criteria.SetProjection(Projections.Max("ResStep"))

        Dim result As Object = criteria.UniqueResult()
        If result IsNot Nothing Then
            Return CType(result, Short)
        End If
        Return 0S
    End Function

    Public Function GetMaxIncremental(ByVal idResolution As Integer) As Short
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Id.IdResolution", idResolution))
        criteria.SetProjection(Projections.Max("Id.Incremental"))

        Dim result As Object = criteria.UniqueResult()
        If result IsNot Nothing Then
            Return DirectCast(result, Short)
        End If
        Return 0S
    End Function

    Public Function SqlResolutionWorkflowSearch(ByVal idResolution As Integer, ByVal SearchStep As Short, Optional ByVal ActiveStep As Boolean = True) As ResolutionWorkflow

        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Id.IdResolution", idResolution))

        If (SearchStep <> 0) Then
            criteria.Add(Restrictions.Eq("ResStep", SearchStep))
        End If
        If (ActiveStep) Then
            criteria.Add(Restrictions.Eq("IsActive", 1S))
        End If

        criteria.SetMaxResults(1)
        criteria.AddOrder(Order.Desc("Id.Incremental"))

        Return criteria.UniqueResult(Of ResolutionWorkflow)()

    End Function

    Public Function GetAllByResolution(ByVal idResolution As Integer, Optional ByVal ActiveStep As Boolean = False) As IList(Of ResolutionWorkflow)

        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Id.IdResolution", idResolution))

        If (ActiveStep) Then
            criteria.Add(Restrictions.Eq("IsActive", 1S))
        End If

        criteria.Add(Restrictions.Not(Restrictions.Eq("IsActive", 2S)))

        criteria.AddOrder(Order.Asc("ResStep"))

        Return criteria.List(Of ResolutionWorkflow)()

    End Function

    Public Function GetByResolutionAndDescription(ByVal idResolution As Integer, ByVal WorkflowType As String, ByVal StepDescription As String) As ResolutionWorkflow

        Dim qry As IQuery = NHibernateSession.CreateQuery("select rw " & _
                "from ResolutionWorkflow rw, TabWorkflow tw " & _
                "where rw.Resolution.Id = ? And rw.IsActive <> 2 " & _
                "and tw.Id.WorkflowType = ? and tw.Description = ? and tw.Id.ResStep = rw.ResStep")

        qry.SetParameter(0, idResolution)
        qry.SetParameter(1, WorkflowType)
        qry.SetParameter(2, StepDescription)
        qry.SetMaxResults(1)

        Return qry.UniqueResult(Of ResolutionWorkflow)()

    End Function

#End Region

End Class