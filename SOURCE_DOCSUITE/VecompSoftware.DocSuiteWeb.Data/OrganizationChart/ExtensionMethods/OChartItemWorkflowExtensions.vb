Imports System.Linq
Imports System.Runtime.CompilerServices

Public Module OChartItemWorkflowExtensions

    <Extension()>
    Public Function ReplicateList(ByVal source As IEnumerable(Of OChartItemWorkflow), item As OChartItem) As IEnumerable(Of OChartItemWorkflow)
        Return source.Select(Function(c) New OChartItemWorkflow(c) With {.OChartItem = item})
    End Function
    <Extension()>
    Public Function ReplicateList(ByVal source As IEnumerable(Of OChartItemWorkflow)) As IEnumerable(Of OChartItemWorkflow)
        Return source.ReplicateList(Nothing)
    End Function


End Module
