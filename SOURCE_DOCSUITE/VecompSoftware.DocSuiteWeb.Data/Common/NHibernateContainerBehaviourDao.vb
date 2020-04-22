Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateContainerBehaviourDao
    Inherits BaseNHibernateDao(Of ContainerBehaviour)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetBehaviours(ByVal container As Container, action As ContainerBehaviourAction, attributeName As String) As IList(Of ContainerBehaviour)
        criteria = NHibernateSession.CreateCriteria(Of ContainerBehaviour)("CB")

        criteria.Add(Restrictions.Eq("CB.Container", container))
        criteria.Add(Restrictions.Eq("CB.Action", action))

        If Not String.IsNullOrEmpty(attributeName) Then
            criteria.Add(Restrictions.Eq("CB.AttributeName", attributeName))
        End If
        criteria.AddOrder(Order.Asc("CB.Id"))
        Return criteria.List(Of ContainerBehaviour)()
    End Function

End Class
