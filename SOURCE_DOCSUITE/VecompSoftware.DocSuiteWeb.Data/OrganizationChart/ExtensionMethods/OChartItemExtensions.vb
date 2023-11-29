Imports System.Linq
Imports System.Runtime.CompilerServices

Public Module OChartItemExtensions

    <Extension()>
    Public Function ReplicateList(ByVal source As IEnumerable(Of OChartItem), parentItem As OChartItem, oChart As OChart, userName As String) As IEnumerable(Of OChartItem)

        Return source.Select(Function(i) New OChartItem(i, parentItem, oChart, userName))
    End Function

    <Extension()>
    Public Function ReplicateListAsDTO(ByVal source As IEnumerable(Of OChartItem), parentDTO As OChartItemDTO) As IEnumerable(Of OChartItemDTO)
        Return source.Select(Function(i) New OChartItemDTO(i) With {.Parent = parentDTO})
    End Function

    <Extension()>
    Public Function FindResourceMaster(source As IEnumerable(Of OChartItem), resource As Container) As OChartItem
        Return source.SelectMany(Function(i) i.Containers) _
            .FindByResource(resource).Where(Function(c) c.IsMaster).Select(Function(c) c.Item).FirstOrDefault()
    End Function
    <Extension()>
    Public Function FindResourceSharers(source As IEnumerable(Of OChartItem), resource As Container) As IEnumerable(Of OChartItem)
        Return source.SelectMany(Function(i) i.Containers) _
            .FindByResource(resource).Where(Function(c) Not c.IsMaster).Select(Function(c) c.Item)
    End Function

    <Extension()>
    Public Function GetContainerRejection(source As IEnumerable(Of OChartItem), resource As Container) As Boolean?
        Return source.SelectMany(Function(i) i.Containers) _
            .FindByResource(resource).Any(Function(c) c.IsRejection)
    End Function

End Module
