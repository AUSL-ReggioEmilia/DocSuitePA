Imports System.Linq
Imports System.Runtime.CompilerServices

Public Module OChartItemRoleExtensions

    <Extension()>
    Public Function ReplicateList(ByVal source As IEnumerable(Of OChartItemRole), item As OChartItem) As IEnumerable(Of OChartItemRole)
        Return source.Select(Function(c) New OChartItemRole(c) With {.Item = item})
    End Function
    <Extension()>
    Public Function ReplicateList(ByVal source As IEnumerable(Of OChartItemRole)) As IEnumerable(Of OChartItemRole)
        Return source.ReplicateList(Nothing)
    End Function

End Module
