Imports VecompSoftware.Helpers.ExtensionMethods

<Serializable()> _
Public Class ContainerExtensionCollection
    Inherits List(Of ContainerExtension)
    Implements IContainerExtensionCollection

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByRef list As IList(Of ContainerExtension))
        MyBase.New(list)
    End Sub

    Public Function FilterType(ByVal pType As ContainerExtensionType) As IContainerExtensionCollection Implements IContainerExtensionCollection.FilterType
        Dim vList As New ContainerExtensionCollection
        For Each vExt As ContainerExtension In Me
            If vExt.KeyType.Eq(pType.ToString()) Then
                vList.Add(vExt)
            End If
        Next
        Return vList
    End Function

End Class