Imports System.Linq
Imports System.Runtime.CompilerServices
Imports VecompSoftware.Helpers.ExtensionMethods

Public Module OChartExtensions

    <Extension()>
    Public Function OrderChronologically(ByVal source As IEnumerable(Of OChart)) As IEnumerable(Of OChart)
        Return source.OrderBy(Function(o) o.StartDateOrDefault) _
            .ThenBy(Function(o) o.EndDateOrDefault) _
            .ThenByDescending(Function(o) o.Id.ToSqlGuid())
    End Function

    <Extension()>
    Public Function PreviousOrDefault(ByVal source As IEnumerable(Of OChart), header As OChart) As OChart
        Dim current As New OChart(header, DocSuiteContext.Current.User.FullUserName)
        current.Id = Guid.NewGuid()

        Dim list As New List(Of OChart)(source)
        list.Add(current)
        Dim ordered As IEnumerable(Of OChart) = list.OrderChronologically()
        Dim previous As IEnumerable(Of OChart) = ordered.TakeWhile(Function(o) Not o.Id.Equals(current.Id))
        If previous.IsNullOrEmpty() Then
            Return header
        End If
        Return previous.Last()
    End Function

    <Extension()>
    Public Function IsSame(source As OChart, current As OChart) As Boolean
        Return source.StartDateOrDefault = current.StartDateOrDefault AndAlso source.EndDateOrDefault = current.EndDateOrDefault
    End Function

End Module
