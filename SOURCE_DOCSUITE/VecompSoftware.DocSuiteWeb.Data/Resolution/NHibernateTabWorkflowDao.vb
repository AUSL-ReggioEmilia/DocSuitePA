Imports System.Collections.Generic
Imports System.Linq
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao
Imports VecompSoftware.Helpers.ExtensionMethods


Public Class NHibernateTabWorkflowDao
    Inherits BaseNHibernateDao(Of TabWorkflow)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByStep(ByVal workflowType As String, ByVal searchStep As Short, ByRef workflow As TabWorkflow) As Boolean
        criteria = NHibernateSession.CreateCriteria(Of TabWorkflow)()
        criteria.Add(Restrictions.Eq("Id.ResStep", searchStep))
        criteria.Add(Restrictions.Eq("Id.WorkflowType", workflowType))

        Dim list As IList(Of TabWorkflow) = criteria.List(Of TabWorkflow)()
        If list.HasSingle() Then
            workflow = list.First()
        End If

        Return list.Count > 0
    End Function

    Public Function GetAllNextStep(ByVal workflowType As String, ByVal activeStep As Short, ByRef tabWorkflows As IList(Of TabWorkflow)) As Boolean

        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Gt("Id.ResStep", activeStep))
        criteria.Add(Restrictions.Eq("Id.WorkflowType", workflowType))

        criteria.AddOrder(Order.Asc("Id.ResStep"))

        tabWorkflows = criteria.List(Of TabWorkflow)()


        Return tabWorkflows.Count > 0
    End Function

    Function GetByResolutionType(ByVal resolutionType As Short) As IList(Of TabWorkflow)
        Dim detach As DetachedCriteria = DetachedCriteria.For(GetType(TabMaster))
        detach.Add(Restrictions.Eq("Id.Configuration", DocSuiteContext.Current.ResolutionEnv.Configuration))
        detach.Add(Restrictions.Eq("Id.ResolutionType", resolutionType))
        detach.SetProjection(Projections.Property("WorkflowType"))

        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Subqueries.PropertyIn("Id.WorkflowType", detach))

        Return criteria.List(Of TabWorkflow)()
    End Function

    Function GetByResolution(ByVal idResolution As Integer) As TabWorkflow
        Dim sqlQuery As String = " SELECT TW.* FROM TabWorkflow AS TW" & _
                                 " LEFT JOIN ResolutionWorkflow AS RW ON RW.STEP = TW.STEP" & _
                                 " LEFT JOIN Resolution AS R ON R.IdResolution = RW.IdResolution AND R.WorkflowType = TW.WorkflowType" & _
                                 " WHERE R.IdResolution = :IdResl" & _
                                 " AND RW.IsActive = :IsActive"
        Dim query As IQuery = NHibernateSession.CreateSQLQuery(sqlQuery).AddEntity("TabWorkflow", GetType(TabWorkflow)) _
                            .SetInt32("IdResl", idResolution) _
                            .SetInt32("IsActive", 1)
        Return query.UniqueResult(Of TabWorkflow)()
    End Function

    Function GetByDescription(ByVal description As String, ByVal workflowType As String) As TabWorkflow
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Description", description))
        criteria.Add(Restrictions.Eq("Id.WorkflowType", workflowType))
        criteria.AddOrder(Order.Asc("Id.ResStep"))

        Return criteria.List(Of TabWorkflow)().First()
    End Function

    Function GetItemsByDescription(ByVal description As String) As IList(Of TabWorkflow)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Description", description))

        Return criteria.List(Of TabWorkflow)()
    End Function


End Class
