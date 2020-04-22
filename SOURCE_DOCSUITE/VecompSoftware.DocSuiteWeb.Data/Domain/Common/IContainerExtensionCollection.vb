Public Interface IContainerExtensionCollection
    Inherits IList(Of ContainerExtension)

    Function FilterType(ByVal type As ContainerExtensionType) As IContainerExtensionCollection

End Interface