Imports System.Runtime.CompilerServices
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.DocSuiteWeb.Data

Public Module ContainerDTOEx

    <Extension()>
    Public Function CopyFrom(source As ContainerDTO, container As Container) As ContainerDTO
        source.Id = container.Id
        source.Name = container.Name
        Return source
    End Function

    <Extension()>
    Public Function CopyFrom(source As AccountingSectionalDTO, container As ContainerExtension) As AccountingSectionalDTO
        source.Id = container.AccountingSectionalNumber
        source.Name = container.KeyValue
        source.Container = New ContainerDTO().CopyFrom(container.Container)
        Return source
    End Function


End Module