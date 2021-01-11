Imports NHibernate.Collection.Generic
Imports NHibernate.Engine

<Serializable()> _
Public Class NHContainerExtensionCollection
    Inherits PersistentGenericBag(Of ContainerExtension)
    Implements IContainerExtensionCollection

    Public Sub New(ByVal session As ISessionImplementor)
        MyBase.New(session)
    End Sub

    Public Sub New(ByVal session As ISessionImplementor, ByVal coll As IList(Of ContainerExtension))
        MyBase.New(session, coll)
    End Sub

    Public Function FilterType(ByVal pType As ContainerExtensionType) As IContainerExtensionCollection Implements IContainerExtensionCollection.FilterType
        Return New ContainerExtensionCollection(Me).FilterType(pType)
    End Function

End Class
