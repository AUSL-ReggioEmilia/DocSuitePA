Imports System.Linq
Imports System.Runtime.CompilerServices

Public Module OChartItemContactExtensions

    <Extension()>
    Public Function ReplicateList(ByVal source As IEnumerable(Of OChartItemContact), item As OChartItem) As IEnumerable(Of OChartItemContact)
        Return source.Select(Function(c) New OChartItemContact(c) With {.Item = item})
    End Function

End Module
