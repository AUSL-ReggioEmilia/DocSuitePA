Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports VecompSoftware.NHibernateManager

Public Class NHibernateTaskDetailFinder
    Inherits NHibernateBaseFinder(Of TaskDetail, TaskDetail)

    Public Sub New()
        SessionFactoryName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
        EnablePaging = True
    End Sub

    Public Property EnablePaging As Boolean
    Public Property TaskHeader As TaskHeader

    Protected ReadOnly Property NHibernateSession As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of TaskDetail)("TD")
        Return criteria
    End Function

    Public Overloads Overrides Function DoSearch() As IList(Of TaskDetail)
        Dim criteria As ICriteria = CreateCriteria()
        SetPaging(criteria)
        DecorateCriteria(criteria)
        AttachSortExpressions(criteria)
        AttachFilterExpressions(criteria)

        Return criteria.List(Of TaskDetail)()
    End Function


    Protected Sub SetPaging(ByRef criteria As ICriteria)
        If Not EnablePaging Then
            Return
        End If

        criteria.SetFirstResult(PageIndex)
        criteria.SetMaxResults(PageSize)
    End Sub

    Protected Overridable Sub DecorateCriteria(ByRef criteria As ICriteria)

        If TaskHeader IsNot Nothing Then
            criteria.Add(Restrictions.Eq("TD.TaskHeader.Id", TaskHeader.Id))
        End If

    End Sub

    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        AttachFilterExpressions(criteria)
        criteria.SetProjection(Projections.CountDistinct("TD.Id"))
        Return criteria.UniqueResult(Of Integer)()
    End Function
End Class
