Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports VecompSoftware.NHibernateManager

Public Class NHibernateTaskHeaderFinder
    Inherits NHibernateBaseFinder(Of TaskHeader, TaskHeader)

    Public Sub New()
        SessionFactoryName = "ProtDB"
        EnablePaging = True
    End Sub

    Public Property EnablePaging As Boolean
    Public Property TaskType As TaskTypeEnum?

    Protected ReadOnly Property NHibernateSession As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of TaskHeader)("TH")
        Return criteria
    End Function

    Public Overloads Overrides Function DoSearch() As IList(Of TaskHeader)
        Dim criteria As ICriteria = CreateCriteria()
        SetPaging(criteria)
        DecorateCriteria(criteria)
        AttachSortExpressions(criteria)
        AttachFilterExpressions(criteria)

        Return criteria.List(Of TaskHeader)()
    End Function

    Protected Sub SetPaging(ByRef criteria As ICriteria)
        If Not EnablePaging Then
            Return
        End If

        criteria.SetFirstResult(PageIndex)
        criteria.SetMaxResults(PageSize)
    End Sub

    Protected Overridable Sub DecorateCriteria(ByRef criteria As ICriteria)

        If TaskType.HasValue Then
            criteria.Add(Restrictions.Eq("TH.TaskType", TaskType.Value))
        End If

    End Sub

    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        AttachFilterExpressions(criteria)
        criteria.SetProjection(Projections.CountDistinct("TH.Id"))
        Return criteria.UniqueResult(Of Integer)()
    End Function

End Class
