Imports System.Linq
Imports System.Runtime.CompilerServices

Public Module OChartItemMailboxExtensions

    <Extension()>
    Public Function ReplicateList(ByVal source As IEnumerable(Of OChartItemMailbox), item As OChartItem) As IEnumerable(Of OChartItemMailbox)
        Return source.Select(Function(c) New OChartItemMailbox(c) With {.Item = item})
    End Function
    <Extension()>
    Public Function ReplicateList(ByVal source As IEnumerable(Of OChartItemMailbox)) As IEnumerable(Of OChartItemMailbox)
        Return source.ReplicateList(Nothing)
    End Function

End Module
