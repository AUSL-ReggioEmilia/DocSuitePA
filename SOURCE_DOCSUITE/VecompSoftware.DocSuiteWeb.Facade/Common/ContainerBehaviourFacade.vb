Imports VecompSoftware.DocSuiteWeb.Data

Public Class ContainerBehaviourFacade
    Inherits BaseProtocolFacade(Of ContainerBehaviour, Integer, NHibernateContainerBehaviourDao)

    Public Function GetBehaviours(ByVal container As Container, action As ContainerBehaviourAction, attributeName As String) As IList(Of ContainerBehaviour)
        Return _dao.GetBehaviours(container, action, attributeName)
    End Function

End Class
