Imports System.Linq
Imports System.Runtime.CompilerServices

Public Module OChartItemContainerExtensions

    <Extension()>
    Public Function ReplicateList(ByVal source As IEnumerable(Of OChartItemContainer), item As OChartItem) As IEnumerable(Of OChartItemContainer)
        Return source.Select(Function(c) New OChartItemContainer(c) With {.Item = item})
    End Function
    <Extension()>
    Public Function ReplicateList(ByVal source As IEnumerable(Of OChartItemContainer)) As IEnumerable(Of OChartItemContainer)
        Return source.ReplicateList(Nothing)
    End Function
    <Extension()>
    Public Function FindByResource(source As IEnumerable(Of OChartItemContainer), container As Container) As IEnumerable(Of OChartItemContainer)
        Return source.Where(Function(c) c.Container.Id.Equals(container.Id))
    End Function

    <Extension()>
    Public Function HasResource(source As IEnumerable(Of OChartItemContainer), container As Container) As Boolean
        Return source.Any(Function(c) c.Container.Id.Equals(container.Id))
    End Function


End Module
